using ColonelPanic.Database.Contexts;
using ColonelPanic.Permissions;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
    [Group("note"),RequireColPermission("note")]
    public class NoteModule : ModuleBase
    {
        [Command("list"), Summary("Gets a list of all note names and Id's")]
        public async Task ListNoteNames()
        {
            await Context.Channel.SendMessageAsync(NoteHandler.GetNoteNames(Context.Guild.Id.ToString()));
        }

        [Command("get"), Summary("Gets the named note. $note get name")]
        public async Task GetNote([Remainder, Summary("Note name.")] string name)
        {
            await Context.Channel.SendMessageAsync(NoteHandler.GetNote(name, Context.Guild.Id.ToString()));
        }

        [Command("all"), Summary("Gets the named note.")]
        public async Task GetAllNote()
        {
            await Context.Channel.SendMessageAsync(NoteHandler.GetAllNotes(Context.Guild.Id.ToString()));
        }

        [Command("add"), Summary("Adds the specified note text for the note name. $note add name all the text you wanna add.")]
        public async Task AddNote([Summary("The name of the note.")]string name, [Remainder, Summary("Note Content")]string noteText)
        {
            await Context.Channel.SendMessageAsync(NoteHandler.AddNote(name, Context.Guild.Id.ToString(), noteText));
        }

        [Command("delete"), Summary("Deletes the specified note, by noteID (obtained from \"$note list\" or \"$note all\"")]
        public async Task DeleteNote([Summary("The Note Id to be deleted.")] int noteId)
        {
            await Context.Channel.SendMessageAsync(NoteHandler.DeleteNote(noteId, Context.Guild.Id.ToString()));
        }
    }
}
