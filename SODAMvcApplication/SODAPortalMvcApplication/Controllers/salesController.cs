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
        [RequireHttps]
        public ActionResult Index(string df, string dt, string sc, int? page)
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            else
            {

                var reportlist = from customer in portalClient.getCustomer()  
                                 join salescode in portalClient.getSaleCode() on customer.SalesCodeId equals salescode.Id
                                 join salesperson in portalClient.getSalePerson() on salescode.SalesPersonID equals salesperson.Id
                                 join cust_accnt in account.getAccount("") on customer.UserId equals cust_accnt.Id 
                                 join contract in portalClient.getCustomerContract() on customer.UserId equals contract.UserId
                                 join sales_accnt in account.getAccount(Session["Username"].ToString()) on salesperson.UserId equals sales_accnt.Id
                                 orderby customer.DatePurchase descending
                                  
                                 select new ViewModel.ReportViewModel() { account = cust_accnt, customer = customer, salesCode = salescode,DateContractEnd=contract.DateEnd };

                if (!string.IsNullOrEmpty(df) && !string.IsNullOrEmpty(dt))
                {
                    reportlist = from report in reportlist
                                 where report.customer.DatePurchase >= DateTime.Parse(df) && report.customer.DatePurchase <= DateTime.Parse(dt)
                                 select report;

                }
                else if (!string.IsNullOrEmpty(df))
                {
                    reportlist = from report in reportlist
                                 where report.customer.DatePurchase == DateTime.Parse(df)
                                 select report;
                }

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
        [HttpPost]
        [RequireHttps]
        public ActionResult index(FormCollection collection)
        {
            DateTime dtstart = DateTime.Parse(collection["start"]);
            DateTime dtend = collection["end"] != "" ? DateTime.Parse(collection["end"]) : default(DateTime);
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            if (collection["end"] != null && collection["end"] != "")
            {
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount(Session["Username"].ToString()) on customer.Id equals accnt.Id
                                 join sc in portalClient.getSaleCode() on customer.SalesCodeId equals sc.Id
                                 join sp in portalClient.getSalePerson() on customer.SalesCodeId equals sp.SalesCodeId
                                 where customer.DatePurchase.Value.DayOfYear >= dtstart.DayOfYear && customer.DatePurchase.Value.DayOfYear < dtend.DayOfYear 
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer,salesCode = sc };
                return View(reportlist);
            }
            else
            {
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount(Session["Username"].ToString()) on customer.Id equals accnt.Id
                                 join sc in portalClient.getSaleCode() on customer.SalesCodeId equals sc.Id
                                 join sp in portalClient.getSalePerson() on customer.SalesCodeId equals sp.SalesCodeId
                                 where customer.DatePurchase.Value.DayOfYear == dtstart.DayOfYear //&& sp.UserId == accnt.Id
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer, salesCode =sc };
                return View(reportlist);
            }
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
                    AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_user, portalClient);
                    account.addAccount(new_user);

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

            EmailHelper.SendEmail("test@sac-iis.com", model.Email, "Verification", "Click the link to continue." + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "home", new { code = EncDec.EncryptData(model.Email) }));
        }
        private bool isUserSessionActive()
        {
            return !string.IsNullOrEmpty(Session["Username"].ToString()) && AccountHelper.isActive(Session["Username"].ToString(), account);
        }
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
