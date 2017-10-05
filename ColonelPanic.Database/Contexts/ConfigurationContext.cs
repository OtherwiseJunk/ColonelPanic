namespace ColonelPanic.Database.Contexts
{
    using ColonelPanic.Database.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext()
            : base("name=BetaDB")
        {
        }
        public DbSet<Configuration> Config { get; set; }
        public DbSet<ChannelState> ChannelStates { get; set; }
    }

    public class ConfigurationHandler
    {

        private static List<ChannelState> GetChannelStates()
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                return db.ChannelStates.ToList();
            }
        }

        public static void AddChannelState(ChannelState channelState)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                if (!ChannelStateExists(channelState.ChannelID))
                {
                    Console.WriteLine("Adding new ChannelState.");
                    db.ChannelStates.Add(channelState);
                    db.SaveChanges();
                }
            }
        }

        public static void ChangePermission(string permissiongType, string permissionName, string channelId, bool newState)
        {
            switch (permissiongType)
            {
                case "chnl":
                    ChangeChannelPermission(permissionName, channelId, newState);
                    break;
            }
        }

        private static void ChangeChannelPermission(string permissionName, string channelId,bool newState)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                ChannelState channelState = db.ChannelStates.FirstOrDefault(cs => cs.ChannelID == channelId);                
                switch (permissionName)
                {
                    case "scrum":
                        channelState.ScrumEnabled = newState;
                        break;
                    case "speak":
                        channelState.CanSpeak = newState;
                        break;
                    case "listen":
                        channelState.CanListen = newState;
                        break;
                }
                db.ChannelStates.Attach(channelState);
                db.Entry(channelState).State = EntityState.Modified;
                db.SaveChanges();
            }            
        }

        public static bool ChannelStateExists(string channelId)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {                
                return db.ChannelStates.FirstOrDefault(cs => cs.ChannelID == channelId) != null;
            }
        }

        public async static Task AddChannelState(string channelId, string channelName)
        {
            ChannelState chnlState = new ChannelState();
            chnlState.ChannelID = channelId.ToString();
            chnlState.ChannelName = channelName;
            chnlState.ScrumEnabled = false;
            chnlState.CanSpeak = false;
            chnlState.CanListen = false;
            using (ConfigurationContext db = new ConfigurationContext())
            {
                if (!ChannelStateExists(channelId))
                {
                    db.ChannelStates.Add(chnlState);
                    db.SaveChanges();
                }
            }
        }

        public static string GetToken()
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                return db.Config.First().Token;
            }
        }
        public static void CreateConfig(string Token, string GithubToken, string GithubLastUpdate)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                db.Config.Add(new Configuration(Token, GithubToken, GithubLastUpdate));
                db.SaveChanges();
            }
        }

        public static void ChangeConfiguration<T>(string Field, T NewValue)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                Configuration config = db.Config.First();
                switch (Field)
                {
                    case "gittoken":
                        config.GithubToken = NewValue as string;
                        break;
                    case "bottoken":
                        config.Token = NewValue as string;
                        break;
                    case "gitcommit":
                        config.LastGithubCommit = NewValue as string;
                        break;
                }
                db.Config.Attach(config);
                db.Entry(config).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }

    public class PermissionHandler
    {
        public static bool PermissionEnabled(string permission, string chnlId)
        {
            if (ConfigurationHandler.ChannelStateExists(chnlId))
            {
                using (ConfigurationContext db = new ConfigurationContext())
                {
                    switch (permission)
                    {
                        case "scrum":
                            return db.ChannelStates.First(cs => cs.ChannelID == chnlId).ScrumEnabled;
                        case "listen":
                            return db.ChannelStates.First(cs => cs.ChannelID == chnlId).CanListen;
                        case "speak":
                            return db.ChannelStates.First(cs => cs.ChannelID == chnlId).CanSpeak;
                        default:
                            return false;
                    }
                }
            }
            else return false;
        }
    }
    
}