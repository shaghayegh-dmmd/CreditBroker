using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace CreditBrokerMvc.Models
{
    public class BusinessVaribales
    {
        /// <summary>
        /// مدت فعالیت (سال)
        /// </summary>
        [DisplayName("مدت فعالیت (سال)")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public short CompanyActivityTime { get; set; }
        /// <summary>
        ///تجربه کاری مرتبط مدیر اجرایی (سال)‍
        /// </summary>
        [DisplayName("تجربه کاری مرتبط مدیر اجرایی (سال)‍")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public short ExperinceOfManager { get; set; }
        /// <summary>
        /// ارتباط تحصیلات مدیر اجرایی با فعالیت
        /// </summary>
        [DisplayName("ارتباط تحصیلات مدیر اجرایی با فعالیت")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public bool RelatedEducationOfManager { get; set; }
        /// <summary>
        /// مالکیت محل کار
        /// </summary>
        [DisplayName("مالکیت محل کار")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public short OwnerShipOfLocation { get; set; }
        /// <summary>
        /// میزان تحصیلات مدیر اجرایی
        /// </summary>
        [DisplayName("میزان تحصیلات مدیر اجرایی")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public short EducationLevelOfManager { get; set; }

    }
}