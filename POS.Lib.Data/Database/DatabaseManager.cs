using Microsoft.Extensions.Logging;
using POS.Lib.Data.Database.Connect;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace POS.Lib.Data.Database
{
    public class DatabaseManager
    {
        #region Private properties
        private readonly ILogger _logger;
        private readonly ILoggerFactory loggerFactory;

        private int PortalId = -1;
        private DBMode Mode = DBMode.Portal;

        #endregion
        #region Contructors

        public DatabaseManager(ILoggerFactory logger)
        {
            this.Mode = DBMode.Meta;

            loggerFactory = logger;
            _logger = logger.CreateLogger<DatabaseManager>();

        }

        public DatabaseManager(ILoggerFactory logger, DBMode mode)
        {
            this.Mode = mode;

            loggerFactory = logger;
            _logger = logger.CreateLogger<DatabaseManager>();

        }

        public DatabaseManager(ILoggerFactory logger, int portalId)
        {
            this.PortalId = portalId;
            this.Mode = DBMode.Portal;

            loggerFactory = logger;
            _logger = logger.CreateLogger<DatabaseManager>();

        }

        public DatabaseManager(ILoggerFactory logger, DBMode mode, int portalId)
        {
            this.PortalId = portalId;
            this.Mode = mode;

            loggerFactory = logger;
            _logger = logger.CreateLogger<DatabaseManager>();

        }
        #endregion
        #region Implementation

        public AbstractDatabase GetDatabase(bool encryptedConnection = false)
        {

            try
            {
                return PortalId != -1 ? DBFactory.CreateDB(loggerFactory).GetDB(DBType.MsSQL, this.Mode, this.PortalId, encryptedConnection) : DBFactory.CreateDB(loggerFactory).GetDB(DBType.MsSQL, this.Mode, encryptedConnection);
            }
            catch (Exception ex)
            {
                //_sci.Error(__logDetails, ex);
                return null;
            }
        }

        public AbstractDatabase GetDatabase(DBType dbType, bool encryptedConnection = false)
        {

            try
            {
                return PortalId != -1 ? DBFactory.CreateDB(loggerFactory).GetDB(dbType, this.Mode, this.PortalId, encryptedConnection) : DBFactory.CreateDB(loggerFactory).GetDB(dbType, this.Mode, encryptedConnection);
            }
            catch (Exception ex)
            {
                //_sci.Error(__logDetails, ex);
                return null;
            }
        }

        public string GetConnectionString(bool encryptedConnection = false)
        {
            string connectionStr = string.Empty;
            try
            {
                connectionStr = DBFactory.CreateDB(loggerFactory).GetDBConnection(DBType.MsSQL, this.Mode, encryptedConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetConnectionString error thrown:{0}", ex);
            }
            return connectionStr;
        }

        public DbContext GetEEContext(PersistanceContextType contextType, bool encryptedConnection = false)
        {
            DbContext context = null;
            try
            {
                context = PortalId != -1 ? DBFactory.CreateDB(loggerFactory).GetEEContext(this.Mode, this.PortalId, contextType, encryptedConnection) : DBFactory.CreateDB(loggerFactory).GetEEContext(this.Mode, contextType, encryptedConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetEEContext error thrown:{0}", ex);
            }
            return context;
        }

        #endregion
    }
}
