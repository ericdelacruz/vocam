//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SODAwcfService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Category
    {
        public Category()
        {
            this.Specifics = new HashSet<Specific>();
        }
    
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string IMG_URL { get; set; }
        public string Metatags { get; set; }
        public string bg_img { get; set; }
        public string banner_img { get; set; }
        public string Overview { get; set; }
        public string Meta_Desc { get; set; }
        public string PageTitle { get; set; }
    
        public virtual ICollection<Specific> Specifics { get; set; }
    }
}
