using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditBrokerMvc.Models
{
    public class ReportModelStr
    {

        public string CompanyName { get; set; }
        public string TotalScore { get; set; }
        public string CompanyNationalCode { get; set; }
        public string DateNow { get; set; }
        public string IsServiceOk { get; set; }

        public CreditBrokerModelStr Finance { get; set; }
        public CreditBrokerModelStr Hesabress { get; set; }
        public CreditBrokerModelStr ManagerScore { get; set; }

        public CreditBrokerModelStr BrokerPerformance { get; set; }
        public CreditBrokerModelStr BussinessVariable { get; set; }

    }

        public class CreditBrokerModelStr
        {
            public string SumScore { get; set; }
            public string CreditName { get; set; }
            public List<StatusStr> statuses { get; set; }
        }


        public class StatusStr
        {
            public string Description { get; set; }
            public string ScoreValue { get; set; }
        }

    }
