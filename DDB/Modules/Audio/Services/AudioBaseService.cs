using DartsDiscordBots.Interfaces;
using Discord;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Services
{
	public class AudioBaseService : IAudioBaseService
	{
		public ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels { get; set; }

		public Process CreateStream(string path)
		{
			return Process.Start(new ProcessStartInfo
			{
				FileName = "ffmpeg.exe",
				Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
		}

		public async Task JoinAudio(IGuild guild, IVoiceChannel target)
		{
			IAudioClient client;
			if (ConnectedChannels.TryGetValue(guild.Id, out client))
			{
				return;
			}
			if (target.Guild.Id != guild.Id)
			{
				return;
			}

			var audioClient = await target.ConnectAsync();

			if (ConnectedChannels.TryAdd(guild.Id, audioClient))
			{
				// If you add a method to log happenings from this service,
				// you can uncomment these commented lines to make use of that.
				//await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
			}
		}

		public async Task LeaveAudio(IGuild guild)
		{
			IAudioClient client;
			if (ConnectedChannels.TryRemove(guild.Id, out client))
			{
				await client.StopAsync();
				//await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
			}
		}
	}
}
