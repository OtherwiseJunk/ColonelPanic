using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class UserState : User
    {
        public int TableFlipPoints { get; set; }
        public bool IsNaughty { get; set; }

        public UserState()
        {
            
        }

        public UserState(string uid, string username) : base(uid, username)
        {
            TableFlipPoints = 0;
            IsNaughty = false;
        }
    }
}
