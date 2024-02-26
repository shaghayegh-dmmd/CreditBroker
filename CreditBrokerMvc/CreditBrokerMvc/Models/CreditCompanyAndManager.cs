using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CreditBrokerMvc.Helper;

namespace CreditBrokerMvc.Models
{
    public class CreditCompanyAndManager
    {
        /// <summary>
        /// شناسه ملی شرکت
        /// </summary>
        //[DisplayName("شناسه ملی شرکت")]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        //public string CompanyNationalCode { get; set; }
        /// <summary>
        /// امتیاز اعتباری
        /// </summary>
        [DisplayName("امتیاز اعتباری")]
       // [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public string CompanyScore { get; set; }
        /// <summary>
        /// کد ملی مدیر اجرایی ۱
        /// </summary>
        [DisplayName("کد ملی مدیر اجرایی ۱")]
        [StringLength(10, MinimumLength = 10, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public string ManagerNationalCode1 { get; set; }

        //  public string Managerscore1 { get; set; }
        /// <summary>
        /// کد ملی مدیر اجرایی ۲
        /// </summary>
        [DisplayName("کد ملی مدیر اجرایی ۲")]
        public string ManagerNationalCode2 { get; set; }
        //   public string Managerscore2 { get; set; }
        /// <summary>
        /// کد ملی مدیر اجرایی ۳
        /// </summary>
        [DisplayName("کد ملی مدیر اجرایی ۳")]
        public string ManagerNationalCode3 { get; set; }
        //   public string Managerscore3 { get; set; }

    }
}