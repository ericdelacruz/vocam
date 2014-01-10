using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SODAwcfService
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
            AccountService accountService = new AccountService();
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
            return result;
        }



        private bool consumeLicense(long userid, int maxLicenses)
        {

            using (PortalDataSetTableAdapters.LicenseConsumptionTableAdapter adapter = new PortalDataSetTableAdapters.LicenseConsumptionTableAdapter())
            {
                var LCrecord = adapter.GetData().Where(lc => lc.UserId == userid).Count() > 0 ? adapter.GetData().Where(lc => lc.UserId == userid).First() :
                                insertLCRecord(userid, adapter);

                if (LCrecord.Consumed < maxLicenses)
                {
                    LCrecord.Consumed++;
                    adapter.Update(LCrecord);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private PortalDataSet.LicenseConsumptionRow insertLCRecord(long userid, PortalDataSetTableAdapters.LicenseConsumptionTableAdapter adapter)
        {
            adapter.Insert(userid, 0, DateTime.Now);
            return adapter.GetData().Where(lc => lc.UserId == userid).First();
        }


        private int getMaxActiveLicenses(long id)
        {
            using (PortalDataSetTableAdapters.CustomerTableAdapter customerAdapter = new PortalDataSetTableAdapters.CustomerTableAdapter())
            {
                //var customer = customerAdapter.GetData().Where(c => c.DateSubscriptionEnd > DateTime.Now);
                var customer = customerAdapter.GetData().Where(c => c.DateSubscriptionEnd > DateTime.Now && c.UserId == id);
                return customer.Count() > 0 ? customer.Sum(cust => cust.Licencses) : 0;
            }
        }
    }
}
