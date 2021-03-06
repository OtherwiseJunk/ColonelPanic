﻿using System;
using System.Collections.Generic;
using System.Linq;
using ColonelPanic.Database.Models;
using ColonelPanic.DatabaseCore.Constants;
using Microsoft.EntityFrameworkCore;

namespace ColonelPanic.Database.Contexts
{
    public class RedditContext : DbContext
    {
		public RedditContext(DbContextOptions<RedditContext> options) : base(options) { }

        
		public DbSet<TopDaily> TopDaily { get; set; }

    }

    public class RedditHandler
    {

		public static DbContextOptionsBuilder<RedditContext> OptionsBuilder { get; set; }
		public static object GetSubredditsLock = new object();
		static RedditHandler()
		{
			OptionsBuilder = new DbContextOptionsBuilder<RedditContext>();
			OptionsBuilder.UseSqlServer(ConnectionStrings.ConnectionString);
		}

		

        public static void AddTopDaily(string channelId, string subreddit, DateTime nextUpdateTime)
        {
            using(RedditContext db = new RedditContext(OptionsBuilder.Options))
            {
                var daily = new TopDaily(channelId, subreddit, nextUpdateTime);
                db.TopDaily.Add(daily);
                db.SaveChanges();
            }
        }

        public static List<TopDaily> GetAllTopDailies()
        {
            using(var db = new RedditContext(OptionsBuilder.Options))
            {
                return db.TopDaily.ToList();
            }
        }

        public static List<TopDaily> GetSubredditsToCheck()
        {
            Boolean EntryChanged = false;
            lock (GetSubredditsLock)
            {
                List<TopDaily> topDailies = new List<TopDaily>();
                using (var db = new RedditContext(OptionsBuilder.Options))
                {
                    var topDailiesFromDB = db.TopDaily.ToList();
                    foreach (TopDaily tdaily in topDailiesFromDB)
                    {
                        if ((DateTime.Now > tdaily.NextTimeToPost) && DateTime.Now.AddMinutes(-1) < tdaily.NextTimeToPost)
                        {
                            topDailies.Add(tdaily);
                            tdaily.NextTimeToPost = tdaily.NextTimeToPost.AddDays(1);
                            db.TopDaily.Attach(tdaily);
                            db.Entry(tdaily).State = EntityState.Modified;
                            EntryChanged = true;
                        }
                        else
                        {
                            while (tdaily.NextTimeToPost < DateTime.Now)
                            {
                                tdaily.NextTimeToPost = tdaily.NextTimeToPost.AddDays(1);
                                EntryChanged = true;
                            }
                            if (EntryChanged == true){
                                db.TopDaily.Attach(tdaily);
                                db.Entry(tdaily).State = EntityState.Modified;
                            }                            
                        }

                    }
                    if (EntryChanged == true)
                    {
                        db.SaveChanges();
                    }                    
                }
                return topDailies;
            }            
        }

        public static string GetTopDailies(string channelId)
        {
            string msg = @"Top Dailies
==========="+Environment.NewLine;
            Console.WriteLine(msg.Length);
            using (RedditContext db = new RedditContext(OptionsBuilder.Options))
            {
                foreach (TopDaily td in db.TopDaily.AsQueryable().Where(td => td.ChannelId == channelId))
                {
                    msg += $"{td.TopDailyNum}. {td.Subreddit} : {td.NextTimeToPost}" + Environment.NewLine;
                }
            }
            if (msg.Length == 23) return "Sorry, there are no \"Top Dailies\" configured for this channel.";
            else return msg;
        }

        public static string DeleteTopDaily(int topDailyId, string channelId)
        {
            using (RedditContext db = new RedditContext(OptionsBuilder.Options))
            {
                if (db.TopDaily.FirstOrDefault(td => td.ChannelId == channelId && td.TopDailyNum == topDailyId) != null)
                {
                    TopDaily topDaily = db.TopDaily.First(td => td.ChannelId == channelId && td.TopDailyNum == topDailyId);
                    db.TopDaily.Attach(topDaily);
                    db.TopDaily.Remove(topDaily);
                    db.SaveChanges();
                    return "Top Daily successfully deleted!";
                }
                else return "Sorry, I don't see that Top Daily in this channel...";
            }
        }
    }
}
