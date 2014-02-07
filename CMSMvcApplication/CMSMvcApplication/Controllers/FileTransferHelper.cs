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
            //find the index of dot then insert datetime
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
         
        internal static string UploadFile(HttpPostedFileBase httpPostedFileBase, HttpServerUtilityBase Server)
        {
            string dest = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(httpPostedFileBase.FileName));
            try
            {
               
               

                httpPostedFileBase.SaveAs(Server.MapPath(dest));
            }
            catch (Exception ex)
            {
                return Server.MapPath(dest) + " " + ex.Message;
            }
            return dest.Replace("~", "");
        }

        internal static string UploadFile(HttpPostedFileBase httpPostedFileBase, HttpServerUtilityBase Server, string FiletobeReplaced)
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
                return Server.MapPath(dest) + " " + ex.Message;
            }
            return dest.Replace("~", "");
        }
        internal static void DeleteFile(string fileName, HttpServerUtilityBase Server)
        {
            string dest = System.IO.Path.Combine(UploadPath, System.IO.Path.GetFileName(fileName));
            try
            {
                if (System.IO.File.Exists(Server.MapPath(dest)))
                    System.IO.File.Delete(Server.MapPath(dest));
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}