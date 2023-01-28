using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.WebSocket;
using System;
using CEC = ColonelPanic.Constants.CustomEmoteConstants;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ColonelPanic.Handlers
{
    internal class OnMessage
    {
        public static async Task AutoEmojiReactCheck(SocketMessage arg)
        {
            Random _rand = new Random(Guid.NewGuid().GetHashCode());
            if (_rand.Next(1000) == 777)
            {
                var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
                await msg.AddReactionAsync(new Emoji("💩"));
            }
            if (_rand.Next(1000) == 777)
            {
                var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
                await msg.AddReactionAsync(new Emoji("👍"));
            }
            if (_rand.Next(1000) == 777)
            {
                var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
                //if Geo
                if (arg.Author.Id == 140620251976040449)
                {
                    await msg.AddReactionAsync(new Emoji("🍠"));
                }
                //Everyone else
                else
                {
                    await msg.AddReactionAsync(new Emoji("🍆"));
                }

            }
            if (arg.Content.ToLower().Contains("pogger") && !arg.Content.Contains(" "))
            {
                await arg.AddReactionAsync(Emote.Parse(CEC.PoggersEmoteCode));
            }
        }
        public static async Task TableFlipCheck(SocketMessage arg)
        {
            if (Regex.IsMatch(arg.Content, @"[)ʔ）][╯ノ┛].+┻━┻"))
            {
                await arg.Channel.SendMessageAsync("┬─┬  ノ( º _ ºノ) ");
                await arg.Channel.SendMessageAsync(GetTableFlipResponse(arg.Author));
            }
            else if (arg.Content == "(ノಠ益ಠ)ノ彡┻━┻")
            {
                await arg.Channel.SendMessageAsync("┬─┬  ノ(ಠ益ಠノ)");
                await arg.Channel.SendMessageAsync(GetTableFlipResponse(arg.Author));
            }
            else if (arg.Content == "┻━┻ ︵ヽ(`Д´)ﾉ︵ ┻━┻")
            {
                await arg.Channel.SendMessageAsync("┬─┬  ノ(`Д´ノ)");
                await arg.Channel.SendMessageAsync("(/¯`Д´ )/¯ ┬─┬");
                await arg.Channel.SendMessageAsync(GetTableFlipResponse(arg.Author));
            }
        }

        public static async Task BroFistCheck(SocketMessage arg, bool mentioningMe)
        {
            if (mentioningMe)
            {
                if (arg.Content.Contains("🤛"))
                {
                    await arg.Channel.SendMessageAsync(":right_facing_fist:");

                }
                else if (arg.Content.Contains("🤜"))
                {
                    await arg.Channel.SendMessageAsync(":left_facing_fist:");
                }
            }
        }

        private static string GetTableFlipResponse(SocketUser author)
        {
            int points = UserDataHandler.IncrementTableFlipPoints(author.Id.ToString(), author.Username);
            if (points >= 81) return String.Format(ResponseCollections.TableFlipResponses[4].GetRandom(), author.Username);
            if (points >= 61) return String.Format(ResponseCollections.TableFlipResponses[3].GetRandom(), author.Username);
            if (points >= 41) return String.Format(ResponseCollections.TableFlipResponses[2].GetRandom(), author.Username);
            if (points >= 21) return String.Format(ResponseCollections.TableFlipResponses[1].GetRandom(), author.Username);
            return String.Format(ResponseCollections.TableFlipResponses[0].GetRandom(), author.Username);
        }
    }
}
