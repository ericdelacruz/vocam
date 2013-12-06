using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAMvcApplication.Models
{
    public class drmModel
    {
        public bool Authorized { get; set; }

        public int AccessExpirationDays { get; set; }
    }
}