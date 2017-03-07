using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using UnionMall.Models;
using UnionMall.ViewModels;

namespace UnionMall.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index()
        {
            return View(ProductModels.selectProducts());
        }

        public ActionResult AddProduct()
        {
            ProductViewModel product = new ProductViewModel();
            product.Categories = CategoryModels.CatDropDown();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertProduct(ProductViewModel model)
        {
           List<string> ImageName = new List<string> ();
           model.Categories = CategoryModels.CatDropDown();
            var selectedItem = model.Categories.Find(p => p.Value == model.CategoryId.ToString());
            selectedItem.Selected = true;
            string textC = selectedItem.Text;
            model.CategoryId = Convert.ToInt32(selectedItem.Value);
            model.CreatedDate = DateTime.Now.ToShortDateString();
            //string subPath ="ImagesPath"; // your code goes here

            bool exists = Directory.Exists(Server.MapPath("~/UploadedFiles/" + model.ProductName+"/"));
            if (!exists) {
                Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/" + model.ProductName + "/"));
            }

           foreach (string file in Request.Files)
            {
                var postedFile = Request.Files[file];
                if (postedFile.FileName != "")
                {
                    postedFile.SaveAs(Server.MapPath("~/UploadedFiles/" + model.ProductName + "/") + Path.GetFileName(postedFile.FileName));
                }
                ImageName.Add(postedFile.FileName);
            }

           string productId = ProductModels.AddProduct(model, ImageName);
           var eventLog = new LogEventViewModel();
           var principal = (ClaimsIdentity)User.Identity;
           string LogId = principal.FindFirst(ClaimTypes.Role).Value;
           //information to be logged on in the event list
           eventLog.LogId = Convert.ToInt32(LogId);
           eventLog.OrderID = "";
           eventLog.Event = "Added a new product";
           TempData["ProductId"] = productId;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProduct(ProductViewModel model)
        {
            model.Categories = CategoryModels.CatDropDown();
            var selectedItem = model.Categories.Find(p => p.Value == model.CategoryId.ToString());
            selectedItem.Selected = true;
            string textC = selectedItem.Text;
            model.CategoryId = Convert.ToInt32(selectedItem.Value);
            model.ModifiedDate = DateTime.Now.ToShortDateString();
            //string subPath ="ImagesPath"; // your code goes here
           string productId = ProductModels.UpdateProduct(model);
            TempData["ProductId"] = productId;
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Updated Product";
            LogEventModels.insertLogEvent(eventLog);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateImage(ProductViewModel model)
        {
            List<string> ImageName = new List<string>();
            
            //string subPath ="ImagesPath"; // your code goes here

            bool exists = Directory.Exists(Server.MapPath("~/UploadedFiles/" + model.ProductName + "/"));
            if (!exists)
            {
                Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/" + model.ProductName + "/"));
            }

            foreach (string file in Request.Files)
            {
                var postedFile = Request.Files[file];
                if (postedFile.FileName != "")
                {
                    postedFile.SaveAs(Server.MapPath("~/UploadedFiles/" + model.ProductName + "/") + Path.GetFileName(postedFile.FileName));
                }
                ImageName.Add(postedFile.FileName);
            }

            string productId = ProductModels.UpdateProductImg(model, ImageName);
            TempData["ProductId"] = productId;
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Updated Product Image";
            LogEventModels.insertLogEvent(eventLog);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(ProductViewModel id)
        {
            string product_name = ProductModels.DeleteProduct(id.ProductId);
            //Change on Productn
            string path = @"~\UploadedFiles\" + product_name;
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(path))
                {
                    System.IO.File.Delete(file);
                }
                //Delete a Directory
                Directory.Delete(path);
            }
            var eventLog = new LogEventViewModel();
            var principal = (ClaimsIdentity)User.Identity;
            string LogId = principal.FindFirst(ClaimTypes.Role).Value;
            //information to be logged on in the event list
            eventLog.LogId = Convert.ToInt32(LogId);
            eventLog.OrderID = "";
            eventLog.Event = "Deleted Product";
            LogEventModels.insertLogEvent(eventLog);
            return Json(true);
        }

        public ActionResult Edit(int id) 
        {

            var editProduct = ProductModels.selectProductToEdit(id);
            ProductViewModel product = new ProductViewModel();
            product.Categories = CategoryModels.CatDropDown();
                   product.ProductId = editProduct.ProductId; 
                   product.ProductName =     editProduct.ProductName; 
                   product.IsActive =   editProduct.IsActive ;
                   product.IsFeatured =  editProduct.IsFeatured; 
                   product.Description =   editProduct.Description; 
                   product.AdditionalInfo =  editProduct.AdditionalInfo ;
                   product.Quantity =  editProduct.Quantity ;
                   product.Price =   editProduct.Price ;
                   product.MainImage = editProduct.MainImage;
                   product.SubImage =  editProduct.SubImage ;
                   product.SubImageII = editProduct.SubImageII; 
                   product.SubImageIII =  editProduct.SubImageIII;
     
            return View(product);
        }
	}
}