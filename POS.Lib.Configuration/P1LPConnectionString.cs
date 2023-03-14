using System;
using System.Collections.Generic;
using System.Text;

namespace POS.Lib.Configuration
{
    public class P1LPConnectionString
    {
        public string Server { get; set; }
        public string DataBase { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IntegratedSecurity { get; set; }
        public string ConnectTimeout { get; set; }
        public string ConnectionMinPoolSize { get; set; }
    }
}
