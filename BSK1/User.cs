using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class User
    {
        public String Email;
        public String publicRSAKey;

        public User(String Email, String publicRSAKey)
        {
            this.Email = Email;
            this.publicRSAKey = publicRSAKey;
        }


        public override string ToString()
        {
            return Email;
        }

    }
}
