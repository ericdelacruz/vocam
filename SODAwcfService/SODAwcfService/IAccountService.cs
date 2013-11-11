using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAccountService" in both code and config file together.
    //Self Note: Double check model for data contract attribute
    [ServiceContract]
    public interface IAccountService
    {
        [OperationContract]
        int addAccount(Models.Account account);
        [OperationContract]
        int updateAccount(Models.Account Account);

        [OperationContract]
        IEnumerable<Models.Account> getAccount(string UserName);

        [OperationContract]
        int deleteAccount(string UserName);

        [OperationContract]
        bool AuthenticateUser(string UserName, string Password);

        [OperationContract]
        bool isUserNameExists(string UserName);

        [OperationContract]
        bool sendEmailForPassword(Models.Account account);

        [OperationContract]
        bool Authenticate(string Password);

    }
}
