namespace quadrolingoBot;
internal class Program
{
	const string Token = "7501961943:AAFF_HDVxHWClSzwAEeOrwJDgpYWYuN6WkY";
	static DbManager db = new DbManager();

	static async Task Main(string[] args)
	{
		QuadrolingoBot bot = new QuadrolingoBot(Token, db);
		
		while (true)
		{
		}
	}
}

