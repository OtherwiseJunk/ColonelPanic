using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Modules.Jackbox;
using DartsDiscordBots.Modules.Jackbox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic
{
    public static class JackboxUtilities
    {
        public static void EnsureDefaultGamesExist(JackboxContext context)
        {
            List<JackboxGame> games = context.JackboxGames.ToList();

            foreach (JackboxGame game in JackboxConstants.DefaultGameData)
            {

                JackboxGame dbGame = games.FirstOrDefault(dbGame => dbGame.Name == game.Name && dbGame.DiscordGuildId == game.DiscordGuildId);
                if (dbGame == null)
                {
                    context.JackboxGames.Add(new JackboxGame()
                    {
                        Name = game.Name,
                        DiscordGuildId = game.DiscordGuildId,
                        PlayerName = game.PlayerName,
                        Description = game.Description,
                        JackboxVersion = game.JackboxVersion,
                        MaxPlayers = game.MaxPlayers,
                        MinPlayers = game.MinPlayers,
                        VotingEmoji = game.VotingEmoji,
                        HasAudience = game.HasAudience
                    });
                }
            }
        }
    }
}
