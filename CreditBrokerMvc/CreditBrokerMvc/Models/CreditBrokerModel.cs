using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditBrokerMvc.Models
{
    public class RequestModel
    {
        /// <summary>
        /// نام شرکت
        /// </summary>
        [DisplayName("نام شرکت")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[\u0600-\u06FF]+$", ErrorMessage = "نام شرکت با فرمت فارسی وارد شود")]
        public string CompanyName { get; set; }
        /// <summary>
        /// شماره ملی
        /// </summary>
        [DisplayName("شناسه ملی شرکت")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(11, MinimumLength = 11, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "شناسه ملی شرکت با صفر شروع نشود")]
        public string CompanyNationalCode { get; set; }

        public string UserId { get; set; }

        public FinancialRatio FinancialRatio { get; set; }

        public CreditCompanyAndManager CreditCompanyAndManager { get; set; }

        public BrokerPerformanceVariables BrokerPerformanceVariables { get; set; }

        public BusinessVaribales BusinessVaribales { get; set; }

    }


    public class ResultModelDTO
    {
        public string CompanyName { get; set; }
        public int TotalScore { get; set; }
        public string CompanyNationalCode { get; set; }
        public string DateNow { get; set; }
        public bool IsServiceOk { get; set; }
        //public int Score { get; set; }
        public ServiceReference1.CreditBrokerModel Finance { get; set; }
        public ServiceReference1.CreditBrokerModel Hesabress { get; set; }
        public ServiceReference1.CreditBrokerModel ManagerScore { get; set; }
        public ServiceReference1.CreditBrokerModel BrokerPerformance { get; set; }
        public ServiceReference1.CreditBrokerModel BussinessVariable { get; set; }

        // public List<CreditBrokerModel> CreditBrokerModel { get; internal set; }
        // public List<CreditBrokerModel> CreditBrokerModel { get; set; }
    }

    public class CreditBrokerModel
    {
        public int Score { get; set; }
        public string CreditName { get; set; }
        public List<Status> statuses { get; set; }
    }


    public class Status
    {
        public string Description { get; set; }
        public string ScoreValue { get; set; }

    }

}