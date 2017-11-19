using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class TrustedUser : User
    {
        public TrustedUser()
        {

        }
        public TrustedUser(string uid, string username) : base(uid,username)
        {

        }
    }
}
