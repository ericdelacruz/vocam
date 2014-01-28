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
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            
            return base.BeginExecute(requestContext, callback, state);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            return base.BeginExecuteCore(callback, state);
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
        protected override void Dispose(bool disposing)
        {
            catClient.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        //
        // GET: /Titles/

        public ActionResult Index(string region, string title, int? page)
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            var catTitleList = getTitles("", "");

            if(!string.IsNullOrEmpty(title))
            {
                catTitleList = from titles in catTitleList
                               join r in portalClient.getRegion() on titles.Specific.RegionId equals r.Id
                               where titles.Specific.Title.ToLower().Contains(title.Trim().ToLower()) 
                               select titles;

            }
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

            int pageSize = 15;
            int pageNum = page ?? 1;

            return View(catTitleList.OrderBy(t=>t.Specific.Title).ToList().ToPagedList(pageNum,pageSize));
        }

        //
        // GET: /Titles/filter?title=test-1&cat=no-cat
        [HttpPost]
        public ActionResult filter(string title, string cat)
        {
            var catList = getTitles(title, cat);

            return View(catList);
        }
        [HttpPost]
        public ActionResult filterTitle(string title)
        {
            var catList = getTitles(title, "");

            return View(catList);
        }
        [HttpPost]
        public ActionResult filterCat(string cat)
        {
            var catList = getTitles("", cat);

            return View(catList);
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
            //string errorMsg = "";
            //if(hasErrorInTitles(collection,out errorMsg))
            //{
            //    ModelState.AddModelError("", errorMsg);
            //    ViewBag.catList = catClient.get_Categories();
            //    return View();
            //}
            //if (Request.Files["importChapter"].ContentLength > 0)
            //{
            //    importchapters();
            //    initCreateViewBagData();
            //    ViewBag.Name = collection["Title"];
            //    ViewBag.TitleCode = collection["Code"];
            //    ViewBag.PageTitle = collection["TitlePageTitle"];
            //    ViewBag.MetaKeywords = collection["TitleMetaKeywords"];
            //    ViewBag.MetaDescription = collection["TitleMetaDescription"];
            //    return View();
            //}
            try
            {
                // TODO: Add insert logic here
                catClient.add_Specific(new CatListingServiceReference.Specific()
                {
                    Title = collection["Title"],
                    TitleCode = collection["Code"],
                    CategoryID = 1,//long.Parse(collection["Category"]),
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    PageTitle = collection["TitlePageTitle"],
                    Metatags = collection["TitleMetaKeywords"],
                    MetaDesc = collection["TitleMetaDescription"],
                    ImageURL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName)?string.Concat(Request.Url.GetLeftPart(UriPartial.Authority) ,FileTransferHelper.UploadImage(Request.Files["Thumb"],Server)):"#",
                    BG_Img = !string.IsNullOrEmpty(Request.Files["BG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority) + "/", FileTransferHelper.UploadImage(Request.Files["BG"], Server)) : "#",
                    VideoURL = collection["VidURL"],
                    FileName = collection["filename"],
                    Duration = collection["duration"].Trim() != ""? int.Parse(collection["duration"]):0,
                    time =collection["time"].Trim() !=""? TimeSpan.FromMilliseconds(double.Parse(collection["time"])):default(TimeSpan),
                     totalChapters =collection["totalChap"].Trim() !=""? int.Parse(collection["totalChap"]):0,
                     Approved = collection["approved"] == "yes",
                     Downloadlable = collection["downloadable"] == "yes",
                      DateQuestionAnswerChange = dateQuestion,
                      InDisc =collection["indisc"].Trim() !=""? int.Parse(collection["indisc"]):0,
                      isDOwnloadNews = collection["isdownloadnews"] == "yes",
                       RegionId = int.Parse(collection["titleRegion"]),
                    Id = 0 
                });

                
                var title = catClient.get().Select(c => c).Where(c => c.TitleCode == collection["Code"]).First();
                if (!string.IsNullOrEmpty(collection["t"]))
                {
                    foreach (string strNum in collection["t"].Trim().Split(','))
                    {
                        catClient.addCatAssign(title.Id, long.Parse(strNum));
                    }
                }
                //addTopics(collection, title);
                addTopics(Request.Form.GetValues("topicName[]"), title);
                //addChapters(collection, title);
                if (Request.Files["importChapter"].ContentLength > 0)
                {
                    var Imported_ChapterList = importchapters();
                    foreach(var chapter in Imported_ChapterList)
                    {
                        catClient.addChapter(title.Id, chapter.ChapterName, chapter.time.Value);
                    }
                }
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
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            //var title = catClient.getSpecificByID(id).First();
            var title = catClient.get().Select(t => t).Where(t => t.Id == id);
            if (title.Count() == 0)
                return Redirect("Titles");


            initEditTitleViewBagData(id, title);
                return View(title.First());
            
        }

        private void initEditTitleViewBagData(long id, IEnumerable<CatListingServiceReference.Specific> title)
        {
            var CAList = catClient.getCatAssign().Where(ca => ca.SpecID == id);
            var excludeIds = new HashSet<long>(CAList.Select(ca => ca.CategoryId));
            ViewBag.CatList = catClient.get_Categories().Where(cat => !excludeIds.Contains(cat.CategoryId));
            ViewBag.TopicList = catClient.getTopics().Select(t => t).Where(t => t.SpecId == id);
            ViewBag.ChapterList = catClient.getChapter().Select(c => c).Where(c => c.SpecID == id);
            ViewBag.CAList = from CA in CAList
                             join cat in catClient.get_Categories() on CA.CategoryId equals cat.CategoryId
                             select cat;

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

            //if (Request.Files["importChapter"].ContentLength > 0)
            //{
            //    importchapters();
            //    var title = catClient.get().Select(t => t).Where(t => t.Id == id);
            //    initEditTitleViewBagData(id, title);
            //    if (TempData["ImportChapterList"] != null)
            //    ViewBag.ChapterList = TempData["ImportChapterList"] as IEnumerable<CatListingServiceReference.Chapter>;
            //    return View(title.First());
            //}
            try
            {
                // TODO: Add update logic here
                catClient.update_Specific(new CatListingServiceReference.Specific()
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
                    ImageURL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : catClient.getSpecificByID(id).First().ImageURL,
                    BG_Img = !string.IsNullOrEmpty(Request.Files["BG_IMG"].FileName)? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["BG_IMG"], Server)) : catClient.getSpecificByID(id).First().BG_Img,
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
                });
                
                
                var title = catClient.get().Select(c => c).Where(c => c.Id == id).First();
                var topics = catClient.getTopics().Select(t => t).Where(t => t.SpecId == id);

                foreach (var topic in topics)
                catClient.deleteTopic(topic.Id);
                

                var chapters = catClient.getChapter().Select(c => c).Where(c => c.SpecID == id);
                foreach (var chap in chapters)
                catClient.deleteChapter(chap.Id);

                var CAs = catClient.getCatAssign().Where(ca => ca.SpecID == id);
                foreach (var ca in CAs)
                    catClient.deleteCatAssign(ca.Id);

                if ( collection["t"] !=null && !string.IsNullOrEmpty(collection["t"]))
                {
                    foreach (string strNum in collection["t"].Trim().Split(','))
                    {
                        catClient.addCatAssign(title.Id, long.Parse(strNum));
                    }
                }
                addTopics(Request.Form.GetValues("topicName[]"), title);
                //addChapters(collection, title);
                if (Request.Files["importChapter"].ContentLength > 0)
                {
                    var Imported_ChapterList = importchapters();
                    foreach (var chapter in Imported_ChapterList)
                    {
                        catClient.addChapter(title.Id, chapter.ChapterName, chapter.time.Value);
                    }
                }
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
            try
            {
                var chapters = catClient.getChapter().Select(c => c).Where(c => c.SpecID == id);
                foreach (var chap in chapters)
                {
                    catClient.deleteChapter(chap.Id);
                }

                var topics = catClient.getTopics().Select(t => t).Where(t => t.SpecId == id);

                foreach (var topic in topics)
                {
                    catClient.deleteTopic(topic.Id);
                }

                var catAssign = catClient.getCatAssign().Where(ca => ca.SpecID == id);
                foreach (var ca in catAssign)
                catClient.deleteCatAssign(ca.Id);

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
