using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AccountService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AccountService.svc or AccountService.svc.cs at the Solution Explorer and start debugging.
    public class AccountService : IAccountService
    {
        SodaDBDataSetTableAdapters.AccountsTableAdapter AccountsTableAdapter;
        private string asdasd = EncDec.EncryptData("myS0D@P@ssw0rd");
        private bool Allowed = false;
        public AccountService()
        {
            AccountsTableAdapter = new SodaDBDataSetTableAdapters.AccountsTableAdapter();
            
        }

        public bool Authenticate(string Password)
        {
            return Allowed = EncDec.DecryptString(asdasd).CompareTo(Password) == 0;
                

        }
        /// <summary>
        /// Inserts Account to table
        /// </summary>
        /// <param name="account">Account Object to be inserted</param>
        /// <returns>Number of rows added</returns>
        public int addAccount(Models.Account account)
        {
            if (isUserNameExists(account.USERNAME))
                throw new FaultException("UserName already existing", new FaultCode("UserExists"));
            else
            return AccountsTableAdapter.InsertAccount(account.USERNAME, account.PASSWORD, account.FirstName, account.LastName, account.Role, account.Status, account.Email, account.Address,
                account.City, account.Country, account.Gender, account.ContactNo, account.Company, account.ContractEndDate);
        }
        /// <summary>
        /// Update Acccount by Username
        /// </summary>
        /// <param name="account">Account object to be updated</param>
        /// <returns></returns>
        public int updateAccount(Models.Account account)
        {
            return AccountsTableAdapter.UpdateAccount(account.FirstName, account.LastName, account.Role, account.Status, account.Email, account.Address,
                account.City, account.Country, account.Gender, account.ContactNo, account.Company, account.ContractEndDate, account.USERNAME);
        }
        /// <summary>
        /// Get Account record
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public IEnumerable<Models.Account> getAccount(string UserName)
        {
            List<Models.Account> listAccounts = new List<Models.Account>();
            SodaDBDataSet.AccountsDataTable tbResult = new SodaDBDataSet.AccountsDataTable();
            try
            {
                AccountsTableAdapter.FillByUserName(tbResult, UserName);
            }
            catch(Exception ex)
            {
                throw (new FaultException("DB", new FaultCode("DB")));
            }

            foreach(DataRow row in tbResult.Rows)
            {
                listAccounts.Add(new Models.Account()
                {
                    USERNAME = row["USERNAME"].ToString(),
                    PASSWORD = row["PASSWORD"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    Role = (int)row["Role"],
                    Status = (short)row["Status"],
                    Email = row["Email"].ToString(),
                    Address = row["Address"].ToString(),
                    City = row["City"].ToString(),
                    Gender = row["Gender"].ToString(),
                    ContactNo = row["ContactNo"].ToString(),
                    Company = row["Company"].ToString(),
                    ContractEndDate = (DateTime)row["ContractEndDate"]
                });
            }
            return listAccounts;
        }
        /// <summary>
        /// Delete Account record. 
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns>Rows Affected</returns>
        public int deleteAccount(string UserName)
        {
            return  AccountsTableAdapter.DeleteAccount(UserName);
        }

        public bool AuthenticateUser(string UserName, string Password)
        {
            throw new NotImplementedException();
        }

        public bool isUserNameExists(string UserName)
        {
            return getAccount(UserName).Count() > 0;
        }

        public bool sendEmailForPassword(Models.Account account)
        {
            throw new NotImplementedException();
        }
    }
}
