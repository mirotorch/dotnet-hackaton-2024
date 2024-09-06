using System.Collections;

namespace quadrolingoBot
{
	internal class WordModel
	{
		public int Id { get; set; }
		public string Word { get; set; }
		public string Translation { get; set; }
		public bool Correct { get; set; }

		public string GetToLearn()
		{
			return $"{Word} - {Translation}";
		}

		public string GetToGuess()
		{
			return "<b>" + Word + """</b>""";
		}
	}

	// move to separate file
	internal class WordCollection
	{
		public int MessageId { get; set; }

		private List<WordModel> words;
		public int Current = 0;

		#region IList members
		public WordModel this[int index] { get => words[index]; set => words[index] = value; }

		public int Count => words.Count;

		public WordCollection(List<WordModel> words)
		{
			this.words = words;
		}
		#endregion
	}
}
