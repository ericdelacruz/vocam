using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
namespace SODAwcfService.XMLModels
{
    
    public class Title
    {
        [XmlAttribute]
        public long id { get; set; }
        [XmlAttribute]
        public string code { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string filename { get; set; }
        [XmlAttribute]
        public string summary { get; set;}
        [XmlAttribute]
        public bool IsDownloadable { get; set; }
        [XmlAttribute]
        public int Chapters { get; set; }
        [XmlAttribute]
        public string InDisc { get; set; }
        [XmlAttribute]
        public string downloadNews { get; set; }
        [XmlAttribute]
        public string questionAnswerChangeDate { get; set; }

        [XmlAttribute]
        public string time { get; set; }

        [XmlArray("channels")]
        [XmlArrayItem("channel")]
        public List<channel> channelList { get; set; }

        [XmlArray("topics")]
        [XmlArrayItem("topic")]
        public List<topic> topicList { get; set; }

        [XmlArray("chapters")]
        [XmlArrayItem("chapter")]
        public List<chapter> chapterList { get; set;}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            }
    
    
    public class topic
    {
        [XmlAttribute]
        public string name { get; set; }
    }

    public class chapter
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string time { get; set; }
    }
 }