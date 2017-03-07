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
    public class AuditLogModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static List<LoginLogViewModel> retrieveLoginLog(string start_date, string end_date)
        {
            DateTime? MyNullableDate = null;
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            List<LoginLogViewModel> loginLog = new List<LoginLogViewModel>();
            connect.Open();
            OracleParameter[] parameters = new OracleParameter[3];
            parameters[0] = con.CreateCursorParameter("loginlog_list");
            parameters[1] = con.CreateInputParameter<string>("start_date", OracleDbType.Varchar2, start_date);
            parameters[2] = con.CreateInputParameter<string>("end_date", OracleDbType.Varchar2, end_date);

            OracleCommand command = connect.CreateCommand();
            command.CommandText = dbSchema + ".umall_search_login";
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
                loginLog.Add(new LoginLogViewModel
                {
                    LoginId = hd["LOGID"].ToString(),
                    StaffId = hd["EMPNUM"].ToString(),
                    LoginTime = hd["LOGINTIME"].ToString(),
                    LoginOutTime = hd["LOGOUTTIME"].ToString(),
                    LoginDate = (hd["LOGINDATE"].ToString() != "") ? Convert.ToDateTime(hd["LOGINDATE"].ToString()) : MyNullableDate,
                    LoginOutDate = (hd["LOGOUTDATE"].ToString() != "") ? Convert.ToDateTime(hd["LOGOUTDATE"].ToString()) : MyNullableDate,
                });

            }
            if (hd != null)
                hd.Close();
            connect.Close();
            return loginLog;

        }

        public static List<PostingViewModel> displayPostingLogCr(string start_date = null, string end_date = null, string reqid = null)
        {

            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            List<PostingViewModel> postingInfo = new List<PostingViewModel>();
            OracleCommand command = connect.CreateCommand();
            OracleParameter[] parameters;
            try
            {
                connect.Open();
                if (reqid == null)
                {
                    parameters = new OracleParameter[3];
                    parameters[0] = con.CreateCursorParameter("request_list");
                    parameters[1] = con.CreateInputParameter<string>("start_date", OracleDbType.Varchar2, start_date);
                    parameters[2] = con.CreateInputParameter<string>("end_date", OracleDbType.Varchar2, end_date);
                    command.CommandText = dbSchema + ".umall_search_credlog";
                }
                else
                {
                    parameters = new OracleParameter[2];
                    parameters[0] = con.CreateCursorParameter("request_list");
                    parameters[1] = con.CreateInputParameter<string>("request_id", OracleDbType.Varchar2, reqid);
                    command.CommandText = dbSchema + ".umall_search_credlogid";
                }
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

                //decimal row_id = 0;
                while (hd.Read())
                {
                    //row_id++;
                    postingInfo.Add(new PostingViewModel
                    {
                        transactionId = hd["REQUESTID"].ToString(),
                        paymentReference = hd["PAYMENTREF"].ToString(),
                        accountNumber = hd["ACCOUNTNUMBER"].ToString(),
                        amount = Convert.ToDecimal(hd["CREDIT"].ToString()),
                        branchCode = hd["INITIATINGBRANCH"].ToString(),
                        glCasaIndicator = hd["GLCASAINDICATOR"].ToString(),
                        narration = hd["NARRATION"].ToString(),
                        transactionDate = hd["TRANSACTIONSTARTDATE"].ToString(),
                        currency = hd["CURRENCY"].ToString(),
                        // RequestAuthorizationDate = Convert.ToDateTime(X),
                        status = hd["STATUS"].ToString(),
                        responseCode = hd["RESPONSECODE"].ToString(),
                        responseMsg = hd["RESPONSEMSG"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return postingInfo;
            }
            catch (Exception ex)
            {
                postingInfo = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + postingInfo);
                return postingInfo;
            }

        }

        public static List<PostingViewModel> displayPostingLogDr(string start_date = null, string end_date = null, string reqid = null)
        {

            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            List<PostingViewModel> postingInfo = new List<PostingViewModel>();
            OracleCommand command = connect.CreateCommand();
            OracleParameter[] parameters;
            try
            {
                connect.Open();
                if (reqid == null)
                {
                    parameters = new OracleParameter[3];
                    parameters[0] = con.CreateCursorParameter("request_list");
                    parameters[1] = con.CreateInputParameter<string>("start_date", OracleDbType.Varchar2, start_date);
                    parameters[2] = con.CreateInputParameter<string>("end_date", OracleDbType.Varchar2, end_date);
                    command.CommandText = dbSchema + ".umall_search_debitlog";
                }
                else
                {
                    parameters = new OracleParameter[2];
                    parameters[0] = con.CreateCursorParameter("request_list");
                    parameters[1] = con.CreateInputParameter<string>("request_id", OracleDbType.Varchar2, reqid);
                    command.CommandText = dbSchema + ".umall_search_debitlogid";
                }
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

                //decimal row_id = 0;
                while (hd.Read())
                {
                    //row_id++;
                    postingInfo.Add(new PostingViewModel
                    {
                        transactionId = hd["REQUESTID"].ToString(),
                        paymentReference = hd["PAYMENTREF"].ToString(),
                        accountNumber = hd["ACCOUNTNUMBER"].ToString(),
                        amount = Convert.ToDecimal(hd["DEBIT"].ToString()),
                        branchCode = hd["INITIATINGBRANCH"].ToString(),
                        glCasaIndicator = hd["GLCASAINDICATOR"].ToString(),
                        narration = hd["NARRATION"].ToString(),
                        transactionDate = hd["TRANSACTIONSTARTDATE"].ToString(),
                        currency = hd["CURRENCY"].ToString(),
                        // RequestAuthorizationDate = Convert.ToDateTime(X),
                        status = hd["STATUS"].ToString(),
                        responseCode = hd["RESPONSECODE"].ToString(),
                        responseMsg = hd["RESPONSEMSG"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return postingInfo;
            }
            catch (Exception ex)
            {
                postingInfo = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + postingInfo);
                return postingInfo;
            }

        }
    }
}