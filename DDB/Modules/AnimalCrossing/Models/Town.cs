using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartsDiscordBots.Models
{
	[Table("AC_Towns")]
	public class Town
	{
		[Key]
		public int TownId { get; set;}
		public ulong MayorDiscordId { get; set; }
		public string TownName { get; set; }
		public int TurnipSellPrice { get; set; }
		public int TurnipBuyPrice { get; set; }
		public ICollection<Fruit> Fruits { get; set; }
		public ICollection<Item> Wishlist { get; set; }
		public string NativeFruit { get; set; }
		public string DodoCode { get; set; }
		public bool BorderOpen { get; set; }
		public string NorthernHempisphere { get; set; }
	}
}
