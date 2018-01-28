using ColonelPanic.Database.Contexts;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
    [Group("config"), ]
    public class ConfigurationModule : ModuleBase
    {
        [Command("change"), RequireOwner,Summary("Changes the specified configuration field to the specified value. Can currently update:\ngittoken\nbottoken\ngitcommit\n")]
        public static async Task Change([Summary("The field to update")] string field, [Summary("The new value")] string newValue)
        {            
            switch (field)
            {
                case "gittoken":
                    ConfigurationHandler.ChangeConfiguration(field, newValue);
                    break;
                case "bottoken":
                    ConfigurationHandler.ChangeConfiguration(field, newValue);
                    break;
                case "gitcommit":
                    ConfigurationHandler.ChangeConfiguration(field, newValue);
                    break;
                case "destinytoken":
                    ConfigurationHandler.ChangeConfiguration(field, newValue);
                    break;
            }
            
        }

    }
}
