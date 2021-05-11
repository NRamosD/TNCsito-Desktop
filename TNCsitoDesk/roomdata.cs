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
        public IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = ConfigurationManager.AppSettings["AUTH"],
            BasePath = ConfigurationManager.AppSettings["BASE_PATH"]
        };

    }
}
