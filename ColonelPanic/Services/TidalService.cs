using DartsDiscordBots.Interfaces;
using DartsDiscordBots.Services;
using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ColonelPanic.Services
{
	class TidalService : AudioBaseService, ITidalService
	{
		public string TidalServerAddress { get; set; } = "138.197.42.213";
		public int TidalInputPort { get; set; } = 9999;
		public int TidalStreamPort { get; set; } = 8081;

		public TidalService()
		{
			ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
		}

		public async Task StartTidalStream(IGuild guild, IMessageChannel channel)
		{
			if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
			{
				using (var output = CreateStream($"http://{TidalServerAddress}:{TidalStreamPort}/stream.mp3").StandardOutput.BaseStream)
				{
					using (var stream = client.CreatePCMStream(AudioApplication.Music))
					{
						try { await output.CopyToAsync(stream); }
						finally { await stream.FlushAsync(); }
					}
				}
			}
		}

		public async Task TransmitPatternToTidal(byte[] data)
		{
			using (UdpClient c = new UdpClient(TidalInputPort))
			{
				await new Task(() => c.Send(data, data.Length, TidalServerAddress, TidalInputPort));
			}
		}
	}
}
