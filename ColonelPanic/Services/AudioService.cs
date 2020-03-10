using DartsDiscordBots.Services;
using DartsDiscordBots.Interfaces;
using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

using System.Threading.Tasks;

namespace ColonelPanic.Services
{
	public class AudioService : AudioBaseService, IAudioService
	{
		public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
		{
			// Your task: Get a full path to the file if the value of 'path' is only a filename.
			if (!File.Exists(path))
			{
				await channel.SendMessageAsync("File does not exist.");
				return;
			}
			IAudioClient client;
			if (ConnectedChannels.TryGetValue(guild.Id, out client))
			{
				//await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");

				using (var output = CreateStream(path).StandardOutput.BaseStream)
				using (var stream = client.CreatePCMStream(AudioApplication.Music))
				{
					try { await output.CopyToAsync(stream); }
					finally { await stream.FlushAsync(); }
				}
			}
		}

		public async Task StartTidalStream(IGuild guild, IMessageChannel channel)
		{
			
		}
		public void TransmitPatternToTidal(byte[] data)
		{
			
		}

		public async Task SendAudioStreamAsync(IGuild guild, IMessageChannel channel, string path)
		{
			IAudioClient client;
			if (ConnectedChannels.TryGetValue(guild.Id, out client))
			{
				//await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");

				using (var output = CreateStream(path).StandardOutput.BaseStream)
				using (var stream = client.CreatePCMStream(AudioApplication.Music))
				{
					try { await output.CopyToAsync(stream); }
					finally { await stream.FlushAsync(); }
				}
			}
		}

		private Process CreateStream(string path)
		{
			return Process.Start(new ProcessStartInfo
			{
				FileName = "ffmpeg.exe",
				Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
		}
	}
}
