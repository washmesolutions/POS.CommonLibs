using System;
using System.Collections.Generic;
using System.Text;

namespace POS.Lib.Configuration
{
    public class P1MongoDBConnectionString
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
