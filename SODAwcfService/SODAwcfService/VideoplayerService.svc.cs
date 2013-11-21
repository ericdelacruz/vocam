using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SODAwcfService.SodaDBDataSetTableAdapters;
using System.ServiceModel.Activation;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "VideoplayerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select VideoplayerService.svc or VideoplayerService.svc.cs at the Solution Explorer and start debugging.
    public class VideoplayerService : IVideoplayerService
    {
        private AccountsTableAdapter account = new AccountsTableAdapter();
        private PortalDataSetTableAdapters.CustomerTableAdapter customer = new PortalDataSetTableAdapters.CustomerTableAdapter();
        private CategoryTableAdapter category = new CategoryTableAdapter();
        private SpecificTableAdapter spec = new SpecificTableAdapter();

        public Users validate(string username, string password)
        {
              var result = account.GetDataByUserName(username);
             if(result.Count() > 0 && EncDec.DecryptString(result.First().PASSWORD).CompareTo(password.Trim()) == 0)
             {
                 var cust = from c in customer.GetData()
                            where c.UserId == result.First().Id
                            select c;
                 int daysremaining = 0;
                 if (cust.Count() > 0 && cust.First().DateSubscriptionEnd != null)
                     daysremaining = ((TimeSpan)(DateTime.Now - cust.First().DateSubscriptionEnd)).Days;
                 else
                     return new Users(false, 0);
                 return new Users(true, daysremaining);
             }
             else
             {
                 return new Users(false, 0);
             }
        }

        public trainnowChannels channels()
        {
            List<channel> channelList = new List<channel>();
            foreach(var cat in category.GetData())
            {
                channelList.Add(new channel(cat.CategoryName, cat.IMG_URL, cat.CategoryId));
            }
            return new trainnowChannels() { channels = channelList };
        }


        public trainnowChannels titles()
        {
            List<Title> titleList = new List<Title>();
            foreach (var title in spec.GetData())
            {
                titleList.Add(new Title()
                {
                    id = title.Id,
                    code = title.TitleCode,
                    name = title.Title,
                    filename = System.IO.Path.GetFileName(title.ImageURL),
                    summary = title.Overview,
                    downloadNews = "False",
                    questionAnswerChangeDate = string.Format("{0:M/d/yyyy}", default(DateTime)),
                    time = string.Format("{0}", default(TimeSpan)).Replace(":","")
                });
            }
            return new trainnowChannels() { titleList=titleList };
        }
    }
}
