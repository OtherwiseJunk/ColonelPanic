using System;
using System.Collections.Generic;
using System.Text;

namespace ColonelPanic.DatabaseCore.Constants
{
	public class ConnectionStrings
	{
		public static string ConnectionString = Environment.GetEnvironmentVariable("COLONELDB") == null ? "data source=EPSILON;initial catalog=MotherBrain;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" : Environment.GetEnvironmentVariable("COLONELDB");
	}
}
