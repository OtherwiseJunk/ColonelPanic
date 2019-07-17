using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
	public class MarioMakerModule : ModuleBase
	{
		string marioMakerCourseIDRegex = "(([A-Z]|[0-9]){3})-(([A-Z]|[0-9]){3})-(([A-Z]|[0-9]){3})";

		[Command("smmc"), Summary("Gets a list of all messages from this channel matching the Mario Maker Course ID format. Checks the last 10,000 messages."), Alias("courses")]
		public async Task GetLevels()
		{
			await Context.Channel.SendMessageAsync("Request received! Processing...");
			var msgCollection = await Context.Channel.GetMessagesAsync(10000).Flatten();
			List<string> MarioMakerMessages = new List<string>();
			Regex regex = new Regex(marioMakerCourseIDRegex);

			foreach(var msg in msgCollection)
			{
				if (regex.IsMatch(msg.Content) && !msg.Author.IsBot)
				{
					if(msg.MentionedUserIds.Count == 0)
					{
						MarioMakerMessages.Add($"{msg.Author.Username} - {msg.Timestamp.ToLocalTime().ToString("MM/dd/yyyy H:mm")}: {msg.Content}");
					}
					else
					{
						string content = msg.Content;
						string[] words = content.Split(' ');
						Dictionary<string, string> replaceValues = new Dictionary<string, string>();

						foreach (string word in words)
						{
							if (word.StartsWith("<@!"))
							{
								string IDString = word.Remove(0, 3);
								IDString = IDString.Substring(0, IDString.Length - 1);
								ulong id = ulong.Parse(IDString);
								if (!replaceValues.ContainsKey(word))
								{									
									replaceValues.Add(word, "<Pinged " + Context.Channel.GetUserAsync(id).Result + ">");
								}								
							}
							else if (word.StartsWith("<@"))
							{
								string IDString = word.Remove(0, 2);
								IDString = IDString.Substring(0, IDString.Length - 1);
								ulong id = ulong.Parse(IDString);
								if (!replaceValues.ContainsKey(word))
								{
									replaceValues.Add(word, "<Pinged " + Context.Channel.GetUserAsync(id).Result + ">");
								}
							}
						}						
						foreach(string key in replaceValues.Keys)
						{
							content = content.Replace(key, replaceValues[key]);
						}

						MarioMakerMessages.Add($"{msg.Author.Username} - {msg.Timestamp.ToLocalTime().ToString("MM/dd/yyyy H:mm")}: {content}");
					}
				}
			}

			if(MarioMakerMessages.Count == 0)
			{
				await Context.Channel.SendMessageAsync("No IDs found.");
			}
			else
			{
				await Context.Channel.SendMessageAsync($"Got {MarioMakerMessages.Count} hit(s)!");
				string msg = "";
				foreach(string idMsg in MarioMakerMessages)
				{
					if (msg.Length + idMsg.Length > 2000)
					{
						await Context.Channel.SendMessageAsync(msg);
						msg = "";
					}
					msg += idMsg + Environment.NewLine;
				}
				if(msg.Length > 1)
				{
					await Context.Channel.SendMessageAsync(msg);
				}
			}

		}
	}
}
