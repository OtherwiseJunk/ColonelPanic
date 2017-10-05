using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class ChannelState
    {
        [Key]
        public int ChannelNum { get; set; }
        public string ChannelID { set; get; }
        public string ChannelName { get; set; }
        public bool ScrumEnabled { get; set; }
        public bool ServerModuleEnabled { get; set; }
        public bool CanSpeak { get; set; }
        public bool CanListen { get; set; }

        public ChannelState()
        {

        }        

        public ChannelState(string channelId, string channelName)
        {
            ChannelID = channelId;
            ChannelName = channelName;
            ScrumEnabled = false;
            ServerModuleEnabled = true;
            CanSpeak = false;
            CanListen = false;
        }
    }
}
