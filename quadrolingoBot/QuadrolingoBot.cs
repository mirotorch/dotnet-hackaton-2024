using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace quadrolingoBot
{
	internal class QuadrolingoBot
	{
		private TelegramBotClient bot;
		private DbManager dbManager;
		private Dictionary<long, UserModel> userBuffer;
		private Dictionary<long, WordCollection> wordsBuffer;

		public QuadrolingoBot(string token, DbManager dbManager)
		{
			this.dbManager = dbManager;
			userBuffer = new Dictionary<long, UserModel>();
			wordsBuffer = new Dictionary<long, WordCollection>();
			bot = new TelegramBotClient(token);
			bot.OnUpdate += OnUpdate;
			bot.OnMessage += OnMessage;
		}

		private async Task OnUpdate(Update update)
		{
			if (update.Type == UpdateType.CallbackQuery)
			{
				await ProcessCallbackAsync(update.CallbackQuery);
			}
		}

		async Task OnMessage(Message msg, UpdateType type)
		{
			if (msg.Text is null) return;
			if (msg.Text.Contains("/start"))
			{
				userBuffer.TryAdd(msg.Chat.Id, new UserModel
				{
					ChatId = msg.Chat.Id,
					UserId = msg.From.Id,
					UserName = msg.From.Username,
					FirstName = msg.From.FirstName,
					LastName = msg.From.LastName
				});
				await bot.SendTextMessageAsync(msg.Chat.Id, MessageConsts.StartMessage, replyMarkup: new InlineKeyboardMarkup(
				dbManager.GetLanguageButtons()));
			}
		}

		async Task ProcessCallbackAsync(CallbackQuery query)
		{
			if (query.Data.StartsWith(MessageConsts.CallbackPrefixLanguage))
			{
				await ProcessLanguageAsync(query);
				await bot.AnswerCallbackQueryAsync(query.Id);
			}
			else if (query.Data == "word_prev")
			{
				await bot.AnswerCallbackQueryAsync(query.Id);
				var words = wordsBuffer[query.Message.Chat.Id];
				if (words != null && words.Count > 0)
				{
					words.Current--;
					await ProcessArrowsAsync(words, query.Message.Chat.Id);
				}
			}
			else if (query.Data == "word_next")
			{
				await bot.AnswerCallbackQueryAsync(query.Id);
				var words = wordsBuffer[query.Message.Chat.Id];
				if (words != null && words.Count > 0)
				{
					words.Current++;
					await ProcessArrowsAsync(words, query.Message.Chat.Id);
				}
			}
			else if (query.Data == "start_excercise")
			{
				await bot.AnswerCallbackQueryAsync(query.Id);
			}
			else
			{
				await bot.AnswerCallbackQueryAsync(query.Id);
			}
		}

		async Task ProcessLanguageAsync(CallbackQuery query)
		{
			userBuffer.TryGetValue(query.Message.Chat.Id, out UserModel? user);
			if (user != null)
			{
				if (user.StudyLang == null)
				{
					user.StudyLang = query.Data.Substring(MessageConsts.CallbackPrefixLanguage.Length);
					await bot.SendTextMessageAsync(query.Message.Chat.Id, MessageConsts.BaseLanguageMessage, replyMarkup: new InlineKeyboardMarkup(dbManager.GetLanguageButtons()));
				}
				else
				{
					user.BaseLang = query.Data.Substring(MessageConsts.CallbackPrefixLanguage.Length);
					dbManager.AddUser(user);
					userBuffer.Remove(query.Message.Chat.Id, out _);
					await bot.SendTextMessageAsync(query.Message.Chat.Id, MessageConsts.InitialWordSetMessage);
					WordCollection wordCollection = new(dbManager.GetNewWords());
					wordsBuffer.Add(query.Message.Chat.Id, wordCollection);
					var message = await bot.SendTextMessageAsync(query.Message.Chat.Id, wordCollection[0].GetToLearn(), replyMarkup: GetMemorizingMarkup(MarkupArrows.Next));
					wordCollection.MessageId = message.MessageId;
				}
			}
		}

		async Task ProcessArrowsAsync(WordCollection words, long ChatId)
		{
			MarkupArrows arrows;
			if (words.Current == 0) arrows = MarkupArrows.Next;
			else if (words.Current == words.Count - 1) arrows = MarkupArrows.Prev;
			else arrows = MarkupArrows.PrevNext;

			await bot.EditMessageTextAsync(ChatId, words.MessageId, words[words.Current].GetToLearn(), replyMarkup: GetMemorizingMarkup(arrows));
		}

		private InlineKeyboardMarkup GetMemorizingMarkup(MarkupArrows arrows)
		{
			return arrows switch
			{
				MarkupArrows.Prev => new InlineKeyboardMarkup(new[]
				{
					new []
					{
						InlineKeyboardButton.WithCallbackData("⬅️", "word_prev"),
					},
					new []
					{
						InlineKeyboardButton.WithCallbackData("Start excercise", "start_excercise"),
					}
				}),
				MarkupArrows.Next => new InlineKeyboardMarkup(new[]
				{
					new []
					{
						InlineKeyboardButton.WithCallbackData("➡️", "word_next"),
					},
					new []
					{
						InlineKeyboardButton.WithCallbackData("Start excercise", "start_excercise"),
					}
				}),
				MarkupArrows.PrevNext => new InlineKeyboardMarkup(new[]
				{
					new []
					{
						InlineKeyboardButton.WithCallbackData("⬅️", "word_prev"),
						InlineKeyboardButton.WithCallbackData("➡️", "word_next"),
					},
					new []
					{
						InlineKeyboardButton.WithCallbackData("Start excercise", "start_excercise"),
					}
				}),
				_ => new InlineKeyboardMarkup(new[]
				{
					new []
					{
						InlineKeyboardButton.WithCallbackData("Start excercise", "start_excercise"),
					}
				}),
			};
		}

		private enum MarkupArrows
		{
			Prev,
			Next,
			PrevNext
		}
	}
}
