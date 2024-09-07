using System.Data.Common;
using System.Data.SQLite;
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
				InlineKeyboardButton.WithCallbackData("Russian", "lang_ru"),
				InlineKeyboardButton.WithCallbackData("English", "lang_en"),
				InlineKeyboardButton.WithCallbackData("German", "lang_de"),
			};
		}

		public List<WordModel> GetNewWords()
		{
			return new List<WordModel>
			{
				new WordModel { Word = "apple", Translation = "яблоко", Correct = false },
				new WordModel { Word = "banana", Translation = "банан", Correct = false },
				new WordModel { Word = "cherry", Translation = "вишня", Correct = false },
				new WordModel { Word = "grape", Translation = "виноград", Correct = false },
			};
		}

		public List<WordModel> GetLearnedWords(int count, int startFrom = -1) // -1 means random
		{
			if (startFrom > 0)
			{
				return new List<WordModel>
				{
					new WordModel { Word = "apple", Translation = "яблоко", Correct = true },
					new WordModel { Word = "banana", Translation = "банан", Correct = true },
				};
			}
			return new List<WordModel>
			{
				new WordModel { Word = "elephant", Translation = "слон", Correct = false },
				new WordModel { Word = "water", Translation = "вода", Correct = false },
			};
		}

		public List<string> GetVariants(int count, string word, long userId)
		{
			return ["слон", "вода"];
		}

		public int GetPageCount(long userId, int wordsPerPage) { return 4; }

		public int GetWordCount(long userId)
		{
			return 4;
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

		public double GetAverageCorrectness(long userId)
		{
			return 0.5;
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
