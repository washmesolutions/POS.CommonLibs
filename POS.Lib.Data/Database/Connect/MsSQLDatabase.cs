using Microsoft.Extensions.Logging;
using POS.Lib.Configuration;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public class MsSQLDatabase : AbstractDatabase
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory loggerFactory;

        private ConfigWrapper config = new ConfigWrapper();
        //protected string spname;
        //protected int paramcount;
        protected SqlConnection msSqlCon;
        protected string conString;
        private SqlTransaction msSQLTran;

        protected SqlParameter[] paraList;
        protected int index = 0;
        int portalId = -1;
        //public int ExecutionMode;

        public MsSQLDatabase(ILoggerFactory logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLDatabase>();
        }

        public MsSQLDatabase(ILoggerFactory logger, DBMode dbMode, bool encryptedConnection)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLDatabase>();

            CreateDBSets(dbMode, encryptedConnection);
        }

        public MsSQLDatabase(ILoggerFactory logger, DBMode dbMode, int portalId, bool encryptedConnection)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLDatabase>();

            this.portalId = portalId;
            CreateDBSets(dbMode, portalId, encryptedConnection);
        }

        public DBFactory DBFactory
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        #region AbsractDatabase Members

        public override string GetConnectionString(DBMode dbMode, bool encryptedConnection)
        {
            NameValueCollection section = new NameValueCollection();
            P1EncryptionHandler encryptionHandler = new P1EncryptionHandler();
            ConfigWrapper wrapper = new ConfigWrapper();
            try
            {
                switch (dbMode)
                {
                    case DBMode.Meta: section = wrapper.GetDBCollection("MetaDB"); break;
                    case DBMode.WAG: section = wrapper.GetDBCollection("WAG"); break;
                    case DBMode.Entity: section = wrapper.GetDBCollection("EMetaDB"); break;
                    case DBMode.Hangfire: section = wrapper.GetDBCollection("Hangfire"); break;
                    case DBMode.DBRead: section = wrapper.GetDBCollection("DBRead"); break;
                    case DBMode.TransactionTracking: section = wrapper.GetDBCollection("TTrackingDB"); break;
                    default: section = wrapper.GetDBCollection("MetaDB"); break;
                }

                string ConnString = "";
                string integratedSecurity = section.GetValues("integratedSecurity")[0];
                string Server = section.GetValues("server")[0];
                string Database = section.GetValues("database")[0];
                string ConnectTimeOut = section.GetValues("connecttimeout") == null ? "" : section.GetValues("connecttimeout")[0];
                string Connectionminpoolsize = section.GetValues("connectionminpoolsize") == null ? "" : section.GetValues("connectionminpoolsize")[0];
                string Connectionmaxpoolsize = section.GetValues("connectionmaxpoolsize") == null ? "" : section.GetValues("connectionmaxpoolsize")[0];
                var alwaysEncryptionEnabled = section.GetValues("alwaysEncryptionEnabled") == null ? false : bool.Parse(section.GetValues("alwaysEncryptionEnabled")[0]);

                if (bool.Parse(integratedSecurity))
                {
                    ConnString = "Server=" + Server + ";database=" + Database + ";Integrated Security=True";
                }
                else
                {
                    string Username = section.GetValues("username")[0];
                    string Password = section.GetValues("password")[0];
                    Password = encryptionHandler.DecryptText(Password);
                    ConnString = "Server=" + Server + ";database=" + Database + ";UID=" + Username + ";PWD=" + Password;
                }

                if (!string.IsNullOrEmpty(ConnectTimeOut))
                {
                    ConnString += ";Connect Timeout=" + ConnectTimeOut;
                }

                if (!string.IsNullOrEmpty(Connectionminpoolsize))
                {
                    ConnString += ";Min Pool Size=" + Connectionminpoolsize;
                }

                if (!string.IsNullOrEmpty(Connectionmaxpoolsize))
                {
                    ConnString += ";Max Pool Size=" + Connectionmaxpoolsize;
                }

                if (DBMode.Entity == dbMode)
                {
                    ConnString += ";MultipleActiveResultSets = True";
                }

                if (encryptedConnection && alwaysEncryptionEnabled)
                {
                    ConnString += ";Column Encryption Setting = Enabled";
                }
                return ConnString;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetConnectionString error thrown:{0}", ex);
                throw ex;
            }
        }

        public override string GetConnectionString(DBMode dbMode, int portalId, bool encryptedConnection)
        {
            NameValueCollection section = new NameValueCollection();
            P1EncryptionHandler encryptionHandler = new P1EncryptionHandler();
            ConfigWrapper wrapper = new ConfigWrapper();
            //CustomInformation cusInfo = new CustomInformation();
            //cusInfo.setInfoField("dbMode : " + dbMode);
            //sci.customInfor(_logDetails, cusInfo);

            try
            {
                switch (dbMode)
                {
                    case DBMode.Portal: section = wrapper.GetDBCollection("DBP" + portalId); break;///section = (NameValueCollection)ConfigurationManager.GetSection("MsSQL/DBP" + portalId); break;
                    case DBMode.WAG: section = wrapper.GetDBCollection("WAG"); break;
                    case DBMode.Entity: section = wrapper.GetDBCollection("EDBP" + portalId); break;
                    case DBMode.DBRead: section = wrapper.GetDBCollection("DBReadP" + portalId); break;
                    default: section = wrapper.GetDBCollection("DBP" + portalId); break;
                }

                string ConnString = "";
                string integratedSecurity = section.GetValues("integratedSecurity")[0];
                string Server = section.GetValues("server")[0];
                string Database = section.GetValues("database")[0];
                string ConnectTimeOut = section.GetValues("connecttimeout") == null ? "" : section.GetValues("connecttimeout")[0];
                string Connectionminpoolsize = section.GetValues("connectionminpoolsize") == null ? "" : section.GetValues("connectionminpoolsize")[0];
                string Connectionmaxpoolsize = section.GetValues("connectionmaxpoolsize") == null ? "" : section.GetValues("connectionmaxpoolsize")[0];
                string NetworkLibrary = section.GetValues("networklibrary") == null ? "" : section.GetValues("networklibrary")[0];
                var alwaysEncryptionEnabled = section.GetValues("alwaysEncryptionEnabled") == null ? false : bool.Parse(section.GetValues("alwaysEncryptionEnabled")[0]);

                if (bool.Parse(integratedSecurity))
                {
                    ConnString = "Server=" + Server + ";database=" + Database + ";Integrated Security=True";
                }
                else
                {
                    string Username = section.GetValues("username")[0];
                    string Password = section.GetValues("password")[0];
                    Password = encryptionHandler.DecryptText(Password);
                    ConnString = "Server=" + Server + ";database=" + Database + ";UID=" + Username + ";PWD=" + Password;
                }

                if (!string.IsNullOrEmpty(ConnectTimeOut))
                {
                    ConnString += ";Connect Timeout=" + ConnectTimeOut;
                }

                if (!string.IsNullOrEmpty(Connectionminpoolsize))
                {
                    ConnString += ";Min Pool Size=" + Connectionminpoolsize;
                }

                if (!string.IsNullOrEmpty(Connectionmaxpoolsize))
                {
                    ConnString += ";Max Pool Size=" + Connectionmaxpoolsize;
                }
                if (DBMode.Entity == dbMode)
                {
                    ConnString += ";MultipleActiveResultSets = True";
                }

                if (!string.IsNullOrEmpty(NetworkLibrary))
                {
                    ConnString += ";Network Library=" + NetworkLibrary;
                }

                if (encryptedConnection && alwaysEncryptionEnabled)
                {
                    ConnString += ";Column Encryption Setting = Enabled";
                }
                return ConnString;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetConnectionString error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode)
        {
            try
            {
                NameValueCollection section = (NameValueCollection)config.GetDBCollection("EMetaDB");

                string Server = section.GetValues("server")[0];
                string Database = section.GetValues("database")[0];
                string entityref = pcTextType + "." + pcTextType;

                string connString = @"data source=" + Server +
                                     ";initial catalog=" + Database +
                                     ";integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;";

                // Build the MetaData... feel free to copy/paste it from the connection string in the config file.
                EntityConnectionStringBuilder esb = new EntityConnectionStringBuilder();

                esb.Metadata = "res://*/ORM." + entityref + ".csdl|res://*/ORM." + entityref + ".ssdl|res://*/ORM." + entityref + ".msl";

                esb.Provider = "System.Data.SqlClient";
                esb.ProviderConnectionString = connString;

                conString = esb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("GetEEConnectionString error thrown:{0}", ex);
            }
        }

        public override void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode, int portalId)
        {
            try
            {
                NameValueCollection section = (NameValueCollection)config.GetDBCollection("EDBP" + portalId);

                string Server = section.GetValues("server")[0];
                string Database = section.GetValues("database")[0];
                string entityref = pcTextType + "." + pcTextType;

                string connString = @"data source=" + Server +
                                     ";initial catalog=" + Database +
                                     ";integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;";

                // Build the MetaData... feel free to copy/paste it from the connection string in the config file.
                EntityConnectionStringBuilder esb = new EntityConnectionStringBuilder();

                esb.Metadata = "res://*/ORM." + entityref + ".csdl|res://*/ORM." + entityref + ".ssdl|res://*/ORM." + entityref + ".msl";

                esb.Provider = "System.Data.SqlClient";
                esb.ProviderConnectionString = connString;


                conString = esb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("GetEEConnectionString error thrown:{0}", ex);
            }
        }

        /// <summary>
        /// @Refactored- Janith
        /// Refer  web.config for the configurations
        /// </summary>
        /// <param name="dbMode"></param>
        protected override void CreateDBSets(DBMode dbMode, bool encryptedConnection)
        {
            NameValueCollection section = new NameValueCollection();

            try
            {
                string ConnString = GetConnectionString(dbMode, encryptedConnection);
                OpenConnection(ConnString);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateDBSets error thrown:{0}", ex);
                throw ex;
            }
        }

        protected override void CreateDBSets(DBMode dbMode, int portalId, bool encryptedConnection)
        {
            NameValueCollection section = new NameValueCollection();

            try
            {
                string ConnString = GetConnectionString(dbMode, portalId, encryptedConnection);
                OpenConnection(ConnString);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateDBSets error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void SetSP(string name)
        {
            try
            {
                spName = name;
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
                paramCount = count;
                paraList = new SqlParameter[count];
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
                if (paramCount > 0)
                {
                    paraList[index] = new SqlParameter(paramName, new InternalOperationHelper().GetMsSQLDBType(paramType));
                    paraList[index].Value = value;
                    index = index + 1;
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
            try
            {
                msSqlCon = new SqlConnection();
                msSqlCon.ConnectionString = conString;

                //check connection is open   
                if (msSqlCon.State == ConnectionState.Open)
                {
                    //if open then close  
                    msSqlCon.Close();
                }

                msSqlCon.Open();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenConnection error thrown:{0}", ex);
                msSqlCon.Dispose();
                return false;
            }
        }

        public override void CloseConnection()
        {
            try
            {
                ExecutionMode = Execution.Single;
                msSqlCon.Close();
                msSqlCon.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("CloseConnection error thrown:{0}", ex);
                throw ex;
            }
        }

        public override int[] CallSP()
        {
            int[] result = new int[1];
            string paramValues = "";
            int commandTimeOut = int.Parse(config.GetAppSettingsItem("TimeOut").ToString());
            DateTime startTime = DateTime.Now;
            try
            {
                //SqlCommand 
                SqlCommand oCommand = msSqlCon.CreateCommand();
                oCommand.CommandText = spName;
                oCommand.CommandType = System.Data.CommandType.StoredProcedure;
                oCommand.Connection = msSqlCon;
                oCommand.CommandTimeout = commandTimeOut;
                if (msSQLTran != null)
                {
                    oCommand.Transaction = msSQLTran;
                }

                if (paraList != null)
                {
                    for (int j = 0; j < paraList.Length; j++)
                    {
                        oCommand.Parameters.Add(paraList[j]);
                        paramValues += paraList[j] + " ";
                    }
                }
                result[0] = oCommand.ExecuteNonQuery();
                LogTimeoutQueries(paramValues, spName, startTime, commandTimeOut);
                return result;

            }
            catch (Exception ex)
            {
                LogTimeoutQueries(paramValues, spName, startTime, commandTimeOut);
                _logger.LogError("CallSP error thrown:{0}", ex);
                throw ex;
            }
            finally
            {
                paraList = null;
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
            }
        }

        public override IDataReader[] CallSPWithDataSet(Timeout timeOut)
        {
            IDataReader[] result = new IDataReader[1];
            try
            {
                SqlCommand cmd = msSqlCon.CreateCommand();
                cmd.CommandText = spName;
                if (timeOut == Timeout.Yes)
                    //cmd.CommandTimeout = int.Parse (System.Configuration.ConfigurationSettings.AppSettings["TimeOut").ToString());
                    cmd.CommandTimeout = int.Parse(config.GetAppSettingsItem("TimeOut").ToString());
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = msSqlCon;

                if (msSQLTran != null)
                {
                    cmd.Transaction = msSQLTran;
                }

                if (paraList != null)
                {
                    for (int j = 0; j < paraList.Length; j++)
                    {
                        cmd.Parameters.Add(paraList[j]);
                    }
                }

                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                result[0] = (IDataReader)dr;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("CallSPWithDataSet error thrown:{0}", ex);
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
                throw ex;
            }
            finally
            {
                paraList = null;

            }
        }

        public override object[] CallSPWithScaler()
        {
            object[] result = new object[1];
            try
            {
                SqlCommand cmd = msSqlCon.CreateCommand();
                cmd.CommandText = spName;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = msSqlCon;

                if (msSQLTran != null)
                {
                    cmd.Transaction = msSQLTran;
                }

                if (paraList != null)
                {
                    for (int j = 0; j < paraList.Length; j++)
                    {
                        cmd.Parameters.Add(paraList[j]);
                    }
                }

                result[0] = cmd.ExecuteScalar();
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError("CallSPWithScaler error thrown:{0}", ex);
                throw ex;
            }
            finally
            {
                paraList = null;
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
            }
        }





        /// <summary>
        /// ExecuteQuery With Audit Trail 
        /// @Refactored Janith with time out/log details
        /// </summary>
        /// <returns>int[]</returns>
        public override int[] ExecuteQuery(ExecuteQueryObj executeObj)
        {
            int[] result = new int[1];
            DateTime startTime = DateTime.Now;
            IList<SqlParameter> toRemove = new List<SqlParameter>();
            string paramValues = "";
            int commandTimeOut = int.Parse(config.GetAppSettingsItem("TimeOut").ToString());

            try
            {
                string sql = executeObj.Sql;
                List<SqlParameter> sqlParams = executeObj.SqlParams;


                foreach (SqlParameter sqlParam in sqlParams)
                {
                    sql = sql.Replace("??" + sqlParam.ParameterName, sqlParam.Value.ToString());
                    sql = sql.Replace("_@" + sqlParam.ParameterName, "_" + sqlParam.Value.ToString());
                    sql = sql.Replace("N@" + sqlParam.ParameterName, "N'" + sqlParam.Value.ToString() + "'");

                    if (null != sqlParam.Value)
                        paramValues += sqlParam.ParameterName + "=" + sqlParam.Value.ToString() + " ";
                }

                SqlCommand sqlCommand = new SqlCommand(sql, msSqlCon);
                sqlCommand.Parameters.Clear();

                DateTime dt1 = DateTime.Now;

                sqlCommand.CommandText = sql;
                sqlCommand.CommandTimeout = commandTimeOut;

                if (msSQLTran != null)
                {
                    sqlCommand.Transaction = msSQLTran;
                }

                //Create view does not support parameterization
                if (!sql.Contains("CREATE VIEW"))
                {
                    foreach (SqlParameter sqlParam in sqlParams)
                    {
                        sqlCommand.Parameters.Add(sqlParam);
                    }
                }

                result[0] = sqlCommand.ExecuteNonQuery();


                SaveToAudit(executeObj, startTime, paramValues, sql, dt1, commandTimeOut);


                return result;
            }
            catch (Exception ex)
            {
                SaveToAudit(executeObj, startTime, paramValues, executeObj.Sql, startTime, commandTimeOut);

                //SqlFunctionDebugInformation sqlFunction = new SqlFunctionDebugInformation("ExecuteQuery", executeObj.Sql);
                _logger.LogError("ExecuteQuery error thrown:{0}", ex);
                throw ex;
            }
            finally
            {
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
            }
        }


        /// <summary>
        /// ExecuteQueryWithScalar With Audit Trail 
        /// @Refactored Janith with time out/log details
        /// </summary>
        /// <returns>object[]</returns>
        public override object[] ExecuteQueryWithScalar(ExecuteQueryObj executeObj)
        {
            object[] result = new object[1];
            string sql = executeObj.Sql;
            List<SqlParameter> sqlParams = executeObj.SqlParams;
            DateTime startTime = DateTime.Now;
            string paramValues = "";
            int commandTimeOut = int.Parse(config.GetAppSettingsItem("TimeOut").ToString());

            try
            {
                foreach (SqlParameter sqlParam in sqlParams)
                {
                    sql = sql.Replace("??" + sqlParam.ParameterName, sqlParam.Value.ToString());
                    sql = sql.Replace("_@" + sqlParam.ParameterName, "_" + sqlParam.Value.ToString());
                    sql = sql.Replace("N@" + sqlParam.ParameterName, "N'" + sqlParam.Value.ToString() + "'");

                    if (null != sqlParam.Value)
                        paramValues += sqlParam.ParameterName + "=" + sqlParam.Value.ToString() + " ";
                }

                SqlCommand command = new SqlCommand(sql, msSqlCon);
                DateTime dt1 = DateTime.Now;

                command.CommandText = sql;
                command.CommandTimeout = commandTimeOut;

                if (msSQLTran != null)
                {
                    command.Transaction = msSQLTran;
                }

                foreach (SqlParameter sqlParam in sqlParams)
                {
                    command.Parameters.Add(sqlParam);
                }
                result[0] = command.ExecuteScalar();

                SaveToAudit(executeObj, startTime, paramValues, sql, dt1, commandTimeOut);


                return result;
            }
            catch (Exception ex)
            {
                SaveToAudit(executeObj, startTime, paramValues, sql, startTime, commandTimeOut);

                //SqlFunctionDebugInformation sqlFunction = new SqlFunctionDebugInformation("ExecuteQuery", executeObj.Sql);
                _logger.LogError("ExecuteQueryWithScalar error thrown:{0}", ex);
                throw ex;
            }
            finally
            {
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
            }
        }

        private readonly object syncLock = new object();
        /// <summary>
        /// Can not use using statement since this is returnning IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public override IDataReader[] ExecuteQueryWithIDataReader(string sql, List<SqlParameter> sqlParams)
        {
            DateTime startTime = DateTime.Now;
            int commandTimeOut = int.Parse(config.GetAppSettingsItem("TimeOut").ToString());
            string paramValues = "";
            lock (syncLock)
            {
                IDataReader[] result = new IDataReader[1];

                try
                {
                    foreach (SqlParameter sqlParam in sqlParams)
                    {
                        sql = sql.Replace("??" + sqlParam.ParameterName, sqlParam.Value.ToString());
                        sql = sql.Replace("_@" + sqlParam.ParameterName, "_" + sqlParam.Value.ToString());
                        sql = sql.Replace("N@" + sqlParam.ParameterName, "N'" + sqlParam.Value.ToString() + "'");

                        if (null != sqlParam.Value)
                            paramValues = sqlParam.ParameterName + "=" + sqlParam.Value.ToString() + " ";
                    }


                    SqlCommand sqlCommand = new SqlCommand(sql, msSqlCon);
                    foreach (SqlParameter sqlParam in sqlParams)
                    {
                        sqlCommand.Parameters.Add(sqlParam);
                    }

                    sqlCommand.CommandTimeout = commandTimeOut;

                    if (msSQLTran != null)
                    {
                        sqlCommand.Transaction = msSQLTran;
                    }

                    SqlDataReader drGet = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    result[0] = (IDataReader)drGet;

                    LogTimeoutQueries(paramValues, sql, startTime, commandTimeOut);

                    return result;

                }
                catch (Exception ex)
                {
                    LogTimeoutQueries(paramValues, sql, startTime, commandTimeOut);

                    //SqlFunctionDebugInformation sqlFunction = new SqlFunctionDebugInformation("ExecuteQuery", sql);
                    _logger.LogError("ExecuteQueryWithIDataReader error thrown:{0}", ex);
                    if (ExecutionMode != Execution.Multiple)
                        CloseConnection();
                    throw;
                }
                finally
                {
                    //if (ExecutionMode != Execution.Multiple)
                    //  CloseConnection();
                }
            }
        }

        public override DataSet ExecuteQueryWithDataSet(string sql, List<SqlParameter> sqlParams)
        {
            DataSet ds = new DataSet();
            DateTime startTime = DateTime.Now;
            int commandTimeOut = int.Parse(config.GetAppSettingsItem("TimeOut").ToString());
            string paramValues = "";
            try
            {
                foreach (SqlParameter sqlParam in sqlParams)
                {
                    sql = sql.Replace("??" + sqlParam.ParameterName, sqlParam.Value.ToString());
                    sql = sql.Replace("_@" + sqlParam.ParameterName, "_" + sqlParam.Value.ToString());
                    sql = sql.Replace("N@" + sqlParam.ParameterName, "N'" + sqlParam.Value.ToString() + "'");

                    if (null != sqlParam.Value)
                        paramValues = sqlParam.ParameterName + "=" + sqlParam.Value.ToString() + " ";
                }

                SqlCommand com = new SqlCommand(sql, msSqlCon);
                com.CommandTimeout = commandTimeOut;
                foreach (SqlParameter sqlParam in sqlParams)
                {
                    com.Parameters.Add(sqlParam);
                }
                SqlDataAdapter da = new SqlDataAdapter(com);


                da.Fill(ds);
                LogTimeoutQueries(paramValues, sql, startTime, commandTimeOut);

                return ds;

            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteQueryWithDataSet error thrown:{0}", ex);

                //SqlFunctionDebugInformation sqlFunction = new SqlFunctionDebugInformation("ExecuteQuery", sql);
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
                throw ex;

            }
            finally
            {
                //if (ExecutionMode != Execution.Multiple)
                //    CloseConnection();
            }
        }

        public override DataSet ExecuteQueryWithDataSetForReports(string sql, List<SqlParameter> sqlParams, string tableName)
        {
            DataSet ds = new DataSet();
            try
            {
                foreach (SqlParameter sqlParam in sqlParams)
                {
                    sql = sql.Replace("??" + sqlParam.ParameterName, sqlParam.Value.ToString());
                    sql = sql.Replace("_@" + sqlParam.ParameterName, "_" + sqlParam.Value.ToString());
                    sql = sql.Replace("N@" + sqlParam.ParameterName, "N'" + sqlParam.Value.ToString() + "'");
                }

                SqlCommand com = new SqlCommand(sql, msSqlCon);
                foreach (SqlParameter sqlParam in sqlParams)
                {
                    com.Parameters.Add(sqlParam);
                }
                SqlDataAdapter da = new SqlDataAdapter(com);
                da.Fill(ds, tableName);
                return ds;

            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteQueryWithDataSetForReports error thrown:{0}", ex);
                if (ExecutionMode != Execution.Multiple)
                    CloseConnection();
                throw ex;

            }
            finally
            {
                //if (ExecutionMode != Execution.Multiple)
                //    CloseConnection();
            }
        }

        public override void BeginTran()
        {
            try
            {
                msSQLTran = msSqlCon.BeginTransaction(IsolationLevel.ReadCommitted);
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
                msSQLTran = msSqlCon.BeginTransaction(transactionIsolation);
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
                msSQLTran.Commit();
                msSQLTran.Dispose();
                msSQLTran = null;
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
                msSQLTran.Rollback();
                msSQLTran.Dispose();
                msSQLTran = null;
            }
            catch (Exception ex)
            {
                _logger.LogError("RollbackTran error thrown:{0}", ex);
                throw ex;
            }
        }

        public override void saveBulkData(ExecuteQueryObj eqo)
        {
            //dataTable=arrangeTableColumnOrder(tableName, dataTable);

            DateTime startTime = DateTime.Now;
            int commandTimeOut = 0;
            try
            {
                commandTimeOut = int.Parse(config.GetAppSettingsItem("BulkCopyTimeout").ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("saveBulkData error thrown:{0}", ex);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(msSqlCon))
            {
                bulkCopy.DestinationTableName = "dbo." + eqo.tableName;

                //mapping the columns
                foreach (DataColumn column in eqo.dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                try
                {
                    bulkCopy.BulkCopyTimeout = commandTimeOut;

                    bulkCopy.WriteToServer(eqo.dataTable);
                    LogTimeoutQueries("", eqo.tableName, startTime, commandTimeOut);
                }
                catch (Exception ex)
                {
                    LogTimeoutQueries("", eqo.tableName, startTime, commandTimeOut);
                    _logger.LogError("WriteToServer error thrown:{0}", ex);
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }

            if (eqo.IsAudited && null != eqo.ActionTypes && eqo.ActionTypes.Count > 0)
            {
                try
                {
                    //Platform1.Standard.TrackingClient.Trace.TrackingClient ttc = new Platform1.Standard.TrackingClient.Trace.TrackingClient();
                    //ttc.TrackEvent(eqo.ActionTypes, eqo.CreatedUser, eqo.NameSpace, eqo.CaptureText, DateTime.Now, DateTime.Now.Subtract(startTime).TotalMilliseconds, eqo.note, eqo.PortalId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("TrackingClient error thrown:{0}", ex);
                    throw ex;
                }

            }
        }

        public override void saveBulkDataInTransaction(string tableName, DataTable dataTable)
        {
            //dataTable = arrangeTableColumnOrder(tableName, dataTable);

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(msSqlCon, SqlBulkCopyOptions.Default, msSQLTran))
            {
                bulkCopy.DestinationTableName = "dbo." + tableName;
                //mapping the columns
                foreach (DataColumn column in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                try
                {
                    bulkCopy.WriteToServer(dataTable);
                }
                catch (Exception ex)
                {
                    _logger.LogError("saveBulkDataInTransaction error thrown:{0}", ex);
                    throw ex;
                }
            }
        }

        public override int BatchWiseBulkSave(ExecuteQueryObj eqo)
        {
            DateTime startTime = DateTime.Now;
            int commandTimeOut = 0;
            int rows = 0;

            try
            {
                try
                {
                    commandTimeOut = int.Parse(config.GetAppSettingsItem("BulkCopyTimeout").ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError("BatchWiseBulkSave error thrown:{0}", ex);
                }




                rows = eqo.dataTable.Rows.Count;

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(msSqlCon, SqlBulkCopyOptions.Default, msSQLTran)
                {
                    DestinationTableName = !eqo.tableName.Contains("dbo.") ? "dbo." + eqo.tableName : eqo.tableName,
                    BulkCopyTimeout = 0,
                    BatchSize = 100000,
                })
                {
                    using (DataTable datatable = eqo.dataTable)
                    {
                        foreach (DataColumn col in datatable.Columns)
                        {
                            bulkcopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        };
                        bulkcopy.WriteToServer(datatable);
                    }
                }
                if (eqo.IsAudited && null != eqo.ActionTypes && eqo.ActionTypes.Count > 0)
                {
                    try
                    {
                        //Platform1.Standard.TrackingClient.Trace.TrackingClient ttc = new Platform1.Standard.TrackingClient.Trace.TrackingClient();
                        //ttc.TrackEvent(eqo.ActionTypes, eqo.CreatedUser, eqo.NameSpace, eqo.CaptureText, DateTime.Now, DateTime.Now.Subtract(startTime).TotalMilliseconds, eqo.note, eqo.PortalId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("TrackingClient error thrown:{0}", ex);
                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("BatchWiseBulkSave error thrown:{0}", ex);
                throw;
            }


            return rows;
        }

        public override void MergeTableData(string SourceTableName, string TargetTableName, List<string> matchingColumns, List<string> updatingColumns)
        {
            MsSQLDatabase sQLDatabase = null;
            try
            {
                sQLDatabase = new MsSQLDatabase(loggerFactory, DBMode.Portal, this.portalId, false);
                string query = "MERGE {0} AS T USING {1} AS S ON( {2}) WHEN MATCHED THEN UPDATE SET {3}; ";

                string str = " T.{0} = S.{0}";
                List<string> matchingColumRebuilt = new List<string>();
                matchingColumns.ForEach(m => matchingColumRebuilt.Add(string.Format(str, m)));
                string matchStr = string.Join(" AND ", matchingColumRebuilt);

                List<string> updatingColumRebuilt = new List<string>();
                updatingColumns.ForEach(m => updatingColumRebuilt.Add(string.Format(str, m)));
                string updateStr = string.Join(" , ", updatingColumRebuilt);

                query = string.Format(query, TargetTableName, SourceTableName, matchStr, updateStr);

                using (SqlCommand command = new SqlCommand(query, sQLDatabase.msSqlCon))
                {
                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("MergeDataTable error thrown:{0}", ex);
            }
            finally
            {
                try { sQLDatabase.CloseConnection(); } catch (Exception) { }
            }
        }
        public override void DropTable(string tableName)
        {
            MsSQLDatabase sQLDatabase = null;

            try
            {
                sQLDatabase = new MsSQLDatabase(loggerFactory, DBMode.Portal, this.portalId, false);
                string query = "IF EXISTS(SELECT 1 FROM sys.tables where name='{0}' and type='U') DROP TABLE {0}";
                query = string.Format(query, tableName);

                using (SqlCommand command = new SqlCommand(query, sQLDatabase.msSqlCon))
                {
                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("DropTable error thrown:{0}", ex);
            }
            finally
            {
                try { sQLDatabase.CloseConnection(); } catch (Exception) { }
            }
        }

        public override int TransferCSVAsBulk(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns)
        {
            int rows = 0;
            /*
            _logDetails.MethodName = "TransferCSVAsBulk";
            DateTime startTime = DateTime.Now;
            int commandTimeOut = 0;
            
            DataColumn identityCoulumn = null;

            try
            {
                try
                {
                    commandTimeOut = int.Parse(config.GetAppSettingsItem("BulkCopyTimeout").ToString());
                }
                catch (Exception ex)
                {
                    sci.Error(_logDetails, ex);
                    //sci.Error(frendlyName, screenName, "", "Unable to set BulkCopyTimeout. Check the key existance. =>" + bex.Message + " : " + bex.StackTrace);
                }

                var ERROR_COLUMN_NAME = "ErrorFlag";
                if (additionalColumns.Where(A => A.AutoIncrement)?.FirstOrDefault() != null)
                {
                    identityCoulumn = additionalColumns.Where(A => A.AutoIncrement == true).FirstOrDefault<DataColumn>();
                }
                var errorColumn = additionalColumns.FirstOrDefault(a => a.ColumnName.Equals(ERROR_COLUMN_NAME) && !a.AutoIncrement && a.DataType == typeof(System.Boolean));
                List<DataColumn> AdditionalColumns = additionalColumns.Where(A => !A.AutoIncrement && A != errorColumn).ToList<DataColumn>();


                rows = 0;

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(msSqlCon, SqlBulkCopyOptions.Default, msSQLTran)
                {
                    DestinationTableName = "dbo." + eqo.tableName,
                    BulkCopyTimeout = 0,
                    BatchSize = 100000,
                })
                {
                    using (DataTable datatable = new DataTable())
                    {
                        var columns = datatable.Columns;

                        using (ValidatedCsvReader tr = new ValidatedCsvReader(filePath, Encoding.UTF8))
                        {
                            // get the first tuple because this contains headers 
                            string[] headerTuple = tr.ReadTuples(delimitter);
                            List<int> colunmPossion = new List<int>();

                            if (identityCoulumn != null)
                            {
                                columns.Add(new DataColumn()
                                {
                                    ColumnName = identityCoulumn.ColumnName,
                                    DataType = identityCoulumn.DataType,
                                    DefaultValue = identityCoulumn.DefaultValue,
                                    AutoIncrement = identityCoulumn.AutoIncrement,
                                    AutoIncrementSeed = identityCoulumn.AutoIncrementSeed,
                                    AutoIncrementStep = identityCoulumn.AutoIncrementStep
                                });
                            }

                            for (int i = 0; i < headerTuple.Count(); i++)
                            {
                                columns.Add(headerTuple[i], typeof(System.String));
                                colunmPossion.Add(i);
                            }
                            if (errorColumn != null)
                            {
                                columns.Add(new DataColumn()
                                {
                                    ColumnName = errorColumn.ColumnName,
                                    DataType = errorColumn.DataType,
                                    DefaultValue = errorColumn.DefaultValue,
                                    AutoIncrement = errorColumn.AutoIncrement,
                                    AutoIncrementSeed = errorColumn.AutoIncrementSeed,
                                    AutoIncrementStep = errorColumn.AutoIncrementStep
                                });
                            }
                            //Add additional column headers
                            //foreach (DataColumn additionalColumn in AdditionalColumns)
                            //{
                            //    columns.Add(additionalColumn);
                            //}

                            //CreateTable("dbo." + eqo.tableName, columns);
                            int batchsize = 0;

                            string[] dataValue = null;
                            var errorRows = new List<int>();

                            while ((dataValue = tr.ReadTuples(delimitter)) != null)
                            {
                                DataRow dr = datatable.NewRow();
                                int additionalStartIndex = colunmPossion.Count;
                                //No need to set value for identity keep null.

                                for (int j = 0; j < colunmPossion.Count; j++)
                                {
                                    if (identityCoulumn != null)
                                    {
                                        try
                                        {
                                            dr[j + 1] = dataValue[colunmPossion[j]];
                                        }
                                        catch (Exception exx)
                                        {
                                            if (errorColumn != null)
                                            {
                                                dr[ERROR_COLUMN_NAME] = true;
                                            }
                                            errorRows.Add(rows);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            dr[j] = dataValue[colunmPossion[j]];
                                        }
                                        catch (Exception)
                                        {
                                            if (errorColumn != null)
                                            {
                                                dr[ERROR_COLUMN_NAME] = true;
                                            }
                                            errorRows.Add(rows);
                                        }
                                    }
                                }

                                foreach (DataColumn additionalColumn in AdditionalColumns)
                                {
                                    additionalStartIndex = additionalStartIndex + 1;
                                    dr[additionalStartIndex] = additionalColumn.DefaultValue;
                                }

                                datatable.Rows.Add(dr);

                                batchsize += 1;
                                if (batchsize == 100000)
                                {
                                    bulkcopy.WriteToServer(datatable);
                                    datatable.Rows.Clear();
                                    batchsize = 0;
                                }
                                rows += 1;
                            }
                        }

                        bulkcopy.WriteToServer(datatable);

                        datatable.Rows.Clear();
                    }
                }




                if (eqo.IsAudited && null != eqo.ActionTypes && eqo.ActionTypes.Count > 0)
                {
                    try
                    {
                        Platform1.Standard.TrackingClient.Trace.TrackingClient ttc = new Platform1.Standard.TrackingClient.Trace.TrackingClient();
                        ttc.TrackEvent(eqo.ActionTypes, eqo.CreatedUser, eqo.NameSpace, eqo.CaptureText, DateTime.Now, DateTime.Now.Subtract(startTime).TotalMilliseconds, eqo.note, eqo.PortalId);
                    }
                    catch (Exception e)
                    {
                        sci.Error(_logDetails, e);
                        // sci.Error(frendlyName, screenName, "", "Error @ Transaction Tracking " + e.Message + " @@ " + e.StackTrace);
                        throw e;
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            */

            return rows;
        }

        public override int CreateTableFromCSV(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns)
        {
            int rows = 0;
            /*
            _logDetails.MethodName = "TransferCSVAsBulk";
            DateTime startTime = DateTime.Now;
            int commandTimeOut = 0;
            
            DataColumn identityCoulumn = null;

            try
            {
                try
                {
                    commandTimeOut = int.Parse(config.GetAppSettingsItem("BulkCopyTimeout").ToString());
                }
                catch (Exception ex)
                {
                    sci.Error(_logDetails, ex);
                }

                if (additionalColumns.Where(A => A.AutoIncrement)?.FirstOrDefault() != null)
                {
                    identityCoulumn = additionalColumns.Where(A => A.AutoIncrement == true).FirstOrDefault<DataColumn>();
                }
                List<DataColumn> AdditionalColumns = additionalColumns.Where(A => A.AutoIncrement == false).ToList<DataColumn>();



                rows = 0;

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(msSqlCon, SqlBulkCopyOptions.Default, msSQLTran)
                {
                    DestinationTableName = "dbo." + eqo.tableName,
                    BulkCopyTimeout = 0,
                    BatchSize = 100000,
                })
                {
                    using (DataTable datatable = new DataTable())
                    {
                        var columns = datatable.Columns;

                        using (ValidatedCsvReader tr = new ValidatedCsvReader(filePath, Encoding.UTF8))
                        {
                            // get the first tuple because this contains headers 
                            string[] headerTuple = tr.ReadTuples(delimitter);
                            List<int> colunmPossion = new List<int>();

                            if (identityCoulumn != null)
                            {
                                columns.Add(identityCoulumn);
                            }


                            for (int i = 0; i < headerTuple.Count(); i++)
                            {
                                columns.Add(headerTuple[i], typeof(System.String));
                                colunmPossion.Add(i);
                            }

                            //Add additional column headers
                            foreach (DataColumn additionalColumn in AdditionalColumns)
                            {
                                columns.Add(additionalColumn);
                            }

                            CreateTable(string.Format("dbo.[{0}]", eqo.tableName), columns);
                        }
                    }
                }

                if (eqo.IsAudited && null != eqo.ActionTypes && eqo.ActionTypes.Count > 0)
                {
                    try
                    {
                        Platform1.Standard.TrackingClient.Trace.TrackingClient ttc = new Platform1.Standard.TrackingClient.Trace.TrackingClient();
                        ttc.TrackEvent(eqo.ActionTypes, eqo.CreatedUser, eqo.NameSpace, eqo.CaptureText, DateTime.Now, DateTime.Now.Subtract(startTime).TotalMilliseconds, eqo.note, eqo.PortalId);
                    }
                    catch (Exception e)
                    {
                        sci.Error(_logDetails, e);
                        throw e;
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            */
            return rows;
        }

        public override void CreateTable(string tableName, DataColumnCollection columns)
        {
            MsSQLDatabase sQLDatabase = null;

            try
            {
                sQLDatabase = new MsSQLDatabase(loggerFactory, DBMode.Portal, this.portalId, false);

                string query = "CREATE TABLE {0} ( {1} );";
                List<string> columnQueries = new List<string>();

                foreach (DataColumn c in columns)
                {
                    if (c.AutoIncrement)
                    {
                        columnQueries.Add("[" + c.ColumnName + "] [int] IDENTITY(1,1) NOT NULL ");
                    }
                    else if (c.DataType.Name == "DateTime")
                    {
                        columnQueries.Add("[" + c.ColumnName + "] Datetime ");
                    }
                    else if (c.DataType.Name == "Boolean")
                    {
                        columnQueries.Add("[" + c.ColumnName + "] bit ");
                    }
                    else if (c.DataType.Name == "int")
                    {
                        columnQueries.Add("[" + c.ColumnName + "] int ");
                    }
                    else if (c.DataType.Name == "bigint")
                    {
                        columnQueries.Add("[" + c.ColumnName + "] bigint ");
                    }
                    else
                    {
                        columnQueries.Add("[" + c.ColumnName + "] nvarchar(max) ");
                    }

                }
                string columnQuery = string.Join(",", columnQueries);
                query = string.Format(query, tableName, columnQuery);


                using (SqlCommand command = new SqlCommand(query, sQLDatabase.msSqlCon))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateTable error thrown:{0}", ex);
            }
            finally
            {
                try { sQLDatabase.CloseConnection(); } catch (Exception) { }
            }

        }
        #endregion


        #region this should go to abstract class
        private void SaveToAudit(ExecuteQueryObj executeObj, DateTime methodStart, string paramValues,
           string sql, DateTime queryStart, int commandTimeOut)
        {
            LogTimeoutQueries(paramValues, sql, queryStart, commandTimeOut);

            if (executeObj.IsAudited && null != executeObj.ActionTypes && executeObj.ActionTypes.Count > 0)
            {
                try
                {
                    //Platform1.Standard.TrackingClient.Trace.TrackingClient ttc = new Platform1.Standard.TrackingClient.Trace.TrackingClient();
                    //ttc.TrackEvent(executeObj.ActionTypes, executeObj.CreatedUser, executeObj.NameSpace, executeObj.CaptureText, DateTime.Now, DateTime.Now.Subtract(methodStart).TotalMilliseconds, executeObj.note, executeObj.PortalId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("SaveToAudit error thrown:{0}", ex);
                }

            }
        }


        private void LogTimeoutQueries(string paramValues, string sql, DateTime queryStart, int commandTimeOut)
        {
            try
            {
                DateTime dt2 = DateTime.Now;
                double timeTaken = (dt2 - queryStart).TotalSeconds;
                if (timeTaken > commandTimeOut)
                {
                    string body = "Executed Query " + sql + " @@Parameters " + paramValues + " Time taken(ms)-" + timeTaken + " commandTimeOut " + commandTimeOut;
                    _logger.LogInformation("body:{0}", body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LogTimeoutQueries error thrown:{0}", ex);
            }
        }




        private void SendMail(int TraceId, bool encryptedConnection)
        {
            AbstractDatabase abdb = null;

            try
            {
                abdb = DBFactory.CreateDB(loggerFactory).GetDB(DBType.MsSQL, DBMode.Portal, this.portalId, encryptedConnection);
                abdb.SetSP("[dbo].[SendLongRunningAlerts]");
                abdb.SetParamCount(1);
                abdb.AddParameter("@TraceId", TraceId, DataType.Int);
                abdb.CallSP();
            }
            catch (Exception ex)
            {
                _logger.LogError("SendMail error thrown:{0}", ex);
                throw ex;
            }
            finally
            {
                if (abdb != null)
                {
                    abdb.CloseConnection();
                }
            }
        }

        public override DbContext CreateEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override DbContext CreateEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override void GetEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            //Not implement
        }

        public override void GetEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        {
            //Not implement
        }

        public override DbContext CreateContext(PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
