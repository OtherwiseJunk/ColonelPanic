using DartsDiscordBots.Utilities;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.Indecision
{
	public class IndecisionModule : ModuleBase
	{
		[Command("pick"), Summary("Picks a random value from a comma separated list. adding + to the end adds slight preference")]
		public async Task Pick([Remainder, Summary("The comma separated list of items to pick from")] string items)
		{
			List<string> choices = items.Split(',').ToList();
			Dictionary<string, int> ChoiceCountByName = new Dictionary<string, int>();
			foreach (string choice in choices)
			{
				if (choice.EndsWith('+'))
				{
					int plusCount = choice.Count(cha => cha == '+');
					ChoiceCountByName.Add(choice.Replace("+", string.Empty), ++plusCount);
				}
				else
				{
					ChoiceCountByName.Add(choice, 1);
				}
			}

			choices = new List<string>();
			foreach (string choiceName in ChoiceCountByName.Keys)
			{
				for (int i = 0; i < ChoiceCountByName[choiceName]; i++)
				{
					choices.Add(choiceName);
				}
			}
			await Context.Channel.SendMessageAsync($"Rolling list: [`{string.Join("`,`", choices)}]`");
			await Context.Channel.SendMessageAsync(choices.GetRandom());
		}

        [Command("roll"), Summary("Roll XdY+/-Z dice.")]
        public async Task Roll([Remainder, Summary("What to roll. Can indicate the number of dice to roll, the number of sides on those dice, and a positive or negative modifier to add to the results. 3d6+2 would roll 3 6-sided dice and add 2 to the final result.")] string rollString)
        {

            var arguments = new List<string>(rollString.ToLower().Split('d'));
            arguments.Remove("");
            int sides, times, modifier;

            if (arguments.Count == 1)
            {
                if (arguments[0].Contains("+"))
                {
                    arguments = new List<string>(arguments[0].Split('+'));
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        var temp = dice.Roll();
                        if (int.TryParse(arguments[1], out modifier))
                        {
                            await Context.Channel.SendMessageAsync(
                                    string.Format("Rolled one d{0} plus {1} and got a total of {2}", sides,
                                        modifier, temp + modifier));

                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
                else if (arguments[0].Contains("-"))
                {
                    arguments = new List<string>(arguments[0].Split('-'));
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        var temp = dice.Roll();
                        if (int.TryParse(arguments[1], out modifier))
                        {
                            await Context.Channel.SendMessageAsync(
                                    string.Format("Rolled one d{0} minus {1} and got a total of {2}", sides,
                                        modifier, temp - modifier));
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
                else
                {
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        await Context.Channel.SendMessageAsync(string.Format("Rolled one d{0} and got a total of {1}",
                                sides, dice.Roll()));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
            }
            else if (arguments.Count == 2)
            {
                if (int.TryParse(arguments[0], out times))
                {
                    if (arguments[1].Contains("+"))
                    {
                        arguments = new List<string>(arguments[1].Split('+'));
                        if (int.TryParse(arguments[0], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            if (int.TryParse(arguments[1], out modifier))
                            {
                                await Context.Channel.SendMessageAsync(
                                        string.Format("Rolled {0} d{1} plus {2} and got a total of {3}",
                                            times, sides, modifier, temp.Total + modifier));
                                await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                        string.Join(",", temp.Rolls)));

                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else if (arguments[1].Contains("-"))
                    {
                        arguments = new List<string>(arguments[1].Split('-'));
                        if (int.TryParse(arguments[0], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            if (int.TryParse(arguments[1], out modifier))
                            {
                                await Context.Channel.SendMessageAsync(
                                        string.Format("Rolled {0} d{1} minus {2} and got a total of {3}", times, sides,
                                            modifier, temp.Total - modifier));
                                await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                        string.Join(",", temp.Rolls)));

                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        if (int.TryParse(arguments[1], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            await Context.Channel.SendMessageAsync(string.Format(
                                    "Rolled {0} d{1} and got a total of {2}", times, sides, temp.Total));
                            await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                    string.Join(",", temp.Rolls)));

                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                }
            }
        }
    }
}
