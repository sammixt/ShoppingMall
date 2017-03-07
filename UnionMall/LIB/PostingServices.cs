using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using UnionMall.Models;
using UnionMall.PostingService;
using UnionMall.ViewModels;

namespace UnionMall.LIB
{
    public class PostingServices
    {
        private static UBNMiddleWareWebServiceClient getPostingService()
        {
            PostingService.UBNMiddleWareWebServiceClient postingClient = new PostingService.UBNMiddleWareWebServiceClient();
            postingClient.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SrvPostUsername"].ToString();
            postingClient.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SrvPostPassword"].ToString();
            return postingClient;
        }

        private gefuTransactionsRequestData addRequestData()
        {
            PostingService.gefuTransactionsRequestData requestData = new PostingService.gefuTransactionsRequestData();
            requestData.requestModule = ConfigurationManager.AppSettings["ServiceModule"].ToString();
            requestData.moduleCredentials = ConfigurationManager.AppSettings["ServiceModuleCred"].ToString();
            requestData.transactionCurrency = "NGN";
            return requestData;
        }

        private UBNHeaderType head()
        {
            PostingService.UBNHeaderType header = new PostingService.UBNHeaderType();
            return header;
        }

        private gefuBatchItemsData[] addRowData()
        {
            PostingService.gefuBatchItemsData[] rowData = new PostingService.gefuBatchItemsData[4];
            return rowData;
        }

        private gefuBatchItemsData addItemDr()
        {
            PostingService.gefuBatchItemsData item_dr = new PostingService.gefuBatchItemsData();
            return item_dr;
        }

        private gefuBatchItemsData addItemCr()
        {
            PostingService.gefuBatchItemsData item_cr = new PostingService.gefuBatchItemsData();
            return item_cr;
        }

        public string postingTransaction(PostingViewModel credit, PostingViewModel debit)
        {
            var postingClient = getPostingService();
            var requestData = addRequestData();
            var header = head();
            var rowData = addRowData();
            var item_dr = addItemDr();
            string responseText = "";
            string response_code = "";
            string response_msg = "";
            requestData.batchId = debit.batchId;
            requestData.initiatingBranch = debit.branchCode;// "682";
            item_dr.acccountNumber = debit.accountNumber; // "0000767641";
            item_dr.amount = debit.amount;
            item_dr.amountSpecified = true;
            item_dr.branchCode = "";
            item_dr.debitCreditIndicator = debit.debitCreditIndicator;
            item_dr.glCasaIndicator = debit.glCasaIndicator ;
            item_dr.narration = debit.narration;
            item_dr.paymentReference = debit.paymentReference;
            item_dr.transactionId = debit.transactionId;
            //Insert Transaction into database
            DateTime start_date = DateTime.Now;
            PostingModel.AddLog(Convert.ToInt32(debit.transactionId), debit.paymentReference, debit.batchId, debit.branchCode,
           debit.accountNumber, debit.amount, debit.narration, start_date, debit.glCasaIndicator,
           "DEBITS", ".umall_insert_dbt_log");
            rowData[0] = item_dr;
            
            var item_cr = addItemCr();
            item_cr.acccountNumber = credit.accountNumber; //"0040059353";
            item_cr.amount = credit.amount; //10;
            item_cr.amountSpecified = true;
            item_cr.branchCode = "";
            item_cr.debitCreditIndicator = credit.debitCreditIndicator;
            item_cr.glCasaIndicator = credit.glCasaIndicator;//"CASA";
            item_cr.narration = credit.narration;
            item_cr.paymentReference = credit.paymentReference; //"t343435";
            item_cr.transactionId = credit.transactionId;
            PostingModel.AddLog(Convert.ToInt32(credit.transactionId), credit.paymentReference, credit.batchId, credit.branchCode,
                credit.accountNumber, credit.amount, credit.narration, DateTime.Now,
                credit.glCasaIndicator,
                "CREDITS", ".umall_insert_crdt_log");
            rowData[1] = item_cr;
            

            requestData.gefuBatchItemsDataList = rowData;
            gefuTransactionsResponseData responseData = postingClient.onlineGefuCbaPosting(ref header, requestData, "");
            if (responseData != null && (responseData.responseCode != null))
            {
                if (responseData.responseCode == "00")
                {
                    responseText = "SUCCESS";
                }
                else
                {
                    responseText = "FAILED";
                }
                response_code = responseData.responseCode;
                response_msg = responseData.responseMessage;
            }
            else
            {
                responseText = "FAILED";
            }
            PostingModel.UpdateLog(Convert.ToInt32(credit.transactionId), credit.paymentReference, DateTime.Now, responseText, response_code, response_msg,
            ".umall_update_crdt_log");
            PostingModel.UpdateLog(Convert.ToInt32(debit.transactionId), debit.paymentReference, DateTime.Now, responseText, response_code, response_msg,
            ".umall_update_dbt_log");
            return responseText;
        }   
    }
}