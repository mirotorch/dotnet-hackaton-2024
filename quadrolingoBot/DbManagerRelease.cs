using Telegram.Bot.Types.ReplyMarkups;
using quadrolingoBot.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Extensions;
using System.Data.SqlClient;
using System.Collections.Specialized;
using quadrolingoBot.BotModels;

namespace quadrolingoBot
{
    internal class DbManagerRelease : DbManager
	{
		private BotContext context;

		public DbManagerRelease(BotContext context)
		{
			this.context = context;
		}

		new public InlineKeyboardButton[] GetLanguageButtons()
		{
			List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
			foreach (var language in context.Languages)
			{
				buttons.Add(InlineKeyboardButton.WithCallbackData(language.LANG_ENG_NAME, $"lang_{language.LANG_CODE}"));

			}
			return buttons.ToArray();
		}

		new public List<WordModel> GetNewWords(int count, long userId)
		{
			string language = context.Users.Find(userId).BASE_LANG;
			var query = context.Words
				 .FromSqlRaw(@"
                SELECT TOP (@count) 
                       w.WORD_BASE AS [Word], 
                       JSON_VALUE(w.WORD_TRANSLATIONS, '$.@language') AS [Translation]
                FROM Word w
                INNER JOIN [User] u ON w.WORD_LANG = u.BASE_LANG
                LEFT JOIN UserWord uw ON w.WORD_ID = uw.WORD_ID AND uw.TELEGRAM_ID = @userId
                WHERE uw.WORD_ID IS NULL AND JSON_VALUE(w.WORD_TRANSLATIONS, '$.@language') IS NOT NULL
                ORDER BY NEWID()
            ",
				 new SqlParameter("@count", count),
				 new SqlParameter("@language", language),
				 new SqlParameter("@userId", userId))
				 .ToList();
			return query.Select(w => new WordModel(w)).ToList();
		}
		

		new public List<WordModel> GetLearnedWords(int count, int userId, int startFrom = -1) // -1 means random
		{
			
			if (startFrom > 0)
			{
				var ids = context.UserWords.Where(uw => uw.USER_ID.TELEGRAM_ID == userId).OrderBy(x => x.TIMESTAMP).ToList()[startFrom..(startFrom+count)];
				var words = context.Words
					.FromSqlRaw(@"
					SELECT 
						   w.WORD_BASE AS [Word], 
						   JSON_VALUE(w.WORD_TRANSLATIONS, '$.@language') AS [Translation]
					FROM Word w
						WHERE w.WORD_ID IN @ids
					",
					 new SqlParameter("@ids", ids),
					 new SqlParameter("@language", context.Users.Where(u => u.TELEGRAM_ID == userId).First().BASE_LANG))
					 .ToList();

				return words.Select(w => new WordModel(w)).ToList();
			}
			else
			{
				var ids = context.UserWords.Where(uw => uw.USER_ID.TELEGRAM_ID == userId).OrderBy(x => Guid.NewGuid()).Take(count);
				var words = context.Words
					.FromSqlRaw(@"
				SELECT 
					   w.WORD_BASE AS [Word], 
					   JSON_VALUE(w.WORD_TRANSLATIONS, '$.@language') AS [Translation]
				FROM Word w
				WHERE w.WORD_ID IN @ids
			",
				 new SqlParameter("@ids", ids),
				 new SqlParameter("@language", context.Users.Where(u => u.TELEGRAM_ID == userId).First().BASE_LANG))
				 .ToList();

				return words.Select(w => new WordModel(w)).ToList();
			}
		}

		new public bool UserExists(long userId)
		{
			return context.Users.Any(u => u.TELEGRAM_ID == userId);
		}

		new public List<string> GetVariants(int count, string word, long userId)
		{
			var list =  context.Words
				.FromSqlRaw(@"
				SELECT TOP (@count) 
					   JSON_VALUE(w.WORD_TRANSLATIONS, '$.@language') AS [Translation]
				FROM Word w
				INNER JOIN [User] u ON w.WORD_LANG = u.BASE_LANG
				WHERE JSON_VALUE(w.WORD_TRANSLATIONS, '$.@language') IS NOT NULL AND w.WORD_BASE != @word
				ORDER BY NEWID()
			",
				 new SqlParameter("@count", count),
				 new SqlParameter("@language", context.Users.Where(u => u.TELEGRAM_ID == userId).First().BASE_LANG),
				 new SqlParameter("@word", word))
				 .Select(w => w.WORD_TRANSLATION)
				 .ToList();
			list.Add(word);
			list.Shuffle();
			return list;
		}

		new public int GetPageCount(long userId, int wordsPerPage) 
		{
			return context.UserWords.Where(uw => uw.USER_ID.TELEGRAM_ID == userId).Count() / wordsPerPage;
		}

		new public int GetWordCount(long userId)
		{
			return context.UserWords.Where(uw => uw.USER_ID.TELEGRAM_ID == userId).Count();
		}

		new public double GetAverageCorrectness(long userId)
		{
			var exercises = context.WordExercises
				.Join(context.Exercises,
					  we => we.EXERCISE_ID.Id,
					  e => e.Id,
					  (we, e) => new { WordExercise = we, Exercise = e })
				.Where(we_e => we_e.Exercise.USER_ID.TELEGRAM_ID == userId)
				.ToList();

			if (exercises.Count == 0)
			{
				return -1; 
			}

			return exercises.Average(we_e => we_e.WordExercise.Guessed ? 1.0 : 0.0);
		}


		/// <summary>
		/// We update both UserWord and WordExercise here to avoid complications with word repetition in <see cref="GetLearnedWords(int)"/>
		/// </summary>
		new public void SaveExerciseResults(long userId, WordExerciseCollection words)
		{

			Exercise ex = new Exercise
			{
				USER_ID = context.Users.Where(u => u.TELEGRAM_ID == userId).First(),
				TIMESTAMP = DateTime.Now
			};
			string language = ex.USER_ID.BASE_LANG;
			context.Exercises.Add(ex);
			context.SaveChanges();

			words.list.ForEach(w =>
			{
				var userWord = context.UserWords
					.Where(uw => uw.USER_ID.TELEGRAM_ID == userId && uw.WORD_ID.WORD_BASE == w.Word && uw.WORD_ID.WORD_LANG == language)
					.FirstOrDefault();

				if (userWord == null)
				{
					userWord = new UserWord
					{
						USER_ID = ex.USER_ID,
						WORD_ID = context.Words.Where(wr => wr.WORD_BASE == w.Word && wr.WORD_LANG == language).First(),
						TIMESTAMP = DateTime.Now
					};
					context.UserWords.Add(userWord);
				}

				var wordExercise = new WordExercise
				{
					WORD_ID = userWord.WORD_ID,
					EXERCISE_ID = ex,
					Guessed = w.Correct
				};
				context.WordExercises.Add(wordExercise);
			});
			context.SaveChanges();
		}

		new public void AddUser(UserModel user)
		{
			User dbUser = new User
			{
				CHAT_ID = user.ChatId,
				TELEGRAM_ID = user.UserId,
				BASE_LANG = user.BaseLang,
				STUDY_LANG = user.StudyLang,
				USERNAME = user.UserName,
				FIRST_NAME = user.FirstName,
				LAST_NAME = user.LastName
			};

			if (!context.Users.Any(u => u.TELEGRAM_ID == dbUser.TELEGRAM_ID))
			{
				context.Users.Add(dbUser);
				context.SaveChanges();
			}
		}

	}
}
