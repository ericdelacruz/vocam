﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICMS_Service" in both code and config file together.
    [ServiceContract]
    public interface ICMS_Service
    {

        
        [OperationContract]
        int addContent(Models.ContentDef contentdef);

        [OperationContract]
        int UpdateContent(Models.ContentDef contentdef);
        
        //[OperationContract]
        //DataTable getContent(string PageCode, string sectionName);
        [OperationContract]
        IEnumerable<Models.ContentDef> getContent(string PageCode, string sectionName);

        [OperationContract]
        int deleteContent(string PageCode, string sectionName);

        [OperationContract]
        bool Authenticate(string Password);

        [OperationContract]
        int addContact(Models.Contact contact);
        [OperationContract]
        IEnumerable<Models.Contact> getContact();
        [OperationContract]
        int updateContact(Models.Contact contact);
        [OperationContract]
        int deleteContact(int id);

        [OperationContract]
        int addfreePPT(Models.FreePPT freeppt);
        [OperationContract]
        IEnumerable<Models.FreePPT> getFreePPT();
        [OperationContract]
        int updateFreePPT(Models.FreePPT freeppt);
        [OperationContract]
        int deleteFreePPT(int id);

        [OperationContract]
        IEnumerable<Models.Region> getRegions();

    }
}
