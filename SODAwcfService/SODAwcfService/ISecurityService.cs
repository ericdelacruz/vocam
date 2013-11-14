using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISecurityService" in both code and config file together.
    [ServiceContract]
    public interface ISecurityService
    {
        [OperationContract]
        bool InitDB();
        [OperationContract]
        bool Login(string Username, string Password, bool remember);
        [OperationContract]
        void LogOff();
        [OperationContract]
        bool Register(string UserName, string Password);
        //[OperationContract]
        //bool Register(Models.Account account);



        
    }
}
