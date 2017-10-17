using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class GuildState
    {
        [Key]
        public int GuildNum { get; set; }
        public string GuildId { set; get; }
        public string GuildName { get; set; }
        public bool ScrumEnabled { get; set; }
        public bool NoteEnabled { get; set; }
        public bool PingGroupEnabled { get; set; }
        public bool CanSpeak { get; set; }
        public bool CanListen { get; set; }
        

        public GuildState()
        {

        }        

        public GuildState(string channelId, string channelName)
        {
            GuildId = channelId;
            GuildName = channelName;
            ScrumEnabled = false;            
            CanSpeak = false;
            CanListen = false;
        }
    }
}
