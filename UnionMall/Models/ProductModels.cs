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
    public class ProductModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static string AddProduct(ProductViewModel model, List<string> img)
        {
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[13];
                parameters[0] = con.CreateInputParameter<string>("product_name", OracleDbType.Varchar2, model.ProductName);
                parameters[1] = con.CreateInputParameter<int>("category_id", OracleDbType.Int32, model.CategoryId);
                parameters[2] = con.CreateInputParameter<string>("is_active", OracleDbType.Varchar2, Convert.ToString(model.IsActive));
                parameters[3] = con.CreateInputParameter<string>("created_date", OracleDbType.Varchar2, model.CreatedDate);
                parameters[4] = con.CreateInputParameter<string>("descptn", OracleDbType.Varchar2, model.Description);
                parameters[5] = con.CreateInputParameter<string>("added_info", OracleDbType.Varchar2, model.AdditionalInfo);
                parameters[6] = con.CreateInputParameter<decimal>("amount", OracleDbType.Decimal, model.Price);
                parameters[7] = con.CreateInputParameter<string>("is_feature", OracleDbType.Varchar2, Convert.ToString(model.IsFeatured));
                parameters[8] = con.CreateInputParameter<string>("qty", OracleDbType.Varchar2, model.Quantity);
                parameters[9] = con.CreateInputParameter<string>("main_img", OracleDbType.Varchar2, img[0]);
                parameters[10] = con.CreateInputParameter<string>("sub_img_i", OracleDbType.Varchar2, img[1]);
                parameters[11] = con.CreateInputParameter<string>("sub_img_ii", OracleDbType.Varchar2, img[2]);
                parameters[12] = con.CreateInputParameter<string>("sub_img_iii", OracleDbType.Varchar2, img[3]);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_INSRTPRDCT";
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
                connect.Close();

            }
            catch (Exception ex)
            {
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
            }
            return bval;
        }

        //Update Product
        public static string UpdateProduct(ProductViewModel model)
        {
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[10];
                parameters[0] = con.CreateInputParameter<int>("product_id", OracleDbType.Varchar2, model.ProductId);
                parameters[1] = con.CreateInputParameter<string>("product_name", OracleDbType.Varchar2, model.ProductName);
                parameters[2] = con.CreateInputParameter<int>("category_id", OracleDbType.Int32, model.CategoryId);
                parameters[3] = con.CreateInputParameter<string>("is_active", OracleDbType.Varchar2, Convert.ToString(model.IsActive));
                parameters[4] = con.CreateInputParameter<string>("modified_date", OracleDbType.Varchar2, model.ModifiedDate);
                parameters[5] = con.CreateInputParameter<string>("descptn", OracleDbType.Varchar2, model.Description);
                parameters[6] = con.CreateInputParameter<string>("added_info", OracleDbType.Varchar2, model.AdditionalInfo);
                parameters[7] = con.CreateInputParameter<decimal>("amount", OracleDbType.Decimal, model.Price);
                parameters[8] = con.CreateInputParameter<string>("is_feature", OracleDbType.Varchar2, Convert.ToString(model.IsFeatured));
                parameters[9] = con.CreateInputParameter<string>("qty", OracleDbType.Varchar2, model.Quantity);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATEPRDCT";
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

        public static string UpdateProductImg(ProductViewModel model, List<string> img)
        {
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[5];
                parameters[0] = con.CreateInputParameter<int>("product_id", OracleDbType.Varchar2, model.ProductId);
                parameters[1] = con.CreateInputParameter<string>("main_img", OracleDbType.Varchar2, img[0]);
                parameters[2] = con.CreateInputParameter<string>("sub_img_i", OracleDbType.Varchar2, img[1]);
                parameters[3] = con.CreateInputParameter<string>("sub_img_ii", OracleDbType.Varchar2, img[2]);
                parameters[4] = con.CreateInputParameter<string>("sub_img_iii", OracleDbType.Varchar2, img[3]);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATEIMG";
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

        public static List<ProductViewModel> selectProducts()
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<ProductViewModel> catInfo = new List<ProductViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateCursorParameter("product_list");

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLCTPRDCTS";
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
                    catInfo.Add(new ProductViewModel
                    {
                        ProductId = Convert.ToInt32(hd["PRODUCTID"].ToString()),
                        ProductName = hd["PRODUCTNAME"].ToString(),
                        IsActive = Convert.ToBoolean(hd["ISACTIVE"].ToString()),
                        Quantity = hd["Quantity"].ToString(),
                        Category = hd["categoryname"].ToString(),
                        Price = Convert.ToDecimal(hd["price"].ToString()),
                        IsFeatured = Convert.ToBoolean(hd["ISFEATURED"].ToString())
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

        public static ProductViewModel selectProductToEdit(int id)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            ProductViewModel catInfo = new ProductViewModel();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateCursorParameter("product_list");
                parameters[1] = con.CreateInputParameter<int>("product_id", OracleDbType.Int32, id);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SELEDITPRDCT";
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

                while (hd.Read())
                {
                                     
                        catInfo.ProductId = Convert.ToInt32(hd["PRODUCTID"].ToString());
                        catInfo.ProductName = hd["productname"].ToString();
                        catInfo.IsActive = Convert.ToBoolean(hd["isactive"].ToString());
                        catInfo.IsFeatured = Convert.ToBoolean(hd["isfeatured"].ToString());
                        catInfo.Description = hd["description"].ToString();
                        catInfo.AdditionalInfo = hd["additionalinfo"].ToString();
                        catInfo.Quantity = hd["quantity"].ToString();
                        catInfo.Price = Convert.ToDecimal(hd["price"].ToString());
                        catInfo.MainImage = hd["mainimg"].ToString();
                        catInfo.SubImage = hd["subimg_i"].ToString();
                        catInfo.SubImageII = hd["subimg_ii"].ToString();
                        catInfo.SubImageIII = hd["subimg_iii"].ToString();
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


        public static string DeleteProduct(int product_id)
        {
            int RETURN_VALUE_BUFFER_SIZE = 32767;
            string bval = "";
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateInputParameter<int>("product_id", OracleDbType.Int32, product_id);
                
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_DELPRDCT";
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
                connect.Close();

            }
            catch (Exception ex)
            {
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace);
            }
            return bval;
        }
    }
}