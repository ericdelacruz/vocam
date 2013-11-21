using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
namespace SODAwcfService
{
    
    public class User
    {
        [XmlAttribute]
        public bool authorized { get; set; }
    }
    public class Subscription
    {
        [XmlAttribute]
        public int daysleft { get; set; }
    }

    public class News
    {
        [XmlAttribute]
        public bool shownews { get; set; }
    }
    public class Image
    {
        [XmlAttribute]
        public string file { get; set; }
    }
    public class ClientWebsite
    {
        [XmlAttribute]
        public string websiteurl { get; set; }
    }
    public class DeviceLicense
    {
        [XmlAttribute]
        public string licenses { get; set; }
        [XmlAttribute]
        public string licensesConsumed { get; set; }
    }
   

    public class ServerViewCount
    {
        [XmlAttribute]
        public string viewCount { get; set; }
        [XmlAttribute]
        public string viewCountConsumed { get; set; }
    }
   
}