using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnionMall.ViewModels
{
    public class CartViewModel
    {
        public string OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? ItemTotal { get; set; }
        public string MainImage { get; set; }
    }

    public class BranchViewModel
    {
        public string BranchId { get; set; }
        public string BranchLocationName { get; set; }
    }

    public class PendingViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Branch { get; set; }

        public string PickUpBranch { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }
        public string Time { get; set; }
        public string Amount { get; set; }
        public string OrderId { get; set; }
        public string Comment { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SearchStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SearchEndDate { get; set; }
    }

    public class PaymentViewModel
    {
        public string CustomerAcctNum { get; set; }
        public string ShopAcctNum { get; set; }
        public string AmountPaid { get; set; }
        public string Token { get; set; }
        public string paymentReference { get; set; }
    }
}