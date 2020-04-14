using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DartsDiscordBots.Models
{
	[Table("AC_Wishlist_Items")]
	public class Item
	{
		[Key]
		public int ItemNum { get; set; }
		public string ItemName { get; set; }
	}
}
