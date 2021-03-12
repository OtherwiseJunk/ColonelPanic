using DartsDiscordBots.Services.Interfaces;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.Indecision
{
	public class IndecisionModule : ModuleBase
	{
        IMessageReliabilityService _messenger { get; set; }

        public IndecisionModule(IMessageReliabilityService messenger)
		{
            _messenger = messenger;
		}
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
            MessageReference reference = Context.Message.Reference ?? new MessageReference(Context.Message.Id);
            await _messenger.SendMessageToChannel($"Rolling list: [`{string.Join("`,`", choices)}]`", Context.Channel, reference, new List<ulong>(Context.Message.MentionedUserIds), ",");
            await _messenger.SendMessageToChannel(choices.GetRandom(), Context.Channel, reference, new List<ulong>(Context.Message.MentionedUserIds), " ");
		}

        [Command("roll"), Summary("Roll XdY+/-Z dice.")]
        public async Task Roll([Remainder, Summary("What to roll. Can indicate the number of dice to roll, the number of sides on those dice, and a positive or negative modifier to add to the results. 3d6+2 would roll 3 6-sided dice and add 2 to the final result.")] string rollString)
        {
            StringBuilder sb = new StringBuilder();
            List<string> arguments = new List<string>(rollString.ToLower().Split('d'));
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
                            sb.AppendLine(string.Format("Rolled one d{0} plus {1} and got a total of {2}", sides,modifier, temp + modifier));
                        }
                        else
                        {
                            sb.AppendLine("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        sb.AppendLine("Sorry, I don't recognize that number.");

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
                            sb.AppendLine(string.Format("Rolled one d{0} plus {1} and got a total of {2}", sides, modifier, temp + modifier));
                            sb.AppendLine(string.Format("Rolled one d{0} minus {1} and got a total of {2}", sides,modifier, temp - modifier));
                        }
                        else
                        {
                            sb.AppendLine("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        sb.AppendLine("Sorry, I don't recognize that number.");

                    }
                }
                else
                {
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);

                        sb.AppendLine(string.Format("Rolled one d{0} and got a total of {1}",sides, dice.Roll()));
                    }
                    else
                    {
                        sb.AppendLine("Sorry, I don't recognize that number.");

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
                                sb.AppendLine(string.Format("Rolled {0} d{1} plus {2} and got a total of {3}", times, sides, modifier, temp.Total + modifier));
                                sb.AppendLine(string.Format("Individual Rolls: {0}",string.Join(",", temp.Rolls)));
                            }
                            else
                            {
                                sb.AppendLine("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            sb.AppendLine("Sorry, I don't recognize that number.");

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
                                sb.AppendLine(string.Format("Rolled {0} d{1} minus {2} and got a total of {3}", times, sides, modifier, temp.Total - modifier));
                                sb.AppendLine(string.Format("Individual Rolls: {0}", string.Join(",", temp.Rolls)));
                            }
                            else
                            {
                                sb.AppendLine("Sorry, I don't recognize that number.");
                            }
                        }
                        else
                        {
                            sb.AppendLine("Sorry, I don't recognize that number.");
                        }
                    }
                    else
                    {
                        if (int.TryParse(arguments[1], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            sb.AppendLine(string.Format("Rolled {0} d{1} and got a total of {2}", times, sides, temp.Total));
                            sb.AppendLine(string.Format("Individual Rolls: {0}",string.Join(",", temp.Rolls)));							
                        }
                        else
                        {
                            sb.AppendLine("Sorry, I don't recognize that number.");

                        }
                    }                    
                }
            }
            MessageReference reference = Context.Message.Reference ?? new MessageReference(Context.Message.Id);
            await _messenger.SendMessageToChannel(sb.ToString(), Context.Channel, reference, new List<ulong>(Context.Message.MentionedUserIds), ",");
        }
    }
}
