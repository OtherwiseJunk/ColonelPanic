using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string NoteText { get; set; }

        public Note()
        {

        }

        public Note(string channelId,string name, string noteText)
        {
            GuildId = channelId;
            Name = name;
            NoteText = noteText;
        }
    }
}
