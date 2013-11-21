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
    }
}