using System.Threading.Tasks;
using Discord.Commands;
using DartsDiscordBots.Utilities;
using ColonelPanic.Utilities;
using ColonelPanic.Database.Contexts;
using ColonelPanic.Utilities.Permissions;
using System.Collections.Generic;
using Discord;
using System;
using System.IO;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

namespace ColonelPanic.Modules
{
    public class ServerModule : ModuleBase
    {
        public static Regex hexColorValidator = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");


        [Command("listchnl"), RequireOwner]
        public async Task listChannels()
        {
            string msg = "I'm in these guilds/channels:" + Environment.NewLine + Environment.NewLine;
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                msg += "**" + guild.Name + "**" + Environment.NewLine;
                foreach (var chnl in guild.GetChannelsAsync().Result)
                {
                    if (msg.Length > 1900)
                    {
                        await Context.Channel.SendMessageAsync(msg);
                        msg = "";
                    }
                    msg += chnl.Name + Environment.NewLine;
                }
            }
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("playing"), RequireTrustedUser]
        public async Task setPlaying([Remainder]string playing)
        {
            var client = Context.Client as DiscordSocketClient;
            await client.SetGameAsync(playing);                
        }

        [Command("rolecolor"), Summary("Creates a role with your name with the specified (in hex) color. Ex) `rolecolor #000000`"), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task RoleColorChange([Remainder, Summary("The hexcode for your desired color.")] string hexText)
        {
            if (hexColorValidator.Match(hexText).Success && (hexText.Length == 7 || hexText.Length == 4)) {
                var role = ModifyUserRoleColor(hexText, this.Context.User).Result;
                await (this.Context.User as IGuildUser).AddRoleAsync(role);
            }
            else
            {
                await this.Context.Channel.SendMessageAsync("Sorry, I can't recognize that hexcode. Maybe I'm an idiot, iunno.");
            }
        }

		[Command("say"), Summary("Echos a message."), RequireOwner]
		public async Task Say([Remainder, Summary("The text to echo")] string echo)
		{
			// ReplyAsync is a method on ModuleBase
			await ReplyAsync(echo);
			Context.Message.DeleteAsync();
		}

        [Command("pick"), Summary("Picks a random value from a comma separated list")]
        public async Task Pick([Remainder, Summary("The comma separated list of items to pick from")] string items)
		{
            List<string> choices = items.Split(',').ToList();
            await Context.Channel.SendMessageAsync(choices.GetRandom());
		}


        [Command("link"), Alias("install"), Summary("Provides a link for installing the bot on other servers. You must be an admin of the target server to use the provided link.")]
        public async Task ProvideInstallLink()
        {
            Context.Channel.SendMessageAsync("https://discordapp.com/oauth2/authorize?&client_id=357910708316274688&scope=bot");
        }

        [Command("roll"), Summary("Roll XdY+/-Z dice.")]
        public async Task Roll([Remainder, Summary("What to roll. Can indicate the number of dice to roll, the number of sides on those dice, and a positive or negative modifier to add to the results. 3d6+2 would roll 3 6-sided dice and add 2 to the final result.")] string rollString)
        {

            var arguments = new List<string>(rollString.ToLower().Split('d'));
            arguments.Remove("");
            int sides, times, modifier;

            if (arguments.Count == 1)
            {
                if (arguments[0].Contains("+"))
                {
                    arguments = new List<string>(arguments[0].Split('+'));
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        var temp = dice.Roll();
                        if (int.TryParse(arguments[1], out modifier))
                        {
                            await Context.Channel.SendMessageAsync(
                                    string.Format("Rolled one d{0} plus {1} and got a total of {2}", sides,
                                        modifier, temp + modifier));

                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
                else if (arguments[0].Contains("-"))
                {
                    arguments = new List<string>(arguments[0].Split('-'));
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        var temp = dice.Roll();
                        if (int.TryParse(arguments[1], out modifier))
                        {
                            await Context.Channel.SendMessageAsync(
                                    string.Format("Rolled one d{0} minus {1} and got a total of {2}", sides,
                                        modifier, temp - modifier));
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
                else
                {
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        await Context.Channel.SendMessageAsync(string.Format("Rolled one d{0} and got a total of {1}",
                                sides, dice.Roll()));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
            }
            else if (arguments.Count == 2)
            {
                if (int.TryParse(arguments[0], out times))
                {
                    if (arguments[1].Contains("+"))
                    {
                        arguments = new List<string>(arguments[1].Split('+'));
                        if (int.TryParse(arguments[0], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            if (int.TryParse(arguments[1], out modifier))
                            {
                                await Context.Channel.SendMessageAsync(
                                        string.Format("Rolled {0} d{1} plus {2} and got a total of {3}",
                                            times, sides, modifier, temp.Total + modifier));
                                await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                        string.Join(",", temp.Rolls)));

                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else if (arguments[1].Contains("-"))
                    {
                        arguments = new List<string>(arguments[1].Split('-'));
                        if (int.TryParse(arguments[0], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            if (int.TryParse(arguments[1], out modifier))
                            {
                                await Context.Channel.SendMessageAsync(
                                        string.Format("Rolled {0} d{1} minus {2} and got a total of {3}", times, sides,
                                            modifier, temp.Total-modifier));
                                await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                        string.Join(",", temp.Rolls)));

                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        if (int.TryParse(arguments[1], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            await Context.Channel.SendMessageAsync(string.Format(
                                    "Rolled {0} d{1} and got a total of {2}", times, sides, temp.Total));
                            await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                    string.Join(",", temp.Rolls)));

                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                }
            }
        }

        [Command("trust"), RequireOwner, Summary("Adds the user's ID to the trusted user list. Trusted users will be able to execute commands regardless of permissions.")]
        public async Task TrustUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.AddTrustedUser(userId, Context.Guild.GetUserAsync(userUlong).Result.Username);
                    await Context.Channel.SendMessageAsync("You really trust that one? Well, _you're_ the boss. :thumbsup:");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }

        [Command("distrust"), RequireOwner, Summary("Removes the user's ID from the trusted user list.")]

        public async Task DistrustUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.RemoveTrustedUser(userId);
                    await Context.Channel.SendMessageAsync("Yeah fuck that guy!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }

        [Command("renick"), RequireOwner, Summary("Renames the bot")]
        public async Task ChangeNickname([Remainder, Summary("what to rename the bot")] string newNick)
        {
            await Context.Guild.GetCurrentUserAsync().Result.ModifyAsync(b => b.Nickname = newNick);
        }

        [Command("8ball"), RequireColPermission("speak"), Summary("Ask Colonel Panic a true or false question.")]
        public async Task Send8BallResponse([Remainder, Summary("The question!")] string question)
        {
            await Context.Channel.SendMessageAsync(ResponseCollections._8BallResponses.GetRandom());
        }

        [Command("naughty"), RequireTrustedUserOrPermission(ChannelPermission.ManageChannels), Summary("Sets the specified user (by id) as being _NAUGHTY_")]
        public async Task NaughtyUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.RemoveTrustedUser(userId);
                    await Context.Channel.SendMessageAsync("Oh yes, I see exactly what you mean. They're definitely _naughty_.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }
        [Command("notnaughty"), RequireTrustedUserOrPermission(ChannelPermission.ManageChannels), Summary("Sets the specified user (by id) as being _NAUGHTY_")]
        public async Task NotNaughtyUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {                
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    UserDataHandler.SetUserNaughtyState(userId,false);
                    await Context.Channel.SendMessageAsync("Ok, they're no longer on the naughty list.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }

        private async Task<IRole> ModifyUserRoleColor(string hexText, IUser user)
        {
            Color roleColor = ParseDiscordColor(hexText);
            IRole usernameRole;
            int topPosition = Context.Guild.Roles.Count - 2;
            if (RoleExists(Context.Guild.Roles, user.Username, out usernameRole))
            {
				try
				{
					await usernameRole.ModifyAsync(x =>
					{
						x.Color = roleColor;
					});
				}
				catch
				{
					await Context.User.SendMessageAsync("Sorry, something went wrong when applying the role color.");
				}
				try
				{
					await usernameRole.ModifyAsync(x =>
					{						
						x.Position = topPosition;
					});
				}
				catch
				{
					await Context.User.SendMessageAsync("Sorry, I created the role but I don't have permission to move it's position, so your color might not change :shrug:");
				}
			}
            else
            {
                usernameRole = Context.Guild.CreateRoleAsync(user.Username, null, roleColor, false, null).Result;
                await usernameRole.ModifyAsync(x => x.Position = topPosition);
            }
            return usernameRole;
        }

        private Color ParseDiscordColor(string hexText)
        {
            string Hex = hexText.Substring(1);
            string B;
            string G;
            string R;
            if (hexText.Length == 7)
            {
                R = Hex.Substring(0, 2);
                G = Hex.Substring(2, 2);
                B = Hex.Substring(4, 2);
            }
            else
            {
                R = Hex.Substring(0, 1) + Hex.Substring(0, 1);
                G = Hex.Substring(1, 1) + Hex.Substring(1, 1);
                B = Hex.Substring(2, 1) + Hex.Substring(2, 1);
            }

            return new Color(int.Parse(R, System.Globalization.NumberStyles.HexNumber), int.Parse(G, System.Globalization.NumberStyles.HexNumber), int.Parse(B, System.Globalization.NumberStyles.HexNumber));
        }

        private static bool RoleExists(IReadOnlyCollection<IRole> roles, string username, out IRole usernameRole)
        {
            foreach (IRole role in roles)
            {
                if (role.Name == username)
                {
                    usernameRole = role;
                    return true;
                }
            }
            usernameRole = null;
            return false;
        }
    }

    public class UserXGuild
    {
        public IUser User;
        public IGuild Guild;

        public UserXGuild(IUser user, IGuild guild)
        {
            User = user;
            Guild = guild;
        }
    }
}
