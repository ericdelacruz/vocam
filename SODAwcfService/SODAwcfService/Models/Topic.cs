using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class Topic
    {
        public long Id { get; set; }
        public Nullable<long> SpecId { get; set; }
        public string Name { get; set; }
    
    }
}