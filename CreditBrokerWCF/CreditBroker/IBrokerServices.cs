using CreditBrokerWCF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using static CreditBroker.Helper.CalcHelper;

namespace CreditBroker
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IBrokerServices
    {
        [OperationContract]
        UserDataCr24SvcModel GetUserData(string userName);

        [OperationContract]
        ResultModel Credit(RequestModel value);

        [OperationContract]
        ResultModel CheckCreditToday(string companyNationalCode);

        // TODO: Add your service operations here
    }


  
}
