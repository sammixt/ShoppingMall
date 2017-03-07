using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnionMall.ViewModels;
using UnionMall.LIB;
using UnionMall.Models;
using System.Configuration;

namespace UnionMall.Controllers
{
    public class HomeController : Controller
    {
        private static Random random = new Random();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            var pass = AuthenticationService.Validate(model.UserName, model.Password);
            //var pass = true;

            if (!pass)
            {
                var claims = new List<Claim>();
                var profile = AuthenticationService.GetUserProfile(model.UserName);
                if(profile.EmployeeNumber == null)
                {
                    ViewBag.ErrorMessage = "Invalid username or password.";
                    return View(model);
                }
                var roles =  HomeModels.CheckAdmin(profile.UserName,profile.EmployeeNumber);
                if (roles == null)
                {
                    ViewBag.ErrorMessage = "You do not have permission to access this Section. Please contact the system administrator for more details.";

                    return View(model);
                }
                var LoginLog = HomeModels.LoginLog(profile);
                if (LoginLog == null)
                {
                    ViewBag.ErrorMessage = "Login Cannot be completed at the moment";

                    return View(model);
                }
                claims.Add(new Claim(ClaimTypes.Name, profile.UserName));
                claims.Add(new Claim(ClaimTypes.GivenName, profile.FullName));
                claims.Add(new Claim(ClaimTypes.SerialNumber, profile.EmployeeNumber));
                claims.Add(new Claim(ClaimTypes.Email, profile.Email));
                claims.Add(new Claim(ClaimTypes.Surname, profile.Title));
                claims.Add(new Claim(ClaimTypes.StateOrProvince, profile.Branch));
                claims.Add(new Claim(ClaimTypes.PostalCode, profile.BranchCode));
                claims.Add(new Claim(ClaimTypes.Role, LoginLog));

                //foreach (var r in roles)
                //{
                //    claims.Add(new Claim(ClaimTypes.Role, r));
                //}

                SignInAsync(new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie));
                return RedirectToAction("Admin","Home");
                //return RedirectToLocal(returnUrl);
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            HomeModels.LogoutLog(LogId);
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Home");
        }
        [Authorize]
        public ActionResult Admin()
        {
            ViewBag.Posting = TempData["FailedPosting"];
            var PendingOrders = PendingOrdersModel.selectPendingOrders();
            return View(PendingOrders);
        }

        [Authorize]
        public ActionResult Detail(string order)
        {
            var PendingOrders = PendingOrdersModel.selectPendingOrders();
            var GetName = PendingOrders.Where(p => p.OrderId == order).FirstOrDefault();
            var  SinglePendingOrders = PendingOrdersModel.selectPendingOrderDetail(order);
            ViewBag.Name = GetName;
            return View(SinglePendingOrders);
        }

        [Authorize]
        public ActionResult Process(string order)
        {
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            SendEmail mailUser = new SendEmail();
            var PendingOrders = PendingOrdersModel.selectPendingOrders();
            var GetName = PendingOrders.Where(p => p.OrderId == order).FirstOrDefault();
            string email = GetName.Email;
            string name = GetName.Name;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = order;
            eventLog.Event = "Processed Request";
            PendingOrdersModel.UpdateAdminStat("PROCESSED", order);
            LogEventModels.insertLogEvent(eventLog);
            List<string> repEmail = new List<string>(); repEmail.Add(email);
            var title = "Order Processed, Order ID: " + order;
            var body = "<p>Your request have been processed, delivery soon</p>";
            mailUser.SendEmailWithoutAttachment(name, title, body, "Union Mall", repEmail, null, null);
            return RedirectToAction("Admin","Home");
        }

        [HttpPost]
        public ActionResult Refund(PendingViewModel order)
        {
            var eventLog = new LogEventViewModel();
            var home = new HomeModels();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            PostingServices gefu = new PostingServices();
            SendEmail mailUser = new SendEmail();
            var PendingOrders = PendingOrdersModel.selectPendingOrders();
            var GetName = PendingOrders.Where(p => p.OrderId == order.OrderId).FirstOrDefault();
            string email = GetName.Email;
            string TransId = RandomString(23);
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = order.OrderId;
            eventLog.Event = "Refunds";
            //get the email address and price of goods purchased by the user.
            var cusAcctPrice = home.GetAcctForRefund(order).FirstOrDefault();
            PostingViewModel credit = new PostingViewModel();
            PostingViewModel debit = new PostingViewModel();
            //debit  customers account
            debit.accountNumber = ConfigurationManager.AppSettings["AcctNumber"].ToString(); ;
            debit.amount = Convert.ToDecimal(cusAcctPrice.AmountPaid);
            debit.branchCode = "";
            debit.debitCreditIndicator = "D";
            debit.glCasaIndicator = "GL";
            debit.narration = "Refunds of Ordered Items with Ref No. " + order.OrderId;
            debit.paymentReference = order.OrderId+"Ref";
            debit.transactionId = LogId;
            debit.batchId = TransId;
            //credit union mall account
            credit.accountNumber = cusAcctPrice.CustomerAcctNum;
            credit.amount = Convert.ToDecimal(cusAcctPrice.AmountPaid);
            credit.branchCode = "";
            credit.debitCreditIndicator = "C";
            credit.glCasaIndicator = "CASA";
            credit.narration = "Refunds of Ordered Items with Ref No. " + order.OrderId;
            credit.paymentReference = order.OrderId + "Ref";
            credit.transactionId = LogId;
            credit.batchId = TransId;
            string status = gefu.postingTransaction(credit, debit);
            LogEventModels.insertLogEvent(eventLog);
            if (status == "FAILED")
            {
                TempData["FailedPosting"] = "Transaction Failed";
                return RedirectToAction("Admin", "Home");
            }
            PendingOrdersModel.UpdateAdminStatRefund("REFUNDED", order);
            List<string> repEmail = new List<string>(); repEmail.Add(email);
            var title = "Order Refund, Order ID: " + order.OrderId;
            var body = "<p>Your request have rejected due to ( "+ order.Comment + " ), and fund's reversed</p>";
            mailUser.SendEmailWithoutAttachment("Admin", title, body, "Union Mall", repEmail, null, null);
            return RedirectToAction("Admin", "Home");
        }

        [Authorize]
        public ActionResult AllOrder(PendingViewModel model)
        {
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            var start_date = DateTime.Now.ToShortDateString();
            var end_date = DateTime.Now.ToShortDateString();
            var PendingOrders = PendingOrdersModel.selectAllOrders(start_date, end_date);
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Viewed all Orders";
            return View("AllOrders", PendingOrders);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AllOrders(PendingViewModel model)
        {
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            string start_date = String.Format("{0:MM/dd/yyyy}", model.SearchStartDate);
            string end_date = String.Format("{0:MM/dd/yyyy}", model.SearchEndDate);
            var PendingOrders = PendingOrdersModel.selectAllOrders(start_date, end_date);
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Searched all Orders from " + start_date + " to " + end_date;
            return View("AllOrders",PendingOrders);
        }

         [Authorize]
        public ActionResult AllDetails(string order)
        {
            var PendingOrders = PendingOrdersModel.selectPendingOrders();
            var GetName = PendingOrders.Where(p => p.OrderId == order).FirstOrDefault();
            var SinglePendingOrders = PendingOrdersModel.selectPendingOrderDetail(order);
            ViewBag.Name = GetName;
            return View("Detail",SinglePendingOrders);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SignInAsync(ClaimsIdentity id)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(id);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}