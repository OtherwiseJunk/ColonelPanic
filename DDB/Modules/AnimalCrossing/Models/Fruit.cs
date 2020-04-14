using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DartsDiscordBots.Models
{
	[Table("AC_Fruits")]
	public class Fruit
	{
		[Key]
		public int FruitId { get; set; }
		public string FruitName { get; set; }
	}
}
