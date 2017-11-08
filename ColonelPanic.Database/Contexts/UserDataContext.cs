using ColonelPanic.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Contexts
{
    public class UserDataContext : DbContext
    {
        public UserDataContext() : base("name=BetaDB") { }

        public DbSet<UserXChannelFlags> UserFlags { get; set; }
        

    }

    public class UserDataHandler
    {
        public static void AddShitlistUser(string channelId, string userId)
        {            
            using (UserDataContext db = new UserDataContext())
            {
                if (UserHasFlags(channelId, userId))
                {
                    db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId).Shitlist = true;

                }
                else
                {
                    var uxcf = new UserXChannelFlags(channelId, userId);
                    uxcf.Shitlist = true;
                    db.UserFlags.Add(uxcf);                                        
                }
                db.SaveChanges();
            }

            
        }
        public static void RemoveShitlistUser(string channelId, string userId)
        {
            using (UserDataContext db = new UserDataContext())
            {
                db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId).EggplantList = false;
                db.SaveChanges();
            }            
        }
        public static void AddEggplantUser(string channelId, string userId)
        {            
            using (UserDataContext db = new UserDataContext())
            {
                if (UserHasFlags(channelId, userId))
                {
                    db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId).EggplantList = true;

                }
                else
                {
                    var uxcf = new UserXChannelFlags(channelId, userId);
                    uxcf.EggplantList = true;
                    db.UserFlags.Add(uxcf);                    
                }
                db.SaveChanges();
            }            
        }
        public static void RemoveEggplantUser(string channelId, string userId)
        {            
            using (UserDataContext db = new UserDataContext())
            {
                db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId).EggplantList = false;                
                db.SaveChanges();
            }            
        }
        public static bool IsShitlistUser(string channelId, string userId)
        {
            using (UserDataContext db = new UserDataContext())
            {
                if (UserHasFlags(channelId, userId))
                {
                    return db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId).Shitlist;
                }
                return false;
            }
        }
        public static bool IsEggplantUser(string channelId, string userId)
        {
            using (UserDataContext db = new UserDataContext())
            {
                if (UserHasFlags(channelId, userId))
                {
                    return db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId).EggplantList;
                }
                return false;
            }
        }
        public static bool UserHasFlags(string channelId, string userId)
        {
            using (UserDataContext db = new UserDataContext())
            {
                return null != db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId);
            }
        }
    }
}
