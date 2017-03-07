using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnionMall.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Display(Name = "Employee #")]
        public string EmployeeNumber { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        [Display(Name = "Job Tiltle")]
        public string Title { get; set; }
        
        public string BranchCode { get; set; }

        [Display(Name = "Branch Name")]
        public string Branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Active")]
        public string IsActive { get; set; }

        [Display(Name = "Application Roles")]
        public ICollection<string> Roles { get; set; }
        //for retrieving users account information
        public string CustId { get; set; }
    }
}

                    