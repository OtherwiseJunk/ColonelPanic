using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace ColonelPanic.Modules
{
    public class AudioModule : ModuleBase
    {
        // Scroll down further for the AudioService.
        // Like, way down
        private readonly AudioService _service;
        

        // Remember to add an instance of the AudioService
        // to your IServiceCollection when you initialize your bot
        public AudioModule(AudioService service)
        {
            _service = service;
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
            else{
                await _service.SendAudioAsync(Context.Guild, Context.Channel, song + ".mp3");
            }
            
        }

        [Command("tidalplay")]
        public async Task PlayTidalSnippet([Remainder]string snippet)
        {
            if (snippet.ToLower().Contains("```haskell"))
            {
                snippet = snippet.Remove(0,10);
                snippet = snippet.Remove(snippet.Length - 3, 3);
                snippet = snippet.Replace("/n", "");
            }            
            _service.SendUdp(9999, "138.197.42.213", 9999, Encoding.ASCII.GetBytes(snippet));
        }

        [Command("tidalradio", RunMode = RunMode.Async)]
        public async Task PlayTidalRadio()
        {
            await _service.StartTidalStream(Context.Guild, Context.Channel);
        }


    }
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

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
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {                
                using (var output = CreateStream("http://138.197.42.213:8090/stream.mp3").StandardOutput.BaseStream)
                {
                    using (var stream = client.CreatePCMStream(AudioApplication.Music))
                    {
                        try { await output.CopyToAsync(stream); }
                        finally { await stream.FlushAsync(); }
                    }
                }                
            }
        }
        public void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
        {
            using (UdpClient c = new UdpClient(srcPort)) c.Send(data, data.Length, dstIp, dstPort);
        }

        public static void SendUdpStatic(int srcPort, string dstIp, int dstPort, byte[] data)
        {
            using (UdpClient c = new UdpClient(srcPort)) c.Send(data, data.Length, dstIp, dstPort);
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
