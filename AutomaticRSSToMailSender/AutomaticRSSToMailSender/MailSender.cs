using System;
using System.Net.Mail;
using System.ServiceModel.Syndication;

namespace AutomaticRSSToMailSender
{
    public class MailSender
    {
        public static void SendMail(SyndicationItem item, string toAddress)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(AppSettings.GetStringValue("SmtpServer"));

                mail.From = new MailAddress(AppSettings.GetStringValue("From"));
                mail.To.Add(toAddress);
                mail.Subject = "Info från Montessori: " + item.Title.Text;

                mail.IsBodyHtml = true;
                string htmlBody;

                htmlBody = item.Summary.Text + " <br> <br>" + item.Content;

                mail.Body = htmlBody;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(
                    AppSettings.GetStringValue("Username"), 
                    AppSettings.GetStringValue("Password"));
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
