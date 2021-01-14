using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	[Table("AC_BuyPrices")]
	public class TurnipBuyPrice : TurnipPrice
	{
		[Key]
		public int BuyPriceId { get; set; }
		public TurnipBuyPrice(int price)
		{
			Price = price;
			Timestamp = DateTime.Now;
		}
	}
}
