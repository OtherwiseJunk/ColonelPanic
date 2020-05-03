using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	[Table("AC_SellPrices")]
	public class TurnipSellPrice : TurnipPrice
	{
		[Key]
		public int SellPriceId { get; set; }
		public bool IsMorningPrice { get; set; }

		public TurnipSellPrice(int price)
		{
			Price = price;
			Timestamp = DateTime.Now;
			IsMorningPrice = DateTime.Now.Hour < 12;
		}
	}
}
