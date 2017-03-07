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
    public class PendingOrdersModel
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static List<PendingViewModel> selectPendingOrders()
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<PendingViewModel> orders = new List<PendingViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateCursorParameter("pending_list");

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLTPDNORD";
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
                    orders.Add(new PendingViewModel
                    {

                        Name = hd["FULLNAME"].ToString(),
                        Email = hd["EMAIL"].ToString(),
                        Branch = hd["BRANCH"].ToString(),
                        PickUpBranch = hd["PICKUPBRANCH"].ToString(),
                        OrderDate = Convert.ToDateTime(hd["ORDERDATE"].ToString()),
                        Amount = hd["TOTALAMOUNT"].ToString(),
                        OrderId = hd["ORDERID"].ToString(),
                        OrderStatus = hd["ADMINSTATUS"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return orders;
            }
            catch (Exception ex)
            {
                orders = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + orders);
                return orders;
            }

        }

        public static List<CartViewModel> selectPendingOrderDetail(string order_id)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<CartViewModel> orders = new List<CartViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateCursorParameter("pending_list");
                parameters[1] = con.CreateInputParameter<string>("order_id", OracleDbType.Varchar2, order_id);

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SINORDDTL";
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
                    orders.Add(new CartViewModel
                    {

                        OrderId = hd["ORDERID"].ToString(),
                        ProductId = Convert.ToInt32(hd["PRODUCTID"].ToString()),
                        ProductName = hd["PRODUCTNAME"].ToString(),
                        Quantity  = Convert.ToInt32(hd["QUANTITY"].ToString()),
                        UnitPrice = Convert.ToDecimal(hd["PRICE"].ToString()),
                        ItemTotal = Convert.ToDecimal(hd["ITEMTOTAL"].ToString()),
                        MainImage = hd["MAINIMG"].ToString()

                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return orders;
            }
            catch (Exception ex)
            {
                orders = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + orders);
                return orders;
            }

        }

        public static void UpdateAdminStat(string stat,string order_id)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateInputParameter<string>("stat", OracleDbType.Varchar2, stat);
                parameters[1] = con.CreateInputParameter<string>("order_id", OracleDbType.Varchar2, order_id);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATEPAYSTAT";
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

        public static void UpdateAdminStatRefund(string stat, PendingViewModel order)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = con.CreateInputParameter<string>("stat", OracleDbType.Varchar2, stat);
                parameters[1] = con.CreateInputParameter<string>("order_id", OracleDbType.Varchar2, order.OrderId);
                parameters[2] = con.CreateInputParameter<string>("refund_comment", OracleDbType.Varchar2, order.Comment);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATEPAYREFUND";
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

        public static List<PendingViewModel> selectAllOrders(string startdate, string enddate)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<PendingViewModel> orders = new List<PendingViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = con.CreateCursorParameter("pending_list");
                parameters[1] = con.CreateInputParameter<string>("start_date", OracleDbType.Varchar2, startdate);
                parameters[2] = con.CreateInputParameter<string>("end_date", OracleDbType.Varchar2, enddate);

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLTALLORD";
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
                    orders.Add(new PendingViewModel
                    {

                        Name = hd["FULLNAME"].ToString(),
                        Email = hd["EMAIL"].ToString(),
                        Branch = hd["BRANCH"].ToString(),
                        PickUpBranch = hd["PICKUPBRANCH"].ToString(),
                        OrderDate = Convert.ToDateTime(hd["ORDERDATE"].ToString()),
                        Amount = hd["TOTALAMOUNT"].ToString(),
                        OrderId = hd["ORDERID"].ToString(),
                        OrderStatus = hd["ADMINSTATUS"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return orders;
            }
            catch (Exception ex)
            {
                orders = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + orders);
                return orders;
            }

        }
        //used to retrieve order history for users.
        public static List<PendingViewModel> selectOrdersHistory(string emp_num)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<PendingViewModel> orders = new List<PendingViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateCursorParameter("orders_list");
                parameters[1] = con.CreateInputParameter<string>("start_date", OracleDbType.Varchar2, emp_num);

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLTALLORDUSR";
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
                    orders.Add(new PendingViewModel
                    {

                        Name = hd["FULLNAME"].ToString(),
                        PickUpBranch = hd["PICKUPBRANCH"].ToString(),
                        OrderDate = Convert.ToDateTime(hd["ORDERDATE"].ToString()),
                        Amount = hd["TOTALAMOUNT"].ToString(),
                        PaymentStatus = hd["STATUS"].ToString(),
                        Time = hd["ORDERTIME"].ToString(),
                        OrderId = hd["ORDERID"].ToString(),
                        OrderStatus = hd["ADMINSTATUS"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return orders;
            }
            catch (Exception ex)
            {
                orders = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + orders);
                return orders;
            }

        }
    }
}