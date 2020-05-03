using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	public class TurnipBuyPrice : TurnipPrice
	{
		[Key]
		public double BuyPriceId { get; set; }
		public TurnipBuyPrice(int price)
		{
			Price = price;
			Timestamp = DateTime.Now;
		}
	}
}
