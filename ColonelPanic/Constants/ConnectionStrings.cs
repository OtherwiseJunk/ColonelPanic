using System;
using System.Collections.Generic;
using System.Text;

namespace ColonelPanic.Constants
{
	public class ConnectionStrings
	{
		public static string ConnectionString = Environment.GetEnvironmentVariable("DATABASE") == null ? "data source=EPSILON;initial catalog=MotherBrain;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" : Environment.GetEnvironmentVariable("DATABASE");
	}
}
