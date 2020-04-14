using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartsDiscordBots.Utilities
{
	public static class Converters
	{
		public static Stream GetImageStreamFromBase64(string base64)
		{
			var bytes = Convert.FromBase64String(base64);
			return new MemoryStream(bytes);
		}
	}
}
