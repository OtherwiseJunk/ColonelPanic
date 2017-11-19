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
        public int UserNum { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        

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
