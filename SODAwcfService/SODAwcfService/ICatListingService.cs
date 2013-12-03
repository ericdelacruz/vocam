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
        #region chapter
        [OperationContract]
        int addChapter(long specID, string name, TimeSpan time);

        [OperationContract]
        IEnumerable<Models.Chapter> getChapter();
        [OperationContract]
        int updateChapter(Models.Chapter chapter);
        [OperationContract]
        int deleteChapter(long id);
        #endregion

        #region topic
        [OperationContract]
        int addTopic(long specID, string name);
        [OperationContract]
        IEnumerable<Models.Topic> getTopics();
        [OperationContract]
        int updateTopic(Models.Topic topic);
        [OperationContract]
        int deleteTopic(long id);
        #endregion

        #region Cat Assign
        [OperationContract]
        IEnumerable<Models.CategoryAssignment> getCatAssign();
        [OperationContract]
        int addCatAssign(long specId, long catId);
        [OperationContract]
        int updateCatAssign(long specId, long catId, int Id);
        [OperationContract]
        int deleteCatAssign(int Id);
        #endregion
    }
}
