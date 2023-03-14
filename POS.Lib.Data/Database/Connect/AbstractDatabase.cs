using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public abstract class AbstractDatabase
    {
        protected string spName;
        protected int paramCount;
        public Execution ExecutionMode;


        public abstract string GetConnectionString(DBMode dbMode, bool encryptedConnection);

        protected abstract void CreateDBSets(DBMode dbMode, bool encryptedConnection);

        public abstract string GetConnectionString(DBMode dbMode, int portalId, bool encryptedConnection);

        protected abstract void CreateDBSets(DBMode dbMode, int portalId, bool encryptedConnection);

        //DB fist
        public abstract void GetEEContext(DBMode dbMode, PersistanceContextType contextType);

        public abstract void GetEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType);

        public abstract void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode);

        public abstract void GetEEConnectionString(PersistanceContextType pcTextType, DBMode dbMode, int portalId);

        /// <summary>
        /// Set Stored Procedure Name
        /// </summary>
        /// <param name="sp">Name of SP</param>
        public abstract void SetSP(string name);

        /// <summary>
        /// Set the count of parameters
        /// </summary>
        /// <param name="count">param count</param>
        public abstract void SetParamCount(int count);

        /// <summary>
        /// Add parameters to the SP
        /// </summary>
        public abstract void AddParameter(string paramName, object value, DataType paramType);

        protected abstract bool OpenConnection(string conString);

        public abstract void CloseConnection();

        /// <summary>
        /// Execute a SP in Databases
        /// </summary>
        /// <returns>Array of Interger</returns>
        public abstract int[] CallSP();

        /// <summary>
        /// Execute a SP in Databases and Get Resultsets
        /// </summary>
        /// <param name="timeOut">Timeout period</param>
        /// <returns>Array of IDataReaders</returns>
        public abstract IDataReader[] CallSPWithDataSet(Timeout timeOut);

        /// <summary>
        /// Execute a SP in Database and Get  Scalar Results
        /// </summary>
        /// <returns>Array of Objects</returns>
        public abstract object[] CallSPWithScaler();

        /// <summary>
        ///   Execute a Query in  Databases with Audit Trail
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Array of Interger</returns>
        public abstract int[] ExecuteQuery(ExecuteQueryObj executeObj);

        /// <summary>
        /// Execute a Query in  Databases and Get Resultsets
        /// </summary>
        /// <param name="sql">SQl query</param>
        /// <returns>Array of IDataReaders</returns>
        public abstract IDataReader[] ExecuteQueryWithIDataReader(string sql, List<SqlParameter> sqlParams);

        /// <summary>
        /// Execute a Query in  Databases and Get Resultsets
        /// </summary>
        /// <param name="sql">SQl query</param>
        /// <returns>DataSet</returns>
        public abstract DataSet ExecuteQueryWithDataSet(string sql, List<SqlParameter> sqlParams);

        /// <summary>
        /// Execute a Query in  Databases and Get Resultsets for reports
        /// </summary>
        /// <param name="sql">SQl query</param>
        /// <returns>DataSet</returns>
        public abstract DataSet ExecuteQueryWithDataSetForReports(string sql, List<SqlParameter> sqlParams, string tableName);

        /// <summary>
        /// Execute a Query in  Databases and Get Scalar Results  without Audit Trail
        /// </summary>
        /// <param name="sql">SQl query</param>
        /// <returns>Array of Objects</returns>
        public abstract object[] ExecuteQueryWithScalar(ExecuteQueryObj executeObj);

        /// <summary>
        /// Begin Transaction
        /// </summary>
        public abstract void BeginTran();

        /// <summary>
        /// Begin Transaction with IsolationLevel
        /// </summary>
        /// <param name="transactionIsolation"> Transaction isolation</param>
        public abstract void BeginTran(IsolationLevel transactionIsolation);

        /// <summary>
        /// Commit Transaction
        /// </summary>
        public abstract void CommitTran();

        /// <summary>
        /// Rollback Transaction
        /// </summary>
        public abstract void RollbackTran();

        /// <summary>
        /// Saving bulk data 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        public abstract void saveBulkData(ExecuteQueryObj eqo);

        public abstract void saveBulkDataInTransaction(String tableName, DataTable dataTable);
        public abstract void MergeTableData(string SourceTableName, string TargetTableName, List<string> matchingColumns, List<string> updatingColumns);
        public abstract void CreateTable(string tableName, DataColumnCollection columns);

        public abstract void DropTable(string tableName);
        //Code first
        public abstract DbContext CreateContext(PersistanceContextType contextType);

        //DB first
        public abstract DbContext CreateEEContext(DBMode dbMode, PersistanceContextType contextType);

        public abstract DbContext CreateEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType);

        public abstract int TransferCSVAsBulk(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns);

        public abstract int CreateTableFromCSV(ExecuteQueryObj eqo, string filePath, char delimitter, List<DataColumn> additionalColumns);

        public abstract int BatchWiseBulkSave(ExecuteQueryObj eqo);
    }
}
