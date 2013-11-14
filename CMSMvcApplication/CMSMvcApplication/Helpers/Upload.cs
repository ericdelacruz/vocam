using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
namespace CMSMvcApplication.Helpers
{
    public static class Upload
    {
         public static string UploadImage(string src)
        {
            WebImage image = WebImage.GetImageFromRequest(src);
            string path = "";
            if(image != null)
            {
                var filename = System.IO.Path.GetFileName(image.FileName);
                path = System.IO.Path.Combine("~/Content/", filename);
                image.Save(path);
            }
            return path;
        }
    }
}