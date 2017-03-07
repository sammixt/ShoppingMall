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
    public class UserModels
    {
        private static string dbSchema = ConfigurationManager.AppSettings["DbSchema"];
        public static string AddUser(ViewModels.UserProfileViewModel model)
        {
            var profile = AuthenticationService.GetUserProfile(model.UserName);
            if (profile.EmployeeNumber != null)
            {
                    DbConnection con = new DbConnection();
                    OracleConnection connect = con.connection();
                    int RETURN_VALUE_BUFFER_SIZE = 32767;
                    string bval = "";
                    try
                    {
                        connect.Open();
                        OracleParameter[] parameters = new OracleParameter[8];
                        parameters[0] = con.CreateInputParameter<string>("user_name", OracleDbType.Varchar2,profile.UserName );
                        parameters[1] = con.CreateInputParameter<string>("full_name", OracleDbType.Varchar2,profile.FullName);
                        parameters[2] = con.CreateInputParameter<string>("emp_num", OracleDbType.Varchar2,profile.EmployeeNumber);
                        parameters[3] = con.CreateInputParameter<string>("branch_code", OracleDbType.Varchar2,profile.BranchCode);
                        parameters[4] = con.CreateInputParameter<string>("branch_name", OracleDbType.Varchar2,profile.Branch);
                        parameters[5] = con.CreateInputParameter<string>("job_title", OracleDbType.Varchar2,profile.Title);
                        parameters[6] = con.CreateInputParameter<string>("dept", OracleDbType.Varchar2, profile.Department);
                        parameters[7] = con.CreateInputParameter<string>("e_mail", OracleDbType.Varchar2, profile.Email);
                        OracleCommand command = connect.CreateCommand();
                        command.CommandText = dbSchema + ".UMALL_INSRTUSER";
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
            else
            {
                return "no record";
            }
        }

        public static List<UserProfileViewModel> selectUser()
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();

            List<UserProfileViewModel> userInfo = new List<UserProfileViewModel>();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[1];
                parameters[0] = con.CreateCursorParameter("cat_list");

                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_SLCTUSER";
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
                    userInfo.Add(new UserProfileViewModel
                    {
                        UserId = Convert.ToInt32(hd["USERID"].ToString()),
                        FullName = hd["FULLNAME"].ToString(),
                        EmployeeNumber = hd["EMPNUM"].ToString(),
                        UserName = hd["USERNAME"].ToString(),
                        Title = hd["TITLE"].ToString(),
                        Branch = hd["BRANCHNAME"].ToString(),
                        Department = hd["DEPARTMENT"].ToString(),
                        IsActive = hd["ISACTIVE"].ToString()
                    });

                }
                if (hd != null)
                    hd.Close();
                connect.Close();
                return userInfo;
            }
            catch (Exception ex)
            {
                userInfo = null;
                ErrorLogs.log(ex.Message + "+ -------------------------------- + " + ex.StackTrace + userInfo);
                return userInfo;
            }

        }

        public static void UpdateUser(UserProfileViewModel model)
        {
            DbConnection con = new DbConnection();
            OracleConnection connect = con.connection();
            try
            {
                connect.Open();
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = con.CreateInputParameter<int>("user_id", OracleDbType.Int64, model.UserId);
                parameters[1] = con.CreateInputParameter<string>("state", OracleDbType.Varchar2, model.IsActive);
                OracleCommand command = connect.CreateCommand();
                command.CommandText = dbSchema + ".UMALL_UPDATEUSER";
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