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
        private SodaDBDataSetTableAdapters.FreePPTFileNamesTableAdapter freePPTAdapter;
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
            freePPTAdapter = new SodaDBDataSetTableAdapters.FreePPTFileNamesTableAdapter();
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
            return CMSTableAdapter.UpdateContent(contedef_new.Type, contedef_new.Value, contedef_new.RegionId, contedef_new.PageCode, contedef_new.SectionName);
           
            
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
                                    Value = row["Value"].ToString(),
                                    RegionId = int.Parse(row["RegionId"].ToString())
                });
                
            }
            return listContent;
        }





        public int addContact(Models.Contact contact)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return contactTableAdapter.Insert(contact.Name, contact.Company, contact.Phone, contact.Email, contact.Postcode, contact.Message,contact.isFreePPT,contact.key, contact.DateLinkEx,contact.isVerified);
        }



        public IEnumerable<Models.Contact> getContact()
        {

            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return contactTableAdapter.GetData().Select(c => new Models.Contact()
            {
                Id = c.Id,
                Company = c.Company,
                Postcode = c.Postcode,
                Phone = c.Phone,
                Name = c.Name,
                Message = c.Message,
                key = c.key,
                DateLinkEx = c.DateLinkEx,
                Email = c.Email,
                isFreePPT = c.isFreePPT,
                isVerified = c.isVerified
            });
        }

        public int updateContact(Models.Contact contact)
        {

            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return contactTableAdapter.Update(contact.Name, contact.Company, contact.Phone, contact.Email, contact.Postcode, contact.Message, contact.isFreePPT, contact.key, contact.DateLinkEx, contact.isVerified, contact.Id);
        }

        public int deleteContact(int id)
        {

            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return contactTableAdapter.Delete(id);
        }

        public int addfreePPT(Models.FreePPT freeppt)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return freePPTAdapter.Insert(freeppt.FileName, freeppt.PPTType, freeppt.RegionId,freeppt.DisplayText);
        }
        public IEnumerable<Models.FreePPT> getFreePPT()
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return freePPTAdapter.GetData().Select(fppt => new Models.FreePPT()
            {
                Id = fppt.Id,
                RegionId = fppt.RegionId,
                FileName = fppt.FileName,
                PPTType = fppt.PPTType,
                 DisplayText = fppt.DisplayText
            });
        }
        public int updateFreePPT(Models.FreePPT freeppt)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return freePPTAdapter.Update(freeppt.FileName, freeppt.PPTType, freeppt.RegionId, freeppt.DisplayText, freeppt.Id);
        }
        public int deleteFreePPT(int id)
        {
            if (!Allowed)
                throw (new FaultException("Access Denied!!!", new FaultCode("AccessDenied")));
            return freePPTAdapter.Delete(id);
        }
    }
}
