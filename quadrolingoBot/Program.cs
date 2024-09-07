using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using quadrolingoBot.DbModels;

namespace quadrolingoBot;
internal class Program
{
	static async Task Main(string[] args)
	{
		var configuration = new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddConfiguration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
		.Build();
		string token = configuration["ApiSettings:Token"];
		string connectionString = configuration["ConnectionStrings:quadrolingoBaza"];

		var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
		optionsBuilder.UseSqlServer(connectionString);

		using (var context = new BotContext(optionsBuilder.Options))
		{
			context.Database.EnsureCreated();
			var manager = new DbManagerRelease(context);
			QuadrolingoBot bot = new QuadrolingoBot(token, manager);

			while (true)
			{
			}
		}
	}
}

