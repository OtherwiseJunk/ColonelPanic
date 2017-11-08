using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class UserXChannelFlags
    {
        [Key]
        public int Id { get; set; }
        public string ChannelId { get; set; }
        public string UserId { get; set; }
        public bool Shitlist { get; set; }
        public bool EggplantList { get; set; }

        public UserXChannelFlags()
        {
        }

        public UserXChannelFlags(string channelId, string userId)
        {
            ChannelId = channelId;
            UserId = userId;
        }
    }
}
