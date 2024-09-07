using Microsoft.EntityFrameworkCore;
namespace quadrolingoBot.DbModels
{
	public class BotContext : DbContext
	{
		public BotContext(DbContextOptions<BotContext> options)
		: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Exercise> Exercises { get; set; } = null!;
		public DbSet<Language> Languages { get; set; } = null!;
		public DbSet<User> Users { get; set; } = null!;
		public DbSet<UserWord> UserWords { get; set; } = null!;
		public DbSet<Word> Words { get; set; } = null!;
		public DbSet<WordExercise> WordExercises { get; set; } = null!;

	}
}
