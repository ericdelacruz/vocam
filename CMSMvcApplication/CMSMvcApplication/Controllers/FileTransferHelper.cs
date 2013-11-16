using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Net;
namespace CMSMvcApplication.Controllers
{
    public static class FileTransferHelper
    {
        //private static string UploadPath = "~/Content/Uploaded";
        private static string UploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        //private static string root = @"C:/Users/jcua/Documents/vocam.git/SODAMvcApplication/SODAMvcApplication";
        //private static string root = "http://localhost:51219/";
        //private static string webPagedir = ConfigurationManager.AppSettings["webpagedir"].ToString();
        //private static string CMSPageDir = ConfigurationManager.AppSettings["cmsdir"].ToString();

        internal static string UploadImage(HttpPostedFileBase httpPostedFileBase)
        {
            string dest = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(httpPostedFileBase.FileName));
            int indexofDot = dest.IndexOf('.');
            dest = dest.Insert(indexofDot, string.Format("{0: MMddyy}", DateTime.Now));
            httpPostedFileBase.SaveAs(dest);
             
            return dest;
        }
        internal static string UploadImage(HttpPostedFileBase httpPostedFileBase, HttpServerUtilityBase Server)
        {
            string dest = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(httpPostedFileBase.FileName));
            int indexofDot = dest.IndexOf(".");
            dest = dest.Insert(indexofDot, string.Format("{0:MMddyy}", DateTime.Now));
            try
            {
               // httpPostedFileBase.SaveAs(Server.MapPath(dest).Replace(CMSPageDir, webPagedir));
                httpPostedFileBase.SaveAs(Server.MapPath(dest));
            }
            catch(Exception ex)
            {
                return Server.MapPath(dest) + " " + ex.Message;
            }
            return dest.Replace("~","");
        }
        //internal static string UploadImageFTP(HttpPostedFileBase httpPostedFileBase, HttpServerUtilityBase Server)
        //{
        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://sac.iis/Content/images/");
        //    request.Method = WebRequestMethods.Ftp.UploadFile;

        //    request.Credentials = new NetworkCredential("jonct2", "P@ssw0rd");

        //    Stream requestStream = request.GetRequestStream();
            
        //}
        
    }
}