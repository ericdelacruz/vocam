using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
namespace SODAwcfService.Models
{
    [DataContract]
    public class ContentDef
    {
        [DataMember]
        public string PageCode { get; set; }
        [DataMember]
        public string SectionName { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public int RegionId { get; set; }
    }
}