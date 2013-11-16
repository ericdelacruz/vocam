using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SODAPortalMvcApplication.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
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
        [HttpPost]
        public ActionResult index(DateTime start, DateTime? end)
        {
            if (end != null)
            {
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("")  on customer.Id equals accnt.Id
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
        [HttpPost]
        public ActionResult index(string salecode)
        {
            var reportlist = from customer in portalClient.getCustomer()
                             join accnt in account.getAccount("") on customer.Id equals accnt.Id
                             where customer.Sales_Code == salecode
                             select new ViewModel.ReportViewModel() { account = accnt, customer = customer };
            return View(reportlist);
        }

        #region sales
        public ActionResult sales()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            else
            {
                var salesList = from salesperson in portalClient.getSalePerson()
                                join accnt in account.getAccount("") on salesperson.UserId equals accnt.Id
                                select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson };
                return View(salesList);
            }

           
        }

        public ActionResult addsale()
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;
            
            
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    select salesCode;


            return View();
        }

        [HttpPost]
        public ActionResult addsale(FormCollection collection)
        {
            if (!account.isUserNameExists(collection["Email"]))
            {
                account.addAccount(new AccountServiceRef.Account()
                {
                    USERNAME = collection["Email"],
                    PASSWORD = collection["Password"],
                    LastName = collection["LastName"],
                    FirstName = collection["FirstName"],
                    Email = collection["Email"],
                    ContactNo = collection["ContactNo"],
                    Company = collection["Company"],
                    Role = 2,
                    Status = 0
                });
                AccountServiceRef.Account account_New = account.getAccount(collection["Email"].ToString()).First();
                
                portalClient.addSalesPerson(new PortalServiceReference.SalesPerson(){
                     UserId = account_New.Id,
                     Sales_Code = collection["SalesCode"],
                     RegionId = int.Parse(collection["Region"])
                });
                 
                var salesCode = from salecode in portalClient.getSaleCode()
                                where salecode.Sales_Code == collection["SalesCode"]
                                select salecode;

                var salesPerson = from salesperson in portalClient.getSalePerson()
                                  where salesperson.UserId == account_New.Id && salesperson.Sales_Code == collection["SalesCode"]
                                  select salesperson;

                portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
                {
                    
                    Id = salesCode.First().Id,
                    Discount = decimal.Parse(collection["Discount"]),
                    DateCreated = DateTime.Now,
                    SalesPersonID = salesPerson.First().Id, 
                    Sales_Code = salesCode.First().Sales_Code,
                    DateEnd = salesCode.First().DateEnd
                });

                return RedirectToAction("addSale");
            }
            else
            {
                //error username/email existing 
               return RedirectToAction("addSale");
            }
            
        }
        public ActionResult editsale(int id)
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region.RegionName;

            
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    select salesCode.Sales_Code;

            var salesPerson_orig = from salesperson in portalClient.getSalePerson()
                                   join accnt in account.getAccount("") on salesperson.UserId equals accnt.Id
                                   select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson };

          
            return View(salesPerson_orig);
        }
        
        [HttpPost]
        public ActionResult editsales(int id, FormCollection collection)
        {
            var salesPerson_orig = from salesPerson in portalClient.getSalePerson()
                                   where salesPerson.Id == id
                                   select salesPerson;
            AccountServiceRef.Account accnt_orig = account.getAccount("").Select(accnt => accnt).Where(accnt => accnt.Id == salesPerson_orig.First().UserId).First();

            account.updateAccount(new AccountServiceRef.Account()
            {
                Id = accnt_orig.Id,
                USERNAME = collection["Email"],
                PASSWORD = collection["Password"],
                LastName = collection["LastName"],
                FirstName = collection["FirstName"],
                Email = collection["Email"],
                ContactNo = collection["ContactNo"],
                Company = collection["Company"],
                Role = 2,
                Status = 0
            });

            portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
            {
                Id = portalClient.getSaleCode().Select(salecode => salecode).Where(sc => sc.Sales_Code == collection["SalesCode"]).First().Id,
                // Id = salesCode.First().Id,
                Discount = decimal.Parse(collection["Discount"]),
                DateCreated = DateTime.Now,
                SalesPersonID = salesPerson_orig.First().Id
            });
            return RedirectToAction("sales");
        }
        #endregion

        #region price
        public ActionResult price()
        {
            var priceList = from price in portalClient.getPrice()
                            select price;
            return View(priceList);
        }
        public ActionResult addprice()
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;
            return View();
        }
        
        [HttpPost]
        public ActionResult addprice(FormCollection collection)
        {
            portalClient.addPrice(new PortalServiceReference.Price()
            {
                RegionName = collection["Region"],
                PriceAmt = decimal.Parse(collection["Price"]),
                FirstMonthFree = collection["monthFree"] == "Yes"
            });
            return RedirectToAction("addprice");
        }
        public ActionResult editprice(int id)
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;
            var priceModel = from price in portalClient.getPrice()
                             where price.Id == id
                             select price;
            return View(priceModel.First());
        }
        [HttpPost]
        public ActionResult editprice(int id, FormCollection collection)
        {

            portalClient.updatePrice(new PortalServiceReference.Price()
            {
                Id = id,
                RegionName = collection["Region"],
                PriceAmt = decimal.Parse(collection["Price"]),
                FirstMonthFree = collection["monthFree"] == "Yes",
                Active = true

            });
            return RedirectToAction("editprice");
        }
        public ActionResult deleteprice(int id)
        {
            portalClient.deletePrice(id);
            return RedirectToAction("price");
        }
#endregion

        #region Sales Code
        public ActionResult salescode()
        {
            var SalesCodeList = from salesCode in portalClient.getSaleCode()
                                select salesCode;
            return View(SalesCodeList);
        }

        
        public ActionResult addsalescode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addsalescode(FormCollection collection)
        {
            if (portalClient.getSaleCode().Select(sales => sales).Where(sales => sales.Sales_Code == collection["SalesCode"]).Count() == 0)
            {
                portalClient.addSalesCode(new PortalServiceReference.SalesCode()
                    {
                        DateCreated = DateTime.Now,
                        Discount = 0,
                        Sales_Code = collection["SalesCode"]
                    });
                return RedirectToAction("salescode");
            }
            else
            {
                //Error sales code already exists
                return RedirectToAction("addsalescode");
            }
            
        }

        public ActionResult editsalescode(int id)
        {
            var salesCode_orig = from salesCode in portalClient.getSaleCode()
                                 where salesCode.Id == id
                                 select salesCode;

            return View(salesCode_orig.First());
        }

        [HttpPost]
        public ActionResult editsalescode(int id, FormCollection collection)
        {
            var salesCode_orig = from salesCode in portalClient.getSaleCode()
                                 where salesCode.Id == id
                                 select salesCode;
            portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
            {
                 Id = id,
                DateCreated  =salesCode_orig.First().DateCreated,
                Discount = salesCode_orig.First().Discount,
                SalesPersonID = salesCode_orig.First().SalesPersonID,
                DateEnd = salesCode_orig.First().DateEnd,
                     
                Sales_Code = collection["SalesCode"]
            });

            return RedirectToAction("salescode");
        }
        
        public ActionResult deletesalescode(int id)
        {
            portalClient.deleteSalesCode(id);
            return RedirectToAction("salescode");
        }

        private bool isUserSessionActive()
        {
            return !string.IsNullOrEmpty(Session["Username"].ToString()) && AccountHelper.isActive(Session["Username"].ToString(), account);
        }

        #endregion

        #region region
        public ActionResult region()
        {
            var regionList = from region in portalClient.getRegion()
                             select region;
            return View(regionList);
        }

        public ActionResult addRegion()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addregion(FormCollection collection)
        {
            portalClient.addRegion(new PortalServiceReference.Region(){
                 RegionName = collection["RegionName"]
            });
            return RedirectToAction("region");
        }

        public ActionResult editregion(int id)
        {
            var region = portalClient.getRegion().Select(r => r).Where(r => r.Id == id).First();

            return View(region);
        }

        [HttpPost]
        public ActionResult editregion(int id, FormCollection collection)
        {
            portalClient.updateRegion(new PortalServiceReference.Region()
            {
                Id = id,
                RegionName = collection["RegionName"]
            });
            return RedirectToAction("region");
        }

        public ActionResult deleteregion(int id)
        {
            portalClient.deleteRegion(id);
            return RedirectToAction("region");
        }
        #endregion  
    }
}
