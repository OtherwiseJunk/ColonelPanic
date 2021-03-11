using Discord;
using Discord.Commands;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace DartsDiscordBots.Services.Interfaces
{
	public interface ILogService
	{
		public Logger _log { get; set; }
		public void LogDiscordMessage(IMessage message);
		public void LogModuleEvent(IMessage triggeringMessage, ICommandContext context);
		public void Log(LogEventInfo logMessage);
	}
}
