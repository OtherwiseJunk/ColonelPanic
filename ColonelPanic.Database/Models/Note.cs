using System.ComponentModel.DataAnnotations;

namespace ColonelPanic.Database.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string NoteText { get; set; }

        public Note()
        {

        }

        public Note(string guildId,string name, string noteText)
        {
            GuildId = guildId;
            Name = name;
            NoteText = noteText;
        }
    }
}
