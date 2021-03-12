using DartsDiscordBots.Services.Interfaces;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Services
{
	public class MessageReliabilityService : IMessageReliabilityService
	{
		//private ILogService _logger { get; set; }
		private int MAX_MESSAGE_LENGTH { get; set; } = 1999;

		/*public MessageReliabilityService(ILogService logger)
		{
			_logger = logger;
		}*/

		public async Task SendMessageToChannel(string messageContent, IChannel channel, MessageReference referencedMessage, List<ulong> mentionedUserIds, string seperatingCharacter)
		{
			IMessageChannel socketChannel = (IMessageChannel)channel;
			List<string> messages;
			AllowedMentions allowMentions = (mentionedUserIds.Count > 0) ? AllowedMentions.All : AllowedMentions.None;

			if (messageContent.Length > MAX_MESSAGE_LENGTH)
			{
				messages = SplitMessageContent(messageContent, seperatingCharacter);
			}
			else
			{
				messages = new List<string> { messageContent };
			}

			foreach(string message in messages)
			{
				IMessage sentMessage = socketChannel.SendMessageAsync(message, messageReference: referencedMessage, allowedMentions: allowMentions).Result;
				referencedMessage = new MessageReference(sentMessage.Id);
			}						
		}

		public List<string> SplitMessageContent(string messageContent, string seperatingCharacter)
		{
			List<string> messages = new List<string>();			

			while (messageContent.Length > MAX_MESSAGE_LENGTH)
			{
				string submessage = "";
				List<string> messageLines = messageContent.Split(seperatingCharacter).ToList();				
				foreach(string line in messageLines)
				{			
					if(submessage.Length + line.Length + seperatingCharacter.Length <= MAX_MESSAGE_LENGTH)
					{
						submessage += line + seperatingCharacter;
					}
					else
					{
						messages.Add(submessage);
						break;
					}
				}
				messageContent = messageContent.Remove(0, submessage.Length);
			}
			if(messageContent.Length != 0)
			{
				messages.Add(messageContent);
			}
			return messages;
		}
	}
}
