using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Web;
using UnionMall.LIB;
using UnionMall.ViewModels;

namespace UnionMall.Models
{
    public class CartModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static void AddOrderItem(CartViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[4];
                parameters[0] = con.CreateInputParameter<string>("order_id ", OracleDbType.Varchar2, model.OrderId);
                parameters[1] = con.CreateInputParameter<int>("product_id  ", OracleDbType.Int32, model.ProductId);
                parameters[2] = con.CreateInputParameter<int>("qty", OracleDbType.Int32, model.Quantity);
                parameters[3] = con.CreateInputParameter<decimal?>("item_total", OracleDbType.Decimal, model.ItemTotal);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_INSRTORDITM";
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

        public static void AddOrderDetails(string order_id,string full_name,string email_add, string emp_num, string branchs,
            string pickup_branch, decimal? total_amt,string payment_status, string order_time)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[9];
                parameters[0] = con.CreateInputParameter<string>("order_id ", OracleDbType.Varchar2, order_id);
                parameters[1] = con.CreateInputParameter<string>("full_name  ", OracleDbType.Varchar2, full_name);
                parameters[2] = con.CreateInputParameter<string>("email_add", OracleDbType.Varchar2, email_add);
                parameters[3] = con.CreateInputParameter<string>("emp_num", OracleDbType.Varchar2, emp_num);
                parameters[4] = con.CreateInputParameter<string>("branchs", OracleDbType.Varchar2, branchs);
                parameters[5] = con.CreateInputParameter<string>("pickup_branch", OracleDbType.Varchar2, pickup_branch);
                parameters[6] = con.CreateInputParameter<decimal?>("total_amt", OracleDbType.Decimal, total_amt);
                parameters[7] = con.CreateInputParameter<string>("payment_status", OracleDbType.Varchar2, payment_status);
                parameters[8] = con.CreateInputParameter<string>("order_time", OracleDbType.Varchar2, order_time);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_INSRTORDDTLS";
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

        public static void UpdatePaymentStat(string order_id)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateInputParameter<string>("order_id", OracleDbType.Varchar2, order_id);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATEORDDT";
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

        public static void UpdateQtyAfterOrder(CartViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateInputParameter<int>("product_id", OracleDbType.Int32, model.ProductId);
                parameters[1] = con.CreateInputParameter<int>("item_qty", OracleDbType.Int32, model.Quantity);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".umall_updtqtyord";
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
    }
}