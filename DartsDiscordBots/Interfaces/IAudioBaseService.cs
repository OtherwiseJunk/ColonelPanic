using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using System.Threading.Tasks;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DartsDiscordBots.Interfaces
{
	public interface IAudioBaseService
	{
		ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels { get; set; }
		Task JoinAudio(IGuild guild, IVoiceChannel target);
		Task LeaveAudio(IGuild guild);
		Process CreateStream(string path);
	}
}
