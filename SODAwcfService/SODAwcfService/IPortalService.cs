using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPortalService" in both code and config file together.
    [ServiceContract]
    public interface IPortalService
    {
        #region Customer
        [OperationContract]
        IEnumerable<Models.Customer> getCustomer();
        [OperationContract]
        IEnumerable<Models.Customer> getByDate(DateTime dateStart, DateTime? dateEnd);

        [OperationContract]
        IEnumerable<Models.Customer> getBySaleCode(string SalesCode);

        [OperationContract]
        int addCustomer(Models.Customer customer);

        [OperationContract]
        int updateCustomer(Models.Customer customer);

        [OperationContract]
        int deleteCustomer(long id);
        #endregion
        #region SalesPerson
        [OperationContract]
        IEnumerable<Models.SalesPerson> getSalePerson();

        [OperationContract]
        int addSalesPerson(Models.SalesPerson salesPerson);

        [OperationContract]
        int updateSalesPerson(Models.SalesPerson salesPerson);

        [OperationContract]
        int deleteSalePerson(int ID);
        #endregion
        #region Sales
                [OperationContract]
                IEnumerable<Models.SalesCode> getSaleCode();

                [OperationContract]
                 int addSalesCode(Models.SalesCode salesCode);

                [OperationContract]
                int updateSalsCode(Models.SalesCode salesCode);

                [OperationContract]
                int deleteSalesCode(long Id);
        #endregion

        #region Price
        [OperationContract]
        IEnumerable<Models.Price> getPrice();

        [OperationContract]
        int addPrice(Models.Price price);

        [OperationContract]
        int updatePrice(Models.Price price);

        [OperationContract]
        int deletePrice(int Id);
        #endregion

        #region Region
        [OperationContract]
        IEnumerable<Models.Region> getRegion();

        [OperationContract]
        int addRegion(Models.Region region);

        [OperationContract]
        int updateRegion(Models.Region region);

        [OperationContract]
        int deleteRegion(int id);
        #endregion

        #region LicenseConsumption
        [OperationContract]
        IEnumerable<Models.LicenseConsumption> getLicenseConsumption();
        [OperationContract]
        int addLicenseConsumption(Models.LicenseConsumption license);
        [OperationContract]
        int updateLicenseConsumption(Models.LicenseConsumption license);
        [OperationContract]
        int delteLicenseConsumption(int id);
        #endregion

        #region CustomerContract
         [OperationContract]
        int addCustomerContract(Models.CustomerContract contract);
         [OperationContract]
        int updateCustomerContract(Models.CustomerContract contract);
         [OperationContract]
        int deleteCustomerContract(long id);
         [OperationContract]
        IEnumerable<Models.CustomerContract> getCustomerContract();
        #endregion
        [OperationContract]
         IEnumerable<string> getPayPalCurrencies();
    }

}
