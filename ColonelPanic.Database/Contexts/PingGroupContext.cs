using ColonelPanic.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Contexts
{
    public class PingGroupContext : DbContext
    {
        public PingGroupContext()
            : base("name=BetaDB") { }

        public DbSet<PingGroup> Groups { get; set; }
        public DbSet<PingGroupUser> Users { get; set; }

    }

    public class PingGroupHandler
    {
        public static List<string> GetPingUsers(string pingGroupName, string guildId)
        {
            List<string> users = new List<string> { "empty" };
            using (PingGroupContext db = new PingGroupContext())
            {
                PingGroup group = db.Groups.FirstOrDefault(g => g.PingGroupName.ToLower() == pingGroupName.ToLower() && g.GuildId == guildId);

                if (db.Users.Where(u => u.PingGroupId == group.PingGroupId).Count() > 0)
                {
                    users = new List<string>();
                    foreach (PingGroupUser user in db.Users.Where(u => u.PingGroupId == group.PingGroupId && u.GuildId == group.GuildId))
                    {
                        users.Add(user.UserId);
                    }
                }
            }
            return users;
        }

        public static void CreatePingGroup(string pingGroupName, string guildId)
        {
            using (PingGroupContext db = new PingGroupContext())
            {
                db.Groups.Add(new PingGroup(guildId, pingGroupName));
            }
        }

        public static bool PingGroupExists(string pingGroupName, string guildId)
        {
            using (PingGroupContext db = new PingGroupContext())
            {
                return db.Groups.FirstOrDefault(group => group.GuildId == guildId && group.PingGroupName.ToLower() == pingGroupName) != null;
            }
        }

        public static string GetPingGroups(string guildId)
        {
            string msg = "";
            using (PingGroupContext db = new PingGroupContext())
            {

                if (db.Groups.FirstOrDefault(g => g.GuildId == guildId) != null)
                {
                    foreach (PingGroup group in db.Groups.Where(g => g.GuildId == guildId).ToList())
                    {
                        msg += group + Environment.NewLine;
                    }
                    msg.TrimEnd(Environment.NewLine.ToCharArray());
                }
                else
                {
                    msg = "Sorry, doesn't look like we have setup any Ping Groups yet.";
                }
            }
            return msg;
        }

        public static void AddPingGroupUser(int pingGroupId, string userId, string guildId)
        {
            using(PingGroupContext db = new PingGroupContext())
            {
                db.Users.Add(new PingGroupUser(guildId, userId, pingGroupId));
            }
        }

        public static int GetPingGroupId(string pingGroupName, string guildId)
        {
            using (PingGroupContext db = new PingGroupContext())
            {
                return db.Groups.First(g => g.PingGroupName.ToLower() == pingGroupName.ToLower() && g.GuildId == guildId).PingGroupId;
            }
        }

        public static void PurgeUser(string userId, string guildId)
        {
            using (PingGroupContext db = new PingGroupContext())
            {
                List<PingGroupUser> userInstances = db.Users.Where(u => u.UserId == userId && u.GuildId == guildId).ToList();
                foreach (PingGroupUser userInstance in userInstances)
                {
                    db.Users.Attach(userInstance);
                    db.Users.Remove(userInstance);
                }
                db.SaveChanges();
            }
        }

        public static bool PingGroupUserExists(int pingGroupId, string userId, string guildId)
        {
            using (PingGroupContext db = new PingGroupContext())
            {
                return db.Users.FirstOrDefault(u => u.PingGroupId == pingGroupId && u.UserId == userId && u.GuildId == guildId) != null;
            }
        }
    }
}
