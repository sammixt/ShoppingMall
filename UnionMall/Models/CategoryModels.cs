using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnionMall.LIB;
using UnionMall.ViewModels;

namespace UnionMall.Models
{
    public class CategoryModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static string AddCat(CategoryViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateInputParameter<string>("catName", OracleDbType.Varchar2, model.CategoryName);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_INSRTCAT";
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
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
            }
            return bval;
        }

        public static List<CategoryViewModel> selectCat()
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<CategoryViewModel> catInfo = new List<CategoryViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateCursorParameter("cat_list");
                
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLCTCAT";
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
                    catInfo.Add(new CategoryViewModel
                    {
                        CategoryId = Convert.ToInt32(hd["CATEGORYID"].ToString()),
                        CategoryName = hd["CATEGORYNAME"].ToString(),
                        IsActive = hd["ISACTIVE"].ToString(),
                        IsDelete = hd["ISDELETE"].ToString(),
                        
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return catInfo;
            }
            catch (Exception ex)
            {
                catInfo = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + catInfo);
                return catInfo;
            }

        }

        public static List<SelectListItem> CatDropDown()
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<SelectListItem> catInfo = new List<SelectListItem>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateCursorParameter("cat_list");

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLCTCAT";
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
                    catInfo.Add(new SelectListItem
                    {
                        Value = hd["CATEGORYID"].ToString(),
                        Text = hd["CATEGORYNAME"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return catInfo;
            }
            catch (Exception ex)
            {
                catInfo = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + catInfo);
                return catInfo;
            }

        }

        public static void DelCat(CategoryViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateInputParameter<int>("cat_id", OracleDbType.Int64, model.CategoryId);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_DELCAT";
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

        public static string UpdateCat(CategoryViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateInputParameter<string>("catName", OracleDbType.Varchar2, model.CategoryName);
                parameters[1] = con.CreateInputParameter<int>("cat_id", OracleDbType.Int32, model.CategoryId);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATECAT";
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
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
            }
            return bval;
        }



        public static void reAddCat(CategoryViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateInputParameter<int>("cat_id", OracleDbType.Int64, model.CategoryId);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_READDCAT";
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