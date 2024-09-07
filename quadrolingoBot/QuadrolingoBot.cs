using Microsoft.AspNetCore.Mvc.RazorPages;
using quadrolingoBot.BotModels;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace quadrolingoBot;

internal class QuadrolingoBot
{
	const int WordsPerPage = 10;

	private TelegramBotClient bot;
	private DbManager dbManager;
	private Dictionary<long, UserModel> userBuffer;
	private Dictionary<long, WordCollection> wordsBuffer;
	private Dictionary<long, Paging> wordPage;

	public QuadrolingoBot(string token, DbManager dbManager)
	{
		this.dbManager = dbManager;
		userBuffer = new Dictionary<long, UserModel>();
		wordsBuffer = new Dictionary<long, WordCollection>();
		wordPage = new Dictionary<long, Paging>();
		bot = new TelegramBotClient(token);
		bot.OnUpdate += OnUpdate;
		bot.OnMessage += OnMessage;
		var commands = new List<BotCommand>
		{
			new BotCommand { Command = "/start", Description = "Start the bot" },
			new BotCommand { Command = "/menu", Description = "Show the menu" },
		};
		Task.Run(() => bot.SetMyCommandsAsync(commands));
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
		else if (msg.Text.Contains("/menu"))
		{
			if (dbManager.UserExists(msg.From.Id))
			{
				await ShowMenuAsync(msg.Chat.Id, msg.From.Id);
			}
			else
			{
				await bot.SendTextMessageAsync(msg.Chat.Id, "You need to start with /start command");
			}
		}
		else
		{
			if (wordsBuffer.TryGetValue(msg.Chat.Id, out WordCollection words))
			{
				if (words is WordExerciseCollection wec)
				{
					var wordCurrent = words[words.Current];
					if (msg.Text.Equals(wordCurrent.Translation, StringComparison.CurrentCultureIgnoreCase))
					{
						wordCurrent.Correct = true;
						await bot.SendTextMessageAsync(msg.Chat.Id, MessageConsts.CorrectAnswerMessage);
					}
					else
					{
						await bot.SendTextMessageAsync(msg.Chat.Id, MessageConsts.WrongAnswerMessage + $"<b>{wordCurrent.Translation}</b>", parseMode: ParseMode.Html);
					}
					if (++words.Current <= words.Count - 1)
					{
						await AskAQuestionAsync(msg.Chat.Id, msg.From.Id);
					}
					else
					{
						int correct = wec.GetCorrectCount();
						string result = correct == wec.Count ? "Congratulations! You have successfully completed the exercise." : $"You have completed the exercise. You have {correct} correct answers out of {wec.Count}.";
						double averageCorrectness = dbManager.GetAverageCorrectness(msg.From.Id);

						string toAverage = averageCorrectness >= 0 ? GetAverageComparison(correct, wec.Count, averageCorrectness) : string.Empty;
						await bot.SendTextMessageAsync(msg.Chat.Id, result + toAverage, replyMarkup: new ReplyKeyboardRemove());
						dbManager.SaveExerciseResults(msg.From.Id, wec);
						wordsBuffer.Remove(msg.Chat.Id, out _);
						await ShowMenuAsync(msg.Chat.Id, msg.From.Id);
					}
				}
			}
			else
			{
				await bot.SendTextMessageAsync(msg.Chat.Id, "I don't understand you. Please, start with /start command");
			}
		}
	}

	string GetAverageComparison(int correct, int total, double averageCorrectness)
	{
		double userCorrectness = (double)correct / total;
		double difference = userCorrectness - averageCorrectness;
		if (difference == 0) return " Your result is equal to your average.";
		string comparison = difference > 0 ? "better" : "worse";
		double percentage = Math.Abs(difference) * 100;
		return $" Your result is {(int)percentage}% {comparison} than your average.";
	}

	async Task StartExerciseAsync(CallbackQuery query)
	{
		await bot.SendTextMessageAsync(query.Message.Chat.Id, MessageConsts.StartExerciseMessage);
		if (!wordsBuffer.TryGetValue(query.Message.Chat.Id, out WordCollection words))
		{
			// user wants to start exercise without learning new words
			words = new WordExerciseCollection(dbManager.GetLearnedWords(WordsPerPage));
			wordsBuffer.Add(query.Message.Chat.Id, words);
		}
		else
		{
			// add some learned words to repeat them
			var learnedWords = dbManager.GetLearnedWords(2);
			wordsBuffer[query.Message.Chat.Id] = new WordExerciseCollection(learnedWords.Concat(words.list).ToList());
		}
		await AskAQuestionAsync(query.Message.Chat.Id, query.From.Id);
	}

	async Task AskAQuestionAsync(long chatId, long userId)
	{
		if (wordsBuffer.TryGetValue(chatId, out WordCollection words))
		{
			var word = words[words.Current];
			var variants = dbManager.GetVariants(2, word.Translation, userId).Append(word.Translation).ToArray();
			var wordOrder = words.Current == 0 ? "first" : words.Current == words.Count ? "last" : "next";
			string text = $"🔤 The {wordOrder} word is {word.GetToGuess()}.\nSelect the translation option or write it manually.";

			await bot.SendTextMessageAsync(chatId, text, replyMarkup: GetExerciseMarkup(variants), parseMode: ParseMode.Html);
		}
	}

	async Task ShowMenuAsync(long chatId, long userId, int messageId = 0)
	{
		int wordCount = dbManager.GetWordCount(userId);
		if (messageId == 0)
			await bot.SendTextMessageAsync(chatId, string.Format(MessageConsts.MenuMessage, wordCount), replyMarkup: GetMenuMarkup());
		else
			await bot.EditMessageTextAsync(chatId, messageId, string.Format(MessageConsts.MenuMessage, wordCount), replyMarkup: GetMenuMarkup());
	}

	async Task StartLearningWordsAsync(CallbackQuery query)
	{
		WordCollection wordCollection = new(dbManager.GetNewWords(WordsPerPage));
		wordsBuffer.Add(query.Message.Chat.Id, wordCollection);
		var message = await bot.SendTextMessageAsync(query.Message.Chat.Id, wordCollection[0].GetToLearn(), replyMarkup: GetMemorizingMarkup(MarkupArrows.Next));
		wordCollection.MessageId = message.MessageId;
	}

	async Task ShowWordsAsync(CallbackQuery query)
	{
		if (!wordPage.TryGetValue(query.Message.Chat.Id, out Paging page))
		{
			page = new Paging(0, dbManager.GetPageCount(query.From.Id, WordsPerPage), query.Message.MessageId);
			wordPage.Add(query.Message.Chat.Id, page);
		}
		var words = dbManager.GetLearnedWords(WordsPerPage, WordsPerPage * page.CurrentPage);
		var text = new StringBuilder();
		text.AppendJoin(Environment.NewLine + Environment.NewLine, words.Select(w => w.GetToLearn()));
		await bot.EditMessageTextAsync(query.Message.Chat.Id, page.MessageId, text.ToString(), replyMarkup: GetWordListMarkup(page));
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
			await StartExerciseAsync(query);
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "learn_words")
		{
			await bot.SendTextMessageAsync(query.Message.Chat.Id, MessageConsts.NewWordSetMessage);
			await StartLearningWordsAsync(query);
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "show_words")
		{
			await ShowWordsAsync(query);
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "page_prev")
		{
			var page = wordPage[query.Message.Chat.Id];
			if (page.CurrentPage > 0)
			{
				page.CurrentPage--;
				await ShowWordsAsync(query);
			}
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "page_next")
		{
			var page = wordPage[query.Message.Chat.Id];
			if (page.CurrentPage < page.PageCount)
			{
				page.CurrentPage++;
				await ShowWordsAsync(query);
			}
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "paging")
		{
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "start_excercise")
		{
			await StartExerciseAsync(query);
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "learn_words")
		{
			await bot.SendTextMessageAsync(query.Message.Chat.Id, MessageConsts.NewWordSetMessage);
			await StartLearningWordsAsync(query);
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "show_words")
		{
			await ShowWordsAsync(query);
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "page_prev")
		{
			var page = wordPage[query.Message.Chat.Id];
			if (page.CurrentPage > 0)
			{
				page.CurrentPage--;
				await ShowWordsAsync(query);
			}
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "page_next")
		{
			var page = wordPage[query.Message.Chat.Id];
			if (page.CurrentPage < page.PageCount)
			{
				page.CurrentPage++;
				await ShowWordsAsync(query);
			}
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "paging")
		{
			await bot.AnswerCallbackQueryAsync(query.Id);
		}
		else if (query.Data == "show_menu")
		{
			await bot.AnswerCallbackQueryAsync(query.Id);
			await ShowMenuAsync(query.Message.Chat.Id, query.From.Id, query.Message.MessageId);
			wordPage.Remove(query.Message.Chat.Id, out _);
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
;				await StartLearningWordsAsync(query);
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

	ReplyKeyboardMarkup GetExerciseMarkup(string[] words)
	{
		List<KeyboardButton> buttons = [.. words.Select(w => new KeyboardButton(w))];
		buttons.Shuffle();
		return new ReplyKeyboardMarkup(buttons);
	}

	InlineKeyboardMarkup GetMenuMarkup()
	{
		return new InlineKeyboardMarkup(new[]
		{
			new []
			{
				InlineKeyboardButton.WithCallbackData("Learn new words 📖", "learn_words"),
			},
			new []
			{
				InlineKeyboardButton.WithCallbackData("Start excercise 📝", "start_excercise"),
			},
			new []
			{
				InlineKeyboardButton.WithCallbackData("Show your known words 📁", "show_words"),
			},
		});
	}

	InlineKeyboardMarkup GetMemorizingMarkup(MarkupArrows arrows)
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

	InlineKeyboardMarkup GetWordListMarkup(Paging paging)
	{
		InlineKeyboardButton[] firstRow;
		if (paging.CurrentPage == paging.PageCount - 1)
		{
			firstRow = new[]
			{
					InlineKeyboardButton.WithCallbackData("⬅️", "page_prev"),
					InlineKeyboardButton.WithCallbackData(paging.ToString(), "paging"),
			};
		}
		else if (paging.CurrentPage == 0)
		{

			firstRow = new[]
			{
					InlineKeyboardButton.WithCallbackData(paging.ToString(), "paging"),
					InlineKeyboardButton.WithCallbackData("➡️", "page_next"),
			};
		}
		else
		{
			firstRow = new[]
{
					InlineKeyboardButton.WithCallbackData("⬅️", "page_prev"),
					InlineKeyboardButton.WithCallbackData(paging.ToString(), "paging"),
					InlineKeyboardButton.WithCallbackData("➡️", "page_next"),
			};
		}
		return new InlineKeyboardMarkup(new[]
		{
			firstRow,
			new []
			{
				InlineKeyboardButton.WithCallbackData("Back to menu", "show_menu"),
			},
		});
	}

	private enum MarkupArrows
	{
		Prev,
		Next,
		PrevNext
	}

	private class Paging
	{
		public Paging(int currentPage, int pageCount, int messageId)
		{
			CurrentPage = currentPage;
			PageCount = pageCount;
			MessageId = messageId;
		}

		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public int MessageId { get; set; } = 0;

		public override string ToString()
		{
			return $"{CurrentPage + 1}/{PageCount}";
		}
	}
}