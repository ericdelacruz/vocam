using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "soda" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select soda.svc or soda.svc.cs at the Solution Explorer and start debugging.
    public class soda : Isoda
    {
        
        
        public string ProductConsumeRegister(string username, string productType)
        {
            string result = "failed";

            //reuse account service
            AccountService accountService = new AccountService();
            var user = accountService.getAccount(username);

            if(user.Count() >0)
            {
                int maxLicenses= getMaxActiveLicenses(user.First().Id);
                if(maxLicenses > 0)
                {
                    switch(productType)
                    {
                        case "pclaptop": 
                               if(consumeLicense(user.First().Id,maxLicenses))
                                        result = "ok";
                            break;

                    }
                }
            }
            return result;
        }

        

        private bool consumeLicense(long userid, int maxLicenses)
        {
            
            using(PortalDataSetTableAdapters.LicenseConsumptionTableAdapter adapter = new PortalDataSetTableAdapters.LicenseConsumptionTableAdapter())
            {
                var LCrecord = adapter.GetData().Where(lc => lc.UserId == userid).Count() > 0 ? adapter.GetData().Where(lc => lc.UserId == userid).First() :
                                insertLCRecord(userid,adapter);

                if(LCrecord.Consumed < maxLicenses)
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
            adapter.Insert(userid, 0,DateTime.Now);
            return adapter.GetData().Where(lc => lc.UserId == userid).First();
        }


        private int getMaxActiveLicenses(long id)
        {
            using(PortalDataSetTableAdapters.CustomerTableAdapter customerAdapter = new PortalDataSetTableAdapters.CustomerTableAdapter())
            {
                //var customer = customerAdapter.GetData().Where(c => c.DateSubscriptionEnd > DateTime.Now);
                var customer = customerAdapter.GetData().Where(c => c.DateSubscriptionEnd > DateTime.Now && c.UserId == id);
                return customer.Count() >0? customer.Sum(cust => cust.Licencses):0;
            }
        }
    }
}
