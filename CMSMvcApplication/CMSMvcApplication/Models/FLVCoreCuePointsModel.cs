using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CMSMvcApplication.Models
{
    [XmlRoot("FLVCoreCuePoints")]
    public class FLVCoreCuePoints
    {
        [XmlElement("CuePoint")]
        public List<CuePoint> Cuepoints { get; set; }
        [XmlAttribute]
        public int Version { get; set; }
    }

    public class CuePoint
    {
        [System.Xml.Serialization.XmlElement("Time")]
        public double Time { get; set; }
        [System.Xml.Serialization.XmlElement("Type")]
        public string Type { get; set; }
        [System.Xml.Serialization.XmlElement("Name")]
        public string Name { get; set; }
    }
}