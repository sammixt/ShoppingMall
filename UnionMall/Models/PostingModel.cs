using Oracle.ManagedDataAccess.Client;
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
    public class PostingModel
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
       
        public static void AddLog(int REQUEST_ID, string PAYMENT_REF, string BATCH_ID, string branch_code,
            string acct_num, decimal amount, string narration, DateTime start_date, string GLCASA_INDICATOR,
            string action_name, string func_proc)
        {

            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            connect.Open();
            OracleParameter[] parameters = new OracleParameter[11];
            parameters[0] = con.CreateInputParameter<int>("REQUEST_ID", OracleDbType.Int32, REQUEST_ID);
            parameters[1] = con.CreateInputParameter<string>("PAYMENT_REF", OracleDbType.Varchar2, PAYMENT_REF);
            parameters[2] = con.CreateInputParameter<string>("BATCH_ID", OracleDbType.Varchar2, BATCH_ID);
            parameters[3] = con.CreateInputParameter<string>("INITIATING_BRANCH", OracleDbType.Varchar2, branch_code);
            parameters[4] = con.CreateInputParameter<string>("ACCOUNT_NUMBER", OracleDbType.Varchar2, acct_num);
            parameters[5] = con.CreateInputParameter<decimal>(action_name, OracleDbType.Decimal, amount);
            parameters[6] = con.CreateInputParameter<string>("CURRENCYS", OracleDbType.Varchar2, "NGN");
            parameters[7] = con.CreateInputParameter<string>("NARRATIONS", OracleDbType.Varchar2, narration);
            parameters[8] = con.CreateInputParameter<DateTime>("TRANSACTION_STARTDATE", OracleDbType.Date, start_date);
            parameters[9] = con.CreateInputParameter<string>("STAT", OracleDbType.Varchar2, "PENDING");
            parameters[10] = con.CreateInputParameter<string>("GLCASA_INDICATOR", OracleDbType.Varchar2, GLCASA_INDICATOR);


            OracleCommand command = connect.CreateCommand();
            command.CommandText = dbSchema + func_proc;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null && parameters.Length > 0)
            {
                OracleParameterCollection cmdParams = command.Parameters;
                for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
            }
            command.ExecuteNonQuery();
        }

        public static void UpdateLog(int REQUEST_ID, string PAYMENT_REF, DateTime end_date, string stat, string response_code, string response_msg,
            string func_proc)
        {

            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            connect.Open();
            OracleParameter[] parameters = new OracleParameter[6];
            parameters[0] = con.CreateInputParameter<int>("REQUEST_ID", OracleDbType.Int32, REQUEST_ID);
            parameters[1] = con.CreateInputParameter<string>("PAYMENT_REF", OracleDbType.Varchar2, PAYMENT_REF);
            parameters[2] = con.CreateInputParameter<DateTime>("TRANSACTION_ENDDATE", OracleDbType.Date, end_date);
            parameters[3] = con.CreateInputParameter<string>("STAT", OracleDbType.Varchar2, stat);
            parameters[4] = con.CreateInputParameter<string>("RESPONSE_CODE", OracleDbType.Varchar2, response_code);
            parameters[5] = con.CreateInputParameter<string>("RESPONSE_MSG", OracleDbType.Varchar2, response_msg);
            OracleCommand command = connect.CreateCommand();
            command.CommandText = dbSchema + func_proc;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null && parameters.Length > 0)
            {
                OracleParameterCollection cmdParams = command.Parameters;
                for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
            }
            command.ExecuteNonQuery();
        }
    }
}