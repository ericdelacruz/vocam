using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class LicenseConsumption
    {
        public int Id { get; set; }

        public int Consumed { get; set; }

        public DateTime DateUpdated { get; set; }

        public long UserId { get; set; }
    }
}