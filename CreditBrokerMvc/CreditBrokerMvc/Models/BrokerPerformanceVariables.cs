using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditBrokerMvc.Models
{
    public class BrokerPerformanceVariables
    {
        /// <summary>
        /// رتبه کارگزاری
        /// </summary>
        [DisplayName("رتبه کارگزاری")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(1,999,ErrorMessageResourceType = typeof(Resources.ErrorMessages),ErrorMessageResourceName = "NotInRange")]
        public short BrokerRank { get; set; }
        /// <summary>
        /// ترکیب سهامداران
        /// </summary>
        [DisplayName("ترکیب سهامداران")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public short TarkibSahamdaran { get; set; }
        /// <summary>
        /// مجموع تسهیلات دریافت شده توسط شرکت
        /// </summary>
        [DisplayName("مجموع تسهیلات دریافت شده توسط شرکت")]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long? SumOfReceiveFacility { get; set; }
        /// <summary>
        /// درآمد سال مالی قبلی گزارش شده (میلیون ریال)
        /// </summary>
        [DisplayName("درآمد سال مالی قبلی گزارش شده")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long PreviousLastYearIncomeReported { get; set; }
        /// <summary>
        /// تعداد شعب
        /// </summary>
        [DisplayName("تعداد شعب")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public int NumberOfbranch { get; set; }
        /// <summary>
        /// مبلغ ارزش در ریسک (میلیون ریال)
        /// </summary>
        [DisplayName("مبلغ ارزش در ریسک ")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long AmountOfValueAtRisk { get; set; }
        /// <summary>
        /// مجوز معاملات کالا (همه بخش‌ها)
        /// </summary>
        [DisplayName("مجوز معاملات کالا (همه بخش‌ها)")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public bool MojavezMoamelatKala { get; set; }
        /// <summary>
        /// سرمایه ثبتی
        /// </summary>
        [DisplayName("سرمایه ثبتی")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long SarmayehSabti { get; set; }
        /// <summary>
        /// مجموع تسهیلات تخصیص داده شده برای مشتریان (میلیون ریال)
        /// </summary>
        [DisplayName("مجموع تسهیلات تخصیص داده شده برای مشتریان")]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long? SumOfFacilityToCustomer { get; set; }
        /// <summary>
        /// درآمد آخرین سال مالی گزارش شده (میلیون ریال)
        /// </summary>
        [DisplayName("درآمد آخرین سال مالی گزارش شده")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public long LastYearIncomeReported { get; set; }
        /// <summary>
        /// تعداد پرسنل
        /// </summary>
        [DisplayName("تعداد پرسنل")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        public int NumberOfPersonnel { get; set; }
        /// <summary>
        /// امتیاز کمیته پایش ریسک بازار 
        /// </summary>
        [DisplayName("امتیاز کمیته پایش ریسک بازار")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [Range(0, 5, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "NotInRange")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "اعداد بین صفر تا پنج تا دورقم اعشار")]
        public float EmtiazCommitteePayeshRisk { get; set; }
        /// <summary>
        /// مجوز سبدگردانی
        /// </summary>
        [DisplayName("مجوز سبدگردانی")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public bool MojavezSabadGardani { get; set; }
    }
}