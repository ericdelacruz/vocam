using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CMS_Service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CMS_Service.svc or CMS_Service.svc.cs at the Solution Explorer and start debugging.
    public class CMS_Service : ICMS_Service
    {

        private SodaDBDataSetTableAdapters.ContentDefTableAdapter CMSTableAdapter;
        private SodaDBDataSetTableAdapters.ContactTableAdapter contactTableAdapter;
        //todo place encrypted string here
        private string asdasd = EncDec.EncryptData("myS0D@P@ssw0rd");
        private static bool Allowed = true;//set this to false if prod
        private const string ACCESS_DENIED = "ACCESS DENIED";
     
        
        /// <summary>
        /// CMS web service 
        /// </summary>
        /// <param name="Password">Password for authentication</param>
        /// <remarks> Throws exception if failed authentication</remarks>
        public CMS_Service()
        {
            
            
            CMSTableAdapter = new SodaDBDataSetTableAdapters.ContentDefTableAdapter();
            contactTableAdapter = new SodaDBDataSetTableAdapters.ContactTableAdapter();
        }

        public bool Authenticate(string Password)
        {
            return Allowed = EncDec.DecryptString(asdasd).CompareTo(Password) == 0;
                

        }
     
        public int addContent(Models.ContentDef contentdef)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return  CMSTableAdapter.InsertContent(contentdef.PageCode, contentdef.SectionName, contentdef.Type, contentdef.Value);
                 
        }

        public int UpdateContent(Models.ContentDef contedef_new)
        {

            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
             return CMSTableAdapter.UpdateContent(contedef_new.Type, contedef_new.Value, contedef_new.PageCode, contedef_new.SectionName);
           
            
        }

       
        public int deleteContent(string PageCode, string sectionName)
        {

            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
           return CMSTableAdapter.DeleteContent(PageCode, sectionName);
           
        }


        public IEnumerable<Models.ContentDef> getContent(string PageCode, string sectionName)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            SodaDBDataSet.ContentDefDataTable tbResult = new SodaDBDataSet.ContentDefDataTable();
            List<Models.ContentDef> listContent = new List<Models.ContentDef>();

            try
            {

                CMSTableAdapter.Fill(tbResult, PageCode, sectionName);
            }
            catch (Exception ex)
            {
                new FaultException("DB error", new FaultCode("DB"));
            }

            foreach (DataRow row in tbResult.Rows)
            {
                listContent.Add(new Models.ContentDef(){
                                    PageCode= row["PageCode"].ToString(),
                                    SectionName = row["SectionName"].ToString(),
                                    Type= row["type"].ToString(),
                                    Value = row["Value"].ToString()
                });
                
            }
            return listContent;
        }





        public int addContact(SODAwcfService.Contact contact)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return contactTableAdapter.Insert(contact.Name, contact.Company, contact.Phone, contact.Email, contact.Postcode, contact.Message);
        }
    }
}
