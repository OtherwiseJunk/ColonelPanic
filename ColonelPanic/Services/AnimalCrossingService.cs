using DartsDiscordBots.Interfaces;
using DartsDiscordBots.Models;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DartsDiscordBots.Database.Constants;
using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Utilities;

namespace DartsDiscordBots.Services
{
	public class AnimalCrossingService : IAnimalCrossingService
	{
		public static DbContextOptionsBuilder<AnimalCrossingContext> OptionsBuilder { get; set; }
		public AnimalCrossingService()
		{
			OptionsBuilder = new DbContextOptionsBuilder<AnimalCrossingContext>();
			OptionsBuilder.UseSqlServer(ConnectionStrings.ConnectionString);
		}
		public static void ClearTurnipBuyPrices()
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				foreach (Town town in db.Towns.ToList())
				{
					town.TurnipBuyPrice = 0;
				};
				db.SaveChanges();
			}
		}
		public static void ClearTurnipSellPrices()
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				foreach (Town town in db.Towns.ToList())
				{
					town.TurnipSellPrice = 0;
				};
				db.SaveChanges();
			}
		}

		public string GetFruitList(List<IGuildUser> users)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sb = new StringBuilder("Town Name - Fruits").AppendLine();
				bool fruitUnset = true;
				foreach(Town town in db.Towns.Include(t => t.Fruits).ToList())
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					if (town.Fruits.Count > 0)
					{
						if (localMayor != null)
						{
							sb.Append($"{town.TownName} (Mayor: {localMayor.Username}) - ");
						}
						else
						{
							sb.Append($"{town.TownName} - ");
						}
						foreach (Fruit fruit in town.Fruits)
						{
							if(town.NativeFruit == fruit.FruitName)
							{
								sb.Append($"**{fruit.FruitName}**, ");
							}
							else
							{
								sb.Append($"{fruit.FruitName}, ");
							}
						}
						sb.Remove(sb.Length - 2, 2).AppendLine();
						fruitUnset = false;
					}
				};
				if (fruitUnset) return "I don't know nothing about no fruit in no towns, man.";
				sb.AppendLine().AppendLine("**Fruit Name** - Denotes a native fruit for the town.");
				return sb.ToString();
			}
		}

		public Town GetTown(int townId)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				return db.Towns.FirstOrDefault(t => t.TownId == townId);
			}
		}

		public Town GetTown(string townName)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				return db.Towns.FirstOrDefault(t => t.TownName.ToLower() == townName.ToLower());
			}
		}

		public Town GetTown(ulong mayorId)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				return db.Towns.FirstOrDefault(t => t.MayorDiscordId == mayorId);
			}
		}

		public string GetTownList(List<IGuildUser> users)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sb = new StringBuilder("Town Id | Town Name").AppendLine();
				var towns = db.Towns.ToList();
				bool townsUnset = true;
				foreach (Town town in towns)
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					string hemisphere = "";
					string dodoCode = "";
					if (town.NorthernHempisphere != String.Empty)
					{
						hemisphere = town.NorthernHempisphere == "n" ? " in the Northern Hemisphere" : " in the Southern Hemisphere";
					}
					if(town.DodoCode != String.Empty)
					{
						dodoCode = $" Dodo Code: `{town.DodoCode}`";
					}
					if (localMayor != null)
					{
						sb.AppendLine($"`{town.TownId}` | `{town.TownName}`{hemisphere} Border Open:`{town.BorderOpen}`{dodoCode} - Mayor: {localMayor.Username}");
					}
					else
					{
						sb.AppendLine($"`{town.TownId}` | `{town.TownName}`{hemisphere} Border Open:`{town.BorderOpen}`{dodoCode}");
					}					
					townsUnset = false;
				}
				if (townsUnset) return "There's no towns :-(";
				return sb.ToString();
			}
		}

		public string GetTurnipPrices(List<IGuildUser> users)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sbBuy = new StringBuilder("Turnip Buy Prices").AppendLine();
				StringBuilder sbSell = new StringBuilder("Turnip Sell Prices").AppendLine();
				bool sellUnset = true;
				bool buyUnset = true;

				foreach(Town town in db.Towns.ToList())
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					if (town.TurnipSellPrice != 0)
					{
						if(localMayor != null)
						{
							sbSell.AppendLine($"{town.TownName} (Mayor: {localMayor.Username}) - {town.TurnipSellPrice} bells");
						}
						else
						{
							sbSell.AppendLine($"{town.TownName} - {town.TurnipSellPrice} bells");
							
						}
						sellUnset = false;
					}
					if (town.TurnipBuyPrice != 0)
					{
						if (localMayor != null)
						{
							sbBuy.AppendLine($"{town.TownName} (Mayor: {localMayor.Username}) - {town.TurnipBuyPrice} bells");
						}
						else
						{
							sbBuy.AppendLine($"{town.TownName} - {town.TurnipBuyPrice} bells");
						}
						buyUnset = false;
					}
				};
				if (sellUnset) sbSell = new StringBuilder("No Sell Prices Found.");
				if (buyUnset) sbBuy = new StringBuilder("No Buy Prices Found.");

				return sbBuy.ToString() + Environment.NewLine + sbSell.ToString();
			}
		}

		public string RegisterFruit(ulong userId, string fruitName)
		{
			fruitName = fruitName.ToPascalCase();
			StringBuilder sb = new StringBuilder();
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Fruits).First(t => t.MayorDiscordId == userId);

					if (town.Fruits.FirstOrDefault(f => f.FruitName == fruitName.ToPascalCase()) != null) return "You already registered that fruit, I think";

					town.Fruits.Add(new Fruit() { FruitName = fruitName });
					db.SaveChanges();
					sb.Append($"Ok, registered that your town has {fruitName} trees");
				}
				catch
				{
					sb.Append("Sorry, something went wrong when I was registering that fruit tree for you :-(");
				}
				return sb.ToString();
			}
		}

		public string RegisterTown(ulong userId, string townName)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					db.Towns.Add(new Town()
					{
						MayorDiscordId = userId,
						TownName = townName,
					});
					db.SaveChanges();
					return "Ok, I've registered you as Mayor of " + townName;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return "Sorry, something went wrong while registering your town " + townName;
				}

			}
		}

		public string RegisterTurnipBuyPrice(ulong userId, int turnipPrice)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sb = new StringBuilder();
				try
				{
					if (TownHasTurnipBuyPrice(userId))
					{
						sb.AppendLine("Ok, I've updated your turnip buy price to " + turnipPrice);
					}
					else
					{
						sb.AppendLine("Ok, I've set your turnip buy price to " + turnipPrice);
					}
					db.Towns.First(t => t.MayorDiscordId == userId).TurnipBuyPrice = turnipPrice;
					db.SaveChanges();
				}
				catch
				{
					sb = new StringBuilder("Sorry, something went wrong while I was trying to update your sell price.");
				}
				return sb.ToString();
			}
		}

		public string RegisterTurnipSellPrice(ulong userId, int turnipPrice)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sb = new StringBuilder();
				try
				{
					if (TownHasTurnipSellPrice(userId))
					{
						sb.AppendLine("Ok, I've updated your turnip sell price to " + turnipPrice);
					}
					else
					{
						sb.AppendLine("Ok, I've set your turnip sell price to " + turnipPrice);
					}
					db.Towns.First(t => t.MayorDiscordId == userId).TurnipSellPrice = turnipPrice;
					db.SaveChanges();
				}
				catch
				{
					sb = new StringBuilder("Sorry, something went wrong while I was trying to update your sell price.");
				}
				return sb.ToString();
			}
		}

		public bool TownHasTurnipBuyPrice(ulong userId)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				return db.Towns.FirstOrDefault(t => t.MayorDiscordId == userId).TurnipBuyPrice != 0;
			}
		}

		public bool TownHasTurnipSellPrice(ulong userId)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				return db.Towns.FirstOrDefault(t => t.MayorDiscordId == userId).TurnipSellPrice != 0;
			}
		}

		public bool UserHasTown(ulong userId)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				return db.Towns.FirstOrDefault(t => t.MayorDiscordId == userId) != null;
			}
		}

		public string SetHemisphere(ulong userId, bool isNorthern)
		{
			string hemisphere = isNorthern ? "Northern" : "Southern";
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					db.Towns.First(t => t.MayorDiscordId == userId).NorthernHempisphere = isNorthern ? "n" : "s";
					db.SaveChanges();
					return $"Updated your hemisphere to be the {hemisphere} Hempisphere!";
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
					return "Sorry, something went wrong updating your hemisphere. Try again later.";
				}
			}
		}

		public string RegisterWishlistItem(ulong userId, string itemName)
		{
			using(AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Wishlist).First(t => t.MayorDiscordId == userId);

					if (town.Wishlist.FirstOrDefault(w => w.ItemName == itemName.ToPascalCase()) != null) return "You already registered that item, I think";
					town.Wishlist.Add(new Item() { ItemName = itemName.ToPascalCase() });
					db.SaveChanges();
					return $"Ok, I've added {itemName.ToPascalCase()} to your wishlist!";
				}
				catch
				{
					return "Sorry, something went wrong registering that item.";
				}

			}
		}

		public string RemoveWishlistItemById(ulong userId, int itemId)
		{
			using(AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Wishlist).First(t => t.MayorDiscordId == userId);
					Item item = town.Wishlist.FirstOrDefault(w => w.ItemNum == itemId);
					if (item == null) return "I don't see an item on your wishlist with that ID.";

					town.Wishlist.Remove(item);
					db.SaveChanges();
					return $"Ok, I've removed {item.ItemName} from your wishlist.";
				}
				catch
				{
					return "Sorry, something went wrong trying to remove that item from your wishlist.";
				}
			}
		}

		public string RemoveWishlistItemByName(ulong userId, string itemName)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Wishlist).First(t => t.MayorDiscordId == userId);
					Item item = town.Wishlist.FirstOrDefault(w => w.ItemName == itemName);
					if (item == null) return "I don't see an item on your wishlist with that ID.";

					town.Wishlist.Remove(item);
					db.SaveChanges();
					return $"Ok, I've removed {item.ItemName} from your wishlist.";
				}
				catch
				{
					return "Sorry, something went wrong trying to remove that item from your wishlist.";
				}
			}
		}

		public string GetWishlist(List<IGuildUser> users)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sb = new StringBuilder("Town Name - Wishlist").AppendLine();
				bool wishlistUnset = true;
				foreach (Town town in db.Towns.Include(t => t.Wishlist).ToList())
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					if (town.Wishlist.Count > 0)
					{
						if (localMayor != null)
						{
							sb.Append($"{town.TownName} (Mayor: {localMayor.Username}) - ");
						}
						else
						{
							sb.Append($"{town.TownName} - ");
						}
						foreach (Item item in town.Wishlist)
						{
							sb.Append($"`{item.ItemNum}`|`{item.ItemName}`, ");
						}
						sb.Remove(sb.Length - 2, 2).AppendLine();
						wishlistUnset = false;
					}
				};
				if (wishlistUnset) return "I don't know nothing about no wishlist items in no towns, man.";
				return sb.ToString();
			}
		}

		public string OpenTownBorder(ulong userId, string dodoCode="")
		{
			using(AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.First(t => t.MayorDiscordId == userId);
					town.BorderOpen = true;
					town.DodoCode = dodoCode;
					db.SaveChanges();
					return "Ok, I'll let everyone know so they can stop by!";
				}
				catch
				{
					return "Sorry, something went wrong trying to set your town as open.";
				}
			}
		}

		public string CloseTownBorder(ulong userId)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.First(t => t.MayorDiscordId == userId);
					town.BorderOpen = false;
					db.SaveChanges();
					return "Ok, I'll let everyone know so they don't try and visit!";
				}
				catch
				{
					return "Sorry, something went wrong trying to set your town as open.";
				}
			}
		}

		public string SetNativeFruit(ulong userId, string fruitName)
		{
			using(AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Fruits).First(t => t.MayorDiscordId == userId);
					if (town.Fruits.FirstOrDefault(f => f.FruitName == fruitName.ToPascalCase()) == null) return "Your native fruit has to be registered to your town, first.";

					town.NativeFruit = fruitName.ToPascalCase();
					db.SaveChanges();
					return $"Ok, registered {fruitName.ToPascalCase()} as your native fruit.";
				}
				catch
				{
					return "Sorry, something went wrong trying to set your native fruit.";
				}
			}
		}
	}
}
