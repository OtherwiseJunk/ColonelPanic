using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
	class MarioMakerModule : ModuleBase
	{
		string marioMakerCourseIDRegex = "(([A-Z]|[0-9]){3})-(([A-Z]|[0-9]){3})-(([A-Z]|[0-9]){3})";

		[Command("smmc"), Summary("Gets a list of all messages from this channel matching the Mario Maker Course ID format"), Alias("courses"), Alias("levels")]
		public async Task GetLevels()
		{
			await Context;
		}
	}
}
