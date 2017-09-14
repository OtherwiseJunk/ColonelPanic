namespace ColonelPanic.Database.Contexts
{
    using ColonelPanic.Database.Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext()
            : base("name=Configuration")
        {
        }
        public DbSet<Configuration> Config { get; set; }
    }

    public class ConfigurationHandler
    {
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
    }
    
}