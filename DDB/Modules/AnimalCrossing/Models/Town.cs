using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	[Table("AC_Towns")]
	public class Town
	{
		[Key]
		public int TownId { get; set; }
		public ulong MayorDiscordId { get; set; }
		public string MayorRealName { get; set; }
		public string TownName { get; set; }
		public ICollection<Fruit> Fruits { get; set; }
		public ICollection<Item> Wishlist { get; set; }
		public ICollection<TurnipBuyPrice> BuyPrices { get; set; }
		public ICollection<TurnipSellPrice> SellPrices { get; set;}
		public string NativeFruit { get; set; }
		public string DodoCode { get; set; }
		public bool BorderOpen { get; set; }
		public string NorthernHempisphere { get; set; }

		public Town()
		{
			Fruits = new List<Fruit>();
			Wishlist = new List<Item>();
			BuyPrices = new List<TurnipBuyPrice>();
			SellPrices = new List<TurnipSellPrice>();
		}
	}
}
