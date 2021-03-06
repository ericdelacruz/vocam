﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
namespace SODAwcfService.Models
{
    [DataContract]
    public class Specific
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long CategoryID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string VideoURL { get; set; }
        [DataMember]
        public string ImageURL { get; set; }
        [DataMember]
        public string Metatags { get; set; }
        [DataMember]
        public string BG_Img { get; set; }
        [DataMember]
        public string Overview { get; set; }
        [DataMember]
        public string TitleCode { get; set; }
        [DataMember]
        public string PageTitle { get; set; }
        [DataMember]
        public string MetaDesc { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public bool isDOwnloadNews { get; set;}
        [DataMember]
        public DateTime DateQuestionAnswerChange { get; set; }
        [DataMember]
        public TimeSpan time { get; set; }
        [DataMember]
        public int totalChapters { get; set; }
        [DataMember]
        public bool Approved { get; set; }
        [DataMember]
        public bool Downloadlable { get; set; }
        [DataMember]
        public int InDisc { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public int? RegionId { get; set; }
    }
}