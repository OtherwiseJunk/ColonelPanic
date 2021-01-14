using System.Linq;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DartsDiscordBots.Modules.Help.Interfaces;

namespace DartsDiscordBots.Modules.Help
{
    public class HelpModule : ModuleBase
    {
        private readonly CommandService _service;
		private readonly IHelpConfig _config;
        

        public HelpModule(CommandService service, IHelpConfig config)
        {
            _service = service;
			_config = config;
        }

        [Command("help"),Summary("Display a list of all commands")]
        public async Task HelpAsync()
        {
			string prefix = _config.Prefix;

			var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help"), Summary("Display help information for the provided command.")]
        public async Task HelpAsync([Summary("The command you would like more information on.")]string command)
        {
            var result = _service.Search(Context, command);
            

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

			string prefix = _config.Prefix;

			var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
