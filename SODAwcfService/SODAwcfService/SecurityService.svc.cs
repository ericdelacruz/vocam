using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebMatrix.WebData;
using System.Web.Security;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SecurityService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SecurityService.svc or SecurityService.svc.cs at the Solution Explorer and start debugging.
    public class SecurityService : ISecurityService
    {
       

        public bool InitDB()
        {

            WebSecurity.InitializeDatabaseConnection("UserDB", "UserProfile", "UserId", "Email", true);
            return WebSecurity.Initialized;

            
        }


        public bool Login(string Username, string Password, bool remember)
        {
            return WebSecurity.Login(Username, Password, remember);
        }

        public void LogOff()
        {
            WebSecurity.Logout();
            
        }

        public bool Register(string UserName, string Password)
        {
            
            try
            {
                WebSecurity.CreateUserAndAccount(UserName, Password);
                 
               
            }
            catch(MemberAccessException e)
            {

                throw new FaultException(e.Message);
            }
            catch(Exception ex)
            {
                throw (ex);
            }
            return WebSecurity.Login(UserName, Password);
        }

        public bool Register(Models.Account account)
        {
            throw new NotImplementedException();
        }

        
    }
}
