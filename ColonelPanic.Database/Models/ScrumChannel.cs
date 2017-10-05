using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class ScrumChannel
    {
        [Key]
        public string ScrumChannelId { get; set; }
        public DateTime ScrumReminderDateTime { get; set; }
        public ScrumChannel()
        {
        }
        public ScrumChannel(string chnlId, DateTime dateTime)
        {
            ScrumChannelId = chnlId;
            ScrumReminderDateTime = dateTime;
        }
    }
}
