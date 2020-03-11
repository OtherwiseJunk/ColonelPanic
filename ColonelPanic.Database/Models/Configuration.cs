using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColonelPanic.Database.Models
{
    [Table("Configuration")]
    public class Configuration
    {
        [Key]
        public int ConfigID { get; set; }
        public string Token { get; set; }        
        public string GithubToken { get; set; }
        public string LastGithubCommit { get; set; }
        public string DestinyAPIToken { get; set; }        

        public Configuration()
        {

        }

        public Configuration(string token, string githubToken, string githubCommit)
        {
            Token = token;
            GithubToken = githubToken;
            LastGithubCommit = githubCommit;
        }
    }
}
