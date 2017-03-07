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
    public class ShopModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static List<ProductViewModel> selectProducts(string product_id, string cat_id)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<ProductViewModel> productList = new List<ProductViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = con.CreateCursorParameter("product_list");
                parameters[1] = con.CreateInputParameter<string>("(product_id ", OracleDbType.Int32, product_id);
                parameters[2] = con.CreateInputParameter<string>("category_id ", OracleDbType.Varchar2, cat_id);

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLTPRDCTFOR";
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
                    productList.Add(new ProductViewModel
                    {
                        ProductId = Convert.ToInt32(hd["PRODUCTID"].ToString()),
                        ProductName = hd["PRODUCTNAME"].ToString(),
                        AdditionalInfo = hd["ADDITIONALINFO"].ToString(),
                        Description = hd["DESCRIPTION"].ToString(),
                        Quantity = hd["Quantity"].ToString(),
                        Category = hd["categoryname"].ToString(),
                        Price = Convert.ToDecimal(hd["price"].ToString()),
                        IsFeatured = Convert.ToBoolean(hd["ISFEATURED"].ToString()),
                        IsActive = Convert.ToBoolean(hd["isactive"].ToString()),
                        MainImage = hd["MAINIMG"].ToString(),
                        SubImage = hd["SUBIMG_I"].ToString(),
                        SubImageII = hd["SUBIMG_II"].ToString(),
                        SubImageIII = hd["SUBIMG_III"].ToString(),
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return productList;
            }
            catch (Exception ex)
            {
                productList = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + productList);
                return productList;
            }

        }

    }
}