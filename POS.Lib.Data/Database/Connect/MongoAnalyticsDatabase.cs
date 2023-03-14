using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using POS.Lib.Configuration;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public class MongoAnalyticsDatabase : AbstractDatabase
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _connString;
        public readonly string _collectionName;
        private readonly string _databaseName;

        private IMongoDatabase _mongoDatabase { get; set; }
        private MongoClient _dbClient;

        private ConfigWrapper _config;
        int portalId = -1;

        public MongoAnalyticsDatabase(ILoggerFactory logger)
        {
            _loggerFactory = logger;
            _logger = _loggerFactory.CreateLogger<MongoAnalyticsDatabase>();
            _config = new ConfigWrapper();

            NameValueCollection section = _config.GetMongoDBCollection();
            _databaseName = section.GetValues("database")[0];
            _collectionName = section.GetValues("collection")[0];
            _connString = GetConnectionString();
            OpenConnection(_connString);
        }

        public MongoAnalyticsDatabase(ILoggerFactory logger, int portalId)
        {
            _loggerFactory = logger;
            _logger = logger.CreateLogger<MongoAnalyticsDatabase>();
            _config = new ConfigWrapper();
            this.portalId = portalId;

            NameValueCollection section = _config.GetMongoDBCollection();
            _databaseName = section.GetValues("database")[0] + this.portalId;
            _collectionName = section.GetValues("collection")[0];
            _connString = GetConnectionString();
            OpenConnection(_connString);
        }

        public IMongoDatabase GetMongoDatabase()
        {
            return _mongoDatabase;
        }

        public IMongoDatabase GetMongoDatabase(string databaseName)
        {
            return _mongoDatabase = _dbClient.GetDatabase(_databaseName);
        }

        public string GetCollectionName()
        {
            return _collectionName;
        }

        public string GetConnectionString()
        {
            P1EncryptionHandler encryptionHandler = new P1EncryptionHandler();
            NameValueCollection section = _config.GetMongoDBCollection();
            try
            {
                var host = section.GetValues("host")[0];
                var port = section.GetValues("port")[0];
                var username = section.GetValues("username")[0];
                username = encryptionHandler.DecryptText(username);
                var password = section.GetValues("password")[0];
                password = encryptionHandler.DecryptText(password);

                string connString;
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    connString = string.Format("mongodb://{0}:{1}/",
                        host ?? "localhost",
                        port ?? "27017");
                }
                else
                {
                    connString = string.Format("mongodb://{2}:{3}@{0}:{1}/",
                        host ?? "localhost",
                        port ?? "27017",
                        username,
                        password);
                }

                connString = string.IsNullOrEmpty(port) ?
                    string.Concat(connString.Replace("mongodb://", "mongodb+srv://").Replace(":27017", ""), "?retryWrites=true&w=majority") :
                    connString;

                return connString;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetConnectionString error thrown:{0}", ex);
                throw ex;
            }
        }

        protected override bool OpenConnection(string conString)
        {
            try
            {
                _dbClient = new MongoClient(_connString);
                _mongoDatabase = _dbClient.GetDatabase(_databaseName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenConnection error thrown:{0}", ex);
                return false;
            }
        }

        #region NOT IMPLEMENTED

        public override void AddParameter(string paramName, object value, DataType paramType)
        {
            throw new NotImplementedException();
        }

        public override int BatchWiseBulkSave(ExecuteQueryObj eqo)
        {
            throw new NotImplementedException();
        }

        public override void BeginTran()
        {
            throw new NotImplementedException();
        }

        public override void BeginTran(IsolationLevel transactionIsolation)
        {
            throw new NotImplementedException();
        }

        public override int[] CallSP()
        {
            throw new NotImplementedException();
        }

        public override IDataReader[] CallSPWithDataSet(Timeout timeOut)
        {
            throw new NotImplementedException();
        }

        public override object[] CallSPWithScaler()
        {
            throw new NotImplementedException();
        }

        public override void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public override void CommitTran()
        {
            throw new NotImplementedException();
        }

        public override DbContext CreateContext(PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override DbContext CreateEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override DbContext CreateEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override int CreateTableFromCSV(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns)
        {
            throw new NotImplementedException();
        }

        public override int[] ExecuteQuery(ExecuteQueryObj executeObj)
        {
            throw new NotImplementedException();
        }

        public override DataSet ExecuteQueryWithDataSet(string sql, List<SqlParameter> sqlParams)
        {
            throw new NotImplementedException();
        }

        public override DataSet ExecuteQueryWithDataSetForReports(string sql, List<SqlParameter> sqlParams, string tableName)
        {
            throw new NotImplementedException();
        }

        public override IDataReader[] ExecuteQueryWithIDataReader(string sql, List<SqlParameter> sqlParams)
        {
            throw new NotImplementedException();
        }

        public override object[] ExecuteQueryWithScalar(ExecuteQueryObj executeObj)
        {
            throw new NotImplementedException();
        }

        public override string GetConnectionString(DBMode dbMode, bool encryptedConnection)
        {
            throw new NotImplementedException();
        }

        public override string GetConnectionString(DBMode dbMode, int portalId, bool encryptedConnection)
        {
            throw new NotImplementedException();
        }

        public override void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode)
        {
            throw new NotImplementedException();
        }

        public override void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode, int portalId)
        {
            throw new NotImplementedException();
        }

        public override void GetEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override void GetEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override void RollbackTran()
        {
            throw new NotImplementedException();
        }

        public override void saveBulkData(ExecuteQueryObj eqo)
        {
            throw new NotImplementedException();
        }

        public override void saveBulkDataInTransaction(string tableName, DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public override void SetParamCount(int count)
        {
            throw new NotImplementedException();
        }

        public override void SetSP(string name)
        {
            throw new NotImplementedException();
        }

        public override int TransferCSVAsBulk(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDBSets(DBMode dbMode, bool encryptedConnection)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDBSets(DBMode dbMode, int portalId, bool encryptedConnection)
        {
            throw new NotImplementedException();
        }

        public override void MergeTableData(string SourceTableName, string TargetTableName, List<string> matchingColumns, List<string> updatingColumns)
        {
            throw new NotImplementedException();
        }

        public override void CreateTable(string tableName, DataColumnCollection columns)
        {
            throw new NotImplementedException();
        }

        public override void DropTable(string tableName)
        {
            throw new NotImplementedException();
        }



        #endregion NOT IMPLEMENTED
    }
}
