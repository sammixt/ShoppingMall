using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnionMall.ViewModels
{
    public class AuditLogViewModel
    {
    }

    public class LogEventViewModel
    {
        public int LogId { get; set; }
        public string Event { get; set; }
        public string OrderID { get; set; }
    }

    public class LoginLogViewModel
    {
        public string LoginId { get; set; }

        [Display(Name = "Staff ID")]
        public string StaffId { get; set; }

        [Display(Name = "Login date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? LoginDate { get; set; }

        [Display(Name = "Login time")]
        public string LoginTime { get; set; }

        [Display(Name = "Logout date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? LoginOutDate { get; set; }

        [Display(Name = "Logout time")]
        public string LoginOutTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SearchStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SearchEndDate { get; set; }
    }
}