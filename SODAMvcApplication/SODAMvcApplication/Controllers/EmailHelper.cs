using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SODAMvcApplication.Controllers
{
    public static class EmailHelper
    {
        public static void SendEmail(string from, string to, string subject, string body)
        {
            //string host = ConfigurationManager.AppSettings["EmailHost"] != "" ? ConfigurationManager.AppSettings["EmailHost"] : "smtpout.asia.secureserver.net";
            //int port = ConfigurationManager.AppSettings["Emailport"] != "" ? int.Parse(ConfigurationManager.AppSettings["Emailport"]) : 80;

            MailMessage msg = new MailMessage(from, to, subject, body);
           
            //SmtpClient smtp = port == 0 ? new SmtpClient(host) : new SmtpClient(host, port);
            SmtpClient smtp = new SmtpClient();
            //string username = "test@sac-iis.com";
            //string password = "P@ssw0rd12345";

            //smtp.Credentials = new NetworkCredential(username, password);
            smtp.Credentials = new NetworkCredential();
            //smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;
           
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