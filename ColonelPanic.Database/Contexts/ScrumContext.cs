using ColonelPanic.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Contexts
{
    public class ScrumHandler
    {
        public static void AddNewUpdate(string update, string updater, ulong userId, ulong channelId)
        {
            using (ScrumContext db = new ScrumContext())
            {
                db.Updates.Add(new ScrumUpdate(update, updater, channelId));
                string usrId = userId.ToString();
                string chnlId = channelId.ToString();
                ScrumUser usr = db.Users.FirstOrDefault(u => u.UserId == usrId && u.UserChannelId == chnlId);
                usr.UpdateCount++;
                usr.LastUpdate = DateTime.Now;
                db.Users.Attach(usr);
                db.Entry(usr).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static bool ScrumChannelExists(string channelId)
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.Channels.FirstOrDefault(cs => cs.ScrumChannelId == channelId) != null;
            }
        }

        public static void AddScrummer(ulong chnlId, ulong userId, string username)
        {
            using (ScrumContext db = new ScrumContext())
            {
                string usrId = userId.ToString();
                if (db.Users.FirstOrDefault(u => u.UserId == usrId) == null)
                {
                    db.Users.Add(new ScrumUser(chnlId.ToString(), usrId, username));
                    db.SaveChanges();
                }
            }
        }

        public static void AddNewChannel(ulong id, DateTime dateTime)
        {
            using (ScrumContext db = new ScrumContext())
            {
                db.Channels.Add(new ScrumChannel(id.ToString(), dateTime));
                db.SaveChanges();
            }
        }

        public static void UpdateScrumDatetime(string channelId, DateTime dateTime)
        {
            using (ScrumContext db = new ScrumContext())
            {
                ScrumChannel chnl = db.Channels.First(cs => cs.ScrumChannelId == channelId);
                chnl.ScrumReminderDateTime = dateTime;
                db.Channels.Attach(chnl);
                db.Entry(chnl).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static string GetScrummers(ulong chnlId)
        {
            string msg = "Here's who I'm tracking currently:" + Environment.NewLine+Environment.NewLine;

            using (ScrumContext db = new ScrumContext())
            {
                if (db.Users.Count() > 0)
                {
                    List<ScrumUser> users = db.Users.ToList();
                    foreach (ScrumUser usr in users)
                    {
                        msg += $"**Username**: {usr.Username+ Environment.NewLine} **Last Update:** {usr.LastUpdate.ToString("dddd, dd MMMM yyyy HH:mm:ss")+Environment.NewLine} **Update Count:** {usr.UpdateCount + Environment.NewLine}" + Environment.NewLine+Environment.NewLine;
                    }
                    msg = msg.Substring(0, msg.Length - 2);
                }
                else
                {
                    msg = "There are not any users being tracked yet.";
                }

            }
            return msg;
        }

        public static List<ScrumUser> GetUsersToHarass()
        {
            List<ScrumUser> users = new List<ScrumUser>();
            using (ScrumContext db = new ScrumContext())
            {
                List<ScrumChannel> channels = db.Channels.ToList();
                foreach (ScrumChannel chnl in channels)
                {
                    if ((DateTime.Now > chnl.ScrumReminderDateTime) && DateTime.Now.AddHours(-1) < chnl.ScrumReminderDateTime)
                    {
                        List<ScrumUser> scrumUsers = db.Users.ToList();
                        foreach (ScrumUser usr in scrumUsers)
                        {
                            if (usr.UserChannelId == chnl.ScrumChannelId)
                            {
                                if (usr.LastUpdate < chnl.ScrumReminderDateTime.AddDays(-6))
                                {
                                    users.Add(usr);
                                }
                            }                            
                        }
                        chnl.ScrumReminderDateTime = chnl.ScrumReminderDateTime.AddDays(7);
                        db.Channels.Attach(chnl);
                        db.Entry(chnl).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        while (!(DateTime.Now.AddHours(-1) < chnl.ScrumReminderDateTime))
                        {
                            chnl.ScrumReminderDateTime = chnl.ScrumReminderDateTime.AddDays(7);
                        }
                        db.Channels.Attach(chnl);
                        db.Entry(chnl).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                }
            }
            return users;
        }

        public static List<ScrumUser> GetUserList()
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.Users.ToList();
            }
        }
        public static List<ScrumChannel> GetChannelList()
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.Channels.ToList();
            }
        }
        public static bool UserIsRegistered(string channelId,string userId)
        {
            if (PermissionHandler.PermissionEnabled("scrum", channelId))
            {
                using (ScrumContext db = new ScrumContext())
                {                    
                    return db.Users.FirstOrDefault(u => u.UserChannelId == channelId && u.UserId == userId) != null;
                }
            }
            else return false;
        }
    }

    class ScrumContext : DbContext
    {
        public ScrumContext() : base("name=BetaDB") { }

        public DbSet<ScrumUpdate> Updates { get; set; }
        public DbSet<ScrumUser> Users { get; set; }
        public DbSet<ScrumChannel> Channels { get; set; }
    }
}
