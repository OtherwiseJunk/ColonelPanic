using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
    [Group("enable"), Summary(" All \"enable\" commands require \"Manage Channel\" permissions. These commands enable the specified permission for the channel.")]
    public class EnableModule : ModuleBase
    {
        [Command("scrum"), Summary("Enables SCRUM reminders for this channel."), RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task EnableScrum([Remainder, Summary("The datetime to start the weekly reminders.")] string Datetime)
        {
            DateTime dateTime;
            if (DateTime.TryParse(Datetime, out dateTime))
            {
                if (!ScrumHandler.ScrumChannelExists(Context.Channel.Id.ToString()))
                {

                    ScrumHandler.AddNewChannel(Context.Channel.Id, dateTime);
                    if (ConfigurationHandler.ChannelStateExists(Context.Channel.Id.ToString()))
                    {
                        ConfigurationHandler.ChangePermission("chnl", "scrum", Context.Channel.Id.ToString(), true);
                    }
                    else
                    {
                        ConfigurationHandler.AddChannelState(new ChannelState { ChannelID = Context.Channel.Id.ToString(), ChannelName = Context.Channel.Name, ScrumEnabled = true, CanListen = false, CanSpeak = false });
                        ConfigurationHandler.ChangePermission("chnl", "scrum", Context.Channel.Id.ToString(), true);
                    }
                    await Context.Channel.SendMessageAsync("Ok, I've enabled Scrum for this channel.");

                }
                else
                {
                    ScrumHandler.UpdateScrumDatetime(Context.Channel.Id.ToString(), dateTime);
                    ConfigurationHandler.ChangePermission("chnl", "scrum", Context.Channel.Id.ToString(), true);
                    await ReplyAsync("Ok, I've updated the next scheduled reminder...");
                }
            }
            else
            {
                await ReplyAsync("Sorry, I don't recognize that datetime...");
            }
        }

        [Command("speak"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task EnableSpeak()
        {
            ConfigurationHandler.ChangePermission("chnl", "speak", Context.Channel.Id.ToString(), true);
            await Context.Channel.SendMessageAsync("Ok, I've enabled the speak permission!");
        }
        [Command("listen"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task EnableListen()
        {
            ConfigurationHandler.ChangePermission("chnl", "listen", Context.Channel.Id.ToString(), true);
            await Context.Channel.SendMessageAsync("Ok, I've enabled the listen permission!");
        }
    }
    [Group("disable"), Summary(" All \"disable\" commands require \"Manage Channel\" permissions. These commands disable the specified permission for the channel.")]
    public class DisableModule : ModuleBase
    {
        [Command("scrum"), Summary("Disables SCRUM reminders for this channel."), RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task DisableScrum()
        {
            ConfigurationHandler.ChangePermission("chnl", "scrum", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the scrum permission!");
        }

        [Command("speak"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task EnableSpeak()
        {
            ConfigurationHandler.ChangePermission("chnl", "speak", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the speak permission!");
        }
        [Command("listen"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task EnableListen()
        {
            ConfigurationHandler.ChangePermission("chnl", "listen", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the listen permission!");
        }
    }
}
