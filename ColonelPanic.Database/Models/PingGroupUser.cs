using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class PingGroupUser
    {
        [Key]
        public int PingGroupUserId { get; set; }        
        public int PingGroupId { get; set; }
        public string GuildId { get; set; }
        public string UserId { get; set; }

        public virtual PingGroup PingGroup { get; set; }

        public PingGroupUser()
        {

        }
        public PingGroupUser(string guildId, string userId, int pingGroupId)
        {
            GuildId = guildId;
            UserId = userId;
            PingGroupId = pingGroupId;
        }
    }
}
