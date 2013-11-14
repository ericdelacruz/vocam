using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class SalesPerson
    {
        public int Id { get; set; }
        public string Sales_Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long UserId { get; set; }
        public int RegionId { get; set; }
       
    }
}