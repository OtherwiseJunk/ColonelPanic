using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DartsDiscordBots.Database.Constants;
using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Utilities;
using DartsDiscordBots.Modules.AnimalCrossing.Models;
using Discord.Commands;
using DartsDiscordBots.Modules.AnimalCrossing.Interfaces;
using System.ComponentModel;

namespace ColonelPanic.Services
{
	public class AnimalCrossingService : IAnimalCrossingService
	{
		public static DbContextOptionsBuilder<AnimalCrossingContext> OptionsBuilder { get; set; }
		public AnimalCrossingService()
		{
			OptionsBuilder = new DbContextOptionsBuilder<AnimalCrossingContext>();
			OptionsBuilder.UseSqlServer(ConnectionStrings.ConnectionString);
        }

        public string GetFruitList(List<IGuildUser> users)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sb = new StringBuilder("Town Name - Fruits").AppendLine();
				bool fruitUnset = true;
				foreach (Town town in db.Towns.Include(t => t.Fruits).ToList())
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					if (town.Fruits.Count > 0)
					{
						if (localMayor != null)
						{
							string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
							sb.Append($"{town.TownName} (Mayor: {mayorIdentifier}) - ");
						}
						else
						{
							sb.Append($"{town.TownName} - ");
						}
						foreach (Fruit fruit in town.Fruits)
						{
							if (town.NativeFruit == fruit.FruitName)
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

		public Embed GetTownList(List<IGuildUser> users)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				EmbedBuilder builder = new();
				builder.Title = "Town List";
				builder.WithFooter("0");
				var towns = db.Towns.ToList();
                if (towns.Count == 0)
                {
					builder.Description= "Sorry, No Towns Found!";
                }
                foreach (Town town in towns.Take(25))
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					string hemisphere = String.IsNullOrEmpty(town.NorthernHempisphere) ? "" : $"({town.NorthernHempisphere.ToUpper()})";
					string dodoCode = "";
					string borderStatus = $"**Border Open:** `{town.BorderOpen}`";
					if (town.DodoCode != null)
					{
						dodoCode = $"{Environment.NewLine}**Dodo Code**: `{town.DodoCode}`";
					}
                    else if (localMayor != null)
					{
						string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
						builder.AddField($"{town.TownId}: {town.TownName} {hemisphere}", $"{borderStatus}{dodoCode}{Environment.NewLine}**Mayor**: `{mayorIdentifier.ToPascalCase()}`");
					}
					else
					{
                        builder.AddField($"{town.TownId}: {town.TownName} {hemisphere}", $"{borderStatus}{dodoCode}");
					}
				}

				return builder.Build();
			}
		}

		public void SendTurnipPriceList(List<IGuildUser> users, ICommandContext context)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				StringBuilder sbBuy = new StringBuilder("**Turnip Buy Prices**").AppendLine();
				StringBuilder sbSell = new StringBuilder("**Turnip Sell Prices**").AppendLine();
				bool sellUnset = true;
				bool buyUnset = true;
				int buyLoopCount = 0;
				int sellLoopCount = 0;

				foreach (Town town in db.Towns.Include(t => t.BuyPrices).Include(t => t.SellPrices).ToList())
				{
					IGuildUser localMayor = users.FirstOrDefault(u => u.Id == town.MayorDiscordId);
					TurnipPrice latestBuy = null;
					TurnipPrice latestSell = null;
					if (town.BuyPrices.Count > 0) latestBuy = town.BuyPrices.OrderByDescending(price => price.Timestamp).First();
					if (town.SellPrices.Count > 0) latestSell = town.SellPrices.OrderByDescending(price => price.Timestamp).First();
					string dodoString = town.DodoCode != null ? $" Dodo Code: {town.DodoCode}" : "";
					string openNowString = $"**Open Now!{dodoString}**";

					if (latestSell != null)
					{
						if (town.BorderOpen)
						{
							if (localMayor != null)
							{
								string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
								sbSell.AppendLine($"{town.TownName} (Mayor: {mayorIdentifier}) - {latestSell.Price} bells as of {latestSell.Timestamp}.");
								sbSell.AppendLine(openNowString);
							}
							else
							{
								sbSell.AppendLine($"{town.TownName} - {latestSell.Price} bells as of {latestSell.Timestamp}.");
								sbSell.AppendLine(openNowString);
							}
						}
						else
						{
							if (localMayor != null)
							{
								string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
								sbSell.AppendLine($"{town.TownName} (Mayor: {mayorIdentifier}) - {latestSell.Price} bells as of {latestSell.Timestamp}.");
							}
							else
							{
								sbSell.AppendLine($"{town.TownName} - {latestSell.Price} bells as of {latestSell.Timestamp}.");
							}
						}
						sellUnset = false;
						if (sbSell.Length >= 1750)
						{
							context.Channel.SendMessageAsync(sbSell.ToString());
							sbSell = new StringBuilder("**Turnip Sell Prices CONT**").AppendLine();
							sellUnset = true;
							sellLoopCount++;
						}
					}
					if (latestBuy != null)
					{
						if (town.BorderOpen)
						{
							if (localMayor != null)
							{
								string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
								sbBuy.AppendLine($"{town.TownName} (Mayor: {mayorIdentifier}) - {latestBuy.Price} bells as of {latestBuy.Timestamp}.");
								sbBuy.AppendLine(openNowString);
							}
							else
							{
								sbBuy.AppendLine($"{town.TownName} - {latestBuy.Price} bells as of {latestBuy.Timestamp}.");
								sbBuy.AppendLine(openNowString);
							}
						}
						else
						{
							if (localMayor != null)
							{
								string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
								sbBuy.AppendLine($"{town.TownName} (Mayor: {mayorIdentifier}) -  {latestBuy.Price} bells as of {latestBuy.Timestamp}.");
							}
							else
							{
								sbBuy.AppendLine($"{town.TownName} -  {latestBuy.Price} bells as of {latestBuy.Timestamp}.");
							}
						}
						buyUnset = false;
						if (sbBuy.Length >= 1750)
						{
							context.Channel.SendMessageAsync(sbBuy.ToString());
							sbBuy = new StringBuilder("**Turnip Buy Prices CONT**").AppendLine();
							buyUnset = true;
							buyLoopCount++;
						}
					}
				};
				if (sellLoopCount > 1)
				{
					if (sellUnset) sbSell = new StringBuilder("");
				}
				else
				{
					if (sellUnset) sbSell = new StringBuilder("No Sell Prices Found.");
				}
				if (buyLoopCount > 1)
				{
					if (sellUnset) sbBuy = new StringBuilder("");
				}
				else
				{
					if (buyUnset) sbBuy = new StringBuilder("No Buy Prices Found.");
				}
				

				context.Channel.SendMessageAsync(sbSell.ToString());
				context.Channel.SendMessageAsync(sbBuy.ToString());
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
				Town town = db.Towns.First(t => t.MayorDiscordId == userId);
				TurnipBuyPrice todaysBuy = null;
				try
				{
					
					if (town.BuyPrices.Count > 0)
					{
						todaysBuy = town.BuyPrices
								.OrderByDescending(price => price.Timestamp)
								.Where(price => price.Timestamp.Date == DateTime.Now.Date).First();
					}
					if (todaysBuy == null)
					{
						town.BuyPrices.Add(new TurnipBuyPrice(turnipPrice));
						sb.AppendLine("Ok, I've registered your buy price for the day as " + turnipPrice);
					}
					else
					{
						todaysBuy = new TurnipBuyPrice(turnipPrice);
						sb.AppendLine("Ok, I've updated your buy price for the day to " + turnipPrice);
						db.Update(todaysBuy);
					}
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
					TurnipSellPrice todaysSell = null;
					Town town = db.Towns.First(t => t.MayorDiscordId == userId);
					if (town.SellPrices.Count > 0)
					{
						if (DateTime.Now.Hour >= 12)
						{
							todaysSell = town.SellPrices
								.OrderByDescending(price => price.Timestamp)
								.Where(price => price.IsMorningPrice = true).First();
						}
						else
						{
							todaysSell = town.SellPrices
								.OrderByDescending(price => price.Timestamp)
								.Where(price => price.IsMorningPrice = false).First();
						}
					}					
					if (todaysSell == null)
					{
						town.SellPrices.Add(new TurnipSellPrice(turnipPrice));
						sb.AppendLine("Ok, I've set your turnip sell price to " + turnipPrice);
					}
					else
					{
						todaysSell = new TurnipSellPrice(turnipPrice);
						db.Update(todaysSell);
						sb.AppendLine("Ok, I've updated your turnip sell price to " + turnipPrice);
					}
					db.SaveChanges();
				}
				catch
				{
					sb = new StringBuilder("Sorry, something went wrong while I was trying to update your sell price.");
				}
				return sb.ToString();
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
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return "Sorry, something went wrong updating your hemisphere. Try again later.";
				}
			}
		}

		public string RegisterWishlistItem(ulong userId, string itemName)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
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
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
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
							string mayorIdentifier = town.MayorRealName != null ? $"{town.MayorRealName} - {localMayor.Username}" : localMayor.Username;
							sb.Append($"{town.TownName} (Mayor: {mayorIdentifier}) - ");
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

		public string OpenTownBorder(ulong userId, string dodoCode = "")
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
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
					town.DodoCode = null;
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
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Fruits).First(t => t.MayorDiscordId == userId);

					town.NativeFruit = fruitName.ToPascalCase();
					db.SaveChanges();
					if (town.Fruits.FirstOrDefault(f => f.FruitName == fruitName.ToPascalCase()) == null)
					{
						RegisterFruit(userId, fruitName);
						return $"Ok, registered {fruitName.ToPascalCase()} as your native fruit, and added it to your towns fruit list.";
					}
					return $"Ok, registered {fruitName.ToPascalCase()} as your native fruit.";
				}
				catch
				{
					return "Sorry, something went wrong trying to set your native fruit.";
				}
			}
		}

		public string SetRealName(ulong userId, string realName)
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				try
				{
					Town town = db.Towns.Include(t => t.Fruits).First(t => t.MayorDiscordId == userId);
					town.MayorRealName = realName;
					db.SaveChanges();
					return $"Ok, registered {realName.ToPascalCase()} as your real name.";
				}
				catch
				{
					return "Sorry, something went wrong trying to set your real name.";
				}
			}
		}
		public static void Cleanup()
		{
			using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
			{
				db.Towns.AsQueryable().ForEachAsync(town =>
				{
					town.BorderOpen = false;
					town.DodoCode = null;
					db.Update(town);
				}).Wait();
				db.SaveChanges();
			}
		}

		public string GetTurnipStats(ulong id)
		{
			if(GetTown(id) != null)
			{
				using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
				{
					Town town = db.Towns.Include(t => t.BuyPrices).Include(t => t.SellPrices).First(t => t.MayorDiscordId == id);

					StringBuilder sbBuy = new StringBuilder().AppendLine();
					StringBuilder sbSell = new StringBuilder().AppendLine();
					bool sellUnset = true;
					bool buyUnset = true;

					if(town.SellPrices.Count > 0)
					{
						int sMin = town.SellPrices.First().Price;
						int sMax = town.SellPrices.First().Price;
						double sSum = 0;
						double sAvg;

						foreach (TurnipSellPrice price in town.SellPrices)
						{
							if (sMin > price.Price) sMin = price.Price;
							if (sMax < price.Price) sMax = price.Price;

							sSum += price.Price;
						}

						sAvg = Math.Round(sSum / town.SellPrices.Count(),2);
						sbSell.AppendLine($"**Best Sell Price**:{sMax}");
						sbSell.AppendLine($"**Worst Sell Price**:{sMin}");
						sbSell.AppendLine($"**Typical Sell Price**:{sAvg}");
						sbSell.AppendLine($"**Sell Price Count**:{town.SellPrices.Count}");
						sellUnset = false;
					}
					if(town.BuyPrices.Count > 0)
					{
						int bMin = town.BuyPrices.First().Price;
						int bMax = town.BuyPrices.First().Price;
						double bSum = 0;
						double bAvg;

						foreach (TurnipBuyPrice price in town.BuyPrices)
						{
							if (bMin > price.Price) bMin = price.Price;
							if (bMax < price.Price) bMax = price.Price;

							bSum += price.Price;
						}

						bAvg = Math.Round(bSum / town.BuyPrices.Count(),2);
						sbBuy.AppendLine($"**Best Buy Price**:{bMin}");
						sbBuy.AppendLine($"**Worst Buy Price**:{bMax}");
						sbBuy.AppendLine($"**Typical Buy Price**:{bAvg}");
						sbBuy.AppendLine($"**Buy Price Count**:{town.BuyPrices.Count}");
						buyUnset = false;
					}
					if (sellUnset) sbSell = new StringBuilder("No Sell Price Data Found.");
					if (buyUnset) sbBuy = new StringBuilder("No Buy Price Data Found.");

					return sbSell.ToString() + Environment.NewLine + sbBuy.ToString();
				}
			}
			else
			{
				return "Sorry, you have to be a registered mayor to get stats on your town's turnip prices.";
			}
		}

		public string GetTurnipPricesForWeek(ulong id)
		{

			if (GetTown(id) != null)
			{
				using (AnimalCrossingContext db = new AnimalCrossingContext(OptionsBuilder.Options))
				{
					Town town = db.Towns.Include(t => t.BuyPrices).Include(t => t.SellPrices).First(t => t.MayorDiscordId == id);
					DateTime sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
					List<TurnipSellPrice> sellPrices = town.SellPrices.Where(s => s.Timestamp.CompareTo(sunday) >= 0).ToList();
					List<TurnipBuyPrice> buyPrices = town.BuyPrices.Where(s => s.Timestamp.CompareTo(sunday) >= 0).ToList();

					StringBuilder sbBuy = new StringBuilder("Buy Prices").AppendLine();
					StringBuilder sbSell = new StringBuilder("Sell Prices").AppendLine();
					bool sellUnset = true;
					bool buyUnset = true;

					if (sellPrices.Count > 0)
					{
						sellPrices.ForEach(s => {
							sbSell.AppendLine($"{s.Timestamp}: {s.Price}");
						});
						sellUnset = false;
					}
					if (buyPrices.Count > 0)
					{
						buyPrices.ForEach(b => {
							sbBuy.AppendLine($"{b.Timestamp}: {b.Price}");
						});
						buyUnset = false;
					}
					if (sellUnset) sbSell = new StringBuilder("No Sell Price Data Found.");
					if (buyUnset) sbBuy = new StringBuilder("No Buy Price Data Found.");

					return sbSell.ToString() + Environment.NewLine + sbBuy.ToString();
				}
			}
			else
			{
				return "Sorry, you have to be a registered mayor to see a weekly summary on your town's turnip prices.";
			}
		}

        Embed IAnimalCrossingService.GetWishlist(List<IGuildUser> users)
        {
            throw new NotImplementedException();
        }

        Embed IAnimalCrossingService.GetFruitList(List<IGuildUser> users)
        {
            throw new NotImplementedException();
        }

        Embed IAnimalCrossingService.GetTurnipStats(ulong mayorDiscordUserId)
        {
            throw new NotImplementedException();
        }

        Embed IAnimalCrossingService.GetTurnipPricesForWeek(ulong mayorDiscordUserId)
        {
            throw new NotImplementedException();
        }
    }
}
