﻿using ColonelPanic.Database.Models;
using ColonelPanic.DatabaseCore.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ColonelPanic.Database.Contexts
{
	public class NoteContext : DbContext
	{
		public NoteContext(DbContextOptions<NoteContext> options) : base(options) { }

		public DbSet<Note> Notes { get; set; }		
	}

    public class NoteHandler
    {
		public static DbContextOptionsBuilder<NoteContext> OptionsBuilder { get; set; }
		static NoteHandler()
		{
			OptionsBuilder = new DbContextOptionsBuilder<NoteContext>();
			OptionsBuilder.UseSqlServer(ConnectionStrings.ConnectionString);
		}
		public static string GetNote(string name, string guildId)
        {
            string msg = "Sorry, couldn't find that note. Maybe you did a bad?";
            using(NoteContext db = new NoteContext(OptionsBuilder.Options))
            {
                if (db.Notes.FirstOrDefault(n=>n.Name == name && n.GuildId == guildId) != null)
                {
                    msg = db.Notes.FirstOrDefault(n => n.Name == name).NoteText;
                }
            }

            return msg;
        }

        public static string GetNoteNames(string guildId)
        {
            string msg = "Doesn't look like we have any of those, boss.";
            using(NoteContext db = new NoteContext(OptionsBuilder.Options)){
                if (db.Notes.Count() > 0)
                {
                    msg = "";
                    foreach (Note note in db.Notes.AsQueryable().Where(n => n.GuildId == guildId).ToList())
                    {
                        msg += $"{note.NoteId}|**{note.Name}**"+Environment.NewLine;
                    }
                }
            }
            return msg;
        }

        public static string GetAllNotes(string guildId)
        {
            string msg = "There's nothing here, bud.";
            using (NoteContext db = new NoteContext(OptionsBuilder.Options))
            {
                if (db.Notes.Count() > 0)
                {
                    msg = "";
                    foreach (Note note in db.Notes.AsQueryable().Where(n => n.GuildId == guildId).ToList())
                    {
                        msg += $"{note.NoteId}|**{note.Name}**: {note.NoteText}" + Environment.NewLine;
                    }
                }
            }
            return msg;
        }

        public static string AddNote(string name, string guildId, string noteText)
        {            
            using(NoteContext db = new NoteContext(OptionsBuilder.Options))
            {
                if (db.Notes.FirstOrDefault(n => n.Name.ToLower() == name.ToLower()) == null)
                {
                    db.Notes.Add(new Note(guildId, name, noteText));
                    db.SaveChanges();
                    return "Added that note for you, pal.";
                }
                else
                {
                    return "That note name is already taken, friend.";
                }
            }
        }

        public static string DeleteNote(int noteId, string guildId)
        {
            string msg = "I don't see that note... you sure it's in this server's notes list?";

            using (NoteContext db = new NoteContext(OptionsBuilder.Options))
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
