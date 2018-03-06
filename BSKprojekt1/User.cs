using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    class User
    {
        public String Email;
        public String SessionKey;

        public User(String Email, String SessionKey)
        {
            this.Email = Email;
            this.SessionKey = SessionKey;
        } 

        
    }
}
