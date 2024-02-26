using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditBrokerWCF.Models
{
    public class RequestModel
    {
        public FinancialRatio FinancialRatio { get; set; }
        public CreditCompanyAndManager CreditCompanyAndManager { get; set; }
        public BrokerPerformanceVariables BrokerPerformanceVariables { get; set; }
        public BusinessVaribales BusinessVaribales { get; set; }
    }


    public class  ResultModel 
    {
        public List<Status> CreditDetialsStatus { get; set; }
        public List<Status> FinancialRatioStatus { get; set; }
        public List<Status> CreditCompanyAndManagerStatus { get; set; }
        public List<Status> BrokerPerformanceVariablesStatus { get; set; }
        public List<Status> BusinessVaribalesStatus { get; set; }
    }

    public class CreditBrokerModel
    {
        public string  CompanyName { get; set; }
        public string  TotalScore { get; set; }
        public string  CompanyNationalCode { get; set; }

    }


    public class Status
    {
        public string Description { get; set; }
        public string ScoreValue { get; set; }
      
    }
}