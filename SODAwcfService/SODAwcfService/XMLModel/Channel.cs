using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
namespace SODAwcfService.XMLModels
{
   
    public class channel
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string image { get; set; }
        [XmlAttribute]
        public long id { get; set; }
        public channel()
        {

        }
        public channel(string name,string image, long id)
        {
            this.name = name;
            this.image = image;
            this.id = id;
        }
    }
}