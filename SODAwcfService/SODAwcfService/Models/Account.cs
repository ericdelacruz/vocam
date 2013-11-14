using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
namespace SODAwcfService.Models
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string USERNAME { get; set; }
        [DataMember]
        public string PASSWORD { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public int Role { get; set; }
        [DataMember]
        public short Status { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public char Gender { get; set; }
        [DataMember]
        public string ContactNo { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DateLogin { get; set; }

        [DataMember]
        public Nullable<System.DateTime> Birthdate { get; set; }
    }
}