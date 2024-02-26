using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CreditBrokerWCF.Models
{
    [DataContract]
    public class RequestModel
    {
        [DataMember]
        public string  CompanyName { get; set; }
        [DataMember]
        public string  CompanyNationalCode { get; set; }
        [DataMember]
        public string  UserName { get; set; }
        [DataMember]
        public FinancialRatio FinancialRatio { get; set; }
        [DataMember]
        public CreditCompanyAndManager CreditCompanyAndManager { get; set; }
        [DataMember]
        public BrokerPerformanceVariables BrokerPerformanceVariables { get; set; }
        [DataMember]
        public BusinessVaribales BusinessVaribales { get; set; }
       
    }


    [DataContract]
    public class  ResultModel 
    {
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public int TotalScore { get; set; }
        [DataMember]
        public string CompanyNationalCode { get; set; }
        [DataMember]
        public bool IsServiceOk { get; set; }
        [DataMember]
        public List<CreditBrokerModel> CreditBrokerModelList { get; set; }
    }

    public class CreditBrokerModel
    {
        [DataMember]
        public string  SumScore { get; set; }
        [DataMember]
        public string  CreditName { get; set; }
        [DataMember]
        public List<Status> statuses { get; set; }
    }

    public class Status
    {
        public string Description { get; set; }
        public string ScoreValue { get; set; }
    }
}