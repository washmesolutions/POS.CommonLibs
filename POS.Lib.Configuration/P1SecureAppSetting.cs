using System;
using System.Collections.Generic;
using System.Text;

namespace POS.Lib.Configuration
{
    public class P1SecureAppSetting
    {
        public string TaskProxyUsername { get; set; }
        public string TaskProxyPassword { get; set; }
        public string DnnDummyPassword { get; set; }
        public string EncryptKey { get; set; }
        public string EncryptionKey { get; set; }
    }
}
