using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using DartsDiscordBots.Interfaces;

namespace DartsDiscordBots.Modules
{
	public class AudioModule : ModuleBase
	{
		// Scroll down further for the AudioService.
		// Like, way down
		private readonly IAudioService _service;
		private readonly ITidalService _tidal;


		// Remember to add an instance of the AudioService
		// to your IServiceCollection when you initialize your bot
		public AudioModule(IAudioService service, ITidalService tidal)
		{
			_service = service;
			_tidal = tidal;
		}

		// You *MUST* mark these commands with 'RunMode.Async'
		// otherwise the bot will not respond until the Task times out.
		[Command("join", RunMode = RunMode.Async)]
		public async Task JoinCmd()
		{
			await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
		}

		// Remember to add preconditions to your commands,
		// this is merely the minimal amount necessary.
		// Adding more commands of your own is also encouraged.
		[Command("leave", RunMode = RunMode.Async)]
		public async Task LeaveCmd()
		{
			await _service.LeaveAudio(Context.Guild);
		}

		[Command("play", RunMode = RunMode.Async)]
		public async Task PlayCmd([Remainder] string song)
		{
			if (song.ToLower().StartsWith("http"))
			{
				await _service.SendAudioStreamAsync(Context.Guild, Context.Channel, song + ".mp3");
			}
			else
			{
				await _service.SendAudioAsync(Context.Guild, Context.Channel, song + ".mp3");
			}

		}

		[Command("tidalplay")]
		public async Task PlayTidalSnippet([Remainder]string snippet)
		{
			if (snippet.ToLower().Contains("```haskell"))
			{
				//removes the 10 characters for ```haskell as well as the newline
				snippet = snippet.Remove(0, 11);
				//removes the final three ``` as well as the new line.
				snippet = snippet.Remove(snippet.Length - 4, 4);
			}
			await _tidal.TransmitPatternToTidal(Encoding.ASCII.GetBytes(snippet));
		}

		[Command("tidalradio", RunMode = RunMode.Async)]
		public async Task PlayTidalRadio()
		{
			await _tidal.StartTidalStream(Context.Guild, Context.Channel);
		}
	}
}
