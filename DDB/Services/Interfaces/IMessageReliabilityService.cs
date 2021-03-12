using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Services.Interfaces
{
	public interface IMessageReliabilityService
	{
		private int MAX_MESSAGE_LENGTH { get => 1999; }
		private ILogService _logger { get => null; }
		public Task SendMessageToChannel(string messageContent, IChannel channel, MessageReference? referencedMessage, List<ulong>? mentionedUserIds, string seperatingCharacter);
		public List<string> SplitMessageContent(string messageContent, string seperatingCharacter);
	}
}
