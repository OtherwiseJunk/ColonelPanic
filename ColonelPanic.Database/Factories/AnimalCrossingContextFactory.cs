using ColonelPanic.Database.Contexts;
using ColonelPanic.DatabaseCore.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColonelPanic.Database.Factories
{
	public class AnimalCrossingContextFactory : IDesignTimeDbContextFactory<AnimalCrossingContext>
	{
		public AnimalCrossingContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<AnimalCrossingContext>();
			optionsBuilder.UseSqlServer(ConnectionStrings.ConnectionString);

			return new AnimalCrossingContext(optionsBuilder.Options);
		}
	}
}
