using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Reflection;
namespace CreditBrokerMvc.Enumerations
{
    public static class EnumHelper
    {
        
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

    }
    public enum EducationLevelOfManager 
    {
        [Description("زیر دیپلم و دیپلم")]
        SubDiplomaAndDiploma = 0,
        [Description("فوق دیپلم")]
        Associate = 1,
        [Description("لیسانس")]
        Bachelor = 2,
        [Description("فوق لیسانس")]
        Masters = 4,
        [Description("دکتری و بالاتر")]
        PhDAndAbove = 8
    }
    public enum OwnerShipOfLocation
    {
        [Description("استیجاری")]
        Rental = 6,
        [Description("رهنی")]
        Mortgage = 17,
        [Description("شخصی")]
        Personal = 23
    }

    public enum EzharHesabres
    {
        [Description("مطلوب (مقبول)")]
        Acceptable = 100,
        [Description("مشروط")]
        Conditionally = 67,
        [Description("عدم اظهار نظر")]
        NoComment = 46,
        [Description("مردود (منفی یا رد)")]
        Rejected = 24
    }
    public enum TarkibSahamdaran
    {
        [Description("حقوقی شامل نهادهای مالی معتبر")]
        LegalFinancialInstitutions = 46,
        [Description("سایر اشخاص حقوقی")]
        OtherLegalEntities = 27,
        [Description("اشخاص حقیقی")]
        LegalEntities = 10
    }

}
