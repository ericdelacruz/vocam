using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.Controllers
{
    public static class FileTransferHelper
    {
        private static string UploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        internal static void UploadFile(HttpPostedFileBase httpPostedFileBase, HttpServerUtilityBase Server)
        {
            string dest = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(httpPostedFileBase.FileName));
      
            try
            {
                // httpPostedFileBase.SaveAs(Server.MapPath(dest).Replace(CMSPageDir, webPagedir));
                if (System.IO.File.Exists(Server.MapPath(dest)))
                    System.IO.File.Delete(Server.MapPath(dest));

                httpPostedFileBase.SaveAs(Server.MapPath(dest));
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }
           
        }
        internal static void UploadFile(HttpPostedFileBase httpPostedFileBase, HttpServerUtilityBase Server,string FiletobeReplaced)
        {
            string dest = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(httpPostedFileBase.FileName));
            string fileForDel = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(FiletobeReplaced));
            try
            {
                // httpPostedFileBase.SaveAs(Server.MapPath(dest).Replace(CMSPageDir, webPagedir));
                if (System.IO.File.Exists(Server.MapPath(dest)))
                    System.IO.File.Delete(Server.MapPath(dest));

                if (System.IO.File.Exists(Server.MapPath(fileForDel)))
                    System.IO.File.Delete(Server.MapPath(fileForDel));

                httpPostedFileBase.SaveAs(Server.MapPath(dest));

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}