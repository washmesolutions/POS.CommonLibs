using Microsoft.Extensions.Logging;
using POS.Lib.Configuration;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public class MsSQLDatabaseRead : MsSQLDatabase
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory loggerFactory;

        private ConfigWrapper config = new ConfigWrapper();
        //protected string spname;
        //protected int paramcount;
        protected SqlConnection msSqlCon;
        private SqlTransaction msSQLTran;

        protected SqlParameter[] paraList;
        protected int index = 0;
        int portalId = -1;
        //public int ExecutionMode;

        public MsSQLDatabaseRead(ILoggerFactory logger) : base(logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLDatabaseRead>();

        }

        public MsSQLDatabaseRead(ILoggerFactory logger, DBMode dbMode, bool encryptedConnection) : base(logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLDatabaseRead>();

            base.CreateDBSets(dbMode, encryptedConnection);
        }

        public MsSQLDatabaseRead(ILoggerFactory logger, DBMode dbMode, int portalId, bool encryptedConnection) : base(logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLDatabaseRead>();

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
                    //cmd.CommandTimeout = int.Parse (System.Configuration.ConfigurationSettings.AppSettings["TimeOut"].ToString());
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

                // SqlFunctionDebugInformation sqlFunction = new SqlFunctionDebugInformation("ExecuteQuery", sql);
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
                _logger.LogInformation("saveBulkData commandTimeOut -> {0}", commandTimeOut);
            }
            catch (Exception ex)
            {
                _logger.LogError("get BulkCopyTimeout error thrown:{0}", ex);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(msSqlCon))
            {
                bulkCopy.DestinationTableName = "dbo." + eqo.tableName;
                _logger.LogInformation("saveBulkData eqo.tableName -> {0} , msSqlCon -> {1}", eqo.tableName, msSqlCon);
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
                    _logger.LogError("saveBulkData error thrown:{0}", ex);
                    LogTimeoutQueries("", eqo.tableName, startTime, commandTimeOut);
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
                    _logger.LogError("TrackEvent error thrown:{0}", ex);
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
                    _logger.LogError("get BulkCopyTimeout error thrown:{0}", ex);
                }




                rows = eqo.dataTable.Rows.Count;

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(msSqlCon, SqlBulkCopyOptions.Default, msSQLTran)
                {
                    DestinationTableName = "dbo." + eqo.tableName,
                    BulkCopyTimeout = 0,
                    BatchSize = 100000,
                })
                {
                    using (DataTable datatable = eqo.dataTable)
                    {
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
                        _logger.LogError("TrackEvent error thrown:{0}", ex);
                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return rows;
        }
        public override void MergeTableData(string SourceTableName, string TargetTableName, List<string> matchingColumns, List<string> updatingColumns)
        {
            MsSQLDatabaseRead sQLDatabase = null;
            try
            {
                sQLDatabase = new MsSQLDatabaseRead(loggerFactory, DBMode.Portal, false);
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
            MsSQLDatabaseRead sQLDatabase = null;

            try
            {
                sQLDatabase = new MsSQLDatabaseRead(loggerFactory, DBMode.Portal, false);
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
            int rows = 0;

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

                DataColumn identityCoulumn = additionalColumns.Where(A => A.AutoIncrement == true).FirstOrDefault<DataColumn>();
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
                            //foreach (DataColumn additionalColumn in AdditionalColumns)
                            //{
                            //    columns.Add(additionalColumn);
                            //}

                            //CreateTable("dbo." + eqo.tableName, columns);
                            int batchsize = 0;

                            string[] dataValue = null;



                            while ((dataValue = tr.ReadTuples(delimitter)) != null)
                            {
                                DataRow dr = datatable.NewRow();
                                int additionalStartIndex = colunmPossion.Count;
                                //No need to set value for identity keep null.

                                for (int j = 0; j < colunmPossion.Count; j++)
                                {
                                    dr[j + 1] = dataValue[colunmPossion[j]];
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

            /*_logDetails.MethodName = "TransferCSVAsBulk";
            DateTime startTime = DateTime.Now;
            int commandTimeOut = 0;
            

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

                DataColumn identityCoulumn = additionalColumns.Where(A => A.AutoIncrement == true).FirstOrDefault<DataColumn>();
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

        private void CreateTable(string tableName, DataColumnCollection columns)
        {
            MsSQLDatabaseRead sQLDatabase = null;

            try
            {
                sQLDatabase = new MsSQLDatabaseRead(loggerFactory, DBMode.Portal, false);

                string query = "CREATE TABLE {0} ( {1} );";
                List<string> columnQueries = new List<string>();

                foreach (DataColumn c in columns)
                {
                    if (c.AutoIncrement == true)
                    {
                        columnQueries.Add("[" + c.ColumnName + "] [int] IDENTITY(1,1) NOT NULL ");
                    }
                    else if (c.DataType.Name == "DateTime")
                    {
                        columnQueries.Add("[" + c.ColumnName + "] Datetime ");
                    }
                    else
                    {
                        columnQueries.Add("[" + c.ColumnName + "] nvarchar(100) ");
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




        public override DbContext CreateContext(PersistanceContextType contextType)
        {
            throw new NotImplementedException();
        }

        public override string GetConnectionString(DBMode dbMode, bool encryptedConnection)
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

        #endregion
    }
}
