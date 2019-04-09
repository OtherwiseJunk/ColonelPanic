using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonelPanic.Utilities.JSONClasses;
using System.Net;
using System.Web;
using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Utilities;
using ColonelPanic.Utilities.Permissions;
using Discord;

namespace ColonelPanic.Modules
{
    
    [Group("topdaily")]
    public class RedditModule : ModuleBase
    {
        [Command("rand")]
        public async Task GetRandomImage([Remainder] string Subreddit) 
        {
            using (var client = new WebClient())
            {
                string url = String.Format(Utilities.APILinkFormats.SubredditTopOneHundredPosts, Subreddit);
                string topTwentyJson = HttpUtility.HtmlDecode(client.DownloadString(url));
                List<string> imageUrls = new List<string>();
                RedditTopTwenty topTwenty = Newtonsoft.Json.JsonConvert.DeserializeObject<RedditTopTwenty>(topTwentyJson);
                if (topTwenty.data.children.Count() > 0)
                {
                    
                    foreach (var child in topTwenty.data.children)
                    {
                        if ((child.data.url.Contains(".gif") || child.data.url.Contains(".jpg") || child.data.url.Contains(".png") || child.data.url.Contains(".mp4") || child.data.url.Contains(".gifv")))
                        {
                            imageUrls.Add(child.data.url);
                        }
                    }
                    if (imageUrls.Count > 0)
                    {
                        await Context.Channel.SendMessageAsync(imageUrls.GetRandom());
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Well that's not a subreddit with posts, yanno?");
                }
            }
        }

        [Command("list")]
        public async Task ListTopDailies()
        {
            await Context.Channel.SendMessageAsync(RedditHandler.GetTopDailies(Context.Channel.Id.ToString()));
        }

        [Command("add"), RequireTrustedUserOrPermission(Discord.GuildPermission.ManageChannels), Summary(@"Will schedule a daily post at the specified time, taking the top image post from the provided subreddit.

Usage: $top ""birbs"" ""17:00"" 
This command would cause the top image post from birbs to be posted at 5:00PM every day!

All posts will occur at that time in ET, sorry lesser timezones :-)")]
        public async Task AddTopDaily([Summary("Subreddit to post")]string Subreddit, [Summary("Time to post daily")]string Time)
        {
            using (var client = new WebClient())
            {
                string url = String.Format(Utilities.APILinkFormats.SubredditTopOneHundredPosts, Subreddit);
                string topTwentyJson = HttpUtility.HtmlDecode(client.DownloadString(url));
                RedditTopTwenty topTwenty = Newtonsoft.Json.JsonConvert.DeserializeObject<RedditTopTwenty>(topTwentyJson);
                if (topTwenty.data.children.Count() > 0)
                {
                    var timeArgs = Time.Split(':');
                    int hour;
                    int minute;
                    if (Int32.TryParse(timeArgs[0], out hour) && hour >= 0 && hour < 24)
                    {
                        if (Int32.TryParse(timeArgs[1], out minute) && minute >= 0 && minute < 60)
                        {
                            DateTime nextPostDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
                            if (nextPostDate < DateTime.Now)
                            {
                                nextPostDate.AddDays(1);
                            }
                            RedditHandler.AddTopDaily(Context.Channel.Id.ToString(), Subreddit, nextPostDate);
                            await Context.Channel.SendMessageAsync("You got it, my dude.");
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Iunno, that minute sure don't look right to me...");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Iunno, that hour looks funny to me... ");
                    }

                }
                else
                {
                    await Context.Channel.SendMessageAsync("Well that's not a subreddit with posts, yanno?");
                }
            }
        }

        [Command("delete"), RequireTrustedUserOrPermission(Discord.GuildPermission.ManageChannels), Summary("Deletes the specified Top Daily (by id, obtained from $topdaily list.")]
        public async Task DeleteTopDaily([Summary("Top Daily to delete. ID number taken from $topdaily list."),Remainder]int topDailyId)
        {
            await Context.Channel.SendMessageAsync(RedditHandler.DeleteTopDaily(topDailyId, Context.Channel.Id.ToString()));
        }


        [Command("now"), Summary("Posts the top image from the provided subreddit.")]
        public async Task GetTopNow([Summary("Subreddit to get top image from.")] string Subreddit)
        {
            using (var client = new WebClient())
            {
                string url = String.Format(Utilities.APILinkFormats.SubredditTopOneHundredPosts, Subreddit);
                string topTwentyJson = HttpUtility.HtmlDecode(client.DownloadString(url));
                RedditTopTwenty topTwenty = Newtonsoft.Json.JsonConvert.DeserializeObject<RedditTopTwenty>(topTwentyJson);
                if (topTwenty.data.children.Count() > 0)
                {
                    int count = 0;
                    foreach (var child in topTwenty.data.children)
                    {
                        if (count == 0 && (child.data.url.Contains(".mp4") || child.data.url.Contains(".gifv")))
                        {
                            count++;
                            await Context.Channel.SendMessageAsync($"{child.data.subreddit}'s top image (Title: {child.data.title}):");
                            await Context.Channel.SendMessageAsync(child.data.url);
                        }
                        else if (count == 0 && (child.data.url.Contains(".gif") || child.data.url.Contains(".jpg") || child.data.url.Contains(".png")))
                        {
                            count++;                            
                            await Context.Channel.SendMessageAsync("", false, buildEmbedForImage(child));
                        }

                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Well that's not a subreddit with posts, yanno?");
                }
            }
        }

        public static Embed buildEmbedForImage(Child redditPost)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle(redditPost.data.title);
            builder.AddField("Subreddit | Score", redditPost.data.subreddit + " | " + redditPost.data.score);
            builder.WithImageUrl(redditPost.data.url);            
            builder.WithColor(Color.DarkPurple);
            return builder.Build();
        }


    }


}
