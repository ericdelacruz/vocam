using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string RegionName { get; set; }

        public string Currency { get; set; }

        public string WebsiteUrl { get; set; }

        public long DefaultSalesCodeId { get; set; }

        public string AirPlayerFileName { get; set; }

        public string PayPalUserName { get; set; }

        public string PayPalPassword { get; set; }

        public string PayPalSignature { get; set; }
    }
}