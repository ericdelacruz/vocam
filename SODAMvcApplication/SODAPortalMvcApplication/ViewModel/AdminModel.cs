using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.ViewModel
{
    public class AdminModel
    {
    }

    public class ReportViewModel
    {
       public AccountServiceRef.Account account { get; set; }
       public PortalServiceReference.Customer customer { get; set; }
       public PortalServiceReference.SalesCode salesCode { get; set; }
    }

    public class SalesViewModel
    {
        public AccountServiceRef.Account account { get; set; }
        public PortalServiceReference.SalesPerson salesPerson { get; set; }
        public PortalServiceReference.SalesCode salesCode { get; set; }
    }
    public class PriceViewModel
    {
        public PortalServiceReference.Price price { get; set; }
        public PortalServiceReference.Region region { get; set; }
    }

}