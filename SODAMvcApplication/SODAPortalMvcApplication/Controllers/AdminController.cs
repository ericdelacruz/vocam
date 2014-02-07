using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace SODAPortalMvcApplication.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private AccountServiceRef.AccountServiceClient account = new AccountServiceRef.AccountServiceClient();
        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
     
        public ActionResult Index(string df,string dt,string sc,int? page)
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            else
            {
                
                var reportlist = from customer in portalClient.getCustomer()
                                 join accnt in account.getAccount("") on customer.UserId equals accnt.Id
                                 join salescode in portalClient.getSaleCode() on customer.SalesCodeId equals salescode.Id
                                 join contract in portalClient.getCustomerContract() on customer.UserId equals contract.UserId
                                 orderby customer.DatePurchase descending
                                 select new ViewModel.ReportViewModel() { account = accnt, customer = customer,salesCode = salescode,DateContractEnd = contract.DateEnd};

                if(!string.IsNullOrEmpty(df) && !string.IsNullOrEmpty(dt))
                {
                    reportlist = from report in reportlist
                                 where report.customer.DatePurchase >= DateTime.Parse(df) && report.customer.DatePurchase <= DateTime.Parse(dt)
                                 select report;

                }
                else if(!string.IsNullOrEmpty(df))
                {
                    reportlist = from report in reportlist
                                 where report.customer.DatePurchase == DateTime.Parse(df)
                                 select report;
                }

                if(!string.IsNullOrEmpty(sc))
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
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            
            base.Initialize(requestContext);
           
        }
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (requestContext.HttpContext.Session["Username"] == null)
                requestContext.HttpContext.Response.Redirect("~/home");
            return base.BeginExecute(requestContext, callback, state);

           
           
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            
                return base.BeginExecuteCore(callback, state);
        }
        protected override void Dispose(bool disposing)
        {
            account.Close();
            portalClient.Close();
            base.Dispose(disposing);
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
                                join region in portalClient.getRegion() on salesperson.RegionId equals region.Id
                                select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson, salesCode = sc,region=region };

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
                AccountServiceRef.Account account_New = addSalesInAccount(collection);

                var new_sales_person = new PortalServiceReference.SalesPerson()
                {
                    UserId = account_New.Id,
                    //Sales_Code = collection["SalesCode"],
                    SalesCodeId = long.Parse(collection["SalesCode"]),
                    RegionId = int.Parse(collection["Region"])
                };
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_sales_person, portalClient);
                portalClient.addSalesPerson(new_sales_person);

                updateSalesCode(collection, account_New);
               
                emailSales(collection);
                return RedirectToAction("Sales");
            }
            else
            {
                //error username/email existing 
               return RedirectToAction("addSale");
            }
            
        }

        private void updateSalesCode(FormCollection collection, AccountServiceRef.Account account_New)
        {
            var salesCode = from salecode in portalClient.getSaleCode()
                            where salecode.Id == long.Parse(collection["SalesCode"])
                            select salecode;

            var salesPerson = from salesperson in portalClient.getSalePerson()
                              where salesperson.UserId == account_New.Id && salesperson.SalesCodeId == long.Parse(collection["SalesCode"])
                              select salesperson;
            var new_SalesCode = new PortalServiceReference.SalesCode()
            {

                Id = salesCode.First().Id,
                Discount = decimal.Parse(collection["Discount"]) / 100,
                DateCreated = DateTime.Now,
                SalesPersonID = salesPerson.First().Id,
                Sales_Code = salesCode.First().Sales_Code,
                DateEnd = salesCode.First().DateEnd
            };
            AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_SalesCode, portalClient);
            portalClient.updateSalsCode(new_SalesCode);
        }

        private AccountServiceRef.Account addSalesInAccount(FormCollection collection)
        {
            var new_account = new AccountServiceRef.Account()
            {
                USERNAME = collection["Email"],
                PASSWORD = collection["Password"].Split(',')[0],
                LastName = collection["LastName"],
                FirstName = collection["FirstName"],
                Email = collection["Email"],
                ContactNo = collection["ContactNo"],
                Company = collection["Company"],
                Role = 2,
                Status = 0
            };
            AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_account, portalClient);
            account.addAccount(new_account);
            AccountServiceRef.Account account_New = account.getAccount(collection["Email"].ToString()).First();
            return account_New;
        }

        private void emailSales(FormCollection collection)
        {
            string strSalesName = collection["FirstName"] + " " + collection["LastName"];
            string strPassword = collection["Password"].Split(',')[0];
            string SalesCodeId = collection["SalesCode"];
            var salesCode = portalClient.getSaleCode().Where(sc => sc.Id == long.Parse(SalesCodeId)).First();

            ViewData.Add("SalesName", strSalesName);
            ViewData.Add("Password", strPassword);
            ViewData.Add("SalesCode", salesCode.Sales_Code);
            string from = Request.Url.Host != "localhost"? "no-reply" + Request.Url.Host.Replace("portal.", "@"):"no-reply@safeyondemand.com.au";
            string to = collection["Email"].ToString();
            string body = EmailHelper.ToHtml("emailsales_new_account", ViewData, this.ControllerContext);
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(to), "Sales Account", body, true, null);
        }

        //private void emailSales(string to)
        //{
        //    EmailHelper.SendEmail("test@sac-iis.com", to, "Verification Email", "Please click the link to process. " + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "sales", new { code = EncDec.EncryptData(to) }));
        //}

        public ActionResult editsale(long id)
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;

            
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    where salesCode.SalesPersonID == -1
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

            //PortalServiceReference.SalesCode sc = portalClient.getSaleCode().Where(model => model.Id == salesPerson_orig.First().SalesCodeId).First();
            //sc.Discount = decimal.Parse(collection["Discount"]) / 100;
            //portalClient.updateSalsCode(sc);
            
            //portalClient.updateSalsCode(new PortalServiceReference.SalesCode()
            //{
            //    Id = long.Parse(collection["SalesCode"]),
                
            //    Discount = decimal.Parse(collection["Discount"])/100,
            //    DateCreated = DateTime.Now,
            //    SalesPersonID = salesPerson_orig.First().Id,
            //    Sales_Code =  
            //});
           
            long Salescodeid = 0;
            
                var salesperson = (portalClient.getSalePerson().Where(sp => sp.Id == id)).First();
                if (long.TryParse(collection["SalesCode"], out Salescodeid) && Salescodeid > 0)
                {
                    var salescode_new = portalClient.getSaleCode().Where(sc => sc.Id == Salescodeid).First();
                    salescode_new.SalesPersonID = salesperson.Id;
                    salescode_new.Discount = decimal.Parse(collection["Discount"]) / 100;
                    portalClient.updateSalsCode(salescode_new);

                    salesperson.SalesCodeId = Salescodeid;
                    portalClient.updateSalesPerson(salesperson);

                    

                }
                else
                {
                    var salescode_new = portalClient.getSaleCode().Where(sc => sc.Id == salesperson.SalesCodeId).First();
                    //salescode_new.SalesPersonID = salesperson.Id;
                    salescode_new.Discount = decimal.Parse(collection["Discount"]) / 100;
                    portalClient.updateSalsCode(salescode_new);

                    salesperson.RegionId = int.Parse(collection["Region"]);
                    portalClient.updateSalesPerson(salesperson);
             
                }
            //var salesperson = portalClient.getSalePerson().Where(sp=>sp.)
            return RedirectToAction("sales");
        }

        public ActionResult deletesale(int id)
        {

            UpdateSalesCodeSalepersonIdToNull(id);
            var old_sp = portalClient.getSalePerson().Where(sp => sp.Id == id).First();
            AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_sp, portalClient);
            portalClient.deleteSalePerson(id);
            return RedirectToAction("sales");
        }

        private void UpdateSalesCodeSalepersonIdToNull(int id)
        {
            var orig_SalesCode = portalClient.getSaleCode().Where(sc => sc.SalesPersonID == id).FirstOrDefault();
            orig_SalesCode.SalesPersonID = null;
            portalClient.updateSalsCode(orig_SalesCode);                 
                                 

            
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
            if (priceList.Count() == 0)
            {
                var price = new PortalServiceReference.Price()
                    {

                        RegionId = int.Parse(collection["Region"]),
                        PriceAmt = decimal.Parse(collection["Price"]),
                        PriceAmt_B = decimal.Parse(collection["Price3"]),
                        priceAmt_C = decimal.Parse(collection["Price6"]),
                        FirstMonthFree = collection["monthFree"] == "Yes"
                    };
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), price, portalClient);
                portalClient.addPrice(price);
            }
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
            var old_price = portalClient.getPrice().Where(p=>p.Id == id).First();
            var new_price = new PortalServiceReference.Price()
            {
                Id = id,
                //RegionName = collection["Region"],
                RegionId = int.Parse(collection["Region"]),
                PriceAmt = decimal.Parse(collection["Price"]),
                FirstMonthFree = collection["monthFree"] == "Yes",
                PriceAmt_B = decimal.Parse(collection["PriceB"].ToString()),
                priceAmt_C = decimal.Parse(collection["PriceC"].ToString()),
               
                Active = true

            };
           
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), new_price, old_price, portalClient);

            portalClient.updatePrice(new_price);
            return RedirectToAction("price");
        }
        public ActionResult deleteprice(int id)
        {

            var old_price = portalClient.getPrice().Where(p=>p.Id==id).FirstOrDefault();
            AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_price, portalClient);
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
            if (TempData["SalesPersonExists"] != null)
                ViewBag.SalesPersonExists = TempData["SalesPersonExists"];
            return View(SalesCodeList);
        }

        
        public ActionResult addsalescode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addsalescode(FormCollection collection)
        {
            if (portalClient.getSaleCode().Select(sales => sales).Where(sales => sales.Sales_Code.ToLower() == collection["SalesCode"].ToLower()).Count() == 0)
            {
                var new_sc = new PortalServiceReference.SalesCode()
                    {
                        DateCreated = DateTime.Now,
                        Discount = 0,
                        Sales_Code = collection["SalesCode"]
                    };
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_sc, portalClient);
                portalClient.addSalesCode(new_sc);
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
            var new_sc = new PortalServiceReference.SalesCode()
            {
                Id = id,
                DateCreated = salesCode_orig.First().DateCreated,
                Discount = salesCode_orig.First().Discount,
                SalesPersonID = salesCode_orig.First().SalesPersonID,
                DateEnd = salesCode_orig.First().DateEnd,

                Sales_Code = collection["SalesCode"]
            };
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), salesCode_orig.First(), new_sc, portalClient);
            portalClient.updateSalsCode(new_sc);

            return RedirectToAction("salescode");
        }
        
        public ActionResult deletesalescode(int id)
        {
            
            if (!hasSalesCodeinSP(id))
            {
                var old_sc = portalClient.getSaleCode().Where(sc => sc.Id == id).FirstOrDefault();
                AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_sc, portalClient);
                portalClient.deleteSalesCode(id);
            }
            else
            {
                //error: salescode currently assigned to error
                
                RedirectToAction("salescode");
            }
            return RedirectToAction("salescode");
        }

        private bool hasSalesCodeinSP(int salescodeid)
        {
            if (TempData["SalesPersonExists"] != null)
                TempData["SalesPersonExists"] = null;

            var salesperson = from sp in portalClient.getSalePerson()
                              where sp.SalesCodeId == salescodeid
                              select sp;

            if (salesperson.Count() > 0)
                TempData["SalesPersonExists"] = "Can't delete because it is currently assigned to sales person with ID=" + salesperson.First().Id;

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
            initAddRegionViewBag();
            return View();
        }

        private void initAddRegionViewBag()
        {
            var currencyList = portalClient.getPayPalCurrencies();
            var SalesCodeList = portalClient.getSaleCode();
            ViewBag.CurrencyList = currencyList.Select(c => new SelectListItem()
            {
                Text = c,
                Value = c,
                Selected = false
            });
            ViewBag.SalesCodeList = SalesCodeList.Select(sc => new SelectListItem()
            {
                Text = sc.Sales_Code,
                Value = sc.Id.ToString(),
                Selected = false
            });
        }
        [HttpPost]
        public ActionResult addregion(FormCollection collection)
        {
            if (portalClient.getRegion().Select(r => r).Where(r => r.RegionName == collection["RegionName"]).Count() == 0)
            {
                string strAirPlayerFileName = Request.Files.Count > 0? Request.Files["air"].FileName:"#";
                if (!strAirPlayerFileName.Contains(".air") && !strAirPlayerFileName.Contains(".zip"))
                {
                    ModelState.AddModelError("", "Invalid file. Please choose a file with a .air or.zip extension");
                    initAddRegionViewBag();
                    return View();
                }
                try
                {
                    FileTransferHelper.UploadFile(Request.Files["air"], Server);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "File was not uploaded successfully. Please try again...");
                    initAddRegionViewBag();
                    return View();
                }

                var new_region = new PortalServiceReference.Region()
                {
                    RegionName = collection["RegionName"],
                    Currency = collection["currency"],
                    WebsiteUrl = collection["siteUrl"],
                    PayPalUserName = collection["usernamePpUser"],
                    PayPalPassword = collection["passwordPpUser"],
                    PayPalSignature = collection["signaturePpUser"],
                    AirPlayerFileName = strAirPlayerFileName,
                    DefaultSalesCodeId = long.Parse(collection["defaultSalesCode"]),
                    ServiceChargeCode = collection["serviceChargeCode"]

                };
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_region, portalClient);
                portalClient.addRegion(new_region);
                

            }
            else
            {
                ModelState.AddModelError("", "Region Name already exists");
                initAddRegionViewBag();
                return View();
            }
            return RedirectToAction("region");
        }

        public ActionResult editregion(int id)
        {
            var region = initEditRegionViewBagData(id);
            return View(region);
        }

        private PortalServiceReference.Region initEditRegionViewBagData(int id)
        {
            var region = portalClient.getRegion().Select(r => r).Where(r => r.Id == id).First();
            var currencyList = portalClient.getPayPalCurrencies();
            var SalesCodeList = portalClient.getSaleCode();
            ViewBag.CurrencyList = currencyList.Select(c => new SelectListItem()
            {
                Text = c,
                Value = c,
                Selected = c == region.Currency
            });
            ViewBag.SalesCodeList = SalesCodeList.Select(sc => new SelectListItem()
            {
                Text = sc.Sales_Code,
                Value = sc.Id.ToString(),
                Selected = sc.Id == region.DefaultSalesCodeId
            });
            return region;
        }

        [HttpPost]
        public ActionResult editregion(int id, FormCollection collection)
        {
            var existing_Region = from region in portalClient.getRegion()
                                  where region.Id != id && region.RegionName == collection["RegionName"]
                                  select region;
            
            if(existing_Region.Count() == 0)
            {
              
                var orig_Region = portalClient.getRegion().Where(r => r.Id == id).First();
                    
                    orig_Region.RegionName = collection["RegionName"];
                    orig_Region.Currency = collection["currency"];
                    orig_Region.WebsiteUrl = collection["siteUrl"];
                    orig_Region.PayPalUserName = collection["usernamePpUser"];
                    orig_Region.PayPalPassword = collection["passwordPpUser"];
                    orig_Region.PayPalSignature = collection["signaturePpUser"];
                    orig_Region.DefaultSalesCodeId = long.Parse(collection["defaultSalesCode"]);
                    orig_Region.ServiceChargeCode = collection["serviceChargeCode"];
                    if (Request.Files["air"].ContentLength > 0)
                    {
                        string strAirPlayerFileName = Request.Files["air"].FileName;
                        if (!strAirPlayerFileName.Contains(".air") && !strAirPlayerFileName.Contains(".zip"))
                        {
                            ModelState.AddModelError("", "Invalid file. Please choose a file with a .air or .zip extension");
                            var region = initEditRegionViewBagData(id);
                            return View(region);
                        }
                        try
                        {
                            FileTransferHelper.UploadFile(Request.Files["air"], Server,orig_Region.AirPlayerFileName);
                            orig_Region.AirPlayerFileName = strAirPlayerFileName;
                        }
                        catch
                        {
                            ModelState.AddModelError("", "Upload File failed. Please try again.");
                            var region = initEditRegionViewBagData(id);
                            return View(region);
                        }
                    }
                    
                    portalClient.updateRegion(orig_Region);
               
            }
            else
            {
                ModelState.AddModelError("", "Region Name already exists");
                var region = initEditRegionViewBagData(id);
                return View(region);
            }
            return RedirectToAction("region");
        }

      
        

        public ActionResult deleteregion(int id)
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            var sales = portalClient.getSalePerson().Select(sp=>sp).Where(sp=>sp.RegionId == id );
            var price = portalClient.getPrice().Select(p => p).Where(p => p.RegionId == id);
            if(sales.Count() > 0)
            {
                TempData["delRegionError"] = "Cannot delete region because it is currently assigned to a Sales Person (id:" + sales.First().Id;
            }
            else if (price.Count() > 0)
            {
                TempData["delRegionError"] = "Cannot delete region because it is currently assigned to a Price (id:" + price.First().Id.ToString();
            }
            else
            {
                var old_region = portalClient.getRegion().Where(r => r.Id == id).First();
                AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_region, portalClient);
                portalClient.deleteRegion(id);
            }
            return RedirectToAction("region");
        }
        #endregion  

        #region Marketer
        public ActionResult marketer()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            var marketerList = from market in account.getAccount("")
                               where market.Role == 1
                               select market;
            return View(marketerList);
        }
        public ActionResult addmarketer()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addmarketer(FormCollection collection)
        {
            
           var user = from accnt in account.getAccount(collection["Email"].Trim())
                      select accnt;
           if (user.Count() == 0)
           {
               var new_account = new AccountServiceRef.Account()
            {
                USERNAME = collection["Email"],
                PASSWORD = collection["Password"],
                LastName = collection["LastName"],
                FirstName = collection["FirstName"],
                Email = collection["Email"],
                ContactNo = collection["ContactNo"],
                Company = collection["Company"],
                //Birthdate =default(DateTime),
                Address = collection["Address"],
                Role = 1,
                Status = 0
            };
               
               AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_account, portalClient);
               
               account.addAccount(new_account);
           }
           else
           {
               ModelState.AddModelError("", "Email already existing");
               return View(collection);
           }
            return RedirectToAction("marketer");
        }

        public ActionResult editMarketer(long id)
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
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
            //DateTime birthdate = DateTime.Parse(collection["yyyy"] + "-" + collection["mm"] + "-" + collection["dd"]);
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }

            var marAccnt = from accnt in account.getAccount("")
                           where accnt.Role == 1 && accnt.Id == id
                           select accnt;
            if (marAccnt.Count() > 0)
            {
                var new_account = new AccountServiceRef.Account()
                {
                    USERNAME = collection["Email"],
                    PASSWORD = marAccnt.First().PASSWORD,
                    LastName = collection["LastName"],
                    FirstName = collection["FirstName"],
                    Email = collection["Email"],
                    ContactNo = collection["ContactNo"],
                    Company = collection["Company"],
                    // Birthdate = birthdate,
                    Address = collection["Address"],
                    Role = 1,
                    Status = 0,
                    Id = marAccnt.First().Id
                };
                AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), marAccnt, new_account, portalClient);
                account.updateAccount(new_account);

                return RedirectToAction("marketer");
            }
            else
                return RedirectToAction("marketer");

        }

        public ActionResult deletemarketer(long id)
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            var marAccnt = from accnt in account.getAccount("")
                           where accnt.Role == 1 && accnt.Id == id
                           select accnt;
            if (marAccnt.Count() > 0)
            {
                AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), marAccnt.First(), portalClient);
                account.deleteAccount(marAccnt.First().USERNAME);
            }
            return RedirectToAction("marketer");
        }
        #endregion
    }
}
