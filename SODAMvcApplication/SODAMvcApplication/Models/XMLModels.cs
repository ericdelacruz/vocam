using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAMvcApplication.Models.XML
{
    public class Users
    {
        public bool authorized { get; set; }
        public int daysleft { get; set; }
        public bool shownews { get; set; }

        public int PCLicenses { get; set; }

       

        public int PCLicenseConsumed { get; set; }
    }
   
}