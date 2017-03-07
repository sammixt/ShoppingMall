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
    public class ShopController : Controller
    {
        //
        // GET: /Shop/
        public ActionResult Index(string category = null, string product = null )
        {
            ViewBag.Category = CategoryModels.selectCat();
            var products = ShopModels.selectProducts(product, category);
            return View(products);
        }

        public ActionResult SingleItem(string category = null, string product = null)
        {
            ViewBag.Category = CategoryModels.selectCat();
            var products = ShopModels.selectProducts(product, category);
            return View(products.FirstOrDefault());
        }

        public ActionResult History(string orderid = null)
        {
            ViewBag.Category = CategoryModels.selectCat();
            var principal = (ClaimsIdentity)User.Identity;
            string emp_num = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            if (orderid == null)
            {
            var orderhistory = PendingOrdersModel.selectOrdersHistory(emp_num);
            return View(orderhistory);
                }else{
                var PendingOrders = PendingOrdersModel.selectPendingOrders();
                var GetName = PendingOrders.Where(p => p.OrderId == orderid).FirstOrDefault();
                var SinglePendingOrders = PendingOrdersModel.selectPendingOrderDetail(orderid);
            ViewBag.Name = GetName;
            return View("HistoryDetails",SinglePendingOrders);
            }
        }

        
	}
}