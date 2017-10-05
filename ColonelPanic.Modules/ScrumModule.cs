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
    [Group("scrum"), RequireContext(ContextType.Guild), Summary("A module which provides weekly reminders to users who haven't submitted an update.")]
    public class ScrumModule : ModuleBase
    {
        [Command("enable"), Summary("Enables SCRUM reminders for this channel. Requires \"Manage Channel\" permissions."), RequireOwner]
        public async Task Enable([Remainder, Summary("The datetime to start the weekly reminders.")] string Datetime)
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

        [Command("adduser"), Summary("Adds the specified user to the scrum users list"), RequireUserPermission(Discord.ChannelPermission.ManageChannel)]
        public async Task AddScrumUser([Remainder, Summary("The user ID for the scrummer to add.")] string userId)
        {
            if (PermissionHandler.PermissionEnabled("scrum", Context.Channel.Id.ToString()))
            {
                ulong usrId;
                if (ulong.TryParse(userId,out usrId))
                {
                    if(await Context.Channel.GetUserAsync(usrId) != null)
                    {
                        ScrumHandler.AddScrummer(Context.Channel.Id, usrId, Context.Channel.GetUserAsync(usrId).Result.Username);
                        await ReplyAsync("Ok, I've added that user.");
                    }
                    else
                    {
                        await ReplyAsync("Sorry, I don't recognize that user.");
                    }
                }
                else
                {
                    await ReplyAsync("Sorry, I didn't recognize that as a valid entry. Please verify only numeric characters are used.");
                }
                
            }
            else
            {
                await ReplyAsync("Unfortunately, that module is not enabled.");
            }
        }



        [Command("scrummers"), Alias("scrumers"), Summary("Returns a list of all registered user's Username, last update date, and update count.")]
        public async Task GetScrummers()
        {
            await ReplyAsync(ScrumHandler.GetScrummers(Context.Channel.Id));
        }

        [Command("update"), Summary("Submit an update.")]
        public async Task SubmitUpdate([Remainder, Summary("The update text.")]string updateMsg)
        {
            if (ScrumHandler.UserIsRegistered(Context.Channel.Id.ToString(), Context.User.Id.ToString()))
            {
                ScrumHandler.AddNewUpdate(updateMsg, Context.User.Username, Context.User.Id, Context.Channel.Id);
                await ReplyAsync("Ok, logged that update.");
            }
            else
            {
                await ReplyAsync("Sorry, something's not right here. Either Scrum isn't enabled for this channel, or you're not registered as a user in this channel.");
            }
            
        }

        
    }
}
