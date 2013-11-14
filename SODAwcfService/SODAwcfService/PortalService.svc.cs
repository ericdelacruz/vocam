﻿using System;
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
        
        public PortalService()
        {
            customerTableAdapter = new PortalDataSetTableAdapters.CustomerTableAdapter();
            logsTableAdapter = new PortalDataSetTableAdapters.LogsTableTableAdapter();
            priceTableAdapter = new PortalDataSetTableAdapters.PriceTableTableAdapter();
            salesCodeTableAdapter = new PortalDataSetTableAdapters.SalesCodeTableAdapter();
            salesPersonTableAdapter = new PortalDataSetTableAdapters.SalesPersonTableAdapter();
            regionTableAdapter = new PortalDataSetTableAdapters.RegionTableAdapter();
        }
       
        public IEnumerable<Models.Customer> getCustomer()
        {
            List<Models.Customer> listCustomer = new List<Models.Customer>();
            foreach(var row in customerTableAdapter.GetData())
            {
                listCustomer.Add(new Models.Customer(){
                     Id= row.Id,
                     UserId = row.Id,
                     DatePurchase = row.DatePurchase,
                     DateSubscriptionEnd = row.DateSubscriptionEnd
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
                   where cust.Sales_Code == SalesCode
                   select cust;
        }

        public int addCustomer(Models.Customer customer)
        {
            return customerTableAdapter.Insert(customer.Id, customer.Sales_Code, customer.DatePurchase, customer.DateSubscriptionEnd);
        }

        public int updateCustomer(Models.Customer customer)
        {
            return customerTableAdapter.Update(customer.UserId, customer.Sales_Code, customer.DatePurchase, customer.DateSubscriptionEnd, customer.Id);
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
                    Sales_Code = person.Sales_Code,
                    UserId = person.UserId,
                    RegionId = person.RegionId

                });
            }
            return salesPerSonList;
        }

        public int addSalesPerson(Models.SalesPerson salesPerson)
        {
            return salesPersonTableAdapter.Insert(salesPerson.UserId, salesPerson.Sales_Code, salesPerson.RegionId);
        }

        public int updateSalesPerson(Models.SalesPerson salesPerson)
        {
            return salesPersonTableAdapter.Update(salesPerson.UserId, salesPerson.Sales_Code, salesPerson.RegionId, salesPerson.Id);
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
                    DateEnd = salesCode.DateEnd
                });
            }
            return listSalesCode;
        }

        public int addSalesCode(Models.SalesCode salesCode)
        {
            return salesCodeTableAdapter.Insert(salesCode.SalesPersonID, salesCode.Sales_Code, salesCode.Discount, salesCode.DateCreated, salesCode.DateEnd);
        }

        public int updateSalsCode(Models.SalesCode salesCode)
        {
            return salesCodeTableAdapter.Update(salesCode.SalesPersonID, salesCode.Sales_Code, salesCode.Discount, salesCode.DateCreated, salesCode.DateEnd, salesCode.Id);
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
                    RegionName = price.RegionName
                });
            }
            return priceList;
        }

        public int addPrice(Models.Price price)
        {
            return priceTableAdapter.Insert(price.PriceAmt, price.FirstMonthFree, price.RegionName, price.Active);
        }

        public int updatePrice(Models.Price price)
        {
            return priceTableAdapter.Update(price.PriceAmt, price.FirstMonthFree, price.RegionName, price.Active, price.Id);
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
                    Id = region.Id
                });
            }
            return regionlist;
        }

        public int addRegion(Models.Region region)
        {
            return regionTableAdapter.Insert(region.RegionName);
        }

        public int updateRegion(Models.Region region)
        {
            return regionTableAdapter.Update(region.RegionName, region.Id);
        }

        public int deleteRegion(int id)
        {
            return regionTableAdapter.Delete(id);
        }
    }
}