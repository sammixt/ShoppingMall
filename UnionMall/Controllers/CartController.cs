using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using UnionMall.LIB;
using UnionMall.Models;
using UnionMall.ViewModels;

namespace UnionMall.Controllers
{
    public class CartController : Controller
    {
        public const string CartSessionKey = "OrderId";
        public string ShoppingCartItems ;
        private static Random random = new Random();
        private static string OrderId;


        //
        // GET: /Cart/
        public ActionResult Index()
        {
            ViewBag.Title = "Cart";
            ViewBag.Category = CategoryModels.selectCat();
            ViewBag.Branches = AcountRetrievalServices.GetBranches();
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            if (HttpContext.Cache[ShoppingCartItems] != null)
            {
            List<CartViewModel> productincart = new List<CartViewModel>();
            productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
            return View(productincart.ToList());
            }
            else
            {
                ViewBag.Category = CategoryModels.selectCat();
                return View();
            }
            
        }

        //use to keep track of orderid
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string TrackOrder(string orderid)
        {
            
                OrderId = RandomString(23);
                string values = OrderId + orderid;
                return values;
            
        }
        public ActionResult AddToCart(CartViewModel model)
        {

            List<CartViewModel> productincart;
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            string loginid = principal.FindFirst(ClaimTypes.Role).Value;
            if (HttpContext.Cache[CartSessionKey] == null)
            {
                HttpContext.Cache[CartSessionKey] = TrackOrder(loginid);
            }
            if (((List<CartViewModel>)HttpContext.Cache[ShoppingCartItems]) == null)
            {
                productincart = new List<CartViewModel>();
                productincart.Add(new CartViewModel
                {
                    OrderId = HttpContext.Cache[CartSessionKey].ToString(),
                    ProductId = model.ProductId,
                    ProductName = model.ProductName,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice,
                    ItemTotal = model.UnitPrice * model.Quantity,
                    MainImage = model.MainImage
                }

                );
                HttpContext.Cache[ShoppingCartItems] = productincart;
            }
            else
            {
                productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                var count = productincart.Where(p => p.ProductId == model.ProductId).FirstOrDefault();
                if(count != null)
                {
                    count.Quantity = model.Quantity;
                    count.ItemTotal = model.UnitPrice * model.Quantity;
                }
                else { 
                productincart.Add(new CartViewModel
                {
                    OrderId = HttpContext.Cache[CartSessionKey].ToString(),
                    ProductId = model.ProductId,
                    ProductName = model.ProductName,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice,
                    ItemTotal = model.UnitPrice * model.Quantity,
                    MainImage = model.MainImage
                    }
                  );
                }
                HttpContext.Cache[ShoppingCartItems] = productincart;
            }
            return Json(true);
        }

        public ActionResult UpdateCart(CartViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            var productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
            var itemToUpdate = productincart.SingleOrDefault(p => p.ProductId == model.ProductId);
            
            if (itemToUpdate != null)
            {
                if(model.Quantity == 0)
                {
                    RedirectToAction("RemoveCart", "Cart", model);
                }
                itemToUpdate.Quantity = model.Quantity;
                itemToUpdate.ItemTotal = itemToUpdate.UnitPrice * model.Quantity;
            }
            HttpContext.Cache[ShoppingCartItems] = productincart;
            
            return Json("Updated",JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveCart(CartViewModel model)
        {
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            var productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
            var itemToRemove = productincart.SingleOrDefault(p => p.ProductId == model.ProductId);
            if (itemToRemove != null)
                productincart.Remove(itemToRemove);
            HttpContext.Cache[ShoppingCartItems] = productincart;
            return Json("Removed",JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountCartQty()
        {
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            if (((List<CartViewModel>)HttpContext.Cache[ShoppingCartItems]) == null)
            {
                return Json("");
            }
            else 
            {
                var productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                double totalQty = productincart.Sum(p => p.Quantity);
                return Json(totalQty,JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult SumTotal()
        {
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            decimal? totalAmt;
            if (HttpContext.Cache[ShoppingCartItems] != null)
            {
                var productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                totalAmt = productincart.Sum(p => p.ItemTotal);
            }
            else { totalAmt = null; }
            return Json(totalAmt,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearCart()
        {
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            var productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
            productincart.Clear();
            HttpContext.Cache.Remove(ShoppingCartItems);
            return Json("clear",JsonRequestBehavior.AllowGet);
        }

        public ActionResult PickUpBranch(BranchViewModel model)
        {
            Session["pickupLocation"] = model.BranchLocationName;
            var principal = (ClaimsIdentity)User.Identity;
            ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            var name = principal.FindFirst(ClaimTypes.GivenName).Value;
            var emailAdd = principal.FindFirst(ClaimTypes.Email).Value;
            var empNum = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            var branch = principal.FindFirst(ClaimTypes.StateOrProvince).Value;
            if (HttpContext.Cache[ShoppingCartItems] != null)
            {
                List<CartViewModel> productincart = new List<CartViewModel>();
                productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                foreach (var productitem in productincart)
                {
                    CartModels.AddOrderItem(productitem);
                }
                var orderID =  productincart.Select(d => d.OrderId).FirstOrDefault();
                decimal? totalAmt = productincart.Sum(p => p.ItemTotal);
                string ordertime = DateTime.Now.ToShortTimeString();
                CartModels.AddOrderDetails(orderID, name, emailAdd, empNum, branch,
                model.BranchLocationName, totalAmt, "UNPAID", ordertime);
                return Json(true);
            }
            else { return Json(false); }
            
        }

	}
}