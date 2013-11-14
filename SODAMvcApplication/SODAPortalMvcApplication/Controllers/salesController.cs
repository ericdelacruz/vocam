using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SODAPortalMvcApplication.Controllers
{
    public class salesController : Controller
    {
        //
        // GET: /sales/
        private AccountServiceRef.AccountServiceClient account = new AccountServiceRef.AccountServiceClient();
        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        public ActionResult Index()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            else
            {

                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("") on customer.Id equals accnt.Id
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer };
                return View(reportlist);
            }
            
        }
        public ActionResult index(DateTime start,DateTime? end)
        {
            if (end != null)
            {
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("") on customer.Id equals accnt.Id
                                 where customer.DatePurchase >= start && customer.DatePurchase < end
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer };
                return View(reportlist);
            }
            else
            {
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("") on customer.Id equals accnt.Id
                                 where customer.DatePurchase == start
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer };
                return View(reportlist);
            }
        }


        private bool isUserSessionActive()
        {
            return !string.IsNullOrEmpty(Session["Username"].ToString()) && AccountHelper.isActive(Session["Username"].ToString(), account);
        }

    }
}
