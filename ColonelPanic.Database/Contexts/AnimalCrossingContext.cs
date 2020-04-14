using DartsDiscordBots.Models;
using Microsoft.EntityFrameworkCore;

namespace ColonelPanic.Database.Contexts
{
	public class AnimalCrossingContext : DbContext
	{
		public AnimalCrossingContext(DbContextOptions<AnimalCrossingContext> options): base(options) { }

		public DbSet<Town> Towns { get; set; }
		public DbSet<Fruit> Fruits { get; set; }
		public DbSet<Item> WishlistItems { get; set; }
	}
}
