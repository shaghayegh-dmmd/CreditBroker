
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditBroker.Models.DataBaseModel
{
    [Table("CreditBrokers")]
    public class CreditBroker
    {
        [Key]
        public long Id { get; set; }
        public string User { get; set; }
        public DateTime ReqDate { get; set; }
        public long CompanyNationalCode { get; set; }
        public System.Guid Guid { get; set; }
        public string  ReqestData { get; set; }
        public string  ResultData { get; set; }
        public string  Description { get; set; }
    }
}