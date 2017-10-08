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
    class NoteModule : ModuleBase
    {
        
    }
}
