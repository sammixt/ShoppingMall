using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnionMall.LIB
{
    public class SendEmail
    {
        public void SendEmailWithoutAttachment(string sender, string subject, string body, string signature, List<string> toRecipientEmailCollection, List<string> ccRecipientEmailCollection = null, List<string> bccRecipientEmailCollection = null)
        {
            EmailServiceReference.Service1Client client = new EmailServiceReference.Service1Client();
            EmailServiceReference.EmailAlertMsgDetails mail = new EmailServiceReference.EmailAlertMsgDetails();

            mail.DisplayName = sender;
            mail.ToRecipientEmailCollection = toRecipientEmailCollection.ToArray();
            if (ccRecipientEmailCollection != null)
                mail.CcRecipientEmailCollection = ccRecipientEmailCollection.ToArray();
            if (bccRecipientEmailCollection != null)
                mail.BccRecipientEmailCollection = bccRecipientEmailCollection.ToArray();
            mail.Subject = subject;
            mail.Msg = body;
            mail.Signature = signature;

            try
            {
                client.SendAlert(mail);
            }
            catch (Exception ex)
            {
                ErrorLogs.log("Error sending mail without attachment: " + ex);
            }
        }

        public void SendEmailWithAttachment(string sender, string subject, string body, string signature, List<string> toRecipientEmailCollection, List<string> ccRecipientEmailCollection, List<string> bccRecipientEmailCollection, string attachmentUrl, string attachementName)
        {
            try
            {

                EmailServiceReference.Service1Client client = new EmailServiceReference.Service1Client();
                EmailServiceReference.EmailAlertMsgDetails mail = new EmailServiceReference.EmailAlertMsgDetails(); ;

                mail.DisplayName = sender;
                mail.ToRecipientEmailCollection = toRecipientEmailCollection.ToArray();
                if (ccRecipientEmailCollection != null)
                    mail.CcRecipientEmailCollection = ccRecipientEmailCollection.ToArray();
                if (bccRecipientEmailCollection != null)
                    mail.BccRecipientEmailCollection = bccRecipientEmailCollection.ToArray();
                mail.Subject = subject;
                mail.Msg = body;
                mail.Signature = signature;

                List<EmailServiceReference.MyAttachment> attachments = new List<EmailServiceReference.MyAttachment>();

                EmailServiceReference.MyAttachment attach = new EmailServiceReference.MyAttachment();
                attach.MimeType = attachementName;
                attach.FileContent = System.IO.File.ReadAllBytes(attachmentUrl);

                attachments.Add(attach);

                mail.Attachments = attachments.ToArray();

                client.SendAlert(mail);
            }
            catch (Exception ex)
            {
                ErrorLogs.log("Error sending mail with attachment: " + ex);
            }
        }
    }
}