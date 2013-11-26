using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Moolah;
using Moolah.PayPal;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISODAPaypalService" in both code and config file together.
    [ServiceContract]
    public interface ISODAPaypalService
    {
        [OperationContract]
        string checkout(decimal amt, CurrencyCodeType cType, string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl);

        [OperationContract]
         string confirmation(long userid,string payorid, string token,decimal amt,decimal recur_amt, CurrencyCodeType cType,string itemName, string itemDesc,DateTime dateStart);

        [OperationContract]
        bool cancelSubscription(long userid);

        [OperationContract]
        PayPal.PayPalAPIInterfaceService.Model.GetRecurringPaymentsProfileDetailsResponseType getRecurProfileDetails(long userid);
    }
}
