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
        private string asdasd = EncDec.EncryptData("myS0D@P@ssw0rd");
       
        private bool Allowed = true;
        public CatListingService()
        {
            //Authenticate(password);
            catTableAdaptor = new SodaDBDataSetTableAdapters.CategoryTableAdapter();
            specTableAdaptor = new SodaDBDataSetTableAdapters.SpecificTableAdapter();
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
            return catTableAdaptor.InsertCategory(category_new.CategoryName, category_new.Description, category_new.IMG_URL, category_new.Metatags,category_new.BG_IMG, category_new.Banner_IMG, category_new.Overview,category_new.MetaDesc, category_new.PageTitile);
        }
        public int update_Category(Models.Category category_new)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return catTableAdaptor.UpdateCategory(category_new.CategoryName, category_new.Description, category_new.IMG_URL, category_new.Metatags, category_new.CategoryId, category_new.BG_IMG, category_new.Banner_IMG, category_new.Overview, category_new.MetaDesc, category_new.PageTitile);
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
            SodaDBDataSet.CategoryDataTable tbResults = new SodaDBDataSet.CategoryDataTable();

            List<Models.Category> listCat = new List<Models.Category>();
            try
            {
                catTableAdaptor.Fill(tbResults);
            }
            catch (Exception ex)
            {
                throw (ex);
                //throw (new FaultException("DB", new FaultCode("DB")));
            }

            foreach (DataRow row in tbResults.Rows)
            {
                listCat.Add(new Models.Category()
                {
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

        #endregion
        #region Specific
        public int add_Specific(Models.Specific specific_new)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return specTableAdaptor.InsertSpecific(specific_new.CategoryID, specific_new.Title, specific_new.Description, 
                            specific_new.VideoURL, specific_new.ImageURL, specific_new.Metatags,specific_new.BG_Img, specific_new.Overview, specific_new.TitleCode, specific_new.PageTitle, specific_new.MetaDesc);
        }

        public int update_Specific(Models.Specific specific_new)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return specTableAdaptor.UpdateSpecific(specific_new.CategoryID, specific_new.Title, specific_new.Description, specific_new.VideoURL, 
                        specific_new.ImageURL, specific_new.Metatags,specific_new.BG_Img, specific_new.Overview, specific_new.TitleCode, specific_new.Id, specific_new.PageTitle, specific_new.MetaDesc);
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
            SodaDBDataSet.SpecificDataTable tbResult = new SodaDBDataSet.SpecificDataTable();
            List<Models.Specific> listSpec = new List<Models.Specific>();
            
            try
            {
                specTableAdaptor.FillById(tbResult, ID);
            }
            catch(Exception ex)
            {
                throw (new FaultException("DB", new FaultCode("DB")));
            }

            foreach(DataRow row in tbResult.Rows)
            {
                listSpec.Add(new Models.Specific() { 
                                 CategoryID = (long)row["CategoryID"],
                                 Id = (long)row["Id"],
                                 Title = row["Title"].ToString(),
                                 Description = row["Description"].ToString(),
                                 VideoURL = row["VideoURL"].ToString(),
                                 ImageURL = row["ImageURL"].ToString(),
                                 Metatags = row["Metatags"].ToString(),
                                 BG_Img = row["bg_img"].ToString(),
                                 Overview = row["Overview"].ToString(),
                                  TitleCode = row["TitleCode"].ToString(),
                                 MetaDesc = row["Meta_Desc"].ToString(),
                                 PageTitle = row["PageTitle"].ToString()
                });
            }

            return listSpec;
        }

        public IEnumerable<Models.Specific> getSpecificByCatID(long CatID)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            SodaDBDataSet.SpecificDataTable tbResult = new SodaDBDataSet.SpecificDataTable();
            List<Models.Specific> listSpec = new List<Models.Specific>();

            try
            {
                specTableAdaptor.FillByCategoryID(tbResult,CatID);
            }
            catch (Exception ex)
            {
                throw (new FaultException("DB", new FaultCode("DB")));
            }

            foreach (DataRow row in tbResult.Rows)
            {
                listSpec.Add(new Models.Specific()
                {
                    CategoryID = (long)row["CategoryID"],
                    Id = (long)row["Id"],
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    VideoURL = row["VideoURL"].ToString(),
                    ImageURL = row["ImageURL"].ToString(),
                    Metatags = row["Metatags"].ToString(),
                    Overview = row["Overview"].ToString(),
                    TitleCode = row["TitleCode"].ToString(),
                    BG_Img = row["bg_img"].ToString(),
                    MetaDesc = row["Meta_Desc"].ToString(),
                    PageTitle = row["PageTitle"].ToString()
                });
            }

            return listSpec;
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
            SodaDBDataSet.SpecificDataTable tbResult = new SodaDBDataSet.SpecificDataTable();
            List<Models.Specific> listSpec = new List<Models.Specific>();

            try
            {
                specTableAdaptor.Fill(tbResult);
            }
            catch (Exception ex)
            {
                throw (new FaultException("DB", new FaultCode("DB")));
            }

            foreach (DataRow row in tbResult.Rows)
            {
                listSpec.Add(new Models.Specific()
                {
                    CategoryID = (long)row["CategoryID"],
                    Id = (long)row["Id"],
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    VideoURL = row["VideoURL"].ToString(),
                    ImageURL = row["ImageURL"].ToString(),
                    Metatags = row["Metatags"].ToString(),
                    Overview = row["Overview"].ToString(),
                    TitleCode = row["TitleCode"].ToString(),
                    BG_Img = row["bg_img"].ToString(),
                     PageTitle = row["PageTitle"].ToString(),
                      MetaDesc = row["Meta_Desc"].ToString()
                });
            }

            return listSpec;
        }
        #endregion
    }
}
