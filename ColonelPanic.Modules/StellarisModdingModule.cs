using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
	public class StellarisModdingModule : ModuleBase
	{
        public string TechFormat = @"`tech_destroyers_unlock = {
    cost = {0}
    area = {1}
    tier = {2}
    icon = {3} #Same Icon Per Cateogry
    is_dangerous = yes #ALWAYS DANGEROUS
    category = {4}

    #potential = {
    #    is_ai = no
    #}

    prerequisites = {5}
    weight = {6}
    gateway = ship

    prereqfor_desc = {
        custom = {
            title = {7}
            desc = {8}
        }
    }
}";


        [Command("tech"), Summary("Responds with a block of text for a tech tree")]
		public async Task GenerateTechBlock(string cost, string tree, string tier, string category, string weight, [Remainder] string PreReqs)
		{            
            await Context.Channel.SendMessageAsync(String.Format(TechFormat,cost,tree,tier,category,category,PreReqs,weight, category, category));
		}
    }
}
