using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace SODAPortalMvcApplication.Controllers
{
    public static  class EmailHelper
    {
        public static void SendEmail(string from, string to, string subject, string body)
        {
            string host = "smtpout.asia.secureserver.net";
            int port = 80;

            MailMessage msg = new MailMessage(from, to, subject, body);
            SmtpClient smtp = new SmtpClient(host, port);
            string username = "test@sac-iis.com";
            string password = "P@ssw0rd12345";

            smtp.Credentials = new NetworkCredential(username, password);
            //smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;

            try
            {
                smtp.Send(msg);
            }
            catch(Exception exp)
            {
                throw (exp);
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }
           

        }
    }
}