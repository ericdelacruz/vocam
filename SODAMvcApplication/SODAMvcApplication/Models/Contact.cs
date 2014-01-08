using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace SODAMvcApplication.CMSServiceReference
{
    [MetadataType(typeof(ContactMD))]
    public partial class Contact
    {

    }
    public class ContactMD
    {
        
        //public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        //public string Company { get; set; }
        
        //public string Phone { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email;
        
        public string Postcode { get; set; }
        
        //public string Message { get; set; }
        
        //public bool isFreePPT { get; set; }
        
        //public string key { get; set; }
        
        //public DateTime? DateLinkEx { get; set; }
        
        //public bool? isVerified { get; set; }
    }
}