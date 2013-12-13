using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAMvcApplication.Models
{
    public class SiteMapModel
    {
        public string CatName { get; set; }
        public List<string> Titles { get; set; }

        public long CatId { get; set; }
    }
}