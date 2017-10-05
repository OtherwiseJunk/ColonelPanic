using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class ScrumUser
    {
        [Key]
        public int ScumUserNum { get; set; }
        public string UserChannelId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime LastUpdate { get; set; }
        public int UpdateCount { get; set; }
        public ScrumUser() { }

        public ScrumUser(string chnlId, string usrId, string username)
        {            
            UserChannelId = chnlId;
            UserId = usrId;
            UpdateCount = 0;
            Username = username;
            LastUpdate = DateTime.Now;
        }
    }
}
