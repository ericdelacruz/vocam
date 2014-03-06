using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace SODAPortalMvcApplication.Controllers
{
    public class salesController : Controller
    {
        //
        // GET: /sales/
        private AccountServiceRef.AccountServiceClient account = new AccountServiceRef.AccountServiceClient();
        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="df">DateFrom</param>
        /// <param name="dt">DateTo</param>
        /// <param name="sc">SalesCode</param>
        /// <param name="page">Page Number</param>
        /// <returns></returns>
        [RequireHttps]
        public ActionResult Index(string df, string dt, string sc, int? page)
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            else
            {

                //list all transactions done by the customer under the assigned sales code
               
                var reportlist = from customer in portalClient.getCustomer()  
                                 join salescode in portalClient.getSaleCode() on customer.SalesCodeId equals salescode.Id
                                 join salesperson in portalClient.getSalePerson() on salescode.SalesPersonID equals salesperson.Id
                                 join cust_accnt in account.getAccount("") on customer.UserId equals cust_accnt.Id 
                                 join contract in portalClient.getCustomerContract() on customer.UserId equals contract.UserId
                                 join sales_accnt in account.getAccount(Session["Username"].ToString()) on salesperson.UserId equals sales_accnt.Id
                                 orderby customer.DatePurchase descending
                                  
                                 select new ViewModel.ReportViewModel() { account = cust_accnt, customer = customer, salesCode = salescode,DateContractEnd=contract.DateEnd };
                //filter list if datefrom add dateto is not null
                if (!string.IsNullOrEmpty(df) && !string.IsNullOrEmpty(dt))
                {
                    reportlist = from report in reportlist
                                 where report.customer.DatePurchase >= DateTime.Parse(df) && report.customer.DatePurchase <= DateTime.Parse(dt)
                                 select report;

                }
                else if (!string.IsNullOrEmpty(df))//filter if only datefrom is not null
                {
                    reportlist = from report in reportlist
                                 where report.customer.DatePurchase == DateTime.Parse(df)
                                 select report;
                }
                //filter list by salescode if sc is not null
                if (!string.IsNullOrEmpty(sc))
                {
                    reportlist = from report in reportlist
                                 where report.salesCode.Sales_Code.ToLower() == sc.ToLower().Trim()
                                 select report;
                }
                int pageSize = 10;
                int pageNumber = page ?? 1;
                ViewBag.dt = dt ?? "";
                ViewBag.df = df ?? "";
                ViewBag.sc = sc ?? "";
                return View(reportlist.ToList().ToPagedList(pageNumber, pageSize));
            }
            
        }
        protected override void Dispose(bool disposing)
        {
            account.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            //security measure. to gain access to account service
            account.Authenticate("myS0D@P@ssw0rd");
            return base.BeginExecute(requestContext, callback, state);
        }
       
         [RequireHttps]
        public ActionResult adduser()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            return View();
        }
        
        [HttpPost]
        [RequireHttps]
        public ActionResult adduser(ViewModel.UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //init new account to be added
                    var new_user = new AccountServiceRef.Account()
                    {
                        USERNAME = model.Email,
                        PASSWORD = model.Password.Split(',')[0],
                        Role = 3,
                        Status = 1,
                        Company = model.Company,
                        ContactNo = model.Contact,
                        Email = model.Email,
                        FirstName = model.FirtName,
                        LastName = model.LastName,

                    };
                    //log account to be added
                    AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_user, portalClient);
                    account.addAccount(new_user);
                    //send email to the customer
                    sendEmailVerification(model);
                    TempData["EmailSent"] = true;
                    return RedirectToAction("adduser");
                }
                catch
                {
                    ModelState.AddModelError("", "Error");
                }
            }
            return View(model);
        }
        private void sendEmailVerification(ViewModel.UserModel model)
        {

            EmailHelper.SendEmail("no-reply@safetyondemand.com.au", model.Email, "Verification", "Click the link to continue." + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "home", new { code = EncDec.EncryptData(model.Email) }));
        }
        private bool isUserSessionActive()
        {
            return !string.IsNullOrEmpty(Session["Username"].ToString()) && AccountHelper.isActive(Session["Username"].ToString(), account);
        }
         /// <summary>
         /// Verify sales account when the sales user clicks the link in the email
         /// </summary>
         /// <param name="code"></param>
         /// <returns></returns>
         [RequireHttps]
        public ActionResult verify(string code)
        {
            string username = EncDec.DecryptString(code);
            if (!string.IsNullOrEmpty(username))
            {
                var user = from accnt in account.getAccount(username)
                           select accnt;

                if (user.Count() > 0)
                {
                    AccountServiceRef.Account accnt = user.First();

                    //accnt.EmailVerified = true;

                    account.updateAccount(new AccountServiceRef.Account()
                    {
                        USERNAME = accnt.Email,
                        PASSWORD = accnt.PASSWORD,
                        Role = 2,
                        Status = 1,
                        Company = accnt.Company,
                        ContactNo = accnt.ContactNo,
                        Email = accnt.Email,
                        FirstName = accnt.FirstName,
                        LastName = accnt.LastName,
                        EmailVerified = true
                    });

                    Session.Add("Username", accnt.USERNAME);
                    return RedirectToAction("index", "sales");
                }
            }
            return RedirectToAction("index");
        }
    }
}
