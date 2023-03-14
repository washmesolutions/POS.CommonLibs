using Microsoft.Extensions.Logging;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public class DBFactory
    {
        #region Private
        private readonly ILogger _logger;
        private readonly ILoggerFactory loggerFactory;

        private static DBFactory instance;
        #endregion

        private DBFactory(ILoggerFactory logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<DBFactory>();
        }

        public AbstractDatabase GetDB(DBType dbType, DBMode dbMode, bool encryptedConnection)
        {
            AbstractDatabase DB = null;
            try
            {
                switch (dbType)
                {
                    case DBType.MsSQL:
                        if (dbMode == DBMode.Meta)
                            DB = new MsSQLDatabase(loggerFactory, dbMode, encryptedConnection);
                        else if (dbMode == DBMode.WAG)
                            DB = new MsSQLDatabase(loggerFactory, dbMode, encryptedConnection);
                        else if (dbMode == DBMode.Entity)
                            DB = new MsSQLEntityDatabase(loggerFactory, dbMode, encryptedConnection);
                        else if (dbMode == DBMode.DBRead)
                            DB = new MsSQLDatabaseRead(loggerFactory, dbMode, encryptedConnection);
                        else if (dbMode == DBMode.Hangfire)
                            DB = new MsSQLDatabase(loggerFactory, dbMode, encryptedConnection);
                        else if (dbMode == DBMode.TransactionTracking)
                            DB = new MsSQLDatabase(loggerFactory, dbMode, encryptedConnection);
                        else if (dbMode == DBMode.All)
                            DB = new CompositeDB(loggerFactory, dbType, dbMode, encryptedConnection);
                        break;
                    case DBType.MongoDB:
                        DB = new MongoAnalyticsDatabase(loggerFactory);
                        break;
                }
                return DB;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetDB error thrown:{0}", ex);
                throw ex;
            }

        }

        public AbstractDatabase GetDB(DBType dbType, DBMode dbMode, int portalId, bool encryptedConnection)
        {
            AbstractDatabase DB = null;
            try
            {
                switch (dbType)
                {
                    case DBType.MsSQL:
                        if (dbMode == DBMode.Portal)
                            DB = new MsSQLDatabase(loggerFactory, dbMode, portalId, encryptedConnection);
                        else if (dbMode == DBMode.WAG)
                            DB = new MsSQLDatabase(loggerFactory, dbMode, portalId, encryptedConnection);
                        else if (dbMode == DBMode.Entity)
                            DB = new MsSQLEntityDatabase(loggerFactory, dbMode, portalId, encryptedConnection);
                        else if (dbMode == DBMode.DBRead)
                            DB = new MsSQLDatabaseRead(loggerFactory, dbMode, portalId, encryptedConnection);
                        else if (dbMode == DBMode.All)
                            DB = new CompositeDB(loggerFactory, dbType, dbMode, portalId, encryptedConnection);
                        break;
                    case DBType.MongoDB:
                        DB = new MongoAnalyticsDatabase(loggerFactory, portalId);
                        break;
                }
                return DB;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetDB error thrown:{0}", ex);
                throw ex;
            }

        }

        public string GetDBConnection(DBType dbType, DBMode dbMode, bool encryptedConnection)
        {
            AbstractDatabase DB = null;
            try
            {
                switch (dbType)
                {
                    case DBType.MsSQL:
                        if (dbMode == DBMode.Hangfire)
                            DB = new MsSQLDatabase(loggerFactory);
                        break;
                }
                return DB.GetConnectionString(dbMode, encryptedConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetDBConnection error thrown:{0}", ex);
                throw ex;
            }
        }

        public DbContext GetEEContext(DBMode dbMode, PersistanceContextType contextType, bool encryptedConnection)
        {
            AbstractDatabase DB = null;
            DbContext context = null;
            try
            {
                if (dbMode == DBMode.EEntity)
                    DB = new MsSQLEntityDatabase(loggerFactory, dbMode, encryptedConnection);

                context = DB.CreateEEContext(dbMode, contextType);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetEEContext error thrown:{0}", ex);
            }
            return context;
        }

        public DbContext GetEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType, bool encryptedConnection)
        {
            AbstractDatabase DB = null;
            DbContext context = null;
            try
            {
                if (dbMode == DBMode.EEntity)
                    DB = new MsSQLEntityDatabase(loggerFactory, dbMode, portalId, encryptedConnection);

                context = DB.CreateEEContext(dbMode, portalId, contextType);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetEEContext error thrown:{0}", ex);
            }
            return context;
        }

        /// <summary>
        /// Create singleton instance from master class
        /// </summary>
        /// <param name="dbType">Database Type</param>
        /// <param name="dbMode">Database Mode</param>
        /// <returns>DBFactory object</returns>
        public static DBFactory CreateDB(ILoggerFactory logger)
        {
            ILoggerFactory loggerFactory = logger;
            ILogger _logger = logger.CreateLogger<DBFactory>();

            try
            {
                lock (typeof(DBFactory))
                {
                    if (instance == null)
                    {
                        instance = new DBFactory(loggerFactory);
                    }
                    return instance;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
