using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace CreditBrokerMvc.Models
{
    /// <summary>
    /// نسبت های مالی
    /// </summary>
    public class FinancialRatio
    {
        /// <summary>
        /// سود خالص
        /// </summary>
        [DisplayName("سود خالص")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public int NetProfit { get; set; }
        /// <summary>
        /// مجموع بدهی
        /// </summary>
        [DisplayName("مجموع بدهی")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public long TotalDebt { get; set; }
        /// <summary>
        /// هزینه مالی
        /// </summary>
        [DisplayName("هزینه مالی")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long FinancialCost { get; set; }
        /// <summary>
        /// اظهار نظر حسابرس
        /// </summary>
        [DisplayName("اظهار نظر حسابرس")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public short EzharHesabres { get; set; }
        /// <summary>
        /// مجموع درامد عملیاتی
        /// </summary>
        [DisplayName("مجموع درامد عملیاتی")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long SumDaramadAmaliaty { get; set; }
        /// <summary>
        /// مجموع دارایی ها
        /// </summary>
        [DisplayName("مجموع دارایی ها")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long SumDaraei { get; set; }
        /// <summary>
        /// حقوق صاحبان سهام
        /// </summary>
        [DisplayName("حقوق صاحبان سهام")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long HoghogheSahebanSaham { get; set; }

    }
}