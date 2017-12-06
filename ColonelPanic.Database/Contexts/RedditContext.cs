using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonelPanic.Utilities.JSONClasses;
using ColonelPanic.Database.Models;

namespace ColonelPanic.Database.Contexts
{
    public class RedditContext : DbContext
    {
        public RedditContext() : base("name=BetaDB"){ }

        public DbSet<TopDaily> TopDaily { get; set; }

    }

    public class RedditHandler
    {
        public static void AddTopDaily(string channelId, string subreddit, DateTime nextUpdateTime)
        {
            using(RedditContext db = new RedditContext())
            {
                var daily = new TopDaily(channelId, subreddit, nextUpdateTime);
                db.TopDaily.Add(daily);
                db.SaveChanges();
            }
        }

        public static List<TopDaily> GetSubredditsToCheck()
        {
            List<TopDaily> topDailies = new List<TopDaily>();
            using (var db = new RedditContext())
            {
                var topDailiesFromDB = db.TopDaily.ToList();
                foreach (TopDaily tdaily in topDailiesFromDB)
                {                    
                    if ((DateTime.Now > tdaily.NextTimeToPost) && DateTime.Now.AddMinutes(-1) < tdaily.NextTimeToPost)
                    {
                        topDailies.Add(tdaily);
                        tdaily.NextTimeToPost =  tdaily.NextTimeToPost.AddDays(1);
                        db.TopDaily.Attach(tdaily);
                        db.Entry(tdaily).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        while (!(DateTime.Now.AddMinutes(-1) < tdaily.NextTimeToPost))
                        {
                            tdaily.NextTimeToPost = tdaily.NextTimeToPost.AddDays(1);
                        }
                        db.TopDaily.Attach(tdaily);
                        db.Entry(tdaily).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            return topDailies;
        }
    }
}
