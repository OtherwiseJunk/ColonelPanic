using Discord;
using System.Threading.Tasks;

namespace DartsDiscordBots.Interfaces
{
	public interface IAudioService : IAudioBaseService
	{
		Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path);
		Task SendAudioStreamAsync(IGuild guild, IMessageChannel channel, string path);

	}
}
