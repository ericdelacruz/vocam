using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace SODAwcfService.Models
{
    [DataContract]
    public class Contact
    {
        
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Postcode { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool isFreePPT { get; set; }
        [DataMember]
        public string key { get; set; }
        [DataMember]
        public DateTime? DateLinkEx { get; set; }
        [DataMember]
        public bool? isVerified { get; set; }
    }
}