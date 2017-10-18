using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class PingGroup
    {
        [Key]
        public int PingGroupId { get; set; }
        public string GuildId { get; set; }
        public string PingGroupName { get; set; }

        public virtual ICollection<PingGroupUser> Users { get; set; }

        public PingGroup()
        {

        }

        public PingGroup(string guildId, string groupName)
        {
            GuildId = guildId;
            PingGroupName = groupName;
        }
    }
}
