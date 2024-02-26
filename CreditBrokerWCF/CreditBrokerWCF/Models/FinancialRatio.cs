using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditBrokerWCF.Models
{
    /// <summary>
    /// نسبت های مالی
    /// </summary>
    public class FinancialRatio
    {
        /// <summary>
        /// سود خالص
        /// </summary>
        public int NetProfit { get; set; }
        /// <summary>
        /// مجموع بدهی
        /// </summary>
        public string TotalDebt { get; set; }
        /// <summary>
        /// هزینه مالی
        /// </summary>
        public string  FinancialCost { get; set; }
        /// <summary>
        /// اظهار نظر حسابرس
        /// </summary>
        public short EzharHesabres { get; set; }
        /// <summary>
        /// مجموع درامد عملیاتی
        /// </summary>
        public string  SumDaramadAmaliaty { get; set; }
        /// <summary>
        /// مجموع دارایی ها
        /// </summary>
        public string    SumDaraei { get; set; }
        /// <summary>
        /// حقوق صاحبان سهام
        /// </summary>
        public string  HoghogheSahebanSaham { get; set; }
    }
}