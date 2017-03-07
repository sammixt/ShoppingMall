using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace UnionMall.LIB
{
   public class DbConnection : IDisposable
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        private string conn = ConfigurationManager.ConnectionStrings["FCUBSConn"].ConnectionString;
        private OracleConnection connect;
        private OracleCommand command;

        public OracleConnection connection()
        {
            connect = new OracleConnection(conn);
            return connect;
        } 
       
        public OracleCommand CreateCommand(string sql, CommandType type, params OracleParameter[] parameters)
        {
            connection().Open();
            command = new OracleCommand(sql, connection());
            command.CommandType = type;
            if (parameters != null && parameters.Length > 0)
            {
                OracleParameterCollection cmdParams = command.Parameters;
                for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
            }


            return command;
        }

        public object GetExecuteScalar(string storedProcedure, params OracleParameter[] parameters)
        {
            return CreateCommand(storedProcedure, CommandType.StoredProcedure, parameters).ExecuteScalar();
        }

        public object GetExecuteNonQuery(string storedProcedure, params OracleParameter[] parameters)
        {
            return CreateCommand(storedProcedure, CommandType.StoredProcedure, parameters).ExecuteNonQuery();
        }

        public OracleDataReader GetDataReader(string storedProcedure, params OracleParameter[] parameters)
        {
            return CreateCommand(storedProcedure, CommandType.StoredProcedure, parameters).ExecuteReader();
        }

        public  OracleParameter CreateCursorParameter(string name)
        {
            OracleParameter prm = new OracleParameter(name, OracleDbType.RefCursor);
            prm.Direction = ParameterDirection.ReturnValue;
            return prm;
        }

        //public static OracleParameter CreateCustomTypeArrayInputOutputParameter2<T>(string name, string oracleUDTName, T value)
        //{
        //    OracleParameter parameter = new OracleParameter();
        //    parameter.ParameterName = name;
        //    parameter.OracleDbType = OracleDbType.Array;
        //    parameter.Direction = ParameterDirection.InputOutput;
        //    parameter.UdtTypeName = oracleUDTName;
        //    parameter.Value = value;
        //    return parameter;
        //}

        //public static OracleParameter CreateCustomTypeOutputParameter<T>(string name, string oracleUDTName, T value) where T : IOracleCustomType, INullable
        //{
        //    OracleParameter parameter = new OracleParameter();
        //    parameter.ParameterName = name;
        //    parameter.OracleDbType = OracleDbType.Object;
        //    parameter.Direction = ParameterDirection.Output;
        //    parameter.UdtTypeName = oracleUDTName;
        //    parameter.Value = value;
        //    return parameter;
        //}

        public  OracleParameter CreateInputParameter<N>(string name, OracleDbType dbType, N value)
        {
            OracleParameter parameter = new OracleParameter();
            parameter.ParameterName = name;
            parameter.OracleDbType = dbType;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = value;
            return parameter;
        }

        public static OracleParameter CreateOutputParameter<N>(string name, OracleDbType dbType, N value)
        {
            OracleParameter parameter = new OracleParameter();
            parameter.ParameterName = name;
            parameter.OracleDbType = dbType;
            parameter.Direction = ParameterDirection.Output;
            parameter.Value = value;
            return parameter;
        }

        public static OracleParameter ReturnValueParameter<N>(string name, OracleDbType dbType, N value)
        {
            OracleParameter parameter = new OracleParameter();
            parameter.ParameterName = name;
            parameter.OracleDbType = dbType;
            parameter.Direction = ParameterDirection.ReturnValue;
            parameter.Value = value;
            return parameter;
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (connection() != null)
                connection().Close();
        }

        #endregion
    }
}
