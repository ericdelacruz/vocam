using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SODAMvcApplication.Services
{
    /// <summary>
    /// Summary description for trainnow
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class trainnow : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
         [WebMethod]
        public string ProductConsumeRegister(string username, string productType)
        {
            string result = "failed";

            //reuse account service
            //AccountService accountService = new AccountService();
            AccountServiceReference.AccountServiceClient accountService = new AccountServiceReference.AccountServiceClient();

            var user = accountService.getAccount(username);

            if (user.Count() > 0)
            {
                int maxLicenses = getMaxActiveLicenses(user.First().Id);
                if (maxLicenses > 0)
                {
                    switch (productType)
                    {
                        case "pclaptop":
                            if (consumeLicense(user.First().Id, maxLicenses))
                                result = "ok";
                            break;

                    }
                }
            }
            accountService.Close();
            return result;
        }



        private bool consumeLicense(long userid, int maxLicenses)
        {


            PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
            var LCrecord = portalClient.getLicenseConsumption().Where(lc => lc.UserId == userid).Count() > 0 ? portalClient.getLicenseConsumption().Where(lc => lc.UserId == userid).First() :
                            insertLCRecord(userid, portalClient);

            if (LCrecord.Consumed < maxLicenses)
            {
                LCrecord.Consumed++;
                portalClient.updateLicenseConsumption(LCrecord);
                portalClient.Close();
                return true;
            }
            else
            {
                portalClient.Close();
                return false;
            }
            

        }

        private PortalServiceReference.LicenseConsumption insertLCRecord(long userid, PortalServiceReference.PortalServiceClient portalClient)
        {
            portalClient.addLicenseConsumption(
                new PortalServiceReference.LicenseConsumption()
                {
                    UserId = userid,
                    Consumed = 0,
                    DateUpdated = DateTime.Now
                }
                );
            return portalClient.getLicenseConsumption().Where(lc => lc.UserId == userid).First();
        }


        private int getMaxActiveLicenses(long id)
        {

            PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
            //var customer = customerAdapter.GetData().Where(c => c.DateSubscriptionEnd > DateTime.Now);
            var customer = portalClient.getCustomer().Where(c => c.DateSubscriptionEnd > DateTime.Now && c.UserId == id);
            portalClient.Close();
            return customer.Count() > 0 ? customer.Sum(cust => cust.Licenses) : 0;

        }
    }
}
