using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
namespace SODAwcfService.Models
{
    [DataContract]
    public class Category
    {
        [DataMember]
        public long CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string IMG_URL { get; set; }
        [DataMember]
        public string Metatags { get; set; }
        [DataMember]
        public string BG_IMG { get; set; }

        [DataMember]
        public string Banner_IMG { get; set; }
        [DataMember]
        public string Overview { get; set; }
        [DataMember]
        public string PageTitile { get; set; }
        [DataMember]
        public string MetaDesc { get; set; }
    }
}