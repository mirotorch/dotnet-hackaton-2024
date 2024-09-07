using System.Collections;
using System.Collections.Generic;

namespace quadrolingoBot.BotModels
{
    internal class WordModel
    {
        public string Word { get; set; }
        public string Translation { get; set; }
        public bool Correct { get; set; } = false;

        public WordModel()
        {

        }

        public WordModel(DbModels.Word word)
        {
            Word = word.WORD_BASE;
            Translation = word.WORD_TRANSLATION;
            Correct = false;
        }

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