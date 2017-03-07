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
    public class HomeModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static string CheckAdmin(string username, string empnum)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateInputParameter<string>("user_name", OracleDbType.Varchar2, username);
                parameters[1] = con.CreateInputParameter<string>("emp_num", OracleDbType.Varchar2, empnum);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SELLOGINAD";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("returnVal", OracleDbType.Varchar2, RETURN_VALUE_BUFFER_SIZE);
                command.Parameters["returnVal"].Direction = ParameterDirection.ReturnValue;

                if (parameters != null && parameters.Length > 0)
                {
                    OracleParameterCollection cmdParams = command.Parameters;
                    for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
                }
                command.ExecuteNonQuery();
                bval = command.Parameters["returnVal"].Value.ToString();
                connect.Close();

            }
            catch (Exception ex)
            {
                bval = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
                return bval;
            }
            return bval;
        }

        public static string LoginLog(UserProfileViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            string logindate = DateTime.Now.ToShortDateString();
            string logintime = DateTime.Now.ToShortTimeString();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[5];
                parameters[0] = con.CreateInputParameter<string>("user_name", OracleDbType.Varchar2, model.UserName);
                parameters[1] = con.CreateInputParameter<string>("emp_num", OracleDbType.Varchar2, model.EmployeeNumber);
                parameters[2] = con.CreateInputParameter<string>("login_date", OracleDbType.Varchar2, logindate);
                parameters[3] = con.CreateInputParameter<string>("login_time", OracleDbType.Varchar2, logintime);
                parameters[4] = con.CreateInputParameter<string>("full_name", OracleDbType.Varchar2, model.FullName);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".umall_insrtlog";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("returnVal", OracleDbType.Varchar2, RETURN_VALUE_BUFFER_SIZE);
                command.Parameters["returnVal"].Direction = ParameterDirection.ReturnValue;

                if (parameters != null && parameters.Length > 0)
                {
                    OracleParameterCollection cmdParams = command.Parameters;
                    for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
                }
                command.ExecuteNonQuery();
                bval = command.Parameters["returnVal"].Value.ToString();
                connect.Close();

            }
            catch (Exception ex)
            {
                bval = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
                return bval;
            }
            return bval;
        }

        public static void LogoutLog(string logid)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            string logintime = DateTime.Now.ToShortTimeString();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateInputParameter<string>("log_id", OracleDbType.Int32, logid);
                parameters[1] = con.CreateInputParameter<string>("logout_time", OracleDbType.Varchar2, logintime);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_LOGOUTLOG";
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

        public List<PaymentViewModel> GetAcctForRefund(PendingViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            List<PaymentViewModel> acctInfo = new List<PaymentViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateCursorParameter("acct_amt_list");
                parameters[1] = con.CreateInputParameter<string>("order_id", OracleDbType.Varchar2, model.OrderId);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_GETACCTREFUND";
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
                    acctInfo.Add(new PaymentViewModel
                    {

                        AmountPaid = hd["DEBIT"].ToString(),
                        CustomerAcctNum = hd["ACCOUNTNUMBER"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return acctInfo;
               

            }
            catch (Exception ex)
            {
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
                return null;
            }
            
        }
    }
}