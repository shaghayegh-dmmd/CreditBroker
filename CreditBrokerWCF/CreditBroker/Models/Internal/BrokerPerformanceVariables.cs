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
        public long?  SumOfReceiveFacility { get; set; }
        public long  PreviousLastYearIncomeReported { get; set; }
        public int NumberOfbranch { get; set; }
        public long  AmountOfValueAtRisk { get; set; }

        public bool MojavezMoamelatKala { get; set; }
        public long  SarmayehSabti { get; set; }
        public long?  SumOfFacilityToCustomer { get; set; }
        public long LastYearIncomeReported { get; set; }
        public int NumberOfPersonnel { get; set; }
        public bool MojavezSabadGardani { get; set; }
        public float EmtiazCommitteePayeshRisk { get; set; }
    }
}