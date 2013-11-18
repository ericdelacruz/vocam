using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.Controllers
{
    public static class AccountHelper
    {
        public static bool isActive(string UserName, AccountServiceRef.AccountServiceClient client)
        {
            return client.getAccount(UserName).First().Status == 1;
        }
        
    }
}