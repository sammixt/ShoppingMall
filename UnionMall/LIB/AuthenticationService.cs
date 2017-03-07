using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnionMall.ADServiceRef;
using UnionMall.ViewModels;

namespace UnionMall.LIB
{
    public class AuthenticationService
    {

        //private static UBN_SMS_ServiceClient client = new UBN_SMS_ServiceClient();
        private static UBN_SMS_ServiceClient client = new UBN_SMS_ServiceClient();
        private static string moduleId = "fcubs";
            //System.Configuration.ConfigurationManager.AppSettings["ModuleId"].ToString()
        public static bool Validate(string username, string password)
        {

            bool value = client.AdAuthenticate(username.ToLower(), password, moduleId);
            if (value == true)
            {
                return value;
            }else{
                return false;
            }
            
        }

        public static UserProfileViewModel GetUserProfile(string username)
        {
            try { 
                UserProfile user = client.GetUserProfile(username.ToLower(), moduleId);

                return new UserProfileViewModel
                {
                    UserName = username.ToLower(),
                    FullName = user.DisplayName,
                    EmployeeNumber = user.EmployeeId,
                    Email = user.Email,
                    Title = user.JobTitle,
                    BranchCode = user.BranchCode,
                    Branch = user.BranchName,
                    Department = user.Department
                };
            }
            catch (Exception e)
            {
                ErrorLogs.log(e.Message + "+ -------------------------------- + " + e.StackTrace );
                return null;
            }
        }

        public static IEnumerable<string> GetRoles(string username)
        {
            var roles = client.GetRolesForUser(username, moduleId).ToList();

            return roles;
        }
    }
}