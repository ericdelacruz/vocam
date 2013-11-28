using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SODAwcfService.SodaDBDataSetTableAdapters;
using System.ServiceModel.Activation;
using SODAwcfService.XMLModels;

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
        private TopicsTableAdapter topicAdapter = new TopicsTableAdapter();
        private chapterTableAdapter chapterAdapter = new chapterTableAdapter();
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
                    filename = title.filename,
                    
                    summary = title.Overview,
                    downloadNews = title.isDownloadNews.ToString(),
                    questionAnswerChangeDate =  string.Format("{0:M/d/yyyy}",title.DateQuestionAnswerChange),
                    time = "0",
                    topicList = getTopics(title.Id),
                    chapterList = getChapters(title.Id)
                });
            }
            return new trainnowChannels() { titleList=titleList };
        }

        private List<XMLModels.chapter> getChapters(long specID)
        {
            var chapterList = chapterAdapter.GetData().Select(c => new { xmlChapter = new XMLModels.chapter() { name = c.ChapterName, time = c.time.TotalMilliseconds.ToString() }, c.SpecID }).Where(c => c.SpecID == specID);

            return chapterList.Count() > 0 ? chapterList.Select(c => c.xmlChapter).ToList() : null;
        }

        private List<XMLModels.topic> getTopics(long specID)
        {
            //var topics = topicAdapter.GetData().Select(t => new { xmlTopic = new topic() { name = t.Name }, t.SpecId }).Where(t => t.SpecId == specID);
            var topics = topicAdapter.GetData().Select(t => new { xmlTopic = new XMLModels.topic { name = t.Name }, t.SpecId }).Where(t => t.SpecId == specID);
            return topics.Count() > 0 ? topics.Select(t => t.xmlTopic).ToList() : null; 
        }
    }
}
