using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{

    public class QuoteModule : ModuleBase
    {
        public static string QuoteFormat = @"""{0}""

-{1}";
        [Command("newquote"), Summary(@"$newquote ""Author"" ""Quote""")]
        public async Task NewQuote(string author, string quote)
        {
            QuoteHandler.NewQuote(author, quote);
            await Context.Channel.SendMessageAsync(":thumbsup:");
        }

        [Command("listauthors"), Summary("Lists the available authors")]
        public async Task ListAuthors()
        {
            var authorList = QuoteHandler.GetAuthorList();
            string msg = "";
            foreach (string author in authorList)
            {
                msg += author + Environment.NewLine;
                if (msg.Length > 1950)
                {
                    var user = Context.User as SocketUser;
                    await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(msg);
                    msg = "";                       
                }
            }
            if (msg != "")
            {
                var user = Context.User as SocketUser;
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(msg);
                msg = "";
            }
        }

        [Command("listquotes")]
        public async Task ListQuotes([Remainder]string author = null)
        {
            if (author != null)
            {
                var quoteList = QuoteHandler.GetAuthorsQuotes(author);
                if (quoteList.Count > 0)
                {
                    string msg = "";
                    foreach (Quote quote in quoteList.OrderBy( q => q.Author))
                    {
                        msg += quote.QuoteNum + ". " + quote.Author + " | " + quote.QuoteText + Environment.NewLine;
                        if (msg.Length > 1900)
                        {
                            var user = Context.User as SocketUser;
                            await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(msg);
                            msg = "";
                        }
                    }
                    if (msg != "")
                    {
                        var user = Context.User as SocketUser;
                        await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(msg);
                        msg = "";
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("I don't have quotes for that author, bub.");
                }
            }
            else
            {
                var quoteList = QuoteHandler.GetQuotes();
            }
        }

        [Command("quote"), Summary("Gets a random quote. Gets a random quote for the specified author if provided.")]
        public async Task GetQuote([Remainder]string author = null)
        {
            
            if (author != null)
            {
                if (QuoteHandler.AuthorExists(author))
                {
                    var quote = QuoteHandler.GetRandomAuthorQuote(author);
                    await Context.Channel.SendMessageAsync(String.Format(QuoteFormat, quote.QuoteText, quote.Author));
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Sorry, I couldn't find that author.");
                }
            }
            else
            {
                var quote = QuoteHandler.GetRandomQuote();
                await Context.Channel.SendMessageAsync(String.Format(QuoteFormat, quote.QuoteText, quote.Author));
            }
            
        }

        [Command("deletequote")]
        public async Task DeleteQuote([Remainder]int quoteNum)
        {
            if (QuoteHandler.DeleteQuote(quoteNum))
            {
                await Context.Channel.SendMessageAsync(":thumbsup:");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Sorry, I couldn't find that quote.");
            }
        }
    }
}
