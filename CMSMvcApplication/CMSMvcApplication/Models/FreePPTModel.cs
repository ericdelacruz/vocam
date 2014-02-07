using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMSMvcApplication.Models
{
    //public class FreePPTModel : CMSServiceReference.FreePPT
    //{
    //    public string vFileName { get { return FileName; } set { FileName = value; } }

    //    public string vPPTType { get { return PPTType; } set { PPTType = value; } }

    //    public int vRegionId { get { return RegionId; } set { RegionId = value; } }

    //    public int vId { get { return Id; } set{Id = value} }

    //    public string vDisplayText { get {return DisplayText} set { DisplayText = value; } }
    //}
    
    [MetadataType(typeof(FreePPT))]
    public partial class FreePPT
    {

    }

    public class FreePPTMetaData
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string PPTType { get; set; }

        public int RegionId { get; set; }

        public int Id { get; set; }
        [Required]
        public string DisplayText { get; set; }
    }
}