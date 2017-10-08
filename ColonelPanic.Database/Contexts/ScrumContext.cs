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
                ScrumUser usr = db.ScrumUsers.FirstOrDefault(u => u.UserId == usrId && u.UserChannelId == chnlId);
                usr.UpdateCount++;
                usr.LastUpdate = DateTime.Now;
                db.ScrumUsers.Attach(usr);
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
                if (db.ScrumUsers.FirstOrDefault(u => u.UserId == usrId) == null)
                {
                    db.ScrumUsers.Add(new ScrumUser(chnlId.ToString(), usrId, username));
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
            string msg = "Here's who I'm tracking currently:" + Environment.NewLine + Environment.NewLine;

            using (ScrumContext db = new ScrumContext())
            {
                if (db.ScrumUsers.Count() > 0)
                {
                    List<ScrumUser> users = db.ScrumUsers.ToList();
                    foreach (ScrumUser usr in users)
                    {
                        msg += $"**Username**: {usr.Username + Environment.NewLine} **Last Update:** {usr.LastUpdate.ToString("dddd, dd MMMM yyyy HH:mm:ss") + Environment.NewLine} **Update Count:** {usr.UpdateCount + Environment.NewLine}" + Environment.NewLine + Environment.NewLine;
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
                        List<ScrumUser> scrumUsers = db.ScrumUsers.ToList();
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

        public static bool UserExists(string channelId, string username)
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.ScrumUsers.FirstOrDefault(u => u.Username.ToLower() == username.ToLower() && u.UserChannelId == channelId) != null;
            }
        }

        public static string BuildAllUpdateLists(string channelId, string channelName)
        {
            string msg = "";
            using (ScrumContext db = new ScrumContext())
            {
                List<ScrumUser> users = db.ScrumUsers.Where(u => u.UserChannelId == channelId).ToList();
                foreach (ScrumUser user in users)
                {
                    msg += $"**{user.Username}'s Updates:**" + Environment.NewLine;
                    msg += BuildUserUpdateList(channelId, user.Username);
                    msg += Environment.NewLine + Environment.NewLine;
                }
            }

            return msg;

        }

        public static string BuildUserUpdateList(string channelId, string username)
        {
            string msg = "";
            using (ScrumContext db = new ScrumContext())
            {
                List<ScrumUpdate> userUpdates = db.Updates.Where(u => u.ChannelID == channelId && u.Updater.ToLower() == username.ToLower()).ToList();
                if (userUpdates.Count > 0)
                {
                    int loopCount = 0;
                    DateTime lastDateTime = new DateTime();
                    foreach (ScrumUpdate update in userUpdates)
                    {
                        msg += $"{update.UpdateID}) {update.Timestamp.ToString("yyyy-MM-dd")}: {update.UpdateText}";
                        if (loopCount != 0)
                        {
                            msg += $" |Days since last update: {(int)(update.Timestamp - lastDateTime).TotalDays}";
                        }
                        msg += Environment.NewLine;
                        lastDateTime = update.Timestamp;
                        loopCount++;
                    }
                    msg = msg.Substring(0, msg.Length - 2);
                }
                else
                {
                    msg = "This user has no updates.";
                }
            }
            return msg;
        }

        public static List<ScrumUser> GetUserList()
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.ScrumUsers.ToList();
            }
        }
        public static List<ScrumChannel> GetChannelList()
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.Channels.ToList();
            }
        }
        public static bool UserIsRegistered(string channelId, string userId)
        {
            using (ScrumContext db = new ScrumContext())
            {
                return db.ScrumUsers.FirstOrDefault(u => u.UserChannelId == channelId && u.UserId == userId) != null;
            }
        }
    }

    class ScrumContext : DbContext
    {
        public ScrumContext() : base("name=BetaDB") { }

        public DbSet<ScrumUpdate> Updates { get; set; }
        public DbSet<ScrumUser> ScrumUsers { get; set; }
        public DbSet<ScrumChannel> Channels { get; set; }
    }
}
