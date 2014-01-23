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
        SodaDBDataSetTableAdapters.ResetPasswordTableAdapter resetAdapter;
        private string asdasd = EncDec.EncryptData("myS0D@P@ssw0rd");
        private bool Allowed = false;
       
        enum Roles
        {
            Admin = 0,
            Market = 1,
            Sales = 2,
            User = 3
        };
        enum AccountStatus
        {
            Suspended = -1,
            InActive = 0,
            Active = 1
            
        }
        public AccountService()
        {
            AccountsTableAdapter = new SodaDBDataSetTableAdapters.AccountsTableAdapter();
            resetAdapter = new SodaDBDataSetTableAdapters.ResetPasswordTableAdapter();
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
                return AccountsTableAdapter.Insert(account.USERNAME, EncDec.EncryptData(account.PASSWORD), account.FirstName, account.LastName, account.Role, account.Status, account.Email, account.Address,
                                                      account.City, account.Country,account.Gender.ToString(), account.ContactNo, account.Company, account.DateLogin, account.Birthdate, account.EmailVerified,account.CompanyUrl); 
            
            //return AccountsTableAdapter.InsertAccount(account.USERNAME, EncDec.EncryptData(account.PASSWORD), account.FirstName, account.LastName, account.Role, account.Status, account.Email, account.Address,
            //    account.City, account.Country, account.Gender.ToString(), account.ContactNo, account.Company, account.DateLogin,account.Birthdate);
        }
        /// <summary>
        /// Update Acccount by Username
        /// </summary>
        /// <param name="account">Account object to be updated</param>
        /// <returns></returns>
        public int updateAccount(Models.Account account)
        {
            //return AccountsTableAdapter.Update(account.USERNAME, account.PASSWORD, account.FirstName, account.LastName, account.Role, account.Status, account.Email, account.Address,
            //                                          account.City, account.Country, account.Gender.ToString(), account.ContactNo, account.Company, account.DateLogin, account.Birthdate, account.EmailVerified, account.Id
            //                                          );

            return AccountsTableAdapter.UpdateAccount(account.FirstName, account.LastName, account.Role, account.Status, account.Email, account.Address,
                account.City, account.Country, account.Gender.ToString(), account.ContactNo, account.Company, account.DateLogin, account.Birthdate, account.EmailVerified, account.USERNAME,account.CompanyUrl);
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
                if (!string.IsNullOrEmpty(UserName))
                    AccountsTableAdapter.FillByUserName(tbResult, UserName);
                else
                    AccountsTableAdapter.Fill(tbResult);
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
                    Gender =  row["Gender"].ToString() == ""?'0': row["Gender"].ToString().ToCharArray()[0],
                    ContactNo = row["ContactNo"].ToString(),
                    Company = row["Company"].ToString(),
                    //DateLogin = row["DateLogin"].ToString() == "" ? default(DateTime) : (DateTime)row["DateLogin"],
                    DateLogin = row["DateLogin"].ToString() == "" ? DateTime.Parse("1/1/1753 12:00:00 AM") : (DateTime)row["DateLogin"],
                    Id = (long) row["Id"],
                    Birthdate = row["Birthdate"].ToString()=="" ? DateTime.Parse("1/1/1753 12:00:00 AM"): (DateTime)row["Birthdate"],
                    EmailVerified =(bool) row["EmailVerify"],
                    CompanyUrl = row["CompanyUrl"].ToString(),
                    Country = row["Country"].ToString()
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
            if (isUserNameExists(UserName))
            {
                string decPassword = EncDec.DecryptString(getAccount(UserName).First().PASSWORD);

                if (decPassword.CompareTo(Password) == 0)
                {
                    UpdateStatus(UserName, (short)AccountStatus.Active);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        
        public bool isUserNameExists(string UserName)
        {
            return getAccount(UserName).Count() > 0;
        }
        
        public void LogOff(string UserName)
        {
            if( isUserNameExists(UserName))
            {
                UpdateStatus(UserName, (short)AccountStatus.InActive);
            }
        }
        private void UpdateStatus(string Username, short status)
        {
            AccountsTableAdapter.UpdateAccountStatus(status, Username);
        }




       


        public int addResetPassword(string key, DateTime dateSent, DateTime dateEx, long userid)
        {
            return resetAdapter.Insert(key, dateSent, dateEx, false,userid);
        }

        public IEnumerable<Models.ResetPassword> getRestPassword()
        {
            return resetAdapter.GetData().Select(rp => new Models.ResetPassword()
            {
                Id = rp.Id,
                key = rp.key,
                dateSent = rp.DateSent,
                dateEx = rp.DateEx,
                isVerified = rp.isVerified,
                UserId =  rp.UserId
            });
        }

        public int updateResetPassword(Models.ResetPassword resetPass)
        {
            return resetAdapter.Update(resetPass.key, resetPass.dateSent, resetPass.dateEx, resetPass.isVerified,resetPass.UserId, resetPass.Id);
        }

        public int deleteResetPassword(int id)
        {
            return resetAdapter.Delete(id);
        }


        public bool updatePassword(long userID, string orig_Enc_pass, string new_pass)
        {
            var accnt = AccountsTableAdapter.GetData().Where(a => a.Id == userID && a.PASSWORD == orig_Enc_pass);
            if (accnt.Count() > 0)
            {
                var accntUser = accnt.First();
                accntUser.PASSWORD = EncDec.EncryptData(new_pass);
                AccountsTableAdapter.Update(accntUser);
                return true;
            }
            else
                return false;
        }
    }
}
