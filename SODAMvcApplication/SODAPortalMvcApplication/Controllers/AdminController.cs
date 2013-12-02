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
                                 join accnt in account.getAccount("") on customer.UserId equals accnt.Id
                                 join sc in portalClient.getSaleCode() on customer.SalesCodeId equals sc.Id
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer,salesCode = sc };
                return View(reportlist);
            }
            
        }
        //[HttpPost]
        //public ActionResult index(FormCollection collection)
        //{
        //    DateTime start = DateTime.Parse(collection["datefrom"]);
        //    DateTime? end = new Nullable<DateTime>();
        //    if (collection["dateto"] != "")
        //        end = DateTime.Parse(collection["dateto"]);

        //    if (end.HasValue)
        //    {
        //        var reportlist = from customer in portalClient.getCustomer()
        //                         join accnt in account.getAccount("") on customer.UserId equals accnt.Id
        //                         where customer.DatePurchase >= start && customer.DatePurchase < end
        //                         select new ViewModel.ReportViewModel() { account = accnt, customer = customer };
        //        return View(reportlist);
        //    }
        //    else
        //    {
        //        var reportlist = from customer in portalClient.getCustomer()
        //                         join accnt in account.getAccount("") on customer.UserId equals accnt.Id
        //                         where customer.DatePurchase == start
        //                         select new ViewModel.ReportViewModel() { account = accnt, customer = customer };
        //        return View(reportlist);
        //    }

        //    return View();
        //}
        [HttpPost]
        public ActionResult index(FormCollection collection)
        {
            DateTime start;
            DateTime? end = new Nullable<DateTime>();

            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }

            if ( collection["dateto"].Trim() != "" && collection["datefrom"].Trim() != "")
            {
                start = DateTime.Parse(collection["datefrom"]); 
                end = DateTime.Parse(collection["dateto"]);
                 var reportlist = from customer in portalClient.getCustomer()
                                  join accnt in account.getAccount("") on customer.UserId equals accnt.Id
                                  join sc in portalClient.getSaleCode() on customer.SalesCodeId equals sc.Id
                                 where customer.DatePurchase >= start && customer.DatePurchase < end
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer,salesCode = sc };

                return View(reportlist);
            }
            else if (collection["datefrom"].Trim() != "")
            {
                start = DateTime.Parse(collection["datefrom"]); 
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("") on customer.UserId equals accnt.Id
                                 join sc in portalClient.getSaleCode() on customer.SalesCodeId equals sc.Id
                                 where customer.DatePurchase == start 
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer, salesCode = sc };
                return View(reportlist);
            }
            else if(collection["salescode"].Trim() != "")
            {
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("") on customer.UserId equals accnt.Id
                                 join sc in portalClient.getSaleCode() on customer.SalesCodeId equals sc.Id
                                 where sc.Sales_Code == collection["salescode"]
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer, salesCode =sc };
                return View(reportlist);
            }
            else//all are empty
            {
                return RedirectToAction("index");//view all
            }
            
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
                                join sc in portalClient.getSaleCode() on salesperson.SalesCodeId equals sc.Id
                                select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson, salesCode = sc };

                return View(salesList);
            }

           
        }

        public ActionResult addsale()   
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;
            
            
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    where salesCode.SalesPersonID == -1
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
                     //Sales_Code = collection["SalesCode"],
                      SalesCodeId = long.Parse(collection["SalesCode"]),
                     RegionId = int.Parse(collection["Region"])
                });
                 
                var salesCode = from salecode in portalClient.getSaleCode()
                                where salecode.Id == long.Parse(collection["SalesCode"])
                                select salecode;

                var salesPerson = from salesperson in portalClient.getSalePerson()
                                  where salesperson.UserId == account_New.Id && salesperson.SalesCodeId == long.Parse(collection["SalesCode"])
                                  select salesperson;

                portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
                {
                    
                    Id = salesCode.First().Id,
                    Discount = decimal.Parse(collection["Discount"])/100,
                    DateCreated = DateTime.Now,
                    SalesPersonID = salesPerson.First().Id, 
                    Sales_Code = salesCode.First().Sales_Code,
                    DateEnd = salesCode.First().DateEnd
                });
                emailSales(account_New.Email);
                return RedirectToAction("Sales");
            }
            else
            {
                //error username/email existing 
               return RedirectToAction("addSale");
            }
            
        }

        private void emailSales(string to)
        {
            EmailHelper.SendEmail("test@sac-iis.com", to, "Verification Email", "Please click the link to process. " + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "sales", new { code = EncDec.EncryptData(to) }));
        }

        public ActionResult editsale(long id)
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;

            
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    where salesCode.SalesPersonID != -1
                                    select salesCode;

            var salesPerson_orig = from salesperson in portalClient.getSalePerson()
                                   join accnt in account.getAccount("") on salesperson.UserId equals accnt.Id
                                   join sc in portalClient.getSaleCode() on salesperson.SalesCodeId equals sc.Id
                                   where salesperson.Id == id
                                   select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson, salesCode = sc };

          
            return View(salesPerson_orig.First());
        }
        
        [HttpPost]
        public ActionResult editsale(int id, FormCollection collection)
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

            PortalServiceReference.SalesCode sc = portalClient.getSaleCode().Select(model=> model).Where(model => model.Id == salesPerson_orig.First().SalesCodeId).First();
            sc.Discount = decimal.Parse(collection["Discount"]) / 100;
            portalClient.updateSalsCode(sc);
            //portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
            //{
            //    Id = long.Parse(collection["SalesCode"]),
                
            //    Discount = decimal.Parse(collection["Discount"])/100,
            //    DateCreated = DateTime.Now,
            //    SalesPersonID = salesPerson_orig.First().Id,
            //    Sales_Code =  
            //});
            return RedirectToAction("sales");
        }

        public ActionResult deletesale(int id)
        {

            setSalepersonToNull(id);
            portalClient.deleteSalePerson(id);
            return RedirectToAction("sales");
        }

        private void setSalepersonToNull(int id)
        {
            var orig_SalesCode = from sc in portalClient.getSaleCode()
                             
                                 where sc.SalesPersonID == id
                                 select sc;

            if(orig_SalesCode.Count() > 0)
            portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
            {

                Id = orig_SalesCode.First().Id,
                SalesPersonID = null,
                DateCreated = orig_SalesCode.First().DateCreated,
                Sales_Code = orig_SalesCode.First().Sales_Code,
                Discount = orig_SalesCode.First().Discount,
                DateEnd = orig_SalesCode.First().DateEnd

            });
        }
        #endregion

        #region price
        public ActionResult price()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            var priceList = from p in portalClient.getPrice()
                            join r in portalClient.getRegion() on p.RegionId equals r.Id
                            select new ViewModel.PriceViewModel() { price = p, region = r };
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
            var priceList = from price in portalClient.getPrice()
                        where price.RegionId == int.Parse(collection["Region"])
                        select price;
            if(priceList.Count() == 0)
            portalClient.addPrice(new PortalServiceReference.Price()
            {
                //RegionName = collection["Region"],
                RegionId = int.Parse(collection["Region"]),
                PriceAmt = decimal.Parse(collection["Price"]),
                FirstMonthFree = collection["monthFree"] == "Yes"
            });
            else
            {
                ModelState.AddModelError("", "There is already a price assign to the selected Region");
                ViewBag.RegionList = from region in portalClient.getRegion()
                                     select region;
                return View("addprice");
            }
            return RedirectToAction("price");
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
                //RegionName = collection["Region"],
                RegionId = int.Parse(collection["Region"]),
                PriceAmt = decimal.Parse(collection["Price"]),
                FirstMonthFree = collection["monthFree"] == "Yes",
                Active = true

            });
            return RedirectToAction("price");
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
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
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
                ModelState.AddModelError("", "Sales Code already Exists");
                return View();
                //return RedirectToAction("addsalescode");
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
            if(!hasSalesCodeinSP(id))
            portalClient.deleteSalesCode(id);
            else
            {
                //error: salescode currently assigned to error
                RedirectToAction("salescode");
            }
            return RedirectToAction("salescode");
        }

        private bool hasSalesCodeinSP(int salescodeid)
        {
            var salesperson = from sp in portalClient.getSalePerson()
                              where sp.SalesCodeId == salescodeid
                              select sp;
            return salesperson.Count() > 0;
        }

        private bool isUserSessionActive()
        {
            return Session["Username"] != null;
            //try
            //{
            //    return !string.IsNullOrEmpty(Session["Username"].ToString()) && AccountHelper.isActive(Session["Username"].ToString(), account);
            //}
            //catch
            //{
            //    return false;
            //}
        }

        #endregion

        #region region
        public ActionResult region()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }

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
            if(portalClient.getRegion().Select(r => r).Where(r => r.RegionName == collection["RegionName"]).Count() ==0)
            portalClient.addRegion(new PortalServiceReference.Region(){
                 RegionName = collection["RegionName"]
            });
            else
            {
                ModelState.AddModelError("","Region Name already exists");
                return View();
            }
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
            var existing_Region = from region in portalClient.getRegion()
                                  where region.Id != id && region.RegionName == collection["RegionName"]
                                  select region;
            //test
            if(existing_Region.Count() == 0)
            portalClient.updateRegion(new PortalServiceReference.Region()
            {
                Id = id,
                RegionName = collection["RegionName"]
            });
            else
            {
                ModelState.AddModelError("", "Region Name already exists");
                var region = portalClient.getRegion().Select(r => r).Where(r => r.Id == id).First();
                return View(region);
            }
            return RedirectToAction("region");
        }

        public ActionResult deleteregion(int id)
        {
            portalClient.deleteRegion(id);
            return RedirectToAction("region");
        }
        #endregion  

        #region Marketer
        public ActionResult marketer()
        {
            var marketerList = from market in account.getAccount("")
                               where market.Role == 1
                               select market;
            return View(marketerList);
        }
        public ActionResult addmarketer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addmarketer(FormCollection collection)
        {
            DateTime birthdate = new DateTime();
            DateTime.TryParse(collection["datefrom"], out birthdate);
           var user = from accnt in account.getAccount(collection["Email"].Trim())
                      select accnt;
            if(user.Count() == 0)
            account.addAccount(new AccountServiceRef.Account()
            {
                USERNAME = collection["Email"],
                PASSWORD = collection["Password"],
                LastName = collection["LastName"],
                FirstName = collection["FirstName"],
                Email = collection["Email"],
                ContactNo = collection["ContactNo"],
                Company = collection["Company"],
                Birthdate = birthdate,
                Address = collection["Address"],
                Role = 1,
                Status = 0
            });
            else
            {
                ModelState.AddModelError("", "Email already existing");
                return View(collection);
            }
            return RedirectToAction("marketer");
        }

        public ActionResult editMarketer(long id)
        {
            var marAccnt = from accnt in account.getAccount("")
                           where accnt.Role == 1 && accnt.Id == id
                           select accnt;
            if (marAccnt.Count() > 0)
                return View(marAccnt.First());
            else
                return RedirectToAction("marketer");
        }
        [HttpPost]
        public ActionResult editmarketer(long id, FormCollection collection)
        {
            DateTime birthdate = DateTime.Parse(collection["yyyy"] + "-" + collection["mm"] + "-" + collection["dd"]);
            var marAccnt = from accnt in account.getAccount("")
                           where accnt.Role == 1 && accnt.Id == id
                           select accnt;
            if (marAccnt.Count() > 0)
            {
                account.updateAccount(new AccountServiceRef.Account()
                {
                    USERNAME = collection["Email"],
                    PASSWORD = marAccnt.First().PASSWORD,
                    LastName = collection["LastName"],
                    FirstName = collection["FirstName"],
                    Email = collection["Email"],
                    ContactNo = collection["ContactNo"],
                    Company = collection["Company"],
                    Birthdate = birthdate,
                    Address = collection["Address"],
                    Role = 1,
                    Status = 0,
                    Id = marAccnt.First().Id
                });

                return RedirectToAction("marketer");
            }
            else
                return RedirectToAction("marketer");

        }

        public ActionResult deletemarketer(long id)
        {
            var marAccnt = from accnt in account.getAccount("")
                           where accnt.Role == 1 && accnt.Id == id
                           select accnt;
            if (marAccnt.Count() > 0)
                account.deleteAccount(marAccnt.First().USERNAME);

            return RedirectToAction("marketer");
        }
        #endregion
    }
}
