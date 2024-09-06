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
			return new InlineKeyboardButton[] // tmp
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

		public List<WordModel> GetLearnedWords()
		{
			return new List<WordModel>
			{
				new WordModel { Word = "apple", Translation = "яблоко", Correct = false },
				new WordModel { Word = "banana", Translation = "банан", Correct = false },
				new WordModel { Word = "cherry", Translation = "вишня", Correct = false },
				new WordModel { Word = "grape", Translation = "виноград", Correct = false },
			};
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
