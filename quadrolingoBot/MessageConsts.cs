using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quadrolingoBot
{
	internal static class MessageConsts
	{
		public static string StartMessage = "Hello! I am a bot that will help you learn foreign language. Which language you want to learn?";

		public static string BaseLanguageMessage = "Choose the language in which you will study.";

		public static string InitialWordSetMessage = "Excellent! Now let's start with some basic words. When you're sure you've memorized them, click “start exercise” to test your knowledge.";


		// callback prefixes
		public static string CallbackPrefixLanguage = "lang_";
	}
}
