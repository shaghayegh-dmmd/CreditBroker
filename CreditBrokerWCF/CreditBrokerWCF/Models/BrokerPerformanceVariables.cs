using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditBrokerWCF.Models
{
    public class BrokerPerformanceVariables
    {
        public short BrokerRank { get; set; }
        public short TarkibSahamdaran { get; set; }
        public string  SumOfReceiveFacility { get; set; }
        public string  PreviousLastYearIncomeReported { get; set; }
        public int NumberOfbranch { get; set; }
        public string  AmountOfValueAtRisk { get; set; }

        public bool MojavezMoamelatKala { get; set; }
        public string  SarmayehSabti { get; set; }
        public string  SumOfFacilityToCustomer { get; set; }
        public string LastYearIncomeReported { get; set; }
        public int NumberOfPersonnel { get; set; }
        public bool MojavezSabadGardani { get; set; }
    }
}