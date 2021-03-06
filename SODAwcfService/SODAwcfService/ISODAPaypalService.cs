﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PayPal.PayPalAPIInterfaceService.Model;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISODAPaypalService" in both code and config file together.
    [ServiceContract]
    public interface ISODAPaypalService
    {
        #region Obselete methods
        [OperationContract]
        string checkout(decimal amt, CurrencyCodeType cType, string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl);
        [OperationContract]
        string confirmation(long userid, string payorid, string token, decimal amt, decimal recur_amt, CurrencyCodeType cType, string itemName, string itemDesc, DateTime dateStart);
        #endregion

        [OperationContract]
        string checkoutModel(Models.PayPalCheckOutModel model);
       
        [OperationContract]
        string confirmationModel(Models.PayPalConfirmModel model);
        [OperationContract]
        bool cancelSubscription(string transid);

        [OperationContract]
        Models.RecuringProfileDetails getRecurProfileDetails(long userid);
        [OperationContract]
        Models.RecuringProfileDetails getRecurProfileDetailsByTransID(string TransID);
        [OperationContract]
        IEnumerable<Models.PayPalTrans> getPayPalTrans();
        [OperationContract]
        bool initPayPalAccountSettings(int regionID);
    }
}
