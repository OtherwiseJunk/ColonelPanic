using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	[Table("AC_Wishlist_Items")]
	public class Item
	{
		[Key]
		public int ItemNum { get; set; }
		public string ItemName { get; set; }
	}
}
