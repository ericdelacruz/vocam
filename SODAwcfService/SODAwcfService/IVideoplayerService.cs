using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Xml.Serialization;
using SODAwcfService.XMLModels;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IVideoplayerService" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IVideoplayerService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "validate/?username={username}&password={password}")]
        Users validate(string username, string password);

        [OperationContract]
        [WebInvoke( Method="GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "channels")]
        trainnowChannels channels();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/titles")]
        trainnowChannels titles();

    }
    [DataContract]
    [XmlRoot(ElementName="Users")]
    public class Users
    {
        [XmlElement("User")]
        public User user;
        [XmlElement("Subsription")]
        public Subscription subscription;
        [XmlElement("News")]
        public News news;
        [XmlElement("Image")]
        public Image image;
        [XmlElement("ClientWebsite")]
        public ClientWebsite clientWebsite;

        [XmlElement("PCLaptop")]
        public DeviceLicense[] PClaptopLicense;
        //[XmlElement("PCLaptop")]
        //public DeviceLicenseConsumed PCLaptopLicenseConsumed;

        [XmlElement("Server")]
        public ServerViewCount[] serverViewcount;
        
        [XmlElement("IPhone")]
        public DeviceLicense[] iponeLicense;
        //[XmlElement("IPhone")]
        //public DeviceLicenseConsumed iphoneLicenseConsumed;

        [XmlElement("IPad")]
        public DeviceLicense[] ipadLicense;
        //[XmlElement("IPad")]
        //public DeviceLicenseConsumed ipadLicenseConsumed;

        [XmlElement("AndroidPhone")]
        public DeviceLicense[] androidPhoneLicense;
        //[XmlElement("AndroidPhone")]
        //public DeviceLicenseConsumed androidPhoneLicenseConsumed;

        [XmlElement("AndroidTab")]
        public DeviceLicense[] androidTabLicense;
        //[XmlElement("AndroidTab")]
        //public DeviceLicenseConsumed androidTabLicenseConsumed;

        public Users(bool authorized,int daysleft)
        {
            this.user = new User() { authorized = authorized };
            this.subscription = new Subscription() { daysleft = daysleft };
            this.news = new News() { shownews = true };
            this.image = new Image() { file = "" };
            this.clientWebsite = new ClientWebsite() { websiteurl = "" };
            this.PClaptopLicense = new DeviceLicense[] {new DeviceLicense(){ licenses=""},new DeviceLicense(){ licensesConsumed =""}};
            this.serverViewcount = new ServerViewCount[] { new ServerViewCount() { viewCount = "" }, new ServerViewCount() { viewCountConsumed = "" } };
            this.iponeLicense = new DeviceLicense[] { new DeviceLicense() { licenses = "" }, new DeviceLicense() { licensesConsumed = "" } };
            this.ipadLicense = new DeviceLicense[] { new DeviceLicense() { licenses = "" }, new DeviceLicense() { licensesConsumed = "" } };
            this.androidPhoneLicense = new DeviceLicense[] { new DeviceLicense() { licenses = "" }, new DeviceLicense() { licensesConsumed = "" } };
            this.androidTabLicense = new DeviceLicense[] { new DeviceLicense() { licenses = "" }, new DeviceLicense() { licensesConsumed = "" } };
        }
        public Users()
        {

        }
    }
    

    [DataContract]
    [XmlRoot(ElementName="trainnow")]
    public class trainnowChannels
    {
        [XmlArray("channels")]
        [XmlArrayItem("channel")]
        public List<channel> channels { get; set; }

        [XmlArray("titles")]
        [XmlArrayItem("title")]
        public List<Title> titleList { get; set; }
    }

    //[DataContract]
    //[XmlRoot("trainnow")]
    //public class titles
    //{
    //    [XmlArray("titles")]
    //    [XmlArrayItem("title")]
    //    public List<Title> titleList {get;set;}
    //}
}
