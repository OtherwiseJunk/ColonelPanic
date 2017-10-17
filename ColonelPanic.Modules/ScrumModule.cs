using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Permissions;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
    [Group("scrum"), RequireContext(ContextType.Guild), RequireColPermission("scrum"), RequireChannelConfiguration("scrum"), Summary("A module which provides weekly reminders to users who haven't submitted an update.")]
    public class ScrumModule : ModuleBase
    {                
        [Command("adduser"), RequireColPermission("scrum"),Summary("Adds the specified user to the scrum users list"), RequireUserPermission(Discord.ChannelPermission.ManageChannel)]
        public async Task AddScrumUser([Remainder, Summary("The user ID for the scrummer to add.")] string userId)
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

        [Command("listupdates"), Summary("Shows a list of updates for the specified user.")]
        public async Task GetUpdateList([Remainder, Summary("The username to get the updates for.")]string username)
        {
            if (username != "*" && ScrumHandler.UserExists(Context.Channel.Id.ToString(), username))
            {
                string msgHeader = $"**{username}'s Updates:**" + Environment.NewLine;
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync(msgHeader+ScrumHandler.BuildUserUpdateList(Context.Channel.Id.ToString(),username));
            }
            else
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync(ScrumHandler.BuildAllUpdateLists(Context.Channel.Id.ToString(),Context.Channel.Name));
            }
        }

        
    }
}
