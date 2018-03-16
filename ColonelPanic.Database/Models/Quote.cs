using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class Quote
    {
        [Key]
        public int QuoteNum { get; set; }
        public string Author { get; set; }
        public string QuoteText { get; set; }

        public Quote()
        {

        }

        public Quote(string author, string text)
        {
            Author = author;
            QuoteText = text;
        }
    }
}
