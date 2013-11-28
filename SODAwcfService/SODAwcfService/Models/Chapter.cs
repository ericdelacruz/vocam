using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class Chapter
    {
        public long Id { get; set; }
        public Nullable<long> SpecID { get; set; }
        public string ChapterName { get; set; }
        public Nullable<System.TimeSpan> time { get; set; }
    }
}