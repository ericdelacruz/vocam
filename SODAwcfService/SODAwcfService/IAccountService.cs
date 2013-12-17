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
        int addResetPassword(string key, DateTime dateSent, DateTime dateEx, long UserId);

        [OperationContract]
        IEnumerable<Models.ResetPassword> getRestPassword();
        [OperationContract]
        int updateResetPassword(Models.ResetPassword resetPass);
        [OperationContract]
        int deleteResetPassword(int id);
        [OperationContract]
        bool Authenticate(string Password);

        [OperationContract]
        void LogOff(string UserName);
        [OperationContract]
        bool updatePassword(long userID, string orig_pass, string new_pass);
        

    }
}
