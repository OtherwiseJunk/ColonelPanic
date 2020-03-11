using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DartsDiscordBots.Interfaces
{
	public interface ITidalService : IAudioBaseService
	{
		string TidalServerAddress { get; set; }
		int TidalInputPort { get; set; }
		int TidalStreamPort { get; set; }
		Task StartTidalStream(IGuild guild, IMessageChannel channel);
		Task TransmitPatternToTidal(byte[] data);
	}
}
