using CreditBroker.Helper;
using CreditBrokerWCF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Caching;
using static CreditBroker.Helper.CalcHelper;

namespace CreditBroker
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BrokerServices : IBrokerServices
    {
        public ResultModel Credit(RequestModel requestModel)
        {
            if (!IsValidLegalNationalCode(requestModel.CompanyNationalCode)) throw new Exception("شناسه ملی صحیح نیست !");
            return Infrastracture.GetCredit(requestModel);
        }

        public ResultModel CheckCreditToday(string companyNationalCode)
        {
            if (!IsValidLegalNationalCode(companyNationalCode)) throw new Exception("شناسه ملی صحیح نیست !");
            return Infrastracture.CheckCreditToday(companyNationalCode);
        }

        private static bool IsValidLegalNationalCode(string nationalCode)
        {
            try
            {
                if (string.IsNullOrEmpty(nationalCode))
                {
                    return false;
                }

                if (nationalCode.Length != 11)
                {
                    return false;
                }

                var nCo = long.Parse(nationalCode);
                if (nCo.ToString().Length == 11)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public UserDataCr24SvcModel GetUserData(string userName)
        {
            if (HttpRuntime.Cache["UserData" + userName] != null)
                return (UserDataCr24SvcModel)HttpRuntime.Cache["UserData" + userName];
            var data = UserAuthService.GetUserData(userName);
            HttpRuntime.Cache.Insert("UserData" + userName, data, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
            return data;
        }
    }
}
