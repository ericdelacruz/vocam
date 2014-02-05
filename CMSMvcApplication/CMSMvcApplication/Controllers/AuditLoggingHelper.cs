using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSMvcApplication.Controllers
{
    public static class AuditLoggingHelper
    {
        public static void LogCreateAction(string Username, object new_obj, CMSMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;

            try
            {
                var properties = new_obj.GetType().GetProperties();
                var values = properties.Select(p => getVal(new_obj, p));
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    Username = Username,
                    Action = "add",
                    DateLog = DateTime.Now,
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    New_values = string.Join(";", values.ToArray()),
                    PropertyName = new_obj.GetType().Name
                });
            }
            catch
            {

            }
        }

        public static void LogUpdateAction(string Username, object old_obj, object new_obj, CMSMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;
            try
            {
                var properties = new_obj.GetType().GetProperties();
                var old_values = properties.Select(p => getVal(old_obj, p));
                var new_values = properties.Select(p => getVal(new_obj, p));
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    Username = Username,
                    Action = "update",
                    DateLog = DateTime.Now,
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    Old_values = string.Join(";", old_values.ToArray()),
                    New_values = string.Join(";", new_values.ToArray()),
                    PropertyName = new_obj.GetType().Name
                });
            }
            catch (Exception ex)
            {

            }
        }

        private static string getVal(object obj, System.Reflection.PropertyInfo p)
        {
            return obj.GetType().GetProperty(p.Name).GetValue(obj,null) != null ? obj.GetType().GetProperty(p.Name).GetValue(obj,null).ToString() : "null";
        }

        public static void LogDeleteAction(string Username, object old_obj, CMSMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;
            try
            {
                var properties = old_obj.GetType().GetProperties();
                var values = properties.Select(p => getVal(old_obj, p));
                var strProperties = string.Join(";", properties.Select(p => p.Name).ToArray());
                var strValues = string.Join(";", values.ToArray());
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    Username = Username,
                    Action = "delete",
                    DateLog = DateTime.Now,
                    Properties = strProperties,
                    Old_values = strValues,
                    PropertyName = old_obj.GetType().Name
                });
            }
            catch (Exception ex)
            {

            }
        }
        public static void LogAuthenticationAction(string Username, bool islogIn, CMSMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;
            client.addToLogsTable(new PortalServiceReference.LogModel()
            {
                Username = Username,
                Action = islogIn ? "LogIn" : "LogOut",
                DateLog = DateTime.Now

            });
        }
    }
}