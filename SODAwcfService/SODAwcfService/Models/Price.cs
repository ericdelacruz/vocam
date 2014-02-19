using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class Price
    {
        public int Id { get; set; }
        public decimal PriceAmt { get; set; }
        public bool FirstMonthFree { get; set; }
        public bool Active { get; set; }
        public int RegionId { get; set; }

        public decimal PriceAmt_B { get; set; }

        public decimal priceAmt_C { get; set; }

        public bool Active_b { get; set; }

        public bool Active_c { get; set; }

        public string Description { get; set; }
    }
}