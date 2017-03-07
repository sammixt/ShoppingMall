using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using UnionMall.Models;
using UnionMall.ViewModels;

namespace UnionMall.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        //
        // GET: /Category/
        public ActionResult Index()
        {
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Visited Category Page";
            LogEventModels.insertLogEvent(eventLog);
            var catInfo = CategoryModels.selectCat();
            return View(catInfo);
        }

        [HttpPost]
        public ActionResult AddCat(CategoryViewModel model)
        {
            string outcome = CategoryModels.AddCat(model);
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Added new Category";
            LogEventModels.insertLogEvent(eventLog);
            return Json(outcome);
        }

        [HttpPost]
        public ActionResult DelCat(CategoryViewModel model)
        {
            CategoryModels.DelCat(model);
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Deleted Category";
            LogEventModels.insertLogEvent(eventLog);
            return Json(true);
        }

        [HttpPost]
        public ActionResult reAddCat(CategoryViewModel model)
        {
            CategoryModels.reAddCat(model);
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Readded Category";
            LogEventModels.insertLogEvent(eventLog);
            return Json(true);
        }

        public ActionResult UpdateCat(CategoryViewModel model)
        {
            string outcome = CategoryModels.UpdateCat(model);
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Updated Category";
            LogEventModels.insertLogEvent(eventLog);
            return Json(outcome);
        }
	}
}