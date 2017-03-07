using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnionMall.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Minimum 5 and Maximum 100 characters are allowed", MinimumLength = 3)]
        //[System.Web.Mvc.Remote("CheckProductExist", "Admin", ErrorMessage = "Product already exist")]
        [Display(Name = "Name")]
        public string ProductName { get; set; }

        public string Quantity { get; set; }
        [Required]
        [Range(1, 50)]
        public int CategoryId { get; set; }

        [Display(Name = "Active")]

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [AllowHtml]
        public string AdditionalInfo { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "1", "2000000", ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Featured")]
        public bool IsFeatured { get; set; }
        [Display(Name = "Main Image")]
        public string MainImage { get; set; }
        [Display(Name = "Sub Image I")]
        public string SubImage { get; set; }
        [Display(Name = "Sub Image II")]
        public string SubImageII { get; set; }
        [Display(Name = "Sub Image III")]
        public string SubImageIII { get; set; }

        public string Category { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}