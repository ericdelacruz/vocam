using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class ResetPassword
    {
        public int Id { get; set; }

        public string key { get; set; }

        public DateTime dateSent { get; set; }

        public DateTime dateEx { get; set; }

        public bool isVerified { get; set; }

        public long UserId { get; set; }
    }
}