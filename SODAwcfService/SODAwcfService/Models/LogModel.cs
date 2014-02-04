using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class LogModel
    {
        //public long UserId { get; set; }

        public string Action { get; set; }

        public string PropertyName { get; set; }

        public string Properties { get; set; }

        public string Old_values { get; set; }

        public string New_values { get; set; }

        public DateTime DateLog { get; set; }

        public string Username { get; set; }
    }
}