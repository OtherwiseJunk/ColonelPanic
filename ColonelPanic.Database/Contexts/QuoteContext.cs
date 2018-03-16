using ColonelPanic.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DartsDiscordBots.Utilities;

namespace ColonelPanic.Database.Contexts
{
    public class QuoteContext : DbContext
    {
        public QuoteContext() : base("name=BetaDB") { }
            
        public DbSet<Quote> Quotes { get; set; }
    }

    public class QuoteHandler
    {
        public static void NewQuote(string author, string text) {
            using (QuoteContext db = new QuoteContext())
            {
                db.Quotes.Add(new Quote(author.ToLower(), text));
                db.SaveChanges();
            }
        }

        public static List<string> GetAuthorList()
        {
            List<string> authors = new List<string>();
            using (QuoteContext db = new QuoteContext())
            {
                foreach(Quote quote in db.Quotes)
                {
                    if (!authors.Contains(quote.Author))
                    {
                        authors.Add(quote.Author);
                    }
                }
            }
            return authors;
        }

        public static bool AuthorExists(string author)
        {
            using(QuoteContext db = new QuoteContext())
            {
                return db.Quotes.FirstOrDefault(q => q.Author == author.ToLower()) != null;
            }
        }

        public static Quote GetQuoteByNum(int quoteNum)
        {
            using (QuoteContext db = new QuoteContext())
            {
                return db.Quotes.FirstOrDefault(q => q.QuoteNum == quoteNum); 
            }
        }

        public static Quote GetRandomAuthorQuote(string author) {
            using (QuoteContext db = new QuoteContext())
            {
                return db.Quotes.Where(q => q.Author == author.ToLower()).ToList().GetRandom();
            }
        }

        public static Quote GetRandomQuote() {
            using (QuoteContext db = new QuoteContext())
            {
                return db.Quotes.ToList().GetRandom();
            }
        }

        public static List<Quote> GetAuthorsQuotes(string Author)
        {
            using (QuoteContext db = new QuoteContext())
            {
                return db.Quotes.Where(q => q.Author == Author.ToLower()).ToList();
            }
        }

        public static List<Quote> GetQuotes()
        {
            using (QuoteContext db = new QuoteContext())
            {
                return db.Quotes.ToList();
            }
        }

        public static bool DeleteQuote(int quoteNum)
        {
            using (QuoteContext db = new QuoteContext())
            {
                var quote = GetQuoteByNum(quoteNum);
                if (quote == null)
                {
                    return false;
                }
                else
                {
                    db.Quotes.Remove(quote);
                    db.SaveChanges();
                    return true;
                }
            }
        }
    }
}
