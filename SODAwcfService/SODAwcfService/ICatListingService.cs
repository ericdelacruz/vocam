using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICatListingService" in both code and config file together.
    [ServiceContract]
    public interface ICatListingService
    {
        

        
        #region Category
        [OperationContract]
        int add_Category(Models.Category category_new);

        [OperationContract]
        int update_Category(Models.Category category_new);

        [OperationContract]
        int delete_Category(long CategoryID);

        [OperationContract]
        IEnumerable<Models.Category>  get_Category(long CategoryID);

        [OperationContract]
        IEnumerable<Models.Category> get_Categories();
        #endregion 

        #region Specific
        [OperationContract]
        int add_Specific(Models.Specific specific_new);

        [OperationContract]
        int update_Specific(Models.Specific specific_new);

        [OperationContract]
        int delete_Specific(long id);
        [OperationContract]
        IEnumerable<Models.Specific> getSpecificByID(long ID);

        [OperationContract]
        IEnumerable<Models.Specific> getSpecificByCatID(long CatID);
        #endregion
        [OperationContract]
        bool Authenticate(string password);

        [OperationContract]
        IEnumerable<Models.Specific> getRelatedByID(long id);

        [OperationContract]
        IEnumerable<Models.Specific> get();
    }
}
