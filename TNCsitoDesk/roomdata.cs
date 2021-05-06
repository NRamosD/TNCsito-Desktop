using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace TNCsitoDesk
{
    class roomdata
    {

        string user;
        string pass;

        public void setUser()
        {
            this.user = Properties.Settings.Default["nameroom"].ToString();
        }
        public string getUser()
        {
            return user;
        }
        public void setPass(string p)
        {
            this.pass = Properties.Settings.Default["passroom"].ToString();
        }
        public string getPass()
        {
            return pass;
        }


        public IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = ConfigurationManager.AppSettings["AUTH"],
            BasePath = ConfigurationManager.AppSettings["BASE_PATH"]
        };

        public void buscarSala()
        {

        }

        




    }
}
