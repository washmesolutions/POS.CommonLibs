using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace POS.Lib.Configuration
{
    public class ConfigWrapper
    {
        public IConfigurationRoot RootConfig { get; set; }
        public ConfigWrapper()
        {
            var envname = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            var pathVariable = Path.Combine(Directory.GetCurrentDirectory(), "appsettings." + envname + ".json");

            configurationBuilder.AddJsonFile(path, false);
            configurationBuilder.AddJsonFile(pathVariable, optional: true);
            RootConfig = configurationBuilder.Build();
        }

        public string GetAppSettingsItem(string name)
        {

            try
            {
                //config.GetAppSettingsItem("PageShellConfigurations:AppSettings:PropFile"));
                return RootConfig.GetSection("PageShellConfigurations").GetSection("AppSettings").GetSection(name)?.Value;
            }
            catch (Exception)
            {
                return "";
            }
        }



        public NameValueCollection GetDBCollection(string key)
        {
            P1LPConnectionString constr = new P1LPConnectionString();

            constr = RootConfig.GetSection("PageShellConfigurations").GetSection("MsSQL").GetSection(key).Get<P1LPConnectionString>();

            //this will pick docker connection string 
            var localdbserver = Environment.GetEnvironmentVariable("localIP");

            NameValueCollection p1LpDBString = new NameValueCollection();
            p1LpDBString.Add("integratedSecurity", constr.IntegratedSecurity);

            if (!string.IsNullOrEmpty(localdbserver))
            {
                p1LpDBString.Add("server", localdbserver);
            }
            else
            {
                p1LpDBString.Add("server", constr.Server);
            }

            p1LpDBString.Add("database", constr.DataBase);
            p1LpDBString.Add("connecttimeout", constr.ConnectTimeout);
            p1LpDBString.Add("connectionminpoolsize", constr.ConnectionMinPoolSize);
            p1LpDBString.Add("username", constr.UserName);
            p1LpDBString.Add("password", constr.Password);

            return p1LpDBString;
        }

        public NameValueCollection GetP1SecureAppSetting()
        {
            P1SecureAppSetting constr = new P1SecureAppSetting();

            constr = RootConfig.GetSection("PageShellConfigurations").GetSection("secureAppSettings").Get<P1SecureAppSetting>();

            NameValueCollection p1LpDBString = new NameValueCollection();
            p1LpDBString.Add("DnnDummyPassword", constr.DnnDummyPassword);
            p1LpDBString.Add("EncryptionKey", constr.EncryptionKey);
            p1LpDBString.Add("EncryptKey", constr.EncryptKey);
            p1LpDBString.Add("TaskProxyPassword", constr.TaskProxyPassword);
            p1LpDBString.Add("TaskProxyUsername", constr.TaskProxyUsername);

            return p1LpDBString;
        }

        public NameValueCollection GetMongoDBCollection()
        {
            P1MongoDBConnectionString constr = RootConfig.GetSection("PageShellConfigurations").GetSection("MongoDB").Get<P1MongoDBConnectionString>();

            if (constr != null)
            {
                NameValueCollection p1MongoDBString = new NameValueCollection();
                p1MongoDBString.Add("host", constr.Host);
                p1MongoDBString.Add("port", constr.Port);
                p1MongoDBString.Add("username", constr.Username);
                p1MongoDBString.Add("password", constr.Password);
                p1MongoDBString.Add("database", constr.Database);
                p1MongoDBString.Add("collection", constr.Collection);

                return p1MongoDBString;
            }

            return null;
        }


    }
}
