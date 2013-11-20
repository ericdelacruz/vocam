using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.ViewModel
{
    public class CustomerModel
    {
        public PortalServiceReference.Customer customer { get; set; }
        public AccountServiceRef.Account account { get; set; }
        public PortalServiceReference.SalesCode salesCode { get; set; }
    }

    public class VerifyModel
    {
        public PortalServiceReference.SalesPerson saleperson { get; set; }
        public PortalServiceReference.SalesCode salescode { get; set; }
        public PortalServiceReference.Price price { get; set; }
    }
}