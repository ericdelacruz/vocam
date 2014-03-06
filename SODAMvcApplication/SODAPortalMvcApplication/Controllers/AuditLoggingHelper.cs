using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.Controllers
{
    public static class AuditLoggingHelper
    {
        public static void LogCreateAction(string Username,object new_obj,SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;
          
            try
            {
                //list all properties if the object
                var properties = new_obj.GetType().GetProperties();
                //list the string values of each property on the object to be added
                var values = properties.Select(p=> getVal(new_obj,p));
                //add to DB
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    Username = Username,
                    Action = "add",
                    DateLog = DateTime.Now,
                    //place all elements into a string in a semi colon seperator.
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    New_values = string.Join(";", values.ToArray()),
                    PropertyName = new_obj.GetType().Name
                });
            }
            catch
            {

            }
        }

        public static void LogUpdateAction(string Username, object old_obj, object new_obj, SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;
            try
            {
                var properties = new_obj.GetType().GetProperties();
                //get values of the previos state of the object
                var old_values = properties.Select(p => getVal(old_obj,p));
                //get values of the updated state of the object
                var new_values = properties.Select(p => getVal(new_obj, p));
                //add to DB
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    Username = Username,
                    Action = "update",
                    DateLog = DateTime.Now,
                    //Join into a string in a semi colon seperator.
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    Old_values = string.Join(";",old_values.ToArray()),
                    New_values = string.Join(";", new_values.ToArray()),
                     PropertyName = new_obj.GetType().Name
                });
            }
            catch(Exception ex)
            {
                  
            }
        }

        private static string getVal(object obj, System.Reflection.PropertyInfo p)
        {
            return obj.GetType().GetProperty(p.Name).GetValue(obj) !=null?obj.GetType().GetProperty(p.Name).GetValue(obj).ToString():"null";
        }

        public static void LogDeleteAction(string Username, object old_obj, SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;
            try
            {
                //get the properties of the object to be deleted
                var properties = old_obj.GetType().GetProperties();
                //get the values
                var values = properties.Select(p => getVal(old_obj,p));
                client.addToLogsTable(new PortalServiceReference.LogModel()
                {
                    Username = Username,
                    Action = "delete",
                    DateLog = DateTime.Now,
                    //Join into a string in a semi colon seperator.
                    Properties = string.Join(";", properties.Select(p => p.Name).ToArray()),
                    Old_values = string.Join(";", values.ToArray()),
                    PropertyName = old_obj.GetType().Name
                });
            }
            catch(Exception ex)
            {

            }
        }

        public static void LogAuthenticationAction(string Username, bool islogIn, SODAPortalMvcApplication.PortalServiceReference.PortalServiceClient client)
        {
            if (string.IsNullOrEmpty(Username))
                return;

            client.addToLogsTable(new PortalServiceReference.LogModel()
            {
                Username = Username,
                Action = islogIn?"LogIn":"LogOut",
                DateLog = DateTime.Now
                
            });
        }
    }
}