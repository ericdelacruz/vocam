using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SODAwcfService.Models
{
    public class PayPalTrans
    {
        public long Id { get; set; }

        public string ECTransID { get; set; }

        public string RPPProfile { get; set; }

        public bool Active { get; set; }

        public DateTime dateLog { get; set; }

        public long SalesCodeID { get; set; }

        public decimal TotalAmt { get; set; }

        public int Qty { get; set; }
    }
}
