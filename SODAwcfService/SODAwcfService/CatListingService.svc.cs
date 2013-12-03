using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CatListingService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CatListingService.svc or CatListingService.svc.cs at the Solution Explorer and start debugging.
    public class CatListingService : ICatListingService
    {

        private SodaDBDataSetTableAdapters.CategoryTableAdapter catTableAdaptor;
        private SodaDBDataSetTableAdapters.SpecificTableAdapter specTableAdaptor;
        private SodaDBDataSetTableAdapters.RelatedTableAdapter relTableAdaptor;
        private SodaDBDataSetTableAdapters.TopicsTableAdapter topicAdapter;
        private SodaDBDataSetTableAdapters.chapterTableAdapter chapterAdapter;
        
        private string asdasd = EncDec.EncryptData("myS0D@P@ssw0rd");
       
        private bool Allowed = true;
        public CatListingService()
        {
            //Authenticate(password);
            catTableAdaptor = new SodaDBDataSetTableAdapters.CategoryTableAdapter();
            specTableAdaptor = new SodaDBDataSetTableAdapters.SpecificTableAdapter();
            relTableAdaptor = new SodaDBDataSetTableAdapters.RelatedTableAdapter();
            chapterAdapter = new SodaDBDataSetTableAdapters.chapterTableAdapter();
            topicAdapter = new SodaDBDataSetTableAdapters.TopicsTableAdapter();
        }
        public bool Authenticate(string Password)
        {
            return Allowed = EncDec.DecryptString(asdasd).CompareTo(Password) == 0;
                
          
        }
        #region Category
        public int add_Category(Models.Category category_new)
        {
            if(!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return catTableAdaptor.Insert(category_new.CategoryName, category_new.Description, category_new.IMG_URL, category_new.Metatags, category_new.BG_IMG, category_new.Banner_IMG, category_new.Overview, category_new.MetaDesc, category_new.PageTitile);
            //return catTableAdaptor.InsertCategory(category_new.CategoryName, category_new.Description, category_new.IMG_URL, category_new.Metatags,category_new.BG_IMG, category_new.Banner_IMG, category_new.Overview,category_new.MetaDesc, category_new.PageTitile);
        }
        public int update_Category(Models.Category category_new)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return catTableAdaptor.Update(category_new.CategoryName, category_new.Description, category_new.IMG_URL, category_new.Metatags, category_new.BG_IMG, category_new.Banner_IMG, category_new.Overview, category_new.MetaDesc, category_new.PageTitile, category_new.CategoryId);
            //return catTableAdaptor.UpdateCategory(category_new.CategoryName, category_new.Description, category_new.IMG_URL, category_new.Metatags, category_new.CategoryId, category_new.BG_IMG, category_new.Banner_IMG, category_new.Overview, category_new.PageTitile, category_new.MetaDesc);
        }
       
        public int delete_Category(long CategoryID)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return catTableAdaptor.DeleteCategoryByID(CategoryID);
        }

        public IEnumerable<Models.Category>  get_Category(long CategoryID)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));

            SodaDBDataSet.CategoryDataTable tbResults = new SodaDBDataSet.CategoryDataTable();
            List<Models.Category> listCat = new List<Models.Category>();
            try
            {
                catTableAdaptor.FillByCategoryID(tbResults, CategoryID);
            }
            catch(System.Data.SqlClient.SqlException ex)
            {
                throw (ex);
                //throw (new FaultException("DB", new FaultCode("DB")));
            }

            foreach(DataRow row in tbResults.Rows)
            {
                listCat.Add(new Models.Category(){
                     CategoryId = (long)row["CategoryID"],
                     CategoryName = row["CategoryName"].ToString(),
                     Description = row["Description"].ToString(),
                     Metatags = row["Metatags"].ToString(),
                     BG_IMG = row["bg_IMG"].ToString(),
                     Banner_IMG = row["banner_IMG"].ToString(),
                     Overview = row["Overview"].ToString(),
                     IMG_URL = row["IMG_URL"].ToString(),
                      MetaDesc = row["Meta_Desc"].ToString(),
                       PageTitile = row["PageTitle"].ToString()
                });
            }
            return listCat;
        }

        public IEnumerable<Models.Category> get_Categories()
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            //SodaDBDataSet.CategoryDataTable tbResults = new SodaDBDataSet.CategoryDataTable();

            //List<Models.Category> listCat = new List<Models.Category>();
            //try
            //{
            //    catTableAdaptor.Fill(tbResults);
            //}
            //catch (Exception ex)
            //{
            //    throw (ex);
            //    //throw (new FaultException("DB", new FaultCode("DB")));
            //}

            //foreach (DataRow row in tbResults.Rows)
            //{
            //    listCat.Add(new Models.Category()
            //    {
            //        CategoryId = (long)row["CategoryID"],
            //        CategoryName = row["CategoryName"].ToString(),
            //        Description = row["Description"].ToString(),
            //        Metatags = row["Metatags"].ToString(),
            //        BG_IMG = row["bg_IMG"].ToString(),
            //        Banner_IMG = row["banner_IMG"].ToString(),
            //        Overview = row["Overview"].ToString(),
            //        IMG_URL = row["IMG_URL"].ToString(),
            //        MetaDesc = row["Meta_Desc"].ToString(),
            //        PageTitile = row["PageTitle"].ToString()
            //    });
            //}
            //return listCat;
           
            return catTableAdaptor.GetData().Select(cat => new Models.Category()
            {
                CategoryId = cat.CategoryId,
                CategoryName = cat.CategoryName,
                PageTitile = cat.PageTitle,
                Description = cat.Description,
                Banner_IMG = cat.banner_img,
                BG_IMG = cat.bg_img,
                IMG_URL = cat.IMG_URL,
                Overview = cat.Overview,
                MetaDesc = cat.Meta_Desc,
                Metatags = cat.Metatags
            });

        }

        #endregion

        #region Specific
        public int add_Specific(Models.Specific specific_new)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            //return specTableAdaptor.InsertSpecific(specific_new.CategoryID, specific_new.Title, specific_new.Description, 
            //                specific_new.VideoURL, specific_new.ImageURL, specific_new.Metatags,specific_new.BG_Img, specific_new.Overview, specific_new.TitleCode, specific_new.PageTitle, specific_new.MetaDesc);
            return specTableAdaptor.Insert(specific_new.CategoryID, specific_new.Title, specific_new.Description,
                            specific_new.VideoURL, specific_new.ImageURL, specific_new.Metatags, specific_new.BG_Img, specific_new.TitleCode,specific_new.Overview, 
                            specific_new.PageTitle, specific_new.MetaDesc,specific_new.FileName, specific_new.isDOwnloadNews,specific_new.DateQuestionAnswerChange,specific_new.time, 
                            specific_new.totalChapters, specific_new.Approved, specific_new.Downloadlable, specific_new.InDisc,specific_new.Duration);
        }

        public int update_Specific(Models.Specific specific_new)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            //return specTableAdaptor.UpdateSpecific(specific_new.CategoryID, specific_new.Title, specific_new.Description, specific_new.VideoURL, 
            //            specific_new.ImageURL, specific_new.Metatags,specific_new.BG_Img, specific_new.Overview, specific_new.TitleCode, specific_new.Id, specific_new.PageTitle, specific_new.MetaDesc);
            return specTableAdaptor.Update(specific_new.CategoryID, specific_new.Title, specific_new.Description,
                            specific_new.VideoURL, specific_new.ImageURL, specific_new.Metatags, specific_new.BG_Img, specific_new.TitleCode,specific_new.Overview,
                            specific_new.PageTitle, specific_new.MetaDesc, specific_new.FileName, specific_new.isDOwnloadNews, specific_new.DateQuestionAnswerChange, specific_new.time,
                            specific_new.totalChapters, specific_new.Approved, specific_new.Downloadlable, specific_new.InDisc, specific_new.Duration,specific_new.Id);
        }

        public int delete_Specific(long id)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return specTableAdaptor.DeleteSpecific(id);
        }

        public IEnumerable<Models.Specific> getSpecificByID(long ID)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            //SodaDBDataSet.SpecificDataTable tbResult = new SodaDBDataSet.SpecificDataTable();
            //List<Models.Specific> listSpec = new List<Models.Specific>();
            
            //try
            //{
            //    specTableAdaptor.FillById(tbResult, ID);
            //}
            //catch(Exception ex)
            //{
            //    throw (new FaultException("DB", new FaultCode("DB")));
            //}

            //foreach(DataRow row in tbResult.Rows)
            //{
            //    listSpec.Add(new Models.Specific() { 
            //                     CategoryID = (long)row["CategoryID"],
            //                     Id = (long)row["Id"],
            //                     Title = row["Title"].ToString(),
            //                     Description = row["Description"].ToString(),
            //                     VideoURL = row["VideoURL"].ToString(),
            //                     ImageURL = row["ImageURL"].ToString(),
            //                     Metatags = row["Metatags"].ToString(),
            //                     BG_Img = row["bg_img"].ToString(),
            //                     Overview = row["Overview"].ToString(),
            //                      TitleCode = row["TitleCode"].ToString(),
            //                     MetaDesc = row["Meta_Desc"].ToString(),
            //                     PageTitle = row["PageTitle"].ToString()
            //    });
            //}

            //return listSpec;

            return specTableAdaptor.GetData().Select(spec => new Models.Specific
            {
                Id = spec.Id,
                BG_Img = spec.bg_img,
                CategoryID = spec.CategoryID,
                DateQuestionAnswerChange = spec.DateQuestionAnswerChange,
                isDOwnloadNews = spec.isDownloadNews,
                Description = spec.Description,
                FileName = spec.filename,
                ImageURL = spec.ImageURL,
                MetaDesc = spec.Meta_Desc,
                Metatags = spec.Metatags,
                Overview = spec.Overview,
                PageTitle = spec.PageTitle,
                Title = spec.Title,
                TitleCode = spec.TitleCode,
                VideoURL = spec.VideoURL
            }).Where(spec => spec.Id== ID);
        }

        public IEnumerable<Models.Specific> getSpecificByCatID(long CatID)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            //SodaDBDataSet.SpecificDataTable tbResult = new SodaDBDataSet.SpecificDataTable();
            //List<Models.Specific> listSpec = new List<Models.Specific>();

            //try
            //{
            //    specTableAdaptor.FillByCategoryID(tbResult,CatID);
            //}
            //catch (Exception ex)
            //{
            //    throw (new FaultException("DB", new FaultCode("DB")));
            //}

            //foreach (DataRow row in tbResult.Rows)
            //{
            //    listSpec.Add(new Models.Specific()
            //    {
            //        CategoryID = (long)row["CategoryID"],
            //        Id = (long)row["Id"],
            //        Title = row["Title"].ToString(),
            //        Description = row["Description"].ToString(),
            //        VideoURL = row["VideoURL"].ToString(),
            //        ImageURL = row["ImageURL"].ToString(),
            //        Metatags = row["Metatags"].ToString(),
            //        Overview = row["Overview"].ToString(),
            //        TitleCode = row["TitleCode"].ToString(),
            //        BG_Img = row["bg_img"].ToString(),
            //        MetaDesc = row["Meta_Desc"].ToString(),
            //        PageTitle = row["PageTitle"].ToString()
            //    });
            //}

            //return listSpec;
            return specTableAdaptor.GetData().Select(spec => new Models.Specific
            {
                Id = spec.Id,
                BG_Img = spec.bg_img,
                CategoryID = spec.CategoryID,
                DateQuestionAnswerChange = spec.DateQuestionAnswerChange,
                isDOwnloadNews = spec.isDownloadNews,
                Description = spec.Description,
                FileName = spec.filename,
                ImageURL = spec.ImageURL,
                MetaDesc = spec.Meta_Desc,
                Metatags = spec.Metatags,
                Overview = spec.Overview,
                PageTitle = spec.PageTitle,
                Title = spec.Title,
                TitleCode = spec.TitleCode,
                VideoURL = spec.VideoURL
            }).Where(spec => spec.CategoryID == CatID);
        }

        public IEnumerable<Models.Specific> getRelatedByID(long ID)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));

            SodaDBDataSet.RelatedDataTable tbResult = new SodaDBDataSet.RelatedDataTable();
            List<Models.Specific> listSpec = new List<Models.Specific>();
            try
            {
                relTableAdaptor.FillByBaseID(tbResult, ID);

            }
            catch(Exception ex)
            {
                throw (new FaultException("DB", new FaultCode("DB")));
            }

            if(tbResult.Count() > 0)
            {
                foreach (DataRow row in tbResult.Rows)
                {
                    listSpec.Add(getSpecificByID(ID).First());
                }
            }

            return listSpec;
        }

        public IEnumerable<Models.Specific> get()
        {
            
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            //SodaDBDataSet.SpecificDataTable tbResult = new SodaDBDataSet.SpecificDataTable();
            //List<Models.Specific> listSpec = new List<Models.Specific>();

            //try
            //{
            //    specTableAdaptor.Fill(tbResult);
            //}
            //catch (Exception ex)
            //{
            //    throw (new FaultException("DB", new FaultCode("DB")));
            //}

            //foreach (DataRow row in tbResult.Rows)
            //{
            //    listSpec.Add(new Models.Specific()
            //    {
            //        CategoryID = (long)row["CategoryID"],
            //        Id = (long)row["Id"],
            //        Title = row["Title"].ToString(),
            //        Description = row["Description"].ToString(),
            //        VideoURL = row["VideoURL"].ToString(),
            //        ImageURL = row["ImageURL"].ToString(),
            //        Metatags = row["Metatags"].ToString(),
            //        Overview = row["Overview"].ToString(),
            //        TitleCode = row["TitleCode"].ToString(),
            //        BG_Img = row["bg_img"].ToString(),
            //         PageTitle = row["PageTitle"].ToString(),
            //          MetaDesc = row["Meta_Desc"].ToString()
            //    });
            //}

            //return listSpec;
            return specTableAdaptor.GetData().Select(spec => new Models.Specific
            {
                Id = spec.Id,
                BG_Img = spec.bg_img,
                CategoryID = spec.CategoryID,
                DateQuestionAnswerChange = spec.DateQuestionAnswerChange,
                isDOwnloadNews = spec.isDownloadNews,
                Description = spec.Description,
                FileName = spec.filename,
                ImageURL = spec.ImageURL,
                MetaDesc = spec.Meta_Desc,
                Metatags = spec.Metatags,
                Overview = spec.Overview,
                PageTitle = spec.PageTitle,
                Title = spec.Title,
                TitleCode = spec.TitleCode,
                VideoURL = spec.VideoURL,
                 Approved = spec.Approved,
                  Downloadlable = spec.Downloadable,
                   Duration = spec.Duration,
                    InDisc = spec.InDisc,
                     time = spec.Time,
                      totalChapters = spec.TotalChapters
            });
        }
        #endregion

        #region chapter
        public int addChapter( long specID,string name, TimeSpan time)
        {
            return chapterAdapter.Insert(specID, name, time);
        }
        public IEnumerable<Models.Chapter> getChapter()
        {
            return chapterAdapter.GetData().Select(chapter => new Models.Chapter() { Id = chapter.Id, ChapterName = chapter.ChapterName, SpecID = chapter.SpecID, time = chapter.time });
        }
        public int updateChapter(Models.Chapter chapter)
        {
            return chapterAdapter.Update(chapter.SpecID, chapter.ChapterName, chapter.time, chapter.Id);
        }
        public int deleteChapter(long id)
        {
            return chapterAdapter.Delete(id);
        }
        #endregion

        #region topic
        public int addTopic(long specID, string name)
        {
            return topicAdapter.Insert(specID, name);
        }
        public IEnumerable<Models.Topic> getTopics()
        {
            return topicAdapter.GetData().Select(topic => new Models.Topic() { Id = topic.Id, Name = topic.Name, SpecId = topic.SpecId });
        }
        public int updateTopic(Models.Topic topic)
        {
            return topicAdapter.Update(topic.SpecId, topic.Name, topic.Id);
        }
        public int deleteTopic(long id)
        {
            return topicAdapter.Delete(id);
        }
        #endregion
    }
}
