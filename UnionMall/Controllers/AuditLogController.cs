using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnionMall.Models;
using UnionMall.ViewModels;

namespace UnionMall.Controllers
{
    [Authorize]
    public class AuditLogController : Controller
    {
        //
        // GET: /AuditLog/
        public ActionResult LoginLog(LoginLogViewModel model)
        {
            string strDate = String.Format("{0:MM/dd/yyyy}", model.SearchStartDate);
            string endDate = String.Format("{0:MM/dd/yyyy}", model.SearchEndDate);
            if (strDate == "" && endDate == "")
            {
                strDate = DateTime.Now.ToShortDateString();
                endDate = DateTime.Now.ToShortDateString();
            }
            var loginLogDetails = AuditLogModels.retrieveLoginLog(strDate, endDate);
            return View(loginLogDetails);
        }

        public ActionResult EventLog(string login_id = null)
        {
            ViewBag.LoginID = login_id;
            var EventLogDetails = LogEventModels.selectLogEvent(Convert.ToInt32(login_id));
            return View(EventLogDetails);
        }

        public ActionResult Detail(string order,string login_id)
        {
            ViewBag.LoginID = login_id;
            var PendingOrders = PendingOrdersModel.selectPendingOrders();
            var GetName = PendingOrders.Where(p => p.OrderId == order).FirstOrDefault();
            var SinglePendingOrders = PendingOrdersModel.selectPendingOrderDetail(order);
            ViewBag.Name = GetName;
            return View("Detail", SinglePendingOrders);
        }

        public ActionResult PostingLog(PostingViewModel model, string order = null)
        {
            if (order == null)
            {
                string strDate = String.Format("{0:MM/dd/yyyy}", model.SearchStartDate);
                string endDate = String.Format("{0:MM/dd/yyyy}", model.SearchEndDate);
                if (strDate == "" && endDate == "")
                {
                    strDate = DateTime.Now.ToShortDateString();
                    endDate = DateTime.Now.ToShortDateString();
                }
                var CreditLogDetails = AuditLogModels.displayPostingLogCr(strDate, endDate, order);
                ViewBag.DebitLogDetails = AuditLogModels.displayPostingLogDr(strDate, endDate, order);
                return View(CreditLogDetails);
            }
            else
            {
                var CreditLogDetails = AuditLogModels.displayPostingLogCr(null, null, order);
                ViewBag.DebitLogDetails = AuditLogModels.displayPostingLogDr(null, null, order);
                return View(CreditLogDetails);
            }

        }

	}
}