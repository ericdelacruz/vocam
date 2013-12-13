using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "Isoda" in both code and config file together.
    [ServiceContract]
    public interface Isoda
    {
        [OperationContract]
        string ProductConsumeRegister(string username, string productType);
    }
}
