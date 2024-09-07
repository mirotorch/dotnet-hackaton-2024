using System.Collections;
using System.Collections.Generic;

namespace quadrolingoBot
{
	internal class WordModel
	{
		public int Id { get; set; }
		public string Word { get; set; }
		public string Translation { get; set; }
		public bool Correct { get; set; } = false;

		public string GetToLearn()
		{
			return $"{Word} - {Translation}";
		}

		public string GetToGuess()
		{
			return "<b>" + Word + """</b>""";
		}
	}
}