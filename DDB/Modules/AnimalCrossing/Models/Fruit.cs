using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DartsDiscordBots.Modules.AnimalCrossing.Models
{
	[Table("AC_Fruits")]
	public class Fruit
	{
		[Key]
		public int FruitId { get; set; }
		public string FruitName { get; set; }
	}
}
