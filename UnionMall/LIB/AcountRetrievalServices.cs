using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using UnionMall.FcubsGetService;
using UnionMall.ViewModels;

namespace UnionMall.LIB
{
    public class AcountRetrievalServices
    {
        private static EnquiryServiceClient RetrievalServices()
        {
            var proxy = new FcubsGetService.EnquiryServiceClient();
            proxy.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SrvUsername"];
            proxy.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SrvPassword"];
            return proxy;
        }


        public static List<BranchViewModel> GetBranches()
        {
            List<BranchViewModel> Branches = new List<BranchViewModel>();
            var proxy = RetrievalServices();
            var acctEnqHeader = new UBNHeaderType();
            var BranchList = proxy.getBranchList(ref acctEnqHeader);
            if (BranchList != null)
            {
                foreach (var item in BranchList)
                {
                    Branches.Add(new BranchViewModel
                    {
                        BranchId = item.branchCode,
                        BranchLocationName = item.branchName
                    });
                }
                return Branches;
            }
            else { return null; }
        }

        public static UserProfileViewModel getAcctAvailabilty(string acctNum)
        {
            var proxy = RetrievalServices();
            var acctEnqHeader = new UBNHeaderType();
            var userAcctDetails = new UserProfileViewModel();
            var AcctInfo = proxy.getCustomerDetailsWithAcctNumber(ref acctEnqHeader, acctNum);
            if (AcctInfo == null) { return null; }
            else
            {
            userAcctDetails.FullName = AcctInfo.customerName;
            userAcctDetails.Email = AcctInfo.custEmail;
            userAcctDetails.CustId = AcctInfo.custid;
                return userAcctDetails;
            }
        }
    }
}