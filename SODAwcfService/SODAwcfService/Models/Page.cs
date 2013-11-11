using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
namespace SODAwcfService.Models
{
    [DataContract]
    public class Page
    {
        [DataMember]
        public string PageCode { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}