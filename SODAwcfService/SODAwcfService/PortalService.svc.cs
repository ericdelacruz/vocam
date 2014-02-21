using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PortalService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PortalService.svc or PortalService.svc.cs at the Solution Explorer and start debugging.
    public class PortalService : IPortalService
    {
        private PortalDataSetTableAdapters.CustomerTableAdapter customerTableAdapter;
        private PortalDataSetTableAdapters.LogsTableTableAdapter logsTableAdapter;
        private PortalDataSetTableAdapters.PriceTableTableAdapter priceTableAdapter;
        private PortalDataSetTableAdapters.SalesCodeTableAdapter salesCodeTableAdapter;
        private PortalDataSetTableAdapters.SalesPersonTableAdapter salesPersonTableAdapter;
        private PortalDataSetTableAdapters.RegionTableAdapter regionTableAdapter;
        private PortalDataSetTableAdapters.LicenseConsumptionTableAdapter licenseAdapter;
        private PortalDataSetTableAdapters.CustomerContractTableAdapter custContractAdapter;
        public PortalService()
        {
            customerTableAdapter = new PortalDataSetTableAdapters.CustomerTableAdapter();
            logsTableAdapter = new PortalDataSetTableAdapters.LogsTableTableAdapter();
            priceTableAdapter = new PortalDataSetTableAdapters.PriceTableTableAdapter();
            salesCodeTableAdapter = new PortalDataSetTableAdapters.SalesCodeTableAdapter();
            salesPersonTableAdapter = new PortalDataSetTableAdapters.SalesPersonTableAdapter();
            regionTableAdapter = new PortalDataSetTableAdapters.RegionTableAdapter();
            licenseAdapter = new PortalDataSetTableAdapters.LicenseConsumptionTableAdapter();
            custContractAdapter = new PortalDataSetTableAdapters.CustomerContractTableAdapter();
        }
       
        public IEnumerable<Models.Customer> getCustomer()
        {
            List<Models.Customer> listCustomer = new List<Models.Customer>();
            foreach(var row in customerTableAdapter.GetData())
            {
                listCustomer.Add(new Models.Customer(){
                     Id= row.Id,
                     UserId = row.UserId,
                     DatePurchase = row.DatePurchase,
                     DateSubscriptionEnd = row.DateSubscriptionEnd,
                     SalesCodeId = row.SalesCodeId,
                      DateUpdated = row.DateUpdated,
                       Licenses = row.Licencses,
                        PPId = row.PPId,
                         RecurringType = row.RecuringType
                });
            }
            return listCustomer;
        }

        public IEnumerable<Models.Customer> getByDate(DateTime dateStart, DateTime? dateEnd)
        {

            if (dateEnd != null)
            {
                return from cust in getCustomer()
                       where cust.DatePurchase >= dateStart && cust.DatePurchase < dateEnd
                       select cust;
            }
            else
                return from cust in getCustomer()
                       where cust.DatePurchase == dateStart
                       select cust;
        }

        public IEnumerable<Models.Customer> getBySaleCode(string SalesCode)
        {
            return from cust in getCustomer()
                   join sc in getSaleCode() on cust.SalesCodeId equals sc.Id
                   where sc.Sales_Code == SalesCode.Trim()
                   select cust;
        }

        public int addCustomer(Models.Customer customer)
        {
            return customerTableAdapter.Insert(customer.UserId, customer.SalesCodeId, customer.DatePurchase, 
                                            customer.DateSubscriptionEnd,customer.RecurringType, customer.Licenses,customer.DateUpdated,customer.PPId );
        }

        public int updateCustomer(Models.Customer customer)
        {
            return customerTableAdapter.Update(customer.UserId, customer.SalesCodeId, customer.DatePurchase, customer.DateSubscriptionEnd,customer.RecurringType, customer.Licenses,customer.DateUpdated,customer.PPId, customer.Id);
        }

        public int deleteCustomer(long id)
        {
            return customerTableAdapter.Delete(id);
        }

        public IEnumerable<Models.SalesPerson> getSalePerson()
        {
            List<Models.SalesPerson> salesPerSonList = new List<Models.SalesPerson>();
            foreach(var person in salesPersonTableAdapter.GetData())
            {
                salesPerSonList.Add(new Models.SalesPerson()
                {
                    Id = person.Id,
                    SalesCodeId = person.SalesCodeId,
                    UserId = person.UserId,
                    RegionId = person.RegionId

                });
            }
            return salesPerSonList;
        }

        public int addSalesPerson(Models.SalesPerson salesPerson)
        {
            return salesPersonTableAdapter.Insert(salesPerson.UserId, salesPerson.SalesCodeId, salesPerson.RegionId);
        }

        public int updateSalesPerson(Models.SalesPerson salesPerson)
        {
            return salesPersonTableAdapter.Update(salesPerson.UserId, salesPerson.SalesCodeId, salesPerson.RegionId, salesPerson.Id);
        }

        public int deleteSalePerson(int ID)
        {
            return salesPersonTableAdapter.Delete(ID);
        }

        public IEnumerable<Models.SalesCode> getSaleCode()
        {
            List<Models.SalesCode> listSalesCode = new List<Models.SalesCode>();

            foreach(var salesCode in salesCodeTableAdapter.GetData())
            {
                listSalesCode.Add(new Models.SalesCode()
                {
                    Id = salesCode.Id,
                    Sales_Code = salesCode.Sales_Code,
                    SalesPersonID = salesCode.SalesPersonID,
                    Discount = salesCode.Discount,
                    DateCreated = salesCode.DateCreated,
                    DateEnd = salesCode.DateEnd,
                    Less_monthly= salesCode.Less_monthly,
                    Less_3months= salesCode.Less_3months,
                    Less_6months = salesCode.Less_6months
                });
            }
            return listSalesCode;
        }

        public int addSalesCode(Models.SalesCode salesCode)
        {
            return salesCodeTableAdapter.Insert(salesCode.SalesPersonID, salesCode.Sales_Code, salesCode.Discount, salesCode.DateCreated, salesCode.DateEnd,salesCode.Less_monthly,salesCode.Less_3months,salesCode.Less_6months);
        }

        public int updateSalsCode(Models.SalesCode salesCode)
        {
            return salesCodeTableAdapter.Update(salesCode.SalesPersonID, salesCode.Sales_Code, salesCode.Discount, salesCode.DateCreated, salesCode.DateEnd,salesCode.Less_monthly,salesCode.Less_3months,salesCode.Less_6months, salesCode.Id);
        }

        public int deleteSalesCode(long Id)
        {
            return salesCodeTableAdapter.Delete(Id);
        }

        public IEnumerable<Models.Price> getPrice()
        {
            List<Models.Price> priceList = new List<Models.Price>();
            foreach(var price in priceTableAdapter.GetData())
            {
                priceList.Add(new Models.Price()
                {
                    Id = price.Id,
                    FirstMonthFree = price.FirstMonthFree,
                    Active = price.Active,
                    PriceAmt = price.Price,
                    RegionId = price.RegionId,
                     PriceAmt_B = price.Price_b,
                      priceAmt_C = price.Price_c,
                       Active_b = price.Active_b,
                        Active_c = price.Active_c,
                         Description = price.Description
                });
            }
            return priceList;
        }

        public int addPrice(Models.Price price)
        {
            return priceTableAdapter.Insert(price.PriceAmt, price.FirstMonthFree, price.RegionId, price.Active, price.PriceAmt_B, price.priceAmt_C,price.Active_b,price.Active_c,price.Description);
        }

        public int updatePrice(Models.Price price)
        {
            return priceTableAdapter.Update(price.PriceAmt, price.FirstMonthFree, price.RegionId, price.Active, price.PriceAmt_B, price.priceAmt_C,price.Active_b,price.Active_c,price.Description, price.Id);
        }

        public int deletePrice(int Id)
        {
            return priceTableAdapter.Delete(Id);
        }




        public IEnumerable<Models.Region> getRegion()
        {
            List<Models.Region> regionlist = new List<Models.Region>();

            foreach(var region in regionTableAdapter.GetData())
            {
                regionlist.Add(new Models.Region()
                {
                    RegionName = region.RegionName,
                    Id = region.Id,
                     Currency = region.Currency,
                     WebsiteUrl = region.WebsiteUrl,
                     DefaultSalesCodeId = region.DefaultSalesCodeId,
                     AirPlayerFileName = region.AirPlayerFileName,
                      PayPalPassword = region.PayPalPassword,
                       PayPalSignature = region.PayPalSignature,
                        PayPalUserName = region.PayPalUserName,
                         ServiceChargeCode = region.ServiceChargeCode
                });
            }
            return regionlist;
        }

        public int addRegion(Models.Region region)
        {
            return regionTableAdapter.Insert(region.RegionName,region.Currency,region.WebsiteUrl,region.DefaultSalesCodeId, region.AirPlayerFileName,region.PayPalUserName, region.PayPalPassword, region.PayPalSignature,region.ServiceChargeCode );
        }

        public int updateRegion(Models.Region region)
        {
            return regionTableAdapter.Update(region.RegionName, region.Currency, region.WebsiteUrl, region.DefaultSalesCodeId, region.AirPlayerFileName, region.PayPalUserName, region.PayPalPassword, region.PayPalSignature,region.ServiceChargeCode , region.Id);
        }

        public int deleteRegion(int id)
        {
            return regionTableAdapter.Delete(id);
        }




        public IEnumerable<Models.LicenseConsumption> getLicenseConsumption()
        {
            return licenseAdapter.GetData().Select(lc => new Models.LicenseConsumption()
            {
                Id = lc.Id,
                Consumed = lc.Consumed,
                DateUpdated = lc.DateUpdated,
                UserId = lc.UserId
            });
        }

        public int addLicenseConsumption(Models.LicenseConsumption license)
        {
            return licenseAdapter.Insert(license.UserId, license.Consumed, license.DateUpdated);
        }

        public int updateLicenseConsumption(Models.LicenseConsumption license)
        {
            return licenseAdapter.Update(license.UserId, license.Consumed, license.DateUpdated, license.Id);
        }

        public int delteLicenseConsumption(int id)
        {
            return licenseAdapter.Delete(id);
        }


        public int addCustomerContract(Models.CustomerContract contract)
        {
            return custContractAdapter.Insert(contract.UserId, contract.DateStart, contract.DateEnd);
        }

        public int updateCustomerContract(Models.CustomerContract contract)
        {
            return custContractAdapter.Update(contract.UserId, contract.DateStart, contract.DateEnd, contract.Id);
        }

        public int deleteCustomerContract(long id)
        {
            return custContractAdapter.Delete(id);
        }

        public IEnumerable<Models.CustomerContract> getCustomerContract()
        {
            return custContractAdapter.GetData().Select(contract => new Models.CustomerContract()
            {
                Id = contract.Id,
                DateEnd = contract.DateEnd,
                DateStart = contract.DateStart,
                UserId = contract.UserId
            });
        }


        public IEnumerable<string> getPayPalCurrencies()
        {
            return Enum.GetNames(typeof(PayPal.PayPalAPIInterfaceService.Model.CurrencyCodeType));
        }


        public int addToLogsTable(Models.LogModel log)
        {
            int result = 0;
            using (SodaDBDataSetTableAdapters.LogsTableTableAdapter logAdapter = new SodaDBDataSetTableAdapters.LogsTableTableAdapter())
            {
                try
                {
                    result = logAdapter.Insert(log.Username, log.Action, log.PropertyName, log.Properties, log.Old_values, log.New_values, log.DateLog);
                }
                catch (Exception ex)
                {

                }

            }
            return result;
        }
    }
}
