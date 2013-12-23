using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class FreePPT
    {
        public string FileName { get; set; }

        public string PPTType { get; set; }

        public int RegionId { get; set; }

        public int Id { get; set; }

        public string DisplayText { get; set; }
    }
}