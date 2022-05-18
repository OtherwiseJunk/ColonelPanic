using DartsDiscordBots.Modules.Jackbox.Models;
using Microsoft.EntityFrameworkCore;
using System;
using JBC = DartsDiscordBots.Modules.Jackbox.JackboxConstants;
using Discord;

namespace ColonelPanic.Database.Contexts
{
	public class JackboxContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=db.cacheblasters.com;Database=MotherBrain;User Id=sa;Password=ASecureP4ssw0rd!");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<JackboxGame>()
				.HasData(JBC.DefaultGameData);

			modelBuilder.Entity<JackboxGame>()
				.Property(jbg => jbg.VotingEmoji)
				.HasConversion(
					ve => ve.ToString(),
					ve => new Emoji(ve)
				);
		}
		public DbSet<JackboxGame> JackboxGames { get; set; }
		public DbSet<GameRating> GameRatings { get; set; }
	}
}
