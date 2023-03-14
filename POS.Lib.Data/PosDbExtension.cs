using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace POS.Lib.Data
{
    public static class PosDbExtension
    {
        public static SqlParameter ToTableTypeParam(this SqlParameter param, string tableType)
        {
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = tableType;
            return param;
        }
        public static SqlParameter ToTableTypeParam<T>(this SqlParameter param, string tableType, List<T> values, List<string> propertyNames)
        {
            var propertyList = new List<PropertyInfo>();
            foreach (var prop in propertyNames)
            {
                var p = typeof(T).GetProperty(prop);
                if (p != null)
                {
                    propertyList.Add(p);
                }
            }
            DataTable dt = new DataTable();
            foreach (var prop in propertyList)
            {
                dt.Columns.Add(new DataColumn(prop.Name, NormalizeType(prop.PropertyType)));
            }
            foreach (var obj in values)
            {
                DataRow row = dt.NewRow();
                foreach (var prop in propertyList)
                {
                    row[prop.Name] = prop.GetValue(obj);
                }
                dt.Rows.Add(row);
            }

            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = tableType;
            param.Value = dt;
            return param;
        }

        public static SqlParameter ToTableTypeParam<T>(this SqlParameter param, string tableType, List<T> values, string columnName)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn(columnName, NormalizeType(typeof(T))));
            foreach (var obj in values)
            {
                DataRow row = dt.NewRow();
                row[columnName] = obj;
                dt.Rows.Add(row);
            }

            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = tableType;
            param.Value = dt;
            return param;
        }

        private static Type NormalizeType(Type type)
        {
            if (type.IsEnum)
            {
                return typeof(Int32);
            }
            return type;
        }
    }
}
