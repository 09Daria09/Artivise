using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artivise.Model
{
    public class UserData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserData() { }
        public UserData(string userName, string email, string password, string firstName, string lastName)
        {
            UserName = userName;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }

    }
}

