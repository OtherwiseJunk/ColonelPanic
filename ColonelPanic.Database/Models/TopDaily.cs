using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ColonelPanic.Database.Models
{
    public class TopDaily
    {
        [Key]
        public int TopDailyNum { get; set; }
        public string ChannelId { get; set; }
        public string Subreddit { get; set; }
        public DateTime NextTimeToPost { get; set; }

        public TopDaily()
        {

        }

        public TopDaily(string channelId, string subreddit, DateTime nextTimeToPost)
        {
            ChannelId = channelId;
            Subreddit = subreddit;
            NextTimeToPost = nextTimeToPost;
        }

        public void Posted()
        {
            this.NextTimeToPost.AddDays(1);
        }
    }

}
