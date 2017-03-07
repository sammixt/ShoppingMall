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
    public class EmailModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];

        public static List<UserProfileViewModel> SelectAdminEmail()
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<UserProfileViewModel> emailInfo = new List<UserProfileViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateCursorParameter("email_list");

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLTEMAILADM";
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
                    emailInfo.Add(new UserProfileViewModel
                    {
                        Email = hd["EMAIL"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return emailInfo;
            }
            catch (Exception ex)
            {
                emailInfo = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + emailInfo);
                return emailInfo;
            }

        }
    }
}