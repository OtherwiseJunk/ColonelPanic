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

namespace ColonelPanic.Modules
{
    public class RedditModule : ModuleBase
    {

        [Command("top"), RequireUserPermission(Discord.GuildPermission.) Summary(@"Will schedule a daily post at the specified time, taking the top image post from the provided subreddit.

Usage: $top ""birbs"" ""17:00"" 
This command would cause the top image post from birbs to be posted at 5:00PM every day!

All posts will occur at that time in ET, sorry lesser timezones :-)")]
        public async Task AddTopDaily([Summary("Subreddit to post")]string Subreddit, [Summary("Time to post daily")]string Time)
        {
            using (var client = new WebClient())
            {
                string url = String.Format(Utilities.APILinkFormats.SubredditTopTwentyPosts, Subreddit);
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
                            DateTime nextPostDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute,0);
                            if (nextPostDate < DateTime.Now)
                            {
                                nextPostDate.AddDays(1);
                            }
                            RedditHandler.AddTopDaily(Context.Channel.Id.ToString(), Subreddit,nextPostDate);
                        }
                        else
                        {
                            Context.Channel.SendMessageAsync("Iunno, that minute sure don't look right to me...");
                        }
                    }
                    else
                    {
                        Context.Channel.SendMessageAsync("Iunno, that hour looks funny to me... ");
                    }
                    
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Well that's not a subreddit with posts, yanno?");
                }
            }
        }
    }
    

}
