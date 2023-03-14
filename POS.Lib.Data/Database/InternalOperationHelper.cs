using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace POS.Lib.Data.Database
{
    internal class InternalOperationHelper
    {
        public SqlDbType GetMsSQLDBType(DataType val)
        {
            switch (val)
            {
                case DataType.Binary: return SqlDbType.Binary;
                case DataType.Bit: return SqlDbType.Bit;
                case DataType.VarChar: return SqlDbType.VarChar;
                case DataType.DateTime: return SqlDbType.DateTime;
                case DataType.Decimal: return SqlDbType.Decimal;
                case DataType.Double: return SqlDbType.Real;
                case DataType.Float: return SqlDbType.Float;
                case DataType.Int: return SqlDbType.Int;
                case DataType.BigInt: return SqlDbType.BigInt;
                case DataType.Image: return SqlDbType.Image;
                case DataType.NVarChar: return SqlDbType.NVarChar;
                default:
                    break;
            }
            return SqlDbType.VarChar;
        }
    }
}
