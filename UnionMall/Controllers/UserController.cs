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
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            var userInfo = UserModels.selectUser();
            return View(userInfo);
        }

        [HttpPost]
        public ActionResult AddUser(UserProfileViewModel model)
        {
            string outcome = UserModels.AddUser(model);
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Added a new user";
            LogEventModels.insertLogEvent(eventLog);
            return Json(outcome);
        }

        public ActionResult UpdateUser(UserProfileViewModel model)
        {
            UserModels.UpdateUser(model);
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Updated user";
            LogEventModels.insertLogEvent(eventLog);
            return Json(true);
        }
	}
}