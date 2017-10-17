﻿using ColonelPanic.Database.Contexts;
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
                    if (ConfigurationHandler.GuildStateExists(Context.Channel.Id.ToString()))
                    {
                        ConfigurationHandler.ChangePermission("guild", "scrum", Context.Channel.Id.ToString(), true);
                    }
                    else
                    {
                        ConfigurationHandler.AddGuildState(new GuildState { GuildId = Context.Channel.Id.ToString(), GuildName = Context.Channel.Name, ScrumEnabled = true, CanListen = false, CanSpeak = false });
                        ConfigurationHandler.ChangePermission("guild", "scrum", Context.Channel.Id.ToString(), true);
                    }
                    await Context.Channel.SendMessageAsync("Ok, I've enabled Scrum for this channel.");

                }
                else
                {
                    ScrumHandler.UpdateScrumDatetime(Context.Channel.Id.ToString(), dateTime);
                    ConfigurationHandler.ChangePermission("guild", "scrum", Context.Channel.Id.ToString(), true);
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
            ConfigurationHandler.ChangePermission("guild", "speak", Context.Channel.Id.ToString(), true);
            await Context.Channel.SendMessageAsync("Ok, I've enabled the speak permission!");
        }
        [Command("listen"), Summary("Allows the user of commands requiring the \"Can Listen\" permissions.")]
        public async Task EnableListen()
        {
            ConfigurationHandler.ChangePermission("guild", "listen", Context.Channel.Id.ToString(), true);
            await Context.Channel.SendMessageAsync("Ok, I've enabled the listen permission!");
        }
        [Command("note"), Summary("Allows the user of commands requiring the \"Note\" permissions.")]
        public async Task EnableNote()
        {
            ConfigurationHandler.ChangePermission("guild", "note", Context.Channel.Id.ToString(), true);
            await Context.Channel.SendMessageAsync("Ok, I've enabled the listen permission!");
        }
    }
    [Group("disable"), Summary(" All \"disable\" commands require \"Manage Channel\" permissions. These commands disable the specified permission for the channel.")]
    public class DisableModule : ModuleBase
    {
        [Command("scrum"), Summary("Disables SCRUM reminders for this channel."), RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task DisableScrum()
        {
            ConfigurationHandler.ChangePermission("guild", "scrum", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the scrum permission!");
        }

        [Command("speak"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task DisableSpeak()
        {
            ConfigurationHandler.ChangePermission("guild", "speak", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the speak permission!");
        }
        [Command("listen"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task DisableListen()
        {
            ConfigurationHandler.ChangePermission("guild", "listen", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the listen permission!");
        }
        [Command("note"), Summary("Allows the user of commands requiring the \"Can Speak\" permissions.")]
        public async Task DisableNote()
        {
            ConfigurationHandler.ChangePermission("guild", "note", Context.Channel.Id.ToString(), false);
            await Context.Channel.SendMessageAsync("Ok, I've disabled the note permission!");
        }
    }
}
