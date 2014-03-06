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
        /* Index is the View reports page */ 
        [RequireHttps]
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
        
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            //if session expired redirect to home page
            if (requestContext.HttpContext.Session["Username"] == null)
                requestContext.HttpContext.Response.Redirect("~/home");
            //security measure. Account service requires to be authenticated to gain access to its methods
            account.Authenticate("myS0D@P@ssw0rd");
            return base.BeginExecute(requestContext, callback, state);

           
           
        }
    
        protected override void Dispose(bool disposing)
        {
            account.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
      
       
        
        
        #region sales
        [RequireHttps]
        public ActionResult sales()
        {
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            else
            {
                /*get salesperson list
                 * joined to accnt for the First Name and Lastname
                 * join region for the region name
                 * joined salesCode for the Sales_code
                 */ 
                var salesList = from salesperson in portalClient.getSalePerson()
                                join accnt in account.getAccount("") on salesperson.UserId equals accnt.Id
                                join sc in portalClient.getSaleCode() on salesperson.SalesCodeId equals sc.Id
                                join region in portalClient.getRegion() on salesperson.RegionId equals region.Id
                                select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson, salesCode = sc,region=region };

                return View(salesList);
            }

           
        }
        [RequireHttps]
        public ActionResult addsale()   
        {
            //get region list for the dropdown box
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;
            
            //get salescode that are unassigned
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    where salesCode.SalesPersonID == -1
                                    select salesCode;


            return View();
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult addsale(FormCollection collection)
        {
            //if email doesnt exists yet, create account and add salesperson then update the salespersonId of salescode.Sends email to the sales about the account created
            if (!account.isUserNameExists(collection["Email"]))
            {
                //add account with role set to sales then get the added row data
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
               return View();
            }
            
        }

        private void updateSalesCode(FormCollection collection, AccountServiceRef.Account account_New)
        {
            //get sales person row data to be updated
            var old_salesCode = from salecode in portalClient.getSaleCode()
                            where salecode.Id == long.Parse(collection["SalesCode"])
                            select salecode;
            //get salesperson data for the salespersonid
            var salesPerson = from salesperson in portalClient.getSalePerson()
                              where salesperson.UserId == account_New.Id && salesperson.SalesCodeId == long.Parse(collection["SalesCode"])
                              select salesperson;
            //init updated salesCode
            var new_SalesCode = new PortalServiceReference.SalesCode()
            {

                Id = old_salesCode.First().Id,
                //Discount = decimal.Parse(collection["Discount"]) / 100,
                DateCreated = DateTime.Now,
                SalesPersonID = salesPerson.First().Id,
                Sales_Code = old_salesCode.First().Sales_Code,
                DateEnd = old_salesCode.First().DateEnd,
                Discount = 0,
                Less_monthly = decimal.Parse(collection["Discountpermonth"]??"0"),
                Less_3months = decimal.Parse(collection["Discountper3months"]??"0"),
                Less_6months = decimal.Parse(collection["Discountper6months"]??"0")
            };
            
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_salesCode, new_SalesCode, portalClient);

            portalClient.updateSalsCode(new_SalesCode);
        }

        private AccountServiceRef.Account addSalesInAccount(FormCollection collection)
        {
            //initialize new account object
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
            //retrive added account
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
            //if localhost then set it to no-reply@safetyondemand. 
            string from = Request.Url.Host != "localhost"? "no-reply" + Request.Url.Host.Replace("portal.", "@"):"no-reply@safeyondemand.com.au";
            string to = collection["Email"].ToString();
            //convert emailsales_new_account.cshtml to string variable. TO view the format goto emailsales_new_account() and Goto View
            string body = EmailHelper.ToHtml("emailsales_new_account", ViewData, this.ControllerContext);
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(to), "Sales Account", body, true, null);
        }

        //private void emailSales(string to)
        //{
        //    EmailHelper.SendEmail("test@sac-iis.com", to, "Verification Email", "Please click the link to process. " + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "sales", new { code = EncDec.EncryptData(to) }));
        //}
        [RequireHttps]
        public ActionResult editsale(long id)
        {
            //region list for the dropdown
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;

            //for salescode dropdown. show only salescode that have unassigned salesperson
            ViewBag.SalesCodeList = from salesCode in portalClient.getSaleCode()
                                    where salesCode.SalesPersonID == -1
                                    select salesCode;
            /*Get saleperson information
             * join account for the First name,last name, email, COntactNo, COmpany,Password
             * join salescode for the discount prices
             */ 
            var salesPerson_orig = from salesperson in portalClient.getSalePerson()
                                   join accnt in account.getAccount("") on salesperson.UserId equals accnt.Id
                                   join sc in portalClient.getSaleCode() on salesperson.SalesCodeId equals sc.Id
                                   where salesperson.Id == id
                                   select new ViewModel.SalesViewModel() { account = accnt, salesPerson = salesperson, salesCode = sc };

            ViewBag.isDefaultSC = portalClient.getRegion().Where(r => r.DefaultSalesCodeId == salesPerson_orig.First().salesCode.Id).Count() > 0;
          
            return View(salesPerson_orig.First());
        }
        
        [HttpPost]
        [RequireHttps]
        public ActionResult editsale(int id, FormCollection collection)
        {
            //get the row data to be updated
            var salesPerson_orig = from salesPerson in portalClient.getSalePerson()
                                   where salesPerson.Id == id
                                   select salesPerson;
            //get the account data of sales person
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

           
            long Salescodeid = 0;
                 
                var salesperson = (portalClient.getSalePerson().Where(sp => sp.Id == id)).First();
                 /*If admin assigned a new sales code to the salesperson, update "previous" SalesCode and update the salespersonid of the newly assigned salescode.
                    else just updae the current salescode and salesperson
                  */
                if (long.TryParse(collection["SalesCode"], out Salescodeid) && Salescodeid > 0)
                {
                    //set the discount prices to 0 and exporiry date to current
                    updatepreviousSC(salesperson);
                    //update the salespersonid of the newly assigned salescode
                    updateNew_SC(collection, Salescodeid, salesperson);

                    //update/assign the salescodeid of salesperson
                    salesperson.SalesCodeId = Salescodeid;
                    portalClient.updateSalesPerson(salesperson);

                    

                }
                else
                {
                    
                    updateNew_SC(collection, salesperson.SalesCodeId, salesperson);
                    salesperson.RegionId = int.Parse(collection["Region"]);
                    portalClient.updateSalesPerson(salesperson);
             
                }
                
            return RedirectToAction("sales");
        }

        private void updatepreviousSC(PortalServiceReference.SalesPerson salesperson)
        {
            var prev_sc = portalClient.getSaleCode().Where(sc => sc.Id == salesperson.SalesCodeId).First();
            prev_sc.Less_monthly = 0;
            prev_sc.Less_6months = 0;
            prev_sc.Less_3months = 0;
            prev_sc.DateEnd = DateTime.Now;
            portalClient.updateSalsCode(prev_sc);
        }

        private void updateNew_SC(FormCollection collection, long Salescodeid, PortalServiceReference.SalesPerson salesperson)
        {
            var salescode_new = portalClient.getSaleCode().Where(sc => sc.Id == Salescodeid).First();
            salescode_new.SalesPersonID = salesperson.Id;
           
            salescode_new.Less_monthly = decimal.Parse(collection["Discountpermonth"] ?? "0");
            salescode_new.Less_3months = decimal.Parse(collection["Discountper3months"] ?? "0");
            salescode_new.Less_6months = decimal.Parse(collection["Discountper6months"] ?? "0");

            portalClient.updateSalsCode(salescode_new);
        }
        [RequireHttps]
        public ActionResult deletesale(int id)
        {
            //update to null to remove dependency
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
        [RequireHttps]
        public ActionResult price()
        {
            //if session not active redirect to homepage
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            var priceList = from p in portalClient.getPrice()
                            join r in portalClient.getRegion() on p.RegionId equals r.Id
                            select new ViewModel.PriceViewModel() { price = p, region = r };
            return View(priceList);
        }
        [RequireHttps]
        public ActionResult addprice()
        {
            ViewBag.RegionList = from region in portalClient.getRegion()
                                 select region;
            return View();
        }
        
        [HttpPost]
        [RequireHttps]
        public ActionResult addprice(FormCollection collection)
        {
            var priceList = from price in portalClient.getPrice()
                        where price.RegionId == int.Parse(collection["Region"])
                        select price;
            //add price if region doesnt have price yet
            if (priceList.Count() == 0)
            {
                //price to be added
                var price = new PortalServiceReference.Price()
                    {

                        RegionId = int.Parse(collection["Region"]),
                        PriceAmt = decimal.Parse(collection["Price"]),
                        PriceAmt_B = decimal.Parse(collection["Price3"]),
                        priceAmt_C = decimal.Parse(collection["Price6"]),
                        FirstMonthFree = collection["monthFree"] == "Yes",
                        Active = collection["pricepermonthavailability"] == "enable",
                        Active_b = collection["priceperthreemonthavailability"] == "enable",
                        Active_c = collection["pricepersixmonthavailability"] == "enable",
                        Description = collection["subscriptionDetails"]
                    };
                //log added price
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
        [RequireHttps]
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
        [RequireHttps]
        public ActionResult editprice(int id, FormCollection collection)
        {
            //get the current price to be updated
            var old_price = portalClient.getPrice().Where(p=>p.Id == id).First();
            //init updated price
            var new_price = new PortalServiceReference.Price()
            {
                Id = id,
                //RegionName = collection["Region"],
                RegionId = int.Parse(collection["Region"]),
                PriceAmt = decimal.Parse(collection["Price"]),
                FirstMonthFree = collection["monthFree"] == "Yes",
                PriceAmt_B = decimal.Parse(collection["PriceB"].ToString()),
                priceAmt_C = decimal.Parse(collection["PriceC"].ToString()),
                Active = collection["pricepermonthavailability"] == "enable",
                Active_b = collection["priceperthreemonthavailability"] == "enable",
                        Active_c = collection["pricepersixmonthavailability"] == "enable",
                        Description = collection["subscriptionDetails"].ToString()
            };
            
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), new_price, old_price, portalClient);

            portalClient.updatePrice(new_price);
            return RedirectToAction("price");
        }
        [RequireHttps]
        public ActionResult deleteprice(int id)
        {

            var old_price = portalClient.getPrice().Where(p=>p.Id==id).FirstOrDefault();
            AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_price, portalClient);
            portalClient.deletePrice(id);
            

            return RedirectToAction("price");
        }
    #endregion

        #region Sales Code
        [RequireHttps]
        public ActionResult salescode()
        {
            //if session is expired redirect to home page
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            var SalesCodeList = from salesCode in portalClient.getSaleCode()
                                select salesCode;
            //TempData["SalesPersonExists"] is set during delete. Cant delete a salescode with a salesperson currently assigned to it. 
            if (TempData["SalesPersonExists"] != null)
                ViewBag.SalesPersonExists = TempData["SalesPersonExists"];
            return View(SalesCodeList);
        }

        [RequireHttps]
        public ActionResult addsalescode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addsalescode(FormCollection collection)
        {
            //add if sales code doesnt exists yet
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
            //get the current salescode to be updated
            var salesCode_orig = from salesCode in portalClient.getSaleCode()
                                 where salesCode.Id == id
                                 select salesCode;
            //init updated sales code
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
            
            //if salescode has a salespersonid assigned then error
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
        /// <summary>
        /// Checks if the salescode has an assigned sales person
        /// </summary>
        /// <param name="salescodeid"></param>
        /// <returns></returns>
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
        [RequireHttps]
        public ActionResult addRegion()
        {
            initAddRegionViewBag();
            return View();
        }

        private void initAddRegionViewBag()
        {
           
            var currencyList = portalClient.getPayPalCurrencies();
           //salescode list for the assignment of default salescode
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
        [RequireHttps]
        public ActionResult addregion(FormCollection collection)
        {
            //if region name doesnt exists yet,then upload the airplayer and add region else error
            if (portalClient.getRegion().Select(r => r).Where(r => r.RegionName == collection["RegionName"]).Count() == 0)
            {
                //if no request file, set the default filename to "#" 
                string strAirPlayerFileName = Request.Files.Count > 0? Request.Files["air"].FileName:"#";
                //checks if the file name is in .air or .zip
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
                //init region to be added
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
                //log new region
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
        [RequireHttps]
        public ActionResult editregion(int id)
        {
            //set the region to be updated/edited
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
        [RequireHttps]
        public ActionResult editregion(int id, FormCollection collection)
        {
            //get other region that has the same region name 
            var existing_Region = from region in portalClient.getRegion()
                                  where region.Id != id && region.RegionName == collection["RegionName"]
                                  select region;
            //if theres another region that has the same regionname, dont proceed with the update
            if(existing_Region.Count() == 0)
            {
                //current region to be updated
                var orig_Region = portalClient.getRegion().Where(r => r.Id == id).First();
                    
                    orig_Region.RegionName = collection["RegionName"];
                    orig_Region.Currency = collection["currency"];
                    orig_Region.WebsiteUrl = collection["siteUrl"];
                    orig_Region.PayPalUserName = collection["usernamePpUser"];
                    orig_Region.PayPalPassword = collection["passwordPpUser"];
                    orig_Region.PayPalSignature = collection["signaturePpUser"];
                    orig_Region.DefaultSalesCodeId = long.Parse(collection["defaultSalesCode"]);
                    orig_Region.ServiceChargeCode = collection["serviceChargeCode"];
                    //if request has file, then upload the air player then set the playername
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

      
        
        [RequireHttps]
        public ActionResult deleteregion(int id)
        {
            //if user session expired, redirect to home page
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            //check for dependencies in sales and price. delete when has no depended
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
        [RequireHttps]
        public ActionResult marketer()
        {
            //if user session expired, redirect to home page
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
            //if user session expired, redirect to home page
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }
            return View();
        }
        [HttpPost]
        [RequireHttps]
        public ActionResult addmarketer(FormCollection collection)
        {
            
           var user = from accnt in account.getAccount(collection["Email"].Trim())
                      select accnt;
           //if the user already exists then error
           if (user.Count() == 0)
           {
           //init new account
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
        [RequireHttps]
        public ActionResult editMarketer(long id)
        {
            //if user session expired, redirect to home page
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
        [RequireHttps]
        public ActionResult editmarketer(long id, FormCollection collection)
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
                //init updated account.
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
        [RequireHttps]
        public ActionResult deletemarketer(long id)
        {
            //if user session expired, redirect to home page
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

        public ActionResult changepassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult changepassword(ViewModel.changePasswordModel model)
        {
            //if user session expired, redirect to home page
            if (!isUserSessionActive())
            {
                return RedirectToAction("login", "Home");
            }

            var accountModel = account.getAccount(Session["Username"].ToString()).First();
            if (ModelState.IsValid)
            {
                account.updatePassword(accountModel.Id, accountModel.PASSWORD, model.Password);
                ViewBag.isPasswordChange = true;
            }
            return View();

            
        }
        private bool isUserSessionActive()
        {
            return Session["Username"] != null;

        }
    }
}
