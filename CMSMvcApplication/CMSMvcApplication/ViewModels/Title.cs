using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSMvcApplication.ViewModels
{
    public class Title
    {
        public CatListingServiceReference.Category Category { get; set; }
        public CatListingServiceReference.Specific Specific { get; set; }

        public string regionName { get; set; }
    }
}