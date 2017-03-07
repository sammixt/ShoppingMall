using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnionMall.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Minimum 3 and Minimum 5 and Maximum 100 characters are allowed", MinimumLength = 3)]
        [System.Web.Mvc.Remote("CheckCategoryExist", "Admin", ErrorMessage = "Category already exist")]
        public string CategoryName { get; set; }
        public string IsActive { get; set; }
        public string IsDelete { get; set; }
    }
}
