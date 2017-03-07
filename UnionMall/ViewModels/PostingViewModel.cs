using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnionMall.ViewModels
{
    public class PostingViewModel
    {
        public string accountNumber { get; set; }

        public decimal amount { get; set; }

        public bool amountSpecified { get; set; }

        public string branchCode { get; set; }

        public string debitCreditIndicator { get; set; }

        public string glCasaIndicator { get; set; }

        public string narration { get; set; }

        public string paymentReference { get; set; }

        public string transactionId { get; set; }

        public string  batchId { get; set; }

        //for audit log purpose
        public string currency { get; set; }
        public string status { get; set; }
        public string responseCode { get; set; }
        public string responseMsg { get; set; }
        public string transactionDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SearchStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SearchEndDate { get; set; }
    }
}