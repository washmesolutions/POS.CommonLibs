using Microsoft.Extensions.Logging;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public class CompositeDB : AbstractDatabase
    {
        #region Private properties
        private readonly ILogger _logger;
        private readonly ILoggerFactory loggerFactory;

        private List<AbstractDatabase> DBPool = new List<AbstractDatabase>();
        private AbstractDatabase DB1;
        private AbstractDatabase DB2;
        private AbstractDatabase DB3;
        private int PortalId = -1;
        private DBMode Mode = DBMode.Portal;
        #endregion

        #region Contructors
        public CompositeDB(ILoggerFactory logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<CompositeDB>();

            this.Mode = DBMode.Meta;
        }

        public CompositeDB(ILoggerFactory logger, DBMode mode)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<CompositeDB>();

            this.Mode = mode;
        }

        public CompositeDB(ILoggerFactory logger, int portalId)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<CompositeDB>();

            this.PortalId = portalId;
            this.Mode = DBMode.Portal;
        }
        #endregion


        /// <summary>
        /// Composite all Databases
        /// </summary>
        /// <param name="dbType">Database Type</param>
        /// <param name="dbMode">Database Mode</param>
        public CompositeDB(ILoggerFactory logger, DBType dbType, DBMode dbMode, bool encryptedConnection)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<CompositeDB>();

            try
            {
                switch (dbType)
                {
                    case DBType.MsSQL:
                        if (dbMode == DBMode.Portal)
                        {
                            DB1 = DBFactory.CreateDB(loggerFactory).GetDB(dbType, dbMode, encryptedConnection);
                            DBPool.Add(DB1);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CompositeDB error thrown:{0}", ex);
                throw ex;
            }
        }

        public CompositeDB(ILoggerFactory logger, DBType dbType, DBMode dbMode, int portalId, bool encryptedConnection)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<CompositeDB>();

            try
            {
                switch (dbType)
                {
                    case DBType.MsSQL:
                        if (dbMode == DBMode.Portal)
                        {
                            DB1 = DBFactory.CreateDB(loggerFactory).GetDB(dbType, dbMode, portalId, encryptedConnection);
                            DBPool.Add(DB1);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CompositeDB error thrown:{0}", ex);
                throw ex;
            }
        }

        #region AbstractDatabase Members

        protected override void CreateDBSets(DBMode dbMOde, bool encryptedConnection)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDBSets(DBMode dbMOde, int portalid, bool encryptedConnection)
        {
            throw new NotImplementedException();
        }

        public override void SetSP(string name)
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.SetSP(name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SetSP error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void SetParamCount(int count)
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.SetParamCount(count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SetParamCount error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void AddParameter(string paramName, object value, DataType paramType)
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.AddParameter(paramName, value, paramType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("AddParameter error thrown:{0}", ex);
                throw ex;
            }
        }

        protected override bool OpenConnection(string conString)
        {
            throw new NotImplementedException();
        }

        public override void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public override int[] CallSP()
        {
            try
            {
                int[] result = new int[3];
                int k = 0;
                foreach (AbstractDatabase DB in DBPool)
                {
                    result[k] = DB.CallSP()[0];
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("CallSP error thrown:{0}", ex);
                throw ex;
            }
        }

        public override IDataReader[] CallSPWithDataSet(Timeout timeOut)
        {
            try
            {
                IDataReader[] result = new IDataReader[3];
                int k = 0;
                foreach (AbstractDatabase DB in DBPool)
                {
                    result[k] = DB.CallSPWithDataSet(timeOut)[0];
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("CallSPWithDataSet error thrown:{0}", ex);
                throw ex;
            }
        }

        public override object[] CallSPWithScaler()
        {
            try
            {
                object[] result = new object[3];
                int k = 0;
                foreach (AbstractDatabase DB in DBPool)
                {
                    result[k] = DB.CallSPWithScaler()[0];
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("CallSPWithScaler error thrown:{0}", ex);
                throw ex;
            }
        }

        public override int[] ExecuteQuery(ExecuteQueryObj executeObj)
        {
            try
            {
                int[] result = new int[3];
                int k = 0;
                foreach (AbstractDatabase DB in DBPool)
                {
                    result[k] = DB.ExecuteQuery(executeObj)[0];
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteQuery error thrown:{0}", ex);
                throw ex;
            }
        }

        public override IDataReader[] ExecuteQueryWithIDataReader(string sql, List<SqlParameter> sqlParams)
        {
            try
            {
                IDataReader[] result = new IDataReader[3];
                int k = 0;
                foreach (AbstractDatabase DB in DBPool)
                {
                    result[k] = DB.ExecuteQueryWithIDataReader(sql, sqlParams)[0];
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteQueryWithIDataReader error thrown:{0}", ex);
                throw ex;
            }
        }

        public override object[] ExecuteQueryWithScalar(ExecuteQueryObj executeObj)
        {
            try
            {
                object[] result = new object[3];
                int k = 0;
                foreach (AbstractDatabase DB in DBPool)
                {
                    result[k] = DB.ExecuteQueryWithScalar(executeObj)[0];
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteQueryWithScalar error thrown:{0}", ex);
                throw ex;
            }
        }

        public override DataSet ExecuteQueryWithDataSet(string sql, List<SqlParameter> sqlParams)
        {
            //ToDo: 
            return null;
        }

        public override void BeginTran()
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.BeginTran();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("BeginTran error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void BeginTran(System.Data.IsolationLevel transactionIsolation)
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.BeginTran(transactionIsolation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("BeginTran error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void CommitTran()
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.CommitTran();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CommitTran error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void RollbackTran()
        {
            try
            {
                foreach (AbstractDatabase DB in DBPool)
                {
                    DB.RollbackTran();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RollbackTran error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void saveBulkData(ExecuteQueryObj eqo)
        {
            throw new NotImplementedException();
        }

        public override void saveBulkDataInTransaction(string tableName, DataTable dataTable)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override DataSet ExecuteQueryWithDataSetForReports(string sql, List<SqlParameter> sqlParams, string tableName)
        {
            throw new NotImplementedException();
        }

        public override DbContext CreateContext(PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override int TransferCSVAsBulk(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns)
        {
            throw new NotImplementedException();
        }

        public override int CreateTableFromCSV(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns)
        {
            throw new NotImplementedException();
        }

        public override int BatchWiseBulkSave(ExecuteQueryObj eqo)
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
            //NotImplementedException
        }

        public override void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode, int portalId)
        {
            //NotImplementedException
        }

        public override void GetEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            //NotImplementedException
        }

        public override void GetEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        {
            //NotImplementedException
        }
        public override DbContext CreateEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override DbContext CreateEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
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

        //public override DbContext GetEEContext(DBMode dbMode, PersistanceContextType contextType)
        //{
        //    throw new NotImplementedException();
        //}

        //public override DbContext GetEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
