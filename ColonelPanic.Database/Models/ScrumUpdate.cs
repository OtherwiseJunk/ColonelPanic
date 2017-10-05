using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    [Table("ScrumUpdates")]
    class ScrumUpdate
    {
        [Key]
        public int UpdateID { get; set; }
        public string UpdateText { get; set; }
        public string Updater { get; set; }
        public string ChannelID { get; set; }
        public DateTime Timestamp { get; set; }

        public ScrumUpdate()
        {

        }
        public ScrumUpdate(string update, string updater, ulong channelId)
        {
            UpdateText = update;
            Updater = updater;
            ChannelID = channelId.ToString();
            Timestamp = DateTime.Now;
        }
    }
}
