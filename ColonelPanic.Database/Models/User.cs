using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Models
{
    public class User
    {
        [Key]
        int UserNum;
        public string UserId;
        public string Username;

        public User()
        {

        }
        public User(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }

    }
}
