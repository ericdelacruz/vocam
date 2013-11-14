using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class SalesCode
    {
        public long Id { get; set; }
        public Nullable<int> SalesPersonID { get; set; }
        public string Sales_Code { get; set; }
        public decimal Discount { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
    }
}