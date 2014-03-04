using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SODAMvcApplication.Controllers
{
    public static class EmailHelper
    {
        
        public static void SendEmail(string from, string to, string subject, string body, string ReplyTo)
        {
            

            MailMessage msg = new MailMessage(from, to, subject, body);

            if (!string.IsNullOrEmpty(ReplyTo))
                msg.ReplyToList.Add(ReplyTo);
            
            SmtpClient smtp = new SmtpClient();
            
           
            try
            {
                smtp.Send(msg);
            }
            catch (Exception exp)
            {
                //SmtpClient smtp_alter = new SmtpClient();

                //throw (exp);
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }


        }
        public static void SendEmail(MailAddress from, MailAddress to, string subject, string body,bool isHTML, string ReplyTo,string password)
        {
            
            MailMessage msg = new MailMessage(from, to);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isHTML;

            /*CC for testing*/
            //msg.CC.Add("timt@vocam.com");
            //msg.CC.Add("ashley.lubguban@gmail.com");
            //msg.Bcc.Add("jon@straightarrow.com.ph");
            if (!string.IsNullOrEmpty(ReplyTo))
                msg.ReplyToList.Add(ReplyTo);
          
            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(from.Address, password);

            try
            {
                smtp.Send(msg);
            }
            catch (Exception exp)
            {
               
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }
        }
        public static void SendEmail(MailAddress from, MailAddress to, string subject, string body, bool isHTML, string ReplyTo)
        {

            MailMessage msg = new MailMessage(from, to);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isHTML;
            //CC for testing
            //msg.CC.Add("timt@vocam.com");
            //msg.CC.Add("ashley.lubguban@gmail.com");
            //msg.Bcc.Add("jon@straightarrow.com.ph");
            if (!string.IsNullOrEmpty(ReplyTo))
                msg.ReplyToList.Add(ReplyTo);

            SmtpClient smtp = new SmtpClient();
            

            try
            {
                smtp.Send(msg);
            }
            catch (Exception exp)
            {
                
                logEx(exp);//log exception if error
               
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }
        }
        
        private static void logEx(Exception exp)
        {
            PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
            portalClient.addToLogsTable(new PortalServiceReference.LogModel()
            {
                Username = "sys",
                Action = "Email",
                DateLog = DateTime.Now,
                New_values = exp.Message
            });
        }
        public static void SendEmail(string to, string subject, string body, string ReplyTo)
        {
            


            MailMessage msg = new MailMessage();
            msg.To.Add(to);
            msg.Body = body;
            msg.Subject = subject;
            if (!string.IsNullOrEmpty(ReplyTo))
                msg.ReplyToList.Add(ReplyTo);
            SmtpClient smtp = new SmtpClient();
           

            try
            {
                smtp.Send(msg);
            }
            catch (Exception exp)
            {
                //SmtpClient smtp_alter = new SmtpClient();

                //throw (exp);
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }


        }
        //Converts cshtml to string variable
        public static string ToHtml(string viewToRender, ViewDataDictionary viewData, ControllerContext controllerContext)
        {
            var result = ViewEngines.Engines.FindView(controllerContext, viewToRender, null);

            StringWriter output;
            using (output = new StringWriter())
            {
                var viewContext = new ViewContext(controllerContext, result.View, viewData, controllerContext.Controller.TempData, output);
                result.View.Render(viewContext, output);
                result.ViewEngine.ReleaseView(controllerContext, result.View);
            }

            return output.ToString();
        }
        
        public static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash. 
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes 
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data  
                // and format each one as a hexadecimal string. 
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string. 
                return sBuilder.ToString();
            }
        }


    }
}