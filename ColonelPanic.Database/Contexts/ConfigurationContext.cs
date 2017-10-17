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
        public DbSet<GuildState> GuildStates { get; set; }
        public DbSet<User> TrustedUsers { get; set; }
    }

    public class ConfigurationHandler
    {

        private static List<GuildState> GetGuildStates()
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                return db.GuildStates.ToList();
            }
        }

        public static void AddGuildState(GuildState guildState)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                if (!GuildStateExists(guildState.GuildId))
                {
                    Console.WriteLine("Adding new Guild State.");
                    db.GuildStates.Add(guildState);
                    db.SaveChanges();
                }
            }
        }

        public static void ChangePermission(string permissiongType, string permissionName, string guildId, bool newState)
        {
            switch (permissiongType)
            {
                case "guild":
                    ChangGuildPermissionState(permissionName, guildId, newState);
                    break;
            }
        }

        private static void ChangGuildPermissionState(string permissionName, string guildId,bool newState)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                GuildState guildStates = db.GuildStates.FirstOrDefault(gs => gs.GuildId == guildId);                
                switch (permissionName)
                {
                    case "scrum":
                        guildStates.ScrumEnabled = newState;
                        break;
                    case "speak":
                        guildStates.CanSpeak = newState;
                        break;
                    case "listen":
                        guildStates.CanListen = newState;
                        break;
                    case "note":
                        guildStates.NoteEnabled = newState;
                        break;
                    case "ping":
                        guildStates.PingGroupEnabled = newState;
                        break;
                }
                db.GuildStates.Attach(guildStates);
                db.Entry(guildStates).State = EntityState.Modified;
                db.SaveChanges();
            }            
        }

        public static bool GuildStateExists(string guildId)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {                
                return db.GuildStates.FirstOrDefault(gs => gs.GuildId == guildId) != null;
            }
        }

        public async static Task AddGuildState(string guildId, string guildName)
        {
            GuildState guildState = new GuildState();
            guildState.GuildId = guildId.ToString();
            guildState.GuildName = guildName;
            guildState.ScrumEnabled = false;
            guildState.NoteEnabled = false;
            guildState.PingGroupEnabled = false;
            guildState.CanSpeak = false;
            guildState.CanListen = false;
            using (ConfigurationContext db = new ConfigurationContext())
            {
                if (!GuildStateExists(guildId))
                {
                    db.GuildStates.Add(guildState);
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
        public static void AddTrustedUser(string userId, string username)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                if (db.TrustedUsers.FirstOrDefault(u=> u.UserId.ToString() == userId ) == null)
                {
                    db.TrustedUsers.Add(new User(userId, username));
                    db.SaveChanges();
                }
            }
        }

        public static bool IsTrustedUser(string userId)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                return db.TrustedUsers.FirstOrDefault(u => u.UserId.ToString() == userId) != null;
            }
        }

        public static bool ChannelConfigured(string configurationType,string channelId)
        {
            bool channelConfigured = false;            
            switch (configurationType)
            {
                case "scrum":
                    using (ScrumContext db = new ScrumContext())
                    {
                        channelConfigured = db.Channels.FirstOrDefault(chnl => chnl.ScrumChannelId == channelId) != null;
                    }
                    break;
            }            
            return channelConfigured;
        }

        public static bool CanExecute(string permissionName,string guildId,string userId)
        {
            bool permEnabled = false;
            using (ConfigurationContext db = new ConfigurationContext())
            {
                GuildState guildState = db.GuildStates.FirstOrDefault(gs => gs.GuildId == guildId);
                if (guildState != null)
                {
                    if (guildState.GuildName.StartsWith("@"))
                    {
                        permEnabled = true;
                    }
                    else
                    {
                        switch (permissionName)
                        {
                            case "scrum":
                                permEnabled = guildState.ScrumEnabled;
                                break;                            
                            case "speak":
                                permEnabled = guildState.CanSpeak;
                                break;
                            case "listen":
                                permEnabled = guildState.CanListen;
                                break;
                            case "note":
                                permEnabled = guildState.NoteEnabled;
                                break;
                        }
                    }
                }
            }

            return IsTrustedUser(userId) || permEnabled;
        }        

        public static void RemoveTrustedUser(string userId)
        {
            using (ConfigurationContext db = new ConfigurationContext())
            {
                User user = db.TrustedUsers.FirstOrDefault(u => u.UserId.ToString() == userId);
                if (user != null)
                {
                    db.TrustedUsers.Remove(user);
                    db.SaveChanges();
                }
            }
        }
    }
    
}