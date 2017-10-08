using ColonelPanic.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Contexts
{
    class NoteContext :DbContext
    {
        public NoteContext() : base("name=BetaDB") { }

        public DbSet<Note> Notes { get; set; }
    }

    public class NoteHandler
    {
        public static string GetNote(string name, string guildId)
        {
            string msg = "Sorry, couldn't find that note. Maybe you did a bad?";
            using(NoteContext db = new NoteContext())
            {
                if (db.Notes.FirstOrDefault(n=>n.Name == name ) != null)
                {
                    msg = db.Notes.FirstOrDefault(n => n.Name == name).NoteText;
                }
            }

            return msg;
        }

        public static string GetNoteNames(string guildId)
        {
            string msg = "Doesn't look like we have any of those, boss.";
            using(NoteContext db = new NoteContext()){
                if (db.Notes.Count() > 0)
                {
                    msg = "";
                    foreach (Note note in db.Notes.Where(n => n.GuildId == guildId).ToList())
                    {
                        msg += $"**{note.Name}**"+Environment.NewLine;
                    }
                }
            }
            return msg;
        }

        public static string GetAllNotes(string guildId)
        {
            string msg = "There's nothing here, bud.";
            using (NoteContext db = new NoteContext())
            {
                if (db.Notes.Count() > 0)
                {
                    msg = "";
                    foreach (Note note in db.Notes.Where(n => n.GuildId == guildId).ToList())
                    {
                        msg += $"{note.NoteId}|**{note.Name}**: {note.NoteText}" + Environment.NewLine;
                    }
                }
            }
            return msg;
        }

        public static string AddNote(string name, string guildId, string noteText)
        {            
            using(NoteContext db = new NoteContext())
            {
                if (db.Notes.FirstOrDefault(n => n.Name.ToLower() == name.ToLower()) == null)
                {
                    db.Notes.Add(new Note(guildId, name, noteText));
                    db.SaveChanges();
                    return "Added that note for you, pal.";
                }
                else
                {
                    return "That note name is already taken, my dude.";
                }
            }
        }

        public static string DeleteNote(int noteId, string guildId)
        {
            string msg = "I don't see that note... you sure it's in this server's notes list?";

            using (NoteContext db = new NoteContext())
            {
                if (db.Notes.FirstOrDefault(n => n.GuildId == guildId && n.NoteId == noteId) != null)
                {
                    Note note = db.Notes.FirstOrDefault(n => n.NoteId == noteId);
                    db.Notes.Attach(note);
                    db.Notes.Remove(note);
                    db.SaveChanges();
                    msg = "Ok, I deleted that note for you, dawg.";
                }
            }

            return msg;
        }
    }
}
