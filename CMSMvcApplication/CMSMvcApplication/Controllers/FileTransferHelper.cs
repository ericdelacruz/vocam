using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSMvcApplication.Controllers
{
    public static class FileTransferHelper
    {
        //private static string UploadPath = "~/Content/Uploaded";
        private static string UploadPath = "/Content/Uploaded/";
        //private static string root = @"C:/Users/jcua/Documents/vocam.git/SODAMvcApplication/SODAMvcApplication";
        //private static string root = "http://localhost:51219/";
        private static string webPagedir = @"SODAMvcApplication\SODAMvcApplication\";
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
            httpPostedFileBase.SaveAs(Server.MapPath(dest).Replace(@"CMSMvcApplication\CMSMvcApplication\",webPagedir));
              
            return dest;
        }
    }
}