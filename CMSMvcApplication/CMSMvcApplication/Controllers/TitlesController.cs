using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Xml.Serialization;
using System.IO;
namespace CMSMvcApplication.Controllers
{
    public class TitlesController : Controller
    {
        private CatListingServiceReference.CatListingServiceClient catClient = new CatListingServiceReference.CatListingServiceClient();
        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
       
        protected override void Dispose(bool disposing)
        {
            catClient.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        //
        // GET: /Titles/
        /// <summary>
        /// List all of the titles
        /// </summary>
        /// <param name="region">filter by region</param>
        /// <param name="title">filter by title</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(string region, string title, int? page)
        {
            //redirect to login page if session expired
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            //get titles with no filters
            var catTitleList = getTitles("", "");
            //filter by title if not null
            if(!string.IsNullOrEmpty(title))
            {
                catTitleList = from titles in catTitleList
                               join r in portalClient.getRegion() on titles.Specific.RegionId equals r.Id
                               where titles.Specific.Title.ToLower().Contains(title.Trim().ToLower()) 
                               select titles;

            }
            //filter by region if not null
            if (!string.IsNullOrEmpty(region) )
            {
                catTitleList = from titles in catTitleList
                               join r in portalClient.getRegion() on titles.Specific.RegionId equals r.Id
                               where r.RegionName == region 
                               select titles;
            }
            
            ViewBag.RegionList = portalClient.getRegion();
            
            ViewBag.RegionName = region ?? null;
            ViewBag.TitleName = title ?? "";

            int pageSize = 15;//max titles per page
            int pageNum = page ?? 1;

            return View(catTitleList.OrderBy(t=>t.Specific.Title).ToList().ToPagedList(pageNum,pageSize));
        }
        
       

        private IEnumerable<ViewModels.Title> getTitles(string title, string cat)
        {
            

            var titleList = catClient.get();
            var catList = catClient.get_Categories();
            var regionList = portalClient.getRegion();
            if (title != "" && cat == "")
            {
                
                var catTitle = from titleRow in titleList
                               join catRow in catList on titleRow.CategoryID equals catRow.CategoryId
                               join region in regionList on titleRow.RegionId equals region.Id
                               where titleRow.Title == title
                               select new ViewModels.Title() { Category = catRow, Specific = titleRow,regionName=region.RegionName };
                return catTitle;
            }
            else if (title == "" && cat != "")
            {
                var catTitle = from titleRow in titleList
                               join catRow in catList on titleRow.CategoryID equals catRow.CategoryId
                               join region in regionList on titleRow.RegionId equals region.Id
                               where catRow.CategoryName == cat
                               select new ViewModels.Title() { Category = catRow, Specific = titleRow, regionName = region.RegionName };

                return catTitle;
            }
            else  
            {
                
                var catTitle = from titleRow in titleList
                               join catRow in catList on titleRow.CategoryID equals catRow.CategoryId
                               join region in regionList on titleRow.RegionId equals region.Id
                               select new ViewModels.Title() { Category = catRow, Specific = titleRow, regionName = region.RegionName };

                return catTitle;
            }
        }

        //
        // GET: /Titles/Create

        public ActionResult Create()
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            initCreateViewBagData();

            return View();
        }

        private void initCreateViewBagData()
        {
            ViewBag.catList = catClient.get_Categories();
            ViewBag.regionList = portalClient.getRegion().Select(r => new SelectListItem()
            {
                Value = r.Id.ToString(),
                Text = r.RegionName,
                Selected = false
            });
        }

        //
        // POST: /Titles/Create

        [HttpPost]
        
        public ActionResult Create(FormCollection collection)
        {
            DateTime dateQuestion = new DateTime();

            DateTime.TryParse(collection["datefrom"], out dateQuestion);
           
            try
            {
                //init title to be added
                var new_title = new CatListingServiceReference.Specific()
                {
                    Title = collection["Title"],
                    TitleCode = collection["Code"],
                    CategoryID = 1,//long.Parse(collection["Category"]),
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    PageTitle = collection["TitlePageTitle"],
                    Metatags = collection["TitleMetaKeywords"],
                    MetaDesc = collection["TitleMetaDescription"],
                    //upload and get fileURL, If no file uploaded default to "#"
                    ImageURL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : "#",
                    BG_Img = !string.IsNullOrEmpty(Request.Files["BG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority) + "/", FileTransferHelper.UploadImage(Request.Files["BG"], Server)) : "#",
                    VideoURL = collection["VidURL"],
                    FileName = collection["filename"],
                    Duration = collection["duration"].Trim() != "" ? int.Parse(collection["duration"]) : 0,
                    time = collection["time"].Trim() != "" ? TimeSpan.FromMilliseconds(double.Parse(collection["time"])) : default(TimeSpan),
                    totalChapters = collection["totalChap"].Trim() != "" ? int.Parse(collection["totalChap"]) : 0,
                    Approved = collection["approved"] == "yes",
                    Downloadlable = collection["downloadable"] == "yes",
                    DateQuestionAnswerChange = dateQuestion,
                    InDisc = collection["indisc"].Trim() != "" ? int.Parse(collection["indisc"]) : 0,
                    isDOwnloadNews = collection["isdownloadnews"] == "yes",
                    RegionId = int.Parse(collection["titleRegion"]),
                    Id = 0
                };
                //log title
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_title, portalClient);
                //add to db
                catClient.add_Specific(new_title);

                //get the title added
                var title = catClient.get().Select(c => c).Where(c => c.TitleCode == collection["Code"]).First();
                //if user assign categories to title
                if (!string.IsNullOrEmpty(collection["t"]))
                {
                    foreach (string strNum in collection["t"].Trim().Split(','))
                    {
                        catClient.addCatAssign(title.Id, long.Parse(strNum));
                    }
                }
                //add topics
                addTopics(Request.Form.GetValues("topicName[]"), title);
                //if import XML
                if (Request.Files["importChapter"].ContentLength > 0)
                {
                    //deserialize XML to list
                    var Imported_ChapterList = importchapters();
                    foreach(var chapter in Imported_ChapterList)
                    {
                        catClient.addChapter(title.Id, chapter.ChapterName, chapter.time.Value);
                    }
                }
                //add chapters that are manually added
                addChapters(Request.Form.GetValues("chapterName[]"), Request.Form.GetValues("chapterTime[]"), title);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {

                ModelState.AddModelError("", "Error.");
                ViewBag.catList = catClient.get_Categories();
                return View();
            }
            
        }

        private void addChapters(string[] chapterNames,string[] chaterTimes, CatListingServiceReference.Specific title)
        {
            //if chapters is not empty if null
            if(chapterNames != null && chapterNames.Count() >0)
            {
                for(int i =0; i< chapterNames.Count();i++)
                {
                    if(chapterNames[i].Trim() != "")  
                    catClient.addChapter(title.Id,chapterNames[i],TimeSpan.FromMilliseconds(double.Parse(chaterTimes[i])));
                }
            }
        }

        private void addTopics(string[] topics, CatListingServiceReference.Specific title)
        {
            //if topics is not null or empty
            if(topics != null && topics.Count() > 0)
            {
                foreach(string topic in topics)
                {
                    if(topic.Trim() != "")
                    {
                        catClient.addTopic(title.Id, topic);
                    }
                }
            }
        }

       

        private bool hasErrorInTitles(FormCollection collection, out string errorMsg)
        {
            int nNum =0;
            double dNum=0;
            if(collection["duration"].Trim() != "" && !int.TryParse(collection["duration"], out nNum))
            {
                errorMsg = "Invalid input for Duration";
                return true;
            }
            if(collection["totalChap"].Trim() != "" &&!int.TryParse(collection["totalChap"],out nNum))
            {
                errorMsg = "Invalid input for total chapters";
                return true;
            }
            if(collection["indisc"].Trim() != "" && !int.TryParse(collection["indisc"],out nNum))
            {
                errorMsg = "Invalid input for In Disc";
                return true;
            }
             
            if(collection["time"].Trim() != "" && !double.TryParse(collection["time"],out dNum))
            {
                errorMsg = "Invalid input for Time";
                return true;
            }
            errorMsg = "";
            return false;
        }

        //
        // GET: /Titles/Edit/5

        public ActionResult Edit(long id)
        {
            //redirect to login page if session expired
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            //get title to be edited
            var title = catClient.get().Select(t => t).Where(t => t.Id == id);

            if (title.Count() == 0)
                return Redirect("Titles");

            initEditTitleViewBagData(id, title);
                return View(title.First());
            
        }

        private void initEditTitleViewBagData(long id, IEnumerable<CatListingServiceReference.Specific> title)
        {
            //get category assignment for the title to be edited
            var CAList = catClient.getCatAssign().Where(ca => ca.SpecID == id);
            //list of cat id's to be excluded in the category list
            var excludeIds = new HashSet<long>(CAList.Select(ca => ca.CategoryId));
            //get the category list except that are in the exclude lists
            ViewBag.CatList = catClient.get_Categories().Where(cat => !excludeIds.Contains(cat.CategoryId));
            //get topics of the edited title
            ViewBag.TopicList = catClient.getTopics().Select(t => t).Where(t => t.SpecId == id);
            //get the chapters of the edited title
            ViewBag.ChapterList = catClient.getChapter().Select(c => c).Where(c => c.SpecID == id);
            //filter categories that are selected
            ViewBag.CAList = from CA in CAList
                             join cat in catClient.get_Categories() on CA.CategoryId equals cat.CategoryId
                             select cat;
            //region list for the dropdown
            ViewBag.regionList = portalClient.getRegion().Select(r => new SelectListItem()
            {
                Value = r.Id.ToString(),
                Text = r.RegionName,
                Selected = r.Id == title.First().RegionId
            });
        }

        //
        // POST: /Titles/Edit/5

        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            
            DateTime dateQuestion = new DateTime();
             
            DateTime.TryParse(collection["datefrom"], out dateQuestion);

          
            try
            {
                // TODO: Add update logic here
                var old_title = catClient.getSpecificByID(id).First();

                var new_title = new CatListingServiceReference.Specific()
                {
                    Id = id,
                    TitleCode = collection["Code"],
                    Title = collection["Title"],
                    CategoryID = 1,
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    PageTitle = collection["TitlePageTitle"],
                    Metatags = collection["TitleMetaKeywords"],
                    MetaDesc = collection["TitleMetaDescription"],
                    //upload and get fileURL, If no file uploaded default to "#"
                    ImageURL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : catClient.getSpecificByID(id).First().ImageURL,
                    BG_Img = !string.IsNullOrEmpty(Request.Files["BG_IMG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["BG_IMG"], Server)) : catClient.getSpecificByID(id).First().BG_Img,
                    VideoURL = collection["VidURL"],
                    FileName = collection["filename"],
                    Duration = collection["duration"].Trim() != "" ? int.Parse(collection["duration"]) : 0,
                    time = collection["time"].Trim() != "" ? TimeSpan.FromMilliseconds(double.Parse(collection["time"])) : default(TimeSpan),
                    totalChapters = collection["totalChap"].Trim() != "" ? int.Parse(collection["totalChap"]) : 0,
                    Approved = collection["approved"] == "yes",
                    Downloadlable = collection["downloadable"] == "yes",
                    DateQuestionAnswerChange = dateQuestion,
                    InDisc = collection["indisc"].Trim() != "" ? int.Parse(collection["indisc"]) : 0,
                    isDOwnloadNews = collection["isdownloadnews"] == "yes",
                    RegionId = int.Parse(collection["titleRegion"])
                };
                //log title to be updated
                AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_title, new_title, portalClient);
                //update to db
                catClient.update_Specific(new_title);
                
                //get updated title
                var title = catClient.get().Select(c => c).Where(c => c.Id == id).First();
                //get topics of the title to be delete
                var topics = catClient.getTopics().Select(t => t).Where(t => t.SpecId == id);
                //delete topics
                foreach (var topic in topics)
                catClient.deleteTopic(topic.Id);
                
                //get chaters of the title to be deleted
                var chapters = catClient.getChapter().Select(c => c).Where(c => c.SpecID == id);
                //delete each chapters
                foreach (var chap in chapters)
                catClient.deleteChapter(chap.Id);
                //get cat assign of title
                var CAs = catClient.getCatAssign().Where(ca => ca.SpecID == id);
                //delete CA
                foreach (var ca in CAs)
                    catClient.deleteCatAssign(ca.Id);
                //if assigned category, add new CA
                if ( collection["t"] !=null && !string.IsNullOrEmpty(collection["t"]))
                {
                    foreach (string strNum in collection["t"].Trim().Split(','))
                    {
                        catClient.addCatAssign(title.Id, long.Parse(strNum));
                    }
                }
                //add topics
                addTopics(Request.Form.GetValues("topicName[]"), title);

                //if import XML
                if (Request.Files["importChapter"].ContentLength > 0)
                {
                    //deserialize chapters to list
                    var Imported_ChapterList = importchapters();
                    //add the imported list
                    foreach (var chapter in Imported_ChapterList)
                    {
                        catClient.addChapter(title.Id, chapter.ChapterName, chapter.time.Value);
                    }
                }
                //add the chapters that are manually added   
                addChapters(Request.Form.GetValues("chapterName[]"), Request.Form.GetValues("chapterTime[]"), title);
                


                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Error.");
                ViewBag.catList = catClient.get_Categories();
                return View();
            }
        }

        //
        // GET: /Titles/Delete/5

        public ActionResult Delete(long id)
        {
            //DO deletion of dependencies before deleting the title
            try
            {
                //get chapters then delete chapters
                var chapters = catClient.getChapter().Select(c => c).Where(c => c.SpecID == id);
                foreach (var chap in chapters)
                {
                    catClient.deleteChapter(chap.Id);
                }
                //get topics then delete
                var topics = catClient.getTopics().Select(t => t).Where(t => t.SpecId == id);

                foreach (var topic in topics)
                {
                    catClient.deleteTopic(topic.Id);
                }
                //get Cat assign then delete
                var catAssign = catClient.getCatAssign().Where(ca => ca.SpecID == id);
                foreach (var ca in catAssign)
                catClient.deleteCatAssign(ca.Id);
                //get the current title to be deleted
                var old_title = catClient.getSpecificByID(id).First();
                //log
                AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_title, portalClient);
                //delete to DB
                catClient.delete_Specific(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult error()
        {
            
            return View();
        }
        
        
        public IEnumerable<CatListingServiceReference.Chapter> importchapters()
        {
            
                try
                {
                    Models.FLVCoreCuePoints cuepoints = xmlSerialize(Request.Files["importChapter"]);
                   
                    if(cuepoints.Cuepoints.Count() > 0)
                    {
                       var importChapterList = cuepoints.Cuepoints.Select(cue => new CatListingServiceReference.Chapter()
                        {
                            ChapterName = cue.Name,
                            time = TimeSpan.FromMilliseconds(cue.Time)

                        }).OrderBy(cue=>cue.time).ToList();

                       return importChapterList;
                    }

                }
                catch
                {
                    //error
                    TempData.Add("ImportError", "Error during import please try again.");
                }
            
            //u is return url
                return null;
        }

        private Models.FLVCoreCuePoints xmlSerialize(HttpPostedFileBase httpPostedFileBase)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Models.FLVCoreCuePoints));
            Models.FLVCoreCuePoints result = new Models.FLVCoreCuePoints();
            TextReader reader;
            // System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create() 
            try
            {
                reader = new StreamReader(httpPostedFileBase.InputStream);
                //System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(reader);
                result = (Models.FLVCoreCuePoints)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }
    }
}
