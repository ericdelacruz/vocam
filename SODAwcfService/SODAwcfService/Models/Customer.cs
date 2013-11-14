using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class Customer
    {
        public long Id { get; set; }
        
        public string Sales_Code { get; set; }
        public System.DateTime DatePurchase { get; set; }
        public System.DateTime DateSubscriptionEnd { get; set; }
        public long UserId { get; set; }
    }
}