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


namespace UnionMall.Controllers
{
    public class IndexController : Controller
    {
        //private string profileImgPath = System.Configuration.ConfigurationManager.AppSettings["ProfileImgPath"].ToString();

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
                if (profile.EmployeeNumber == null)
                {
                    ViewBag.ErrorMessage = "Invalid username or password.";
                    return View(model);
                }
                var LoginLog = HomeModels.LoginLog(profile);
                if (LoginLog == null)
                {
                    ViewBag.ErrorMessage = "Login Cannot be completed at the moment";

                    return View(model);
                }
                //var roles = AuthenticationService.GetRoles(model.UserName);
                //if (roles.Count() == 0)
                //{
                //    ViewBag.ErrorMessage = "You do not have permission to access this application. Please contact the system administrator for more details.";

                //    return View(model);
                //}

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
                return RedirectToAction("Index", "Shop");
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
            var ShoppingCartItems = principal.FindFirst(ClaimTypes.Name).Value;
            if (HttpContext.Cache[ShoppingCartItems] != null)
            {
                var productincart = (List<CartViewModel>)HttpContext.Cache[ShoppingCartItems];
                productincart.Clear();
                HttpContext.Cache.Remove(ShoppingCartItems);
            }
            HomeModels.LogoutLog(LogId);
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Index");
        }

        public ActionResult MyProfile()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = new List<string>();

            foreach (var r in identity.FindAll(ClaimTypes.Role))
            {
                roles.Add(r.Value.ToUpper());
            }

            return View(new UserProfileViewModel
            {
                UserName = identity.FindFirst(ClaimTypes.Name).Value,
                FullName = identity.FindFirst(ClaimTypes.GivenName).Value,
                EmployeeNumber = identity.FindFirst(ClaimTypes.SerialNumber).Value,
                Email = identity.FindFirst(ClaimTypes.Email).Value,
                Branch = identity.FindFirst(ClaimTypes.StateOrProvince).Value,
                BranchCode = identity.FindFirst(ClaimTypes.PostalCode).Value,
                Roles = roles,
                //ProfileImg = profileImgPath + identity.FindFirst(ClaimTypes.SerialNumber).Value + ".jpg"
            });
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
	}
}