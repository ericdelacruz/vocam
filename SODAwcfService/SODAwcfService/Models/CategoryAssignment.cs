using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SODAwcfService.Models
{
     [DataContract]
    public class CategoryAssignment
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public long SpecID { get; set; }
        [DataMember]
        public long CategoryId { get; set; }
    }
}