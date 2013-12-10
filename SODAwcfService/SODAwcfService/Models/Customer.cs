using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class Customer
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> DatePurchase { get; set; }
        public Nullable<System.DateTime> DateSubscriptionEnd { get; set; }
        public long UserId { get; set; }
        public long SalesCodeId { get; set; }

        public short RecurringType { get; set; }

        public int Licenses { get; set; }

        public DateTime? DateUpdated { get; set; }

        public long? PPId { get; set; }
    }
}