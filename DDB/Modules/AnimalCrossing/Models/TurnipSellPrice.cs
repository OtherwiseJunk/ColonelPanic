using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	public class TurnipSellPrice : TurnipPrice
	{
		[Key]
		public double SellPriceId { get; set; }
		public bool IsMorningPrice { get; set; }

		public TurnipSellPrice(int price)
		{
			Price = price;
			Timestamp = DateTime.Now;
			IsMorningPrice = DateTime.Now.Hour < 12;
		}
	}
}
