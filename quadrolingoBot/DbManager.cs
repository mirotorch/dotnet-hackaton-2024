using System.Data.Common;
using System.Data.SQLite;
using Microsoft.Identity.Client;
using quadrolingoBot.BotModels;
using Telegram.Bot.Types.ReplyMarkups;

namespace quadrolingoBot
{
	internal class DbManager
	{

		public DbManager()
		{

		}

		public InlineKeyboardButton[] GetLanguageButtons()
		{
			return new InlineKeyboardButton[]
			{
				InlineKeyboardButton.WithCallbackData("🇺🇸🇬🇧English", "lang_en"),
				InlineKeyboardButton.WithCallbackData("🇩🇪German", "lang_de"),
				InlineKeyboardButton.WithCallbackData("🇺🇦Ukrainian", "lang_ua"),
				InlineKeyboardButton.WithCallbackData("🏳️‍🌈Russian", "lang_ru"),
			};
		}

		public List<WordModel> GetNewWords(int count)
		{
			return new List<WordModel>
			{
				new WordModel { Word = "apple", Translation = "яблуко", Correct = false },
				new WordModel { Word = "banana", Translation = "банан", Correct = false },
				new WordModel { Word = "cherry", Translation = "вишня", Correct = false },
				new WordModel { Word = "grape", Translation = "виноград", Correct = false },
				new WordModel { Word = "lemon", Translation = "лимон", Correct = false },
				//------------------------------------------------------------------------
				new WordModel { Word = "orange", Translation = "апельсин", Correct = false },
				new WordModel { Word = "peach", Translation = "персик", Correct = false },
			};
		}

		public List<WordModel> GetLearnedWords(int count, int startFrom = -1) // -1 means random
		{
			if (startFrom >= 0)
			{
				if (startFrom == 0)
				{
					return new List<WordModel>
					{
						new WordModel { Word = "apple", Translation = "яблуко", Correct = false },
						new WordModel { Word = "banana", Translation = "банан", Correct = false },
						new WordModel { Word = "cherry", Translation = "вишня", Correct = false },
						new WordModel { Word = "grape", Translation = "виноград", Correct = false },
						new WordModel { Word = "lemon", Translation = "лимон", Correct = false },
					};
				}
				if (startFrom > 0)
				{
					return new List<WordModel>
					{
						new WordModel { Word = "orange", Translation = "апельсин", Correct = false },
						new WordModel { Word = "peach", Translation = "персик", Correct = false },
					};
				}
			}
			return new List<WordModel>
			{
				new WordModel { Word = "apple", Translation = "яблуко", Correct = false },
				new WordModel { Word = "banana", Translation = "банан", Correct = false },
				new WordModel { Word = "cherry", Translation = "вишня", Correct = false },
				new WordModel { Word = "grape", Translation = "виноград", Correct = false },
				new WordModel { Word = "lemon", Translation = "лимон", Correct = false },
				//------------------------------------------------------------------------
				new WordModel { Word = "orange", Translation = "апельсин", Correct = false },
				new WordModel { Word = "peach", Translation = "персик", Correct = false },
			};
		}

		public bool UserExists(long userId)
		{
			return true;
		}

		public List<string> GetVariants(int count, string word, long userId)
		{
			List<string> variants = [ "яблуко", "банан", "вишня", "виноград", "слон", "вода" ];
			variants.Shuffle();
			var shuffled = variants.Take(2).ToList().Append(word).ToList();
			shuffled.Shuffle();
			return shuffled;
		}

		public int GetPageCount(long userId, int wordsPerPage) { return 2; }

		public int GetWordCount(long userId)
		{
			return 7;
		}

		/// <summary>
		/// We update both UserWord and WordExercise here to avoid complications with word repetition in <see cref="GetLearnedWords(int)"/>
		/// </summary>
		public void SaveExerciseResults(long userId, WordExerciseCollection words)
		{
			//using (SQLiteConnection connection = new SQLiteConnection("Data Source=quadrolingo.db"))
			//{
			//	connection.Open();
			//	using (SQLiteCommand command = new SQLiteCommand(connection))
			//	{
			//		command.CommandText = "INSERT INTO Results (UserId, WordId, Correct) VALUES (@UserId, @WordId, @Correct)";
			//		command.Parameters.AddWithValue("@UserId", userId);
			//		command.Parameters.AddWithValue("@WordId", 1);
			//		command.Parameters.AddWithValue("@Correct", true);
			//		command.ExecuteNonQuery();
			//	}
			//}
		}
		static bool a = false;
		public double GetAverageCorrectness(long userId)
		{
			if (a == false)
			{
				a = true;
				return -1;
			}
			else
			{
				return 0.5;
			}
		}

		public void AddUser(UserModel user)
		{
			//using (SQLiteConnection connection = new SQLiteConnection("Data Source=quadrolingo.db"))
			//{
			//	connection.Open();
			//	using (SQLiteCommand command = new SQLiteCommand(connection))
			//	{
			//		command.CommandText = "INSERT INTO Users (ChatId, UserId, BaseLang, StudyLang, UserName, FirstName, LastName) VALUES (@ChatId, @UserId, @BaseLang, @StudyLang, @UserName, @FirstName, @LastName)";
			//		command.Parameters.AddWithValue("@ChatId", user.ChatId);
			//		command.Parameters.AddWithValue("@UserId", user.UserId);
			//		command.Parameters.AddWithValue("@BaseLang", user.BaseLang);
			//		command.Parameters.AddWithValue("@StudyLang", user.StudyLang);
			//		command.Parameters.AddWithValue("@UserName", user.UserName);
			//		command.Parameters.AddWithValue("@FirstName", user.FirstName);
			//		command.Parameters.AddWithValue("@LastName", user.LastName);
			//		command.ExecuteNonQuery();
			//	}
			//}
		}
	}
}
