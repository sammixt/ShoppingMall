using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using UnionMall.LIB;
using UnionMall.ViewModels;

namespace UnionMall.Models
{
    public class LogEventModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static void insertLogEvent(LogEventViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = con.CreateInputParameter<int>("login_id", OracleDbType.Int64, model.LogId);
                parameters[1] = con.CreateInputParameter<string>("event_type", OracleDbType.Varchar2, model.Event);
                parameters[2] = con.CreateInputParameter<string>("order_id", OracleDbType.Varchar2, model.OrderID);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_INSTEVNTLOG";
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    OracleParameterCollection cmdParams = command.Parameters;
                    for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
                }
                command.ExecuteNonQuery();
                connect.Close();
            }
            catch (Exception ex)
            {
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
            }
        }

        public static List<LogEventViewModel> selectLogEvent(int login_id)
        {
            List<LogEventViewModel> logEvenInfo = new List<LogEventViewModel>();
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateCursorParameter("event_list");
                parameters[1] = con.CreateInputParameter<int>("login_id", OracleDbType.Int64, login_id);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLTEVNTLOG";
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    OracleParameterCollection cmdParams = command.Parameters;
                    for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
                }
                command.ExecuteNonQuery();

                OracleRefCursor r = (OracleRefCursor)parameters[0].Value;
                OracleDataReader hd = null;
                if (r != null)
                {
                    hd = r.GetDataReader();
                }

                decimal row_id = 0;
                while (hd.Read())
                {
                    row_id++;
                    logEvenInfo.Add(new LogEventViewModel
                    {
                        LogId = Convert.ToInt32(hd["LOGID"].ToString()),
                        Event = hd["EVENT"].ToString(),
                        OrderID = hd["ORDERID"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return logEvenInfo;
            }
            catch (Exception ex)
            {
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
                return null;
            }
        }
    }
}