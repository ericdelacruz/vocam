using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.Controllers
{
    public static class AuditLoggingHelper
    {
        public static void LogCreateAction(long? UserId,object new_obj,SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (!UserId.HasValue)
                return;
            try
            {
                var properties = new_obj.GetType().GetProperties();
                var values = properties.Select(p=> new_obj.GetType().GetProperty(p.Name).GetValue(new_obj).ToString());
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    UserId = UserId.Value,
                    Action = "add",
                    DateLog = DateTime.Now,
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    New_values = string.Join(";", values.ToArray())
                });
            }
            catch
            {

            }
        }

        public static void LogUpdateAction(long? UserId,object old_obj, object new_obj,SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client )
        {
            if (!UserId.HasValue)
                return;
            try
            {
                var properties = new_obj.GetType().GetProperties();
                var old_values = properties.Select(p => old_obj.GetType().GetProperty(p.Name).GetValue(old_obj).ToString());
                var new_values = properties.Select(p => new_obj.GetType().GetProperty(p.Name).GetValue(new_obj).ToString());
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    UserId = UserId.Value,
                    Action = "update",
                    DateLog = DateTime.Now,
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    New_values = string.Join(";", new_values.ToArray())
                });
            }
            catch
            {
                    
            }
        }
        public static void LogDeleteAction(long? UserId,object old_obj,SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client )
        {
            if (!UserId.HasValue)
                return;
            try
            {
                var properties = old_obj.GetType().GetProperties();
                var values = properties.Select(p => old_obj.GetType().GetProperty(p.Name).GetValue(old_obj).ToString());
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    UserId = UserId.Value,
                    Action = "delete",
                    DateLog = DateTime.Now,
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    Old_values = string.Join(";", values.ToArray())
                });
            }
            catch
            {

            }
        }
        public static void LogAuthenticationAction(long? UserId, bool islogIn, SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (!UserId.HasValue)
                return;
            client.addToLogsTable(new PortalServiceReference.LogModel()
            {
                UserId = UserId.Value,
                Action = islogIn?"LogIn":"LogOut",
                DateLog = DateTime.Now,
                
            });
        }
    }
}