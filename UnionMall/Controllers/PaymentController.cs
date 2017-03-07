using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using UnionMall.LIB;
using UnionMall.Models;
using UnionMall.ViewModels;

namespace UnionMall.Controllers
{
    public class PaymentController : Controller
    {
        public string ShoppingCartItems;
        public const string CartSessionKey = "OrderId";
        private static Random random = new Random();
        //
        // GET: /Payment/
        public ActionResult Index()
        {
            ViewBag.Title = "Payment";
            ViewBag.Category = CategoryModels.selectCat();
            ViewBag.AcctNum = ConfigurationManager.AppSettings["AcctNumber"];
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            if (HttpContext.Cache[ShoppingCartItems] != null)
            {
                List<CartViewModel> productincart = new List<CartViewModel>();
                productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                
                var orderID = productincart.Select(d => d.OrderId).FirstOrDefault();
                decimal? totalAmt = productincart.Sum(p => p.ItemTotal);
                ViewBag.OrderId = orderID;
                ViewBag.TotalAmount = totalAmt;
            }
            return View("Payment");
        }

        public ActionResult GenerateToken(PaymentViewModel model)
        {
            string resp = "";
            var userdetail = AcountRetrievalServices.getAcctAvailabilty(model.CustomerAcctNum);
            if (userdetail == null)
            {
                resp = "Account does not Exist";
            }
            else { 
            TokenService.sendToken(model.CustomerAcctNum, model.AmountPaid);
            resp = "success";
            }
            return Json(resp);
        }

        public ActionResult ValidateToken(PaymentViewModel model)
        {
            PostingServices gefu = new PostingServices(); 
            var principal = (ClaimsIdentity)User.Identity;
            string TransId = principal.FindFirst(ClaimTypes.Role).Value;
             var branch = principal.FindFirst(ClaimTypes.PostalCode).Value;
             var eventLog = new LogEventViewModel();
             //information to be logged on in the event list
             eventLog.LogId = Convert.ToInt32(TransId);
             eventLog.OrderID = model.paymentReference;
             eventLog.Event = "Validated Token";
             string BatchId = RandomString(23);
             LogEventModels.insertLogEvent(eventLog);
            var response = TokenService.validateOTP(model.CustomerAcctNum, model.Token);
            if (response == "SUCCESS")
            {
                PostingViewModel credit = new PostingViewModel();
                PostingViewModel debit = new PostingViewModel();
                //debit  customers account
                debit.accountNumber = model.CustomerAcctNum; 
                debit.amount = Convert.ToDecimal(model.AmountPaid);
                debit.branchCode = branch;
                debit.debitCreditIndicator = "D";
                debit.glCasaIndicator = "CASA";
                debit.narration = "Purchase of Items from Union Mall with Ref No. "+  model.paymentReference ;
                debit.paymentReference = model.paymentReference;
                debit.transactionId = TransId;
                debit.batchId = BatchId;
                //credit union mall account
                 credit.accountNumber = ConfigurationManager.AppSettings["AcctNumber"].ToString();
                credit.amount = Convert.ToDecimal(model.AmountPaid);
                credit.branchCode = branch;
                credit.debitCreditIndicator = "C";
                credit.glCasaIndicator = "GL";
                credit.narration = "Payment for purchase of Items from Union Mall with Ref No. "+  model.paymentReference ;
                credit.paymentReference = model.paymentReference;
                credit.transactionId = TransId;
                credit.batchId = BatchId;
                string status = gefu.postingTransaction(credit, debit);

                return Json(status);
            }
            else
            {
                return Json(response);
            }
            
        }

        public ActionResult PaymentSuccess()
        {
            ViewBag.Title = "Paymnent Success";
            ViewBag.Category = CategoryModels.selectCat();
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            var name = principal.FindFirst(ClaimTypes.GivenName).Value;
            var emailAdd = principal.FindFirst(ClaimTypes.Email).Value;
            var empNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            var branch = principal.FindFirst(ClaimTypes.StateOrProvince).Value;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            var eventLog = new LogEventViewModel();
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.Event = "Made Payment";
            SendEmail mailAdmin = new SendEmail();
            if (HttpContext.Cache[ShoppingCartItems] != null)
            {
                List<CartViewModel> productincart = new List<CartViewModel>();
                productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                foreach (var productitem in productincart)
                {
                    CartModels.UpdateQtyAfterOrder(productitem);
                }
                var orderID = productincart.Select(d => d.OrderId).FirstOrDefault();
                decimal? totalAmt = productincart.Sum(p => p.ItemTotal);
                CartModels.UpdatePaymentStat(orderID);
                eventLog.OrderID = orderID;
                LogEventModels.insertLogEvent(eventLog);
                List<string> repEmail = new List<string>(); repEmail.Add("scezeala@unionbankng.com");
                var title = "New Order, Order ID: " + orderID;
                var body = "<p>Name: " + name + "</p>";
                body += "<p>Email: " + emailAdd + "</p>";
                body += "<p>Branch: " + branch + "</p>";
                body += "<p>Preferred Pickup Location: " + Session["pickupLocation"] + "</p>";
                body += "<table><thead><tr><td>Product Name</td><td>Quantity</td><td>Unit Price</td> <td>Total</td><tbody>" ;
                          foreach(var items in productincart){
                              body += "<tr><td>" + items.ProductName + "</td>" +
                                   "<td>" + items.Quantity + "</td>" +
                                   "<td>" + items.UnitPrice + "</td>" +
                                   "<td>" + items.ItemTotal + "</td>"
                              + "</tr>";
                          }
                          body += "<tr><td colspan='3'>Total</td><td>" + totalAmt + "</td></tr>";
                    body += "</tbody></table>";
                    productincart.Clear();
                    HttpContext.Cache.Remove(ShoppingCartItems);
                    HttpContext.Cache[CartSessionKey] = "";
                    HttpContext.Cache.Remove(CartSessionKey);
                    mailAdmin.SendEmailWithoutAttachment(name, title, body, "Union Mall", repEmail, null, null);
            }
            return View();
        }

        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
	}
}