using CreditBroker.AuthenticationServiceRefrence;
using CreditBroker.InquiryService;
using CreditBroker.RecordsServiceRef;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Caching;
using static CreditBroker.Helper.CalcHelper;

namespace CreditBroker.Helper
{
    public static class CalcHelper
    {



        public static UserDataCr24SvcModel GetUserData(string userName)
        {
            if (HttpRuntime.Cache["UserData" + userName] != null)
                return (UserDataCr24SvcModel)HttpRuntime.Cache["UserData" + userName];
            var data = UserAuthService.GetUserData(userName);
            HttpRuntime.Cache.Insert("UserData" + userName, data, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
            return data;
        }


        public static ScoreResultModel CalculateActualScoreV02(string nationalCode, SamaChequesResponseResultWithJson chq, CbiSamatResponseResultWithJson fac)
        {
            var res = new ScoreResultModel
            {
                Score = 0,
                ListOfCreditReportList = new List<CreditReportFull>(),
            };
            var scoreModel = new ScoreModelV02
            {
                HistoryRecordScore = 0,

                ChequeBouncedScore = 0,

                ChequeClearedScore = 0,

                FacilityAsliScore = 0,

                FacilityTasviehScore = 0,

                FacilityZamenScore = 0
            };

            //HistoryRecords
            #region History
            var historyRecordData = GetRecordsData(nationalCode);
            var historyRecordReportList = new List<CreditReportModel>();
            if (historyRecordData == null || historyRecordData.CustomersVarietyCount <= 0)

            {
                scoreModel.HistoryRecordScore = 24;

                historyRecordReportList.Add(new CreditReportModel
                {
                    VariableDescription = "آخرین وضعیت اعتبارسنجی",
                    ScoreValue = "24",
                    ScoreDescription = "شخص سابقه اعتبارسنجی ندارد."
                });
                historyRecordReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                    ScoreValue = "-",
                    ScoreDescription = "شخص سابقه اعتبارسنجی ندارد."
                });
                historyRecordReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                    ScoreValue = "-",
                    ScoreDescription = "شخص سابقه اعتبارسنجی ندارد."
                });
                historyRecordReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد شرکت‌هایی که مشتری توسط آن‌ها اعتبارسنجی شده‌است",
                    ScoreValue = "-",
                    ScoreDescription = "شخص سابقه اعتبارسنجی ندارد."
                });
            }
            else
            {
              

                var allCnt = historyRecordData.OkCsCountOlderThanSixMonths + historyRecordData.OkCsCountInSixMonths;
                var allNotOkCnt = historyRecordData.NotOkCsCountOlderThanSixMonths + historyRecordData.NotOkCsCountInSixMonths;

                if (historyRecordData.LastCsStatusIsOk)
                {
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "آخرین وضعیت اعتبارسنجی",
                        ScoreValue = "28",
                        ScoreDescription = "آخرین وضعیت اعتبارسنجی مشتری، مثبت بوده است."
                    });
                    scoreModel.HistoryRecordScore += 28;
                }
                else
                {
                    
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "آخرین وضعیت اعتبارسنجی",
                        ScoreValue = "20",
                        ScoreDescription = "آخرین وضعیت اعتبارسنجی مشتری، منفی بوده است."
                    });
                    scoreModel.HistoryRecordScore += 20;
                }

                if (allCnt <= 0)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری مثبت بوده، صفر است."
                    });
                    scoreModel.HistoryRecordScore += 0;
                }

                else if (allCnt == 1)
                {
                  
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                        ScoreValue = "4",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری مثبت بوده، 1 است."
                    });
                    scoreModel.HistoryRecordScore += 4;
                }
                else if (allCnt == 2)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                        ScoreValue = "8",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری مثبت بوده، 2 است."
                    });
                    scoreModel.HistoryRecordScore += 8;
                }
                else if (allCnt == 3)
                {
                  
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                        ScoreValue = "12",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری مثبت بوده، 3 است."
                    });
                    scoreModel.HistoryRecordScore += 12;
                }
                else if (allCnt == 4)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                        ScoreValue = "16",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری مثبت بوده، 4 است."
                    });
                    scoreModel.HistoryRecordScore += 16;
                }
                else //if (allCnt >= 5)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی مثبت ",
                        ScoreValue = "21",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری مثبت بوده، بیشتر از 5 مرتبه است."
                    });
                    scoreModel.HistoryRecordScore += 21;
                }


                if (allNotOkCnt == 0)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری منفی بوده، صفر است."
                    });
                    scoreModel.HistoryRecordScore += 0;
                }
                else if (allNotOkCnt == 1)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                        ScoreValue = "-4",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری منفی بوده، 1 است."
                    });
                    scoreModel.HistoryRecordScore += -4;
                }
                else if (allNotOkCnt == 2)
                {
                  
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                        ScoreValue = "-8",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری منفی بوده، 2 است."
                    });
                    scoreModel.HistoryRecordScore += -8;
                }
                else if (allNotOkCnt == 3)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                        ScoreValue = "-12",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری منفی بوده، 3 است."
                    });
                    scoreModel.HistoryRecordScore += -12;
                }
                else if (allNotOkCnt == 4)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                        ScoreValue = "-16",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری منفی بوده، 4 است."
                    });
                    scoreModel.HistoryRecordScore += -16;
                }
                else
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد دفعات اعتبارسنجی منفی ",
                        ScoreValue = "-20",
                        ScoreDescription = "تعداد دفعاتی که نتیجه اعتبارسنجی مشتری منفی بوده، بیشتر از 5 مرتبه است."
                    });
                    scoreModel.HistoryRecordScore += -20;
                }


                //تعداد شرکت هایی که مشتری توسط آن اعتبار سنجی شده

                if (historyRecordData.CustomersVarietyCount <= 1)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد شرکت‌هایی که مشتری توسط آن‌ها اعتبارسنجی شده‌است",
                        ScoreValue = "11",
                        ScoreDescription = "مشتری تنها توسط یک شرکت اعتبارسنجی شده‌ است."
                    });
                    scoreModel.HistoryRecordScore += 11;
                }
                else if (historyRecordData.CustomersVarietyCount == 2)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد شرکت‌هایی که مشتری توسط آن‌ها اعتبارسنجی شده‌است",
                        ScoreValue = "6",
                        ScoreDescription = "تعداد شرکت‌های مختلفی که مشتری توسط آن‌ها اعتبارسنجی شده‌، 2 عدد است."
                    });
                    scoreModel.HistoryRecordScore += 6;
                }
                else if (historyRecordData.CustomersVarietyCount == 3)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد شرکت‌هایی که مشتری توسط آن‌ها اعتبارسنجی شده‌است",
                        ScoreValue = "3",
                        ScoreDescription = "تعداد شرکت‌های مختلفی که مشتری توسط آن‌ها اعتبارسنجی شده‌، 3 عدد است."
                    });
                    scoreModel.HistoryRecordScore += 3;
                }
                else if (historyRecordData.CustomersVarietyCount <= 5)
                {
                   
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد شرکت‌هایی که مشتری توسط آن‌ها اعتبارسنجی شده‌است",
                        ScoreValue = "2",
                        ScoreDescription = "تعداد شرکت‌های مختلفی که مشتری توسط آن‌ها اعتبارسنجی شده‌، 4 یا 5 عدد است."
                    });
                    scoreModel.HistoryRecordScore += 2;
                }
                else
                {
                    
                    historyRecordReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد شرکت‌هایی که مشتری توسط آن‌ها اعتبارسنجی شده‌است",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد شرکت‌های مختلفی که مشتری توسط آن‌ها اعتبارسنجی شده‌، بیش از 6 عدد است."
                    });
                    scoreModel.HistoryRecordScore += 0;
                }
            }
            res.Score += scoreModel.HistoryRecordScore;
            #endregion History

            //BouncedCheque
            #region BouncedCheque
            var bouncedChequeReportList = new List<CreditReportModel>();
            var chqCount = chq.SamaBouncedChequesResponse.Cheques?.Count();
            var bouncedChecksAmountSum = chq.SamaBouncedChequesResponse.Cheques?.Sum(e => e.Amount);
            if (chqCount == null || bouncedChecksAmountSum <= 0 || chqCount <= 0)
            {
                scoreModel.ChequeBouncedScore = 345;

                bouncedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد چک‌های برگشتی",
                    ScoreValue = "345",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                bouncedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "مجموع مبالغ چک های برگشتی",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                bouncedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "نزدیک ترین زمان چک برگشتی",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                bouncedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین علت برگشت چک های برگشتی",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                bouncedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد حساب های دارای چک برگشتی",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
            }
            else
            {

                var lastDate = chq.SamaBouncedChequesResponse.Cheques.OrderByDescending(o => o.BouncedDate).First().BouncedDate;
                var lastChequeMiladiDate = ToDateTimeMiladi(lastDate);
                var countOfAccounts = chq.SamaBouncedChequesResponse.Cheques.Select(x => x.IBAN).Distinct().Count();

                var reason = new List<int>();
                foreach (var ch in chq.SamaBouncedChequesResponse.Cheques)
                {
                    reason.AddRange(ch.BouncedReason);
                }


                if (chqCount <= 1)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک های برگشتی",
                        ScoreValue = "17",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری 1 عدد است."
                    });

                    scoreModel.ChequeBouncedScore += 17;
                }
                else if (chqCount <= 2)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک های برگشتی",
                        ScoreValue = "8",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری 2 عدد است."
                    });

                    scoreModel.ChequeBouncedScore += 8;
                }

                else if (chqCount <= 4)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک های برگشتی",
                        ScoreValue = "5",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری 3 یا 4 عدد است."
                    });

                    scoreModel.ChequeBouncedScore += 5;
                }
                else if (chqCount <= 6)
                {

                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک های برگشتی",
                        ScoreValue = "3",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری 5 یا 6 عدد است."
                    });

                    scoreModel.ChequeBouncedScore += 3;
                }
                else
                {



                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک های برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری 7 عدد یا بیشتر است."
                    });

                    scoreModel.ChequeBouncedScore += 0;
                }



                if (bouncedChecksAmountSum <= 40000000)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های برگشتی",
                        ScoreValue = "42",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری، کمتر از 4 میلیون تومان است."
                    });

                    scoreModel.ChequeBouncedScore += 42;
                }
                else if (bouncedChecksAmountSum <= 110000000)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های برگشتی",
                        ScoreValue = "19",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری، بین 4 تا 11 میلیون تومان است."
                    });

                    scoreModel.ChequeBouncedScore += 19;
                }

                else if (bouncedChecksAmountSum <= 350000000)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های برگشتی",
                        ScoreValue = "15",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری، بین 11 تا 35 میلیون تومان است."
                    });

                    scoreModel.ChequeBouncedScore += 15;
                }
                else if (bouncedChecksAmountSum <= 1310000000)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های برگشتی",
                        ScoreValue = "8",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری، بین 35 تا 131 میلیون تومان است."
                    });

                    scoreModel.ChequeBouncedScore += 8;
                }
                else
                {

                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری، بیش از 131 میلیون تومان است."
                    });

                    scoreModel.ChequeBouncedScore += 0;
                }


                if (lastChequeMiladiDate >= DateTime.Now.AddDays(-2 * 30))
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "نزدیک ترین زمان چک برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه دو ماه گذشته بوده است."
                    });

                    scoreModel.ChequeBouncedScore += 0;
                }
                else if (lastChequeMiladiDate >= DateTime.Now.AddDays(-4 * 30))
                {

                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "نزدیک ترین زمان چک برگشتی",
                        ScoreValue = "7",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه دو تا چهار ماه گذشته بوده است."
                    });

                    scoreModel.ChequeBouncedScore += 7;
                }
                else if (lastChequeMiladiDate >= DateTime.Now.AddDays(-6 * 30))
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "نزدیک ترین زمان چک برگشتی",
                        ScoreValue = "12",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه چهار تا شش ماه گذشته بوده است."
                    });

                    scoreModel.ChequeBouncedScore += 12;
                }
                else if (lastChequeMiladiDate >= DateTime.Now.AddDays(-21 * 30))
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "نزدیک ترین زمان چک برگشتی",
                        ScoreValue = "17",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه شش تا بیست و یک ماه گذشته بوده است."
                    });

                    scoreModel.ChequeBouncedScore += 17;
                }
                else if (lastChequeMiladiDate >= DateTime.Now.AddDays(-36 * 30))
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "نزدیک ترین زمان چک برگشتی",
                        ScoreValue = "22",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه بیست و یک ماه تا سه سال گذشته بوده است."
                    });

                    scoreModel.ChequeBouncedScore += 22;
                }
                else
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "نزدیک ترین زمان چک برگشتی",
                        ScoreValue = "28",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بیش از سه سال گذشته بوده است."
                    });

                    scoreModel.ChequeBouncedScore += 28;
                }




                if (reason.Any(o => o == 403))
                {

                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "عدم موجودی"
                    });

                    scoreModel.ChequeBouncedScore += 0;
                }
                else if (reason.Any(o => o == 402))
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های برگشتی",
                        ScoreValue = "2",
                        ScoreDescription = "کسری موجودی"
                    });

                    scoreModel.ChequeBouncedScore += 2;
                }
                else if (reason.Any(o => o == 412 || o == 411))
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های برگشتی",
                        ScoreValue = "7",
                        ScoreDescription = "حساب مسدود یا ماده 14"
                    });

                    scoreModel.ChequeBouncedScore += 7;
                }
                else
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های برگشتی",
                        ScoreValue = "9",
                        ScoreDescription = "حساب بسته، عدم تطابق امضا و غیره"
                    });

                    scoreModel.ChequeBouncedScore += 9;
                }


                if (countOfAccounts <= 1)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "7",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده است 1 حساب است."
                    });

                    scoreModel.ChequeBouncedScore += 7;
                }
                else if (countOfAccounts <= 2)
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "5",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده است 2 حساب است."
                    });

                    scoreModel.ChequeBouncedScore += 5;
                }
                else if (countOfAccounts <= 5)
                {

                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "2",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده است 3 ، 4 یا 5 حساب است."
                    });

                    scoreModel.ChequeBouncedScore += 2;
                }
                else
                {


                    bouncedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده است 6 و بیشتر است."
                    });

                    scoreModel.ChequeBouncedScore += 0;
                }
            }

            res.Score += scoreModel.ChequeBouncedScore;

            #endregion BouncedCheque


            //ClearedCheque
            #region ClearedCheque

            var clearedChequeReportList = new List<CreditReportModel>();
            var clearedChqCount = chq.SamaClearedChequesResponse.Cheques?.Count();
            var clearedChqAmount = chq.SamaClearedChequesResponse.Cheques?.Sum(e => e.Amount);
            if (clearedChqCount == null || clearedChqAmount <= 0 || clearedChqCount <= 0)
            {
                scoreModel.ChequeClearedScore = 130;

                clearedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد چک های رفع سو اثر شده",
                    ScoreValue = "130",
                    ScoreDescription = "مشتری چک رفع سو اثر شده ندارد."
                });
                clearedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک رفع سو اثر شده ندارد."
                });
                clearedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "مجموع مبالغ چک‌های رفع سوءاثر شده",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک رفع سو اثر شده ندارد."
                });
                clearedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین علت برگشت چک‌های رفع سوءاثر شده",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک رفع سوءاثر شده ندارد."
                });

                clearedChequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین نوع رفع سوءاثر چک",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک رفع سو اثر شده ندارد."
                });

            }
            else
            {

                var countOfAccounts = chq.SamaClearedChequesResponse.Cheques.Select(x => x.IBAN).Distinct().Count();

                var reason = new List<int>();
                var cType = new List<int>();
                var maxDifDays = 0;
                foreach (var ch in chq.SamaClearedChequesResponse.Cheques)
                {
                    reason.AddRange(ch.BouncedReason);
                    cType.Add(ch.ClearedType);

                    var clrMiladiDate = ToDateTimeMiladi(ch.ClearedDate);
                    var bncMiladiDate = ToDateTimeMiladi(ch.BouncedDate);
                    var difDays = ((DateTime)clrMiladiDate - (DateTime)bncMiladiDate).Days;
                    if (difDays > maxDifDays) maxDifDays = difDays;
                }

                if (clearedChqCount <= 1)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک‌های رفع سوء‌اثر شده",
                        ScoreValue = "20",
                        ScoreDescription = "تعداد چک های رفع سوء اثر شده مشتری 1 عدد است."
                    });

                    scoreModel.ChequeClearedScore += 20;
                }
                else if (clearedChqCount <= 2)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک‌های رفع سوء‌اثر شده",
                        ScoreValue = "18",
                        ScoreDescription = "تعداد چک های رفع سوء اثر شده مشتری 2 عدد است."
                    });

                    scoreModel.ChequeClearedScore += 18;
                }

                else if (clearedChqCount <= 4)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک‌های رفع سوء‌اثر شده",
                        ScoreValue = "15",
                        ScoreDescription = "تعداد چک های رفع سوء اثر شده مشتری 3 یا 4 عدد است."
                    });

                    scoreModel.ChequeClearedScore += 15;
                }
                else if (clearedChqCount <= 6)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک‌های رفع سوء‌اثر شده",
                        ScoreValue = "12",
                        ScoreDescription = "تعداد چک های رفع سوء اثر شده مشتری 5 یا 6 عدد است."
                    });

                    scoreModel.ChequeClearedScore += 12;
                }
                else
                {



                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد چک‌های رفع سوء‌اثر شده",
                        ScoreValue = "10",
                        ScoreDescription = "تعداد چک های رفع سوء اثر شده مشتری 7 عدد یا بیشتر است."
                    });

                    scoreModel.ChequeClearedScore += 10;
                }


                if (maxDifDays <= 30)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک",
                        ScoreValue = "37",
                        ScoreDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک کمتر از یک ماه بوده است."
                    });

                    scoreModel.ChequeClearedScore += 37;
                }
                else if (maxDifDays <= 90)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک",
                        ScoreValue = "35",
                        ScoreDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک بازه یک ماه تا سه ماه بوده است."
                    });

                    scoreModel.ChequeClearedScore += 35;
                }
                else if (maxDifDays <= 180)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک",
                        ScoreValue = "29",
                        ScoreDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک بازه سه تا شش ماه بوده است."
                    });

                    scoreModel.ChequeClearedScore += 29;
                }
                else if (maxDifDays <= 360)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک",
                        ScoreValue = "26",
                        ScoreDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک بازه شش تا ۱۲ ماه بوده است."
                    });

                    scoreModel.ChequeClearedScore += 26;
                }
                else
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک",
                        ScoreValue = "20",
                        ScoreDescription = "بیشترین فاصله زمانی بین رفع سوءاثر با زمان برگشت چک بیش از یک سال بوده است."
                    });

                    scoreModel.ChequeClearedScore += 20;
                }


                if (clearedChqAmount <= 40000000)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های رفع سوءاثر شده",
                        ScoreValue = "31",
                        ScoreDescription = "مجموع مبالغ چک‌های رفع سوء اثر شده مشتری، کمتر از 4 میلیون تومان است."
                    });

                    scoreModel.ChequeClearedScore += 31;
                }
                else if (clearedChqAmount <= 110000000)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های رفع سوءاثر شده",
                        ScoreValue = "24",
                        ScoreDescription = "مجموع مبالغ چک‌های رفع سوء اثر شده مشتری، بین 4 تا ۱۱ میلیون تومان است."
                    });

                    scoreModel.ChequeClearedScore += 24;
                }
                else if (clearedChqAmount <= 350000000)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های رفع سوءاثر شده",
                        ScoreValue = "19",
                        ScoreDescription = "مجموع مبالغ چک‌های رفع سوء اثر شده مشتری، بین 11 تا 35 میلیون تومان است."
                    });

                    scoreModel.ChequeClearedScore += 19;
                }
                else if (clearedChqAmount <= 1310000000)
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های رفع سوءاثر شده",
                        ScoreValue = "15",
                        ScoreDescription = "مجموع مبالغ چک‌های رفع سوء اثر شده مشتری، بین ۳۵ تا ۱۳۱ میلیون تومان است."
                    });

                    scoreModel.ChequeClearedScore += 15;
                }
                else
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ چک های رفع سوءاثر شده",
                        ScoreValue = "12",
                        ScoreDescription = "مجموع مبالغ چک‌های رفع سوء اثر شده مشتری، بیش از ۱۳۱ میلیون تومان است."
                    });

                    scoreModel.ChequeClearedScore += 12;
                }


                if (reason.Any(o => o == 403))
                {

                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های رفع سوءاثر شده",
                        ScoreValue = "3",
                        ScoreDescription = "عدم موجودی"
                    });

                    scoreModel.ChequeClearedScore += 3;
                }
                else if (reason.Any(o => o == 402))
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های رفع سوءاثر شده",
                        ScoreValue = "5",
                        ScoreDescription = "کسری موجودی"
                    });

                    scoreModel.ChequeClearedScore += 5;
                }

                else if (reason.Any(o => o == 412 || o == 411))
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های رفع سوءاثر شده",
                        ScoreValue = "9",
                        ScoreDescription = "حساب مسدود یا ماده 14"
                    });

                    scoreModel.ChequeClearedScore += 9;
                }
                else
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین علت برگشت چک‌های رفع سوءاثر شده",
                        ScoreValue = "13",
                        ScoreDescription = "حساب بسته، عدم تطابق امضا و غیره"
                    });

                    scoreModel.ChequeClearedScore += 13;
                }


                if (cType.Any(o => o == 4 || o == 6 || o == 7))
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین نوع رفع سوءاثر چک",
                        ScoreValue = "2",
                        ScoreDescription = "بخش نامه بانکی، بخش نامه بانک مرکزی یا مشمول مرور زمان"
                    });

                    scoreModel.ChequeClearedScore += 2;
                }
                else if (cType.Any(o => o == 5))
                {

                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین نوع رفع سوءاثر چک",
                        ScoreValue = "4",
                        ScoreDescription = "دستور قاضی"
                    });

                    scoreModel.ChequeClearedScore += 4;
                }
                else if (cType.Any(o => o == 3))
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین نوع رفع سوءاثر چک",
                        ScoreValue = "7",
                        ScoreDescription = "مسدودی مبلغ یک ساله چک"
                    });

                    scoreModel.ChequeClearedScore += 7;
                }
                else if (cType.Any(o => o == 2))
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین نوع رفع سوءاثر چک",
                        ScoreValue = "9",
                        ScoreDescription = "رضایتنامه محضری"
                    });

                    scoreModel.ChequeClearedScore += 9;
                }
                else
                {


                    clearedChequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین نوع رفع سوءاثر چک",
                        ScoreValue = "10",
                        ScoreDescription = "استرداد لاشه چک"
                    });

                    scoreModel.ChequeClearedScore += 10;
                }
            }

            res.Score += scoreModel.ChequeClearedScore;

            #endregion ClearedCheque


            //FacilityAsli
            #region FacilityAsli

            var lstFacAsliItm = new List<SamatEstelamAsliRow>();
            var facilityAsliReportList = new List<CreditReportModel>();

            var numberOfFacilitiesAsli = 0;
            var totalAmTashilatAsli = 0L;
            var totalAmOriginalAsli = 0L;
            var totalAmBedehiAsli = 0L;
            var totalAmMashkukAsli = 0L;
            var totalAmMoavaghAsli = 0L;
            var totalAmSarResidAsli = 0L;

            if (fac.CbiSamatEstelamAsli2Response.ReturnValue.EstelamAsliRows != null && fac.CbiSamatEstelamAsli2Response.ReturnValue.EstelamAsliRows.Any())
            {
                foreach (var fcccc in fac.CbiSamatEstelamAsli2Response.ReturnValue.EstelamAsliRows)
                {
                    try
                    {
                        //changed to 1000000
                        if (StringToLong(fcccc.AmBedehiKol) >= 1000000 && StringToLong(fcccc.AmOriginal) >= 1000000)
                        {
                            lstFacAsliItm.Add(fcccc);
                            numberOfFacilitiesAsli += 1;
                            totalAmTashilatAsli += StringToLong(fcccc.AmBenefit) + StringToLong(fcccc.AmOriginal);
                            totalAmOriginalAsli += StringToLong(fcccc.AmOriginal);
                            totalAmBedehiAsli += StringToLong(fcccc.AmBedehiKol);
                            totalAmMashkukAsli += StringToLong(fcccc.AmMashkuk);
                            totalAmMoavaghAsli += StringToLong(fcccc.AmMoavagh);
                            totalAmSarResidAsli += StringToLong(fcccc.AmSarResid);
                        }
                    }
                    catch
                    {
                        //fac.FacilityInformationItems.Remove(fcccc);
                    }
                }
            }

            //changed to 1000000
            if (numberOfFacilitiesAsli <= 0 || totalAmOriginalAsli <= 1000000)
            {
                scoreModel.FacilityAsliScore = 153;

                facilityAsliReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد تسهیلات در حال بازپرداخت",
                    ScoreValue = "153",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityAsliReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityAsliReportList.Add(new CreditReportModel
                {
                    VariableDescription = "مجموع مبالغ تسهیلات در حال بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityAsliReportList.Add(new CreditReportModel
                {
                    VariableDescription = "اقساط دارای دیرکرد در بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityAsliReportList.Add(new CreditReportModel
                {
                    VariableDescription = "اقساط باقیمانده از تسهیلات",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityAsliReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد تسهیلات با دیرکرد در بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
            }
            else
            {


                var cntMashkuk = 0;
                var cntMoavagh = 0;
                var cntSarResid = 0;
                var badFac = lstFacAsliItm.Where(o => o.AmMashkuk != "0" || o.AmMoavagh != "0" || o.AmSarResid != "0").OrderByDescending(o => o.AmMashkuk).ThenByDescending(o => o.AmMoavagh).ThenByDescending(o => o.AmSarResid).ToList();
                if (badFac.Any())
                {
                    foreach (var fFacilityInformationItem in badFac)
                    {
                        if (fFacilityInformationItem.AmMashkuk != "0")
                        {
                            cntMashkuk++;
                        }
                        else if (fFacilityInformationItem.AmMoavagh != "0")
                        {
                            cntMoavagh++;
                        }
                        else
                        {
                            cntSarResid++;
                        }
                    }

                }



                if (numberOfFacilitiesAsli <= 1)
                {

                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات در حال بازپرداخت",
                        ScoreValue = "38",
                        ScoreDescription = "تعداد وام‌های در حال بازپرداخت 1 مورد است."
                    });

                    scoreModel.FacilityAsliScore += 38;
                }
                else if (numberOfFacilitiesAsli <= 2)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات در حال بازپرداخت",
                        ScoreValue = "43",
                        ScoreDescription = "تعداد وام‌های در حال بازپرداخت 2 مورد است."
                    });

                    scoreModel.FacilityAsliScore += 43;
                }
                else if (numberOfFacilitiesAsli <= 3)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات در حال بازپرداخت",
                        ScoreValue = "31",
                        ScoreDescription = "تعداد وام‌های در حال بازپرداخت 3 مورد است."
                    });

                    scoreModel.FacilityAsliScore += 31;
                }
                else if (numberOfFacilitiesAsli <= 4)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات در حال بازپرداخت",
                        ScoreValue = "27",
                        ScoreDescription = "تعداد وام‌های در حال بازپرداخت 4 مورد است."
                    });

                    scoreModel.FacilityAsliScore += 27;
                }
                else
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات در حال بازپرداخت",
                        ScoreValue = "20",
                        ScoreDescription = "تعداد وام‌های در حال بازپرداخت ۵ مورد یا بیشتر است."
                    });

                    scoreModel.FacilityAsliScore += 20;
                }


                if (totalAmMashkukAsli > 0)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات",
                        ScoreValue = "-61",
                        ScoreDescription = "بیشترین تاخیر در پرداخت اقساط بیشتر از 18 ماه بوده است."
                    });

                    scoreModel.FacilityAsliScore += -61;
                }
                else if (totalAmMoavaghAsli > 0)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات",
                        ScoreValue = "-49",
                        ScoreDescription = "بیشترین تاخیر در پرداخت اقساط بین 6 تا 18 ماه بوده است."
                    });

                    scoreModel.FacilityAsliScore += -49;
                }
                else if (totalAmSarResidAsli > 0)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات",
                        ScoreValue = "-37",
                        ScoreDescription = "بیشترین تاخیر در پرداخت اقساط بین 2 تا 6 ماه بوده است."
                    });

                    scoreModel.FacilityAsliScore += -37;
                }
                else
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات",
                        ScoreValue = "91",
                        ScoreDescription = "درحال حاضر، اقساط تمامی وام‌ها بازپرداخت شده است."
                    });

                    scoreModel.FacilityAsliScore += 91;
                }

                if (totalAmTashilatAsli <= 280000000)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات در حال بازپرداخت",
                        ScoreValue = "31",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال بازپرداخت در کل بانک‌های کشور کمتر از 28 میلیون تومان است."
                    });

                    scoreModel.FacilityAsliScore += 31;
                }
                else if (totalAmTashilatAsli <= 600000000)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات در حال بازپرداخت",
                        ScoreValue = "42",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال بازپرداخت در کل بانک‌های کشور بین 28 تا 60 میلیون تومان است."
                    });

                    scoreModel.FacilityAsliScore += 42;
                }
                else if (totalAmTashilatAsli <= 1200000000)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات در حال بازپرداخت",
                        ScoreValue = "60",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال بازپرداخت در کل بانک‌های کشور بین 60 تا 120 میلیون تومان است."
                    });

                    scoreModel.FacilityAsliScore += 60;
                }
                else if (totalAmTashilatAsli <= 2150000000)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات در حال بازپرداخت",
                        ScoreValue = "35",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال بازپرداخت در کل بانک‌های کشور بین 120 تا 215 میلیون تومان است."
                    });

                    scoreModel.FacilityAsliScore += 35;
                }
                else
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات در حال بازپرداخت",
                        ScoreValue = "29",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال بازپرداخت در کل بانک‌های کشور بیش از 215 میلیون تومان است."
                    });

                    scoreModel.FacilityAsliScore += 29;
                }


                var pMbDtoAll = (100 * (double)(totalAmMashkukAsli + totalAmMoavaghAsli + totalAmSarResidAsli)) / (double)totalAmTashilatAsli;
                if (pMbDtoAll <= 0)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط دارای دیرکرد در بازپرداخت",
                        ScoreValue = "31",
                        ScoreDescription = "درحال حاضر، اقساط بازپرداخت نشده وجود ندارد."
                    });

                    scoreModel.FacilityAsliScore += 31;
                }
                else if (pMbDtoAll <= 35)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط دارای دیرکرد در بازپرداخت",
                        ScoreValue = "-35",
                        ScoreDescription = "موعد بازپرداختِ تا ۳۵ درصد از اقساط گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityAsliScore += -35;
                }
                else if (pMbDtoAll <= 70)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط دارای دیرکرد در بازپرداخت",
                        ScoreValue = "-41",
                        ScoreDescription = "موعد بازپرداختِ 35 تا 70 درصد از اقساط گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityAsliScore += -41;
                }
                else if (pMbDtoAll <= 90)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط دارای دیرکرد در بازپرداخت",
                        ScoreValue = "-49",
                        ScoreDescription = "موعد بازپرداختِ 70 تا 90 درصد از اقساط گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityAsliScore += -49;
                }
                else
                {

                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط دارای دیرکرد در بازپرداخت",
                        ScoreValue = "-53",
                        ScoreDescription = "موعد بازپرداختِ بیش از 90 درصد از اقساط گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityAsliScore += -53;
                }


                var pMande = (100 * (double)totalAmBedehiAsli) / (double)totalAmTashilatAsli;
                if (pMande <= 25)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات",
                        ScoreValue = "22",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور بین 0 تا 25 درصد باقیمانده است."
                    });

                    scoreModel.FacilityAsliScore += 22;
                }
                else if (pMande <= 50)
                {

                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات",
                        ScoreValue = "18",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بین 25 تا 50 درصد باقیمانده است."
                    });

                    scoreModel.FacilityAsliScore += 18;
                }
                else if (pMande <= 75)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات",
                        ScoreValue = "12",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بین 50 تا 75 درصد باقیمانده است."
                    });

                    scoreModel.FacilityAsliScore += 12;
                }
                else if (pMande <= 100)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات",
                        ScoreValue = "9",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بین 75 تا 100 درصد باقیمانده است."
                    });

                    scoreModel.FacilityAsliScore += 9;
                }
                else
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات",
                        ScoreValue = "4",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بیش از 100 درصد باقیمانده است."
                    });

                    scoreModel.FacilityAsliScore += 4;
                }


                var cTbDdP =
                    lstFacAsliItm.Count(
                        o =>
                            (o.AmMashkuk != null && o.AmMashkuk != "0") ||
                            (o.AmMoavagh != null && o.AmMoavagh != "0") ||
                            (o.AmSarResid != null && o.AmSarResid != "0"));

                if (cTbDdP <= 0)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات با دیرکرد در بازپرداخت",
                        ScoreValue = "16",
                        ScoreDescription = "درحال حاضر، تاخیری در بازپرداخت وام‌ها مشاهده نشده است."
                    });

                    scoreModel.FacilityAsliScore += 16;
                }
                else if (cTbDdP <= 1)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات با دیرکرد در بازپرداخت",
                        ScoreValue = "-11",
                        ScoreDescription = "در بازپرداخت 1 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityAsliScore += -11;
                }
                else if (cTbDdP <= 2)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات با دیرکرد در بازپرداخت",
                        ScoreValue = "-13",
                        ScoreDescription = "در بازپرداخت 2 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityAsliScore += -13;
                }
                else if (cTbDdP <= 3)
                {


                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات با دیرکرد در بازپرداخت",
                        ScoreValue = "-15",
                        ScoreDescription = "در بازپرداخت 3 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityAsliScore += -15;
                }
                else
                {

                    facilityAsliReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات با دیرکرد در بازپرداخت",
                        ScoreValue = "-17",
                        ScoreDescription = "در بازپرداخت 4 مورد یا بیشتر از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityAsliScore += -17;
                }
            }

            res.Score += scoreModel.FacilityAsliScore;

            #endregion FacilityAsli


            //FacilityTasvieh
            #region FacilityTasvieh

            var lstFacTasviehItm = new List<SamatEstelamTasviehRow>();
            var facilityTasviehReportList = new List<CreditReportModel>();

            var numberOfFacilitiesTasvieh = 0;
            var numberOfEmhalFacilitiesTasvieh = 0;
            var totalAmTashilatTasvieh = 0L;
            var totalAmOriginalTasvieh = 0L;
            var totalAmBedehiTasvieh = 0L;
            var totalAmMashkukTasvieh = 0L;
            var totalAmMoavaghTasvieh = 0L;
            var totalAmSarResidTasvieh = 0L;

            if (fac.CbiSamatEstelamTasviehResponse.ReturnValue.EstelamTasviehRow != null && fac.CbiSamatEstelamTasviehResponse.ReturnValue.EstelamTasviehRow.Any())
            {
                foreach (var fcccc in fac.CbiSamatEstelamTasviehResponse.ReturnValue.EstelamTasviehRow)
                {
                    try
                    {
                        lstFacTasviehItm.Add(fcccc);
                        if (fcccc.Estehal != "0") numberOfEmhalFacilitiesTasvieh++;
                        numberOfFacilitiesTasvieh += 1;
                        totalAmTashilatTasvieh += StringToLong(fcccc.AmBenefit) + StringToLong(fcccc.AmOriginal);
                        totalAmOriginalTasvieh += StringToLong(fcccc.AmOriginal);
                        totalAmBedehiTasvieh += StringToLong(fcccc.AmBedehiKol);
                        totalAmMashkukTasvieh += StringToLong(fcccc.AmMashkuk);
                        totalAmMoavaghTasvieh += StringToLong(fcccc.AmMoavagh);
                        totalAmSarResidTasvieh += StringToLong(fcccc.AmSarResid);
                    }
                    catch
                    {
                        //fac.FacilityInformationItems.Remove(fcccc);
                    }
                }
            }

            if (numberOfFacilitiesTasvieh <= 0)
            {
                scoreModel.FacilityTasviehScore = 58;

                facilityTasviehReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد تعهدات تسویه شده",
                    ScoreValue = "58",
                    ScoreDescription = "هیچ تعهد تسویه شده‌ای در نظام بانکی کشور یافت نشد."
                });
                facilityTasviehReportList.Add(new CreditReportModel
                {
                    VariableDescription = "مجموع مبالغ تعهدات تسویه شده",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تعهد تسویه شده‌ای در نظام بانکی کشور یافت نشد."
                });
                facilityTasviehReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین وضعیت در بازپرداخت تعهدات تسویه شده",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تعهد تسویه شده‌ای در نظام بانکی کشور یافت نشد."
                });
                facilityTasviehReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد امهال تعهدات",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تعهد تسویه شده‌ای در نظام بانکی کشور یافت نشد."
                });
                facilityTasviehReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد تعهدات تسویه شده با دیرکرد در بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تعهد تسویه شده‌ای در نظام بانکی کشور یافت نشد."
                });

            }
            else
            {


                var cntMashkukTasvieh = 0;
                var cntMoavaghTasvieh = 0;
                var cntSarResidTasvieh = 0;
                var badFacTasvieh = lstFacTasviehItm.Where(o => o.AmMashkuk != "0" || o.AmMoavagh != "0" || o.AmSarResid != "0").OrderByDescending(o => o.AmMashkuk).ThenByDescending(o => o.AmMoavagh).ThenByDescending(o => o.AmSarResid).ToList();
                if (badFacTasvieh.Any())
                {
                    foreach (var fFacilityInformationItem in badFacTasvieh)
                    {
                        if (fFacilityInformationItem.AmMashkuk != "0")
                        {
                            cntMashkukTasvieh++;
                        }
                        else if (fFacilityInformationItem.AmMoavagh != "0")
                        {
                            cntMoavaghTasvieh++;
                        }
                        else
                        {
                            cntSarResidTasvieh++;
                        }
                    }
                }

                if (numberOfFacilitiesTasvieh <= 1)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده",
                        ScoreValue = "11",
                        ScoreDescription = "تعداد تعهدات تسویه شده 1 مورد است."
                    });

                    scoreModel.FacilityTasviehScore += 11;
                }
                else if (numberOfFacilitiesTasvieh <= 2)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده",
                        ScoreValue = "12",
                        ScoreDescription = "تعداد تعهدات تسویه شده 2 مورد است."
                    });

                    scoreModel.FacilityTasviehScore += 12;
                }
                else if (numberOfFacilitiesTasvieh <= 4)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده",
                        ScoreValue = "14",
                        ScoreDescription = "تعداد تعهدات تسویه شده 3 یا ۴ مورد است."
                    });

                    scoreModel.FacilityTasviehScore += 14;
                }
                else if (numberOfFacilitiesTasvieh <= 6)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده",
                        ScoreValue = "16",
                        ScoreDescription = "تعداد تعهدات تسویه شده 5 یا ۶ مورد است."
                    });

                    scoreModel.FacilityTasviehScore += 16;
                }
                else
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده",
                        ScoreValue = "18",
                        ScoreDescription = "تعداد تعهدات تسویه شده ۷ مورد یا بیشتر است."
                    });

                    scoreModel.FacilityTasviehScore += 18;
                }


                if (totalAmTashilatTasvieh <= 150000000)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تعهدات تسویه شده",
                        ScoreValue = "20",
                        ScoreDescription = "مجموع مبالغ تعهدات تسویه شده در کل بانک‌های کشور کمتر از 15 میلیون تومان است."
                    });

                    scoreModel.FacilityTasviehScore += 20;
                }
                else if (totalAmTashilatTasvieh <= 400000000)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تعهدات تسویه شده",
                        ScoreValue = "26",
                        ScoreDescription = "مجموع مبالغ تعهدات تسویه شده در کل بانک‌های کشور 15 تا 40 میلیون تومان است."
                    });

                    scoreModel.FacilityTasviehScore += 26;
                }
                else if (totalAmTashilatTasvieh <= 730000000)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تعهدات تسویه شده",
                        ScoreValue = "39",
                        ScoreDescription = "مجموع مبالغ تعهدات تسویه شده در کل بانک‌های کشور 40 تا 73 میلیون تومان است."
                    });

                    scoreModel.FacilityTasviehScore += 39;
                }
                else if (totalAmTashilatTasvieh <= 1600000000)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تعهدات تسویه شده",
                        ScoreValue = "48",
                        ScoreDescription = "مجموع مبالغ تعهدات تسویه شده در کل بانک‌های کشور 73 تا 160 میلیون تومان است."
                    });

                    scoreModel.FacilityTasviehScore += 48;
                }
                else
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تعهدات تسویه شده",
                        ScoreValue = "51",
                        ScoreDescription = "مجموع مبالغ تعهدات تسویه شده در کل بانک‌های کشور بیش از 160 میلیون تومان است."
                    });

                    scoreModel.FacilityTasviehScore += 51;
                }


                if (totalAmMashkukTasvieh > 0)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تعهدات تسویه شده",
                        ScoreValue = "3",
                        ScoreDescription = "بیشترین تاخیر در بازپرداخت اقساط تعهدات تسویه شده بیشتر از 18 ماه بوده است."
                    });

                    scoreModel.FacilityTasviehScore += 3;
                }
                else if (totalAmMoavaghTasvieh > 0)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تعهدات تسویه شده",
                        ScoreValue = "7",
                        ScoreDescription = "بیشترین تاخیر در بازپرداخت اقساط تعهدات تسویه شده بین 6 تا 18 ماه بوده است."
                    });

                    scoreModel.FacilityTasviehScore += 7;
                }
                else if (totalAmSarResidTasvieh > 0)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تعهدات تسویه شده",
                        ScoreValue = "15",
                        ScoreDescription = "بیشترین تاخیر در بازپرداخت اقساط تعهدات تسویه شده بین 2 تا 6 ماه بوده است."
                    });

                    scoreModel.FacilityTasviehScore += 15;
                }
                else
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تعهدات تسویه شده",
                        ScoreValue = "31",
                        ScoreDescription = "اقساط تمامی تعهدات تسویه شده بازپرداخت شده است."
                    });

                    scoreModel.FacilityTasviehScore += 31;
                }

                if (numberOfEmhalFacilitiesTasvieh <= 0)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد امهال تعهدات",
                        ScoreValue = "11",
                        ScoreDescription = "هیچ امهالی در تعهدات تسویه شده وجود ندارد."
                    });

                    scoreModel.FacilityTasviehScore += 11;
                }
                else if (numberOfEmhalFacilitiesTasvieh <= 1)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد امهال تعهدات",
                        ScoreValue = "9",
                        ScoreDescription = "در بازپرداخت 1 مورد تعهد تسویه شده امهال مشاهده شده‌است."
                    });

                    scoreModel.FacilityTasviehScore += 9;
                }
                else if (numberOfEmhalFacilitiesTasvieh <= 2)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد امهال تعهدات",
                        ScoreValue = "6",
                        ScoreDescription = "در بازپرداخت 2 مورد تعهد تسویه شده امهال مشاهده شده‌است."
                    });

                    scoreModel.FacilityTasviehScore += 6;
                }
                else if (numberOfEmhalFacilitiesTasvieh <= 3)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد امهال تعهدات",
                        ScoreValue = "2",
                        ScoreDescription = "در بازپرداخت 3 مورد تعهد تسویه شده امهال مشاهده شده‌است."
                    });

                    scoreModel.FacilityTasviehScore += 2;
                }
                else
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد امهال تعهدات",
                        ScoreValue = "0",
                        ScoreDescription = "در بازپرداخت 4 مورد و بیشتر تعهد تسویه شده امهال مشاهده شده‌است."
                    });

                    scoreModel.FacilityTasviehScore += 0;
                }

                var cTbDdPTasvieh =
                    lstFacTasviehItm.Count(
                        o =>
                            (o.AmMashkuk != null && o.AmMashkuk != "0") ||
                            (o.AmMoavagh != null && o.AmMoavagh != "0") ||
                            (o.AmSarResid != null && o.AmSarResid != "0"));

                if (cTbDdPTasvieh <= 0)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده با دیرکرد در بازپرداخت",
                        ScoreValue = "7",
                        ScoreDescription = "تاخیری در بازپرداخت تعهدات تسویه شده مشاهده نشده است."
                    });

                    scoreModel.FacilityTasviehScore += 7;
                }
                else if (cTbDdPTasvieh <= 1)
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده با دیرکرد در بازپرداخت",
                        ScoreValue = "6",
                        ScoreDescription = "در بازپرداخت 1 مورد از تعهدات تسویه شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityTasviehScore += 6;
                }
                else if (cTbDdPTasvieh <= 2)
                {


                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده با دیرکرد در بازپرداخت",
                        ScoreValue = "3",
                        ScoreDescription = "در بازپرداخت 2 مورد از تعهدات تسویه شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityTasviehScore += 3;
                }
                else
                {

                    facilityTasviehReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تعهدات تسویه شده با دیرکرد در بازپرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "در بازپرداخت ۳ مورد یا بیشتر از تعهدات تسویه شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityTasviehScore += 0;
                }


            }

            res.Score += scoreModel.FacilityTasviehScore;
            #endregion FacilityTasvieh


            //FacilityZamen
            #region FacilityZamen

            var lstFacZamenItm = new List<SamatEstelamZamenRow>();
            var facilityZamenReportList = new List<CreditReportModel>();

            var numberOfFacilitiesZamen = 0;
            var totalAmTashilatZamen = 0L;
            var totalAmOriginalZamen = 0L;
            var totalAmBedehiZamen = 0L;
            var totalAmMashkukZamen = 0L;
            var totalAmMoavaghZamen = 0L;
            var totalAmSarResidZamen = 0L;

            if (fac.CbiSamatEstelamZamenResponse.ReturnValue.EstelamZamenRows != null && fac.CbiSamatEstelamZamenResponse.ReturnValue.EstelamZamenRows.Any())
            {
                foreach (var fcccc in fac.CbiSamatEstelamZamenResponse.ReturnValue.EstelamZamenRows)
                {
                    try
                    {
                        if (StringToLong(fcccc.AmBedehiKol) >= 1000000 && StringToLong(fcccc.AmOriginal) >= 1000000)
                        {
                            lstFacZamenItm.Add(fcccc);
                            numberOfFacilitiesZamen += 1;
                            totalAmTashilatZamen += StringToLong(fcccc.AmBenefit) + StringToLong(fcccc.AmOriginal);
                            totalAmOriginalZamen += StringToLong(fcccc.AmOriginal);
                            totalAmBedehiZamen += StringToLong(fcccc.AmBedehiKol);
                            totalAmMashkukZamen += StringToLong(fcccc.AmMashkuk);
                            totalAmMoavaghZamen += StringToLong(fcccc.AmMoavagh);
                            totalAmSarResidZamen += StringToLong(fcccc.AmSarResid);
                        }
                    }
                    catch
                    {
                        //fac.FacilityInformationItems.Remove(fcccc);
                    }
                }
            }

            if (numberOfFacilitiesZamen <= 0 || totalAmTashilatZamen <= 1000000)
            {
                scoreModel.FacilityZamenScore = 84;

                facilityZamenReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت",
                    ScoreValue = "84",
                    ScoreDescription = "هیچ تسهیلاتی در نظام بانکی کشور توسط مشتری ضمانت نشده‌است."
                });
                facilityZamenReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات ضمانت شده",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تسهیلاتی در نظام بانکی کشور توسط مشتری ضمانت نشده‌است."
                });
                facilityZamenReportList.Add(new CreditReportModel
                {
                    VariableDescription = "اقساط وام‌های ضمانت شده دارای دیرکرد در بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تسهیلاتی در نظام بانکی کشور توسط مشتری ضمانت نشده‌است."
                });
                facilityZamenReportList.Add(new CreditReportModel
                {
                    VariableDescription = "تعداد تسهیلات ضمانت شده با دیرکرد در بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تسهیلاتی در نظام بانکی کشور توسط مشتری ضمانت نشده‌است."
                });
                facilityZamenReportList.Add(new CreditReportModel
                {
                    VariableDescription = "اقساط باقیمانده از تسهیلات ضمانت شده",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تسهیلاتی در نظام بانکی کشور توسط مشتری ضمانت نشده‌است."
                });
                facilityZamenReportList.Add(new CreditReportModel
                {
                    VariableDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ تسهیلاتی در نظام بانکی کشور توسط مشتری ضمانت نشده‌است."
                });
            }
            else
            {


                var cntMashkukZamen = 0;
                var cntMoavaghZamen = 0;
                var cntSarResidZamen = 0;
                var badFacZamen = lstFacZamenItm.Where(o => o.AmMashkuk != "0" || o.AmMoavagh != "0" || o.AmSarResid != "0").OrderByDescending(o => o.AmMashkuk).ThenByDescending(o => o.AmMoavagh).ThenByDescending(o => o.AmSarResid).ToList();

                if (badFacZamen.Any())
                {
                    foreach (var fFacilityInformationItem in badFacZamen)
                    {
                        if (fFacilityInformationItem.AmMashkuk != "0")
                        {
                            cntMashkukZamen++;
                        }
                        else if (fFacilityInformationItem.AmMoavagh != "0")
                        {
                            cntMoavaghZamen++;
                        }
                        else
                        {
                            cntSarResidZamen++;
                        }
                    }

                }

                if (numberOfFacilitiesZamen <= 1)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "4",
                        ScoreDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت 1 مورد است."
                    });

                    scoreModel.FacilityZamenScore += 4;
                }
                else if (numberOfFacilitiesZamen <= 2)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "3",
                        ScoreDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت 2 مورد است."
                    });

                    scoreModel.FacilityZamenScore += 3;
                }
                else if (numberOfFacilitiesZamen <= 3)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "2",
                        ScoreDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت 3 مورد است."
                    });

                    scoreModel.FacilityZamenScore += 2;
                }
                else if (numberOfFacilitiesZamen <= 4)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "1",
                        ScoreDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت 4 مورد است."
                    });

                    scoreModel.FacilityZamenScore += 1;
                }
                else
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد تسهیلات ضمانت شده در حال بازپرداخت ۵ مورد یا بیشتر است."
                    });

                    scoreModel.FacilityZamenScore += 0;
                }


                if (totalAmMashkukZamen > 0)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات ضمانت شده",
                        ScoreValue = "5",
                        ScoreDescription = "بیشترین تاخیر در بازپرداخت اقساط تسهیلات ضمانت شده بیشتر از 18 ماه بوده است."
                    });

                    scoreModel.FacilityZamenScore += 5;
                }
                else if (totalAmMoavaghZamen > 0)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات ضمانت شده",
                        ScoreValue = "11",
                        ScoreDescription = "بیشترین تاخیر در بازپرداخت اقساط تسهیلات ضمانت شده بین 6 تا 18 ماه بوده است."
                    });

                    scoreModel.FacilityZamenScore += 11;
                }
                else if (totalAmSarResidZamen > 0)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات ضمانت شده",
                        ScoreValue = "16",
                        ScoreDescription = "بیشترین تاخیر در بازپرداخت اقساط تسهیلات ضمانت شده بین 2 تا 6 ماه بوده است."
                    });

                    scoreModel.FacilityZamenScore += 16;
                }
                else
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در بازپرداخت تسهیلات ضمانت شده",
                        ScoreValue = "26",
                        ScoreDescription = "درحال حاضر، اقساط تمامی تسهیلات ضمانت شده بازپرداخت شده است."
                    });

                    scoreModel.FacilityZamenScore += 26;
                }


                var pMbDtoAllZamen = (100 * (double)(totalAmMashkukZamen + totalAmMoavaghZamen + totalAmSarResidZamen)) / (double)totalAmTashilatZamen;
                if (pMbDtoAllZamen <= 0)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط وام‌های ضمانت شده دارای دیرکرد در بازپرداخت",
                        ScoreValue = "15",
                        ScoreDescription = "درحال حاضر، اقساط بازپرداخت نشده در تسهیلات ضمانت شده وجود ندارد."
                    });

                    scoreModel.FacilityZamenScore += 15;
                }
                else if (pMbDtoAllZamen <= 35)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط وام‌های ضمانت شده دارای دیرکرد در بازپرداخت",
                        ScoreValue = "12",
                        ScoreDescription = "موعد بازپرداختِ تا ۳۵ درصد از اقساط تسهیلات ضمانت شده گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityZamenScore += 12;
                }
                else if (pMbDtoAllZamen <= 70)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط وام‌های ضمانت شده دارای دیرکرد در بازپرداخت",
                        ScoreValue = "8",
                        ScoreDescription = "موعد بازپرداختِ 35 تا 70 درصد از اقساط تسهیلات ضمانت شده گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityZamenScore += 8;
                }
                else if (pMbDtoAllZamen <= 90)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط وام‌های ضمانت شده دارای دیرکرد در بازپرداخت",
                        ScoreValue = "6",
                        ScoreDescription = "موعد بازپرداختِ 70 تا 90 درصد از اقساط تسهیلات ضمانت شده گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityZamenScore += 6;
                }
                else
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط وام‌های ضمانت شده دارای دیرکرد در بازپرداخت",
                        ScoreValue = "4",
                        ScoreDescription = "موعد بازپرداختِ بیش از 90 درصد از اقساط تسهیلات ضمانت شده گذشته است و هنوز بازپرداخت نشده اند."
                    });

                    scoreModel.FacilityZamenScore += 4;
                }

                var cTbDdPZamen =
                    lstFacZamenItm.Count(
                        o =>
                            (o.AmMashkuk != null && o.AmMashkuk != "0") ||
                            (o.AmMoavagh != null && o.AmMoavagh != "0") ||
                            (o.AmSarResid != null && o.AmSarResid != "0"));

                if (cTbDdPZamen <= 0)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده با دیرکرد در بازپرداخت",
                        ScoreValue = "11",
                        ScoreDescription = "درحال حاضر، تاخیری در بازپرداخت تسهیلات ضمانت شده مشاهده نشده است."
                    });

                    scoreModel.FacilityZamenScore += 11;
                }
                else if (cTbDdPZamen <= 1)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده با دیرکرد در بازپرداخت",
                        ScoreValue = "7",
                        ScoreDescription = "در بازپرداخت 1 مورد از تسهیلات ضمانت شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityZamenScore += 7;
                }
                else if (cTbDdPZamen <= 2)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده با دیرکرد در بازپرداخت",
                        ScoreValue = "4",
                        ScoreDescription = "در بازپرداخت 2 مورد از تسهیلات ضمانت شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityZamenScore += 4;
                }
                else if (cTbDdPZamen <= 3)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده با دیرکرد در بازپرداخت",
                        ScoreValue = "2",
                        ScoreDescription = "در بازپرداخت 3 مورد از تسهیلات ضمانت شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityZamenScore += 2;
                }
                else
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "تعداد تسهیلات ضمانت شده با دیرکرد در بازپرداخت",
                        ScoreValue = "1",
                        ScoreDescription = "در بازپرداخت 4 مورد یا بیشتر از تسهیلات ضمانت شده تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityZamenScore += 1;
                }

                var pMandeZamen = (100 * (double)totalAmBedehiZamen) / (double)totalAmTashilatZamen;
                if (pMandeZamen <= 25)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات ضمانت شده",
                        ScoreValue = "7",
                        ScoreDescription = "از اقساط تسهیلات ضمانت شده در کل بانک‌های کشور، بین 0 تا 25 درصد باقیمانده است."
                    });

                    scoreModel.FacilityZamenScore += 7;
                }
                else if (pMandeZamen <= 50)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات ضمانت شده",
                        ScoreValue = "5",
                        ScoreDescription = "از اقساط تسهیلات ضمانت شده در کل بانک‌های کشور، بین 25 تا 50 درصد باقیمانده است."
                    });

                    scoreModel.FacilityZamenScore += 5;
                }
                else if (pMandeZamen <= 75)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات ضمانت شده",
                        ScoreValue = "4",
                        ScoreDescription = "از اقساط تسهیلات ضمانت شده در کل بانک‌های کشور، بین 50 تا 75 درصد باقیمانده است."
                    });

                    scoreModel.FacilityZamenScore += 4;
                }
                else if (pMandeZamen <= 100)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات ضمانت شده",
                        ScoreValue = "3",
                        ScoreDescription = "از اقساط تسهیلات ضمانت شده در کل بانک‌های کشور، بین 75 تا 100 درصد باقیمانده است."
                    });

                    scoreModel.FacilityZamenScore += 3;
                }
                else
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از تسهیلات ضمانت شده",
                        ScoreValue = "1",
                        ScoreDescription = "از اقساط تسهیلات ضمانت شده در کل بانک‌های کشور، بیشتر از 100 درصد باقیمانده است."
                    });

                    scoreModel.FacilityZamenScore += 1;
                }

                if (totalAmTashilatZamen <= 280000000)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "4",
                        ScoreDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت در کل بانک‌های کشور کمتر از 28 میلیون تومان است."
                    });

                    scoreModel.FacilityZamenScore += 4;
                }
                else if (totalAmTashilatZamen <= 600000000)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "3",
                        ScoreDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت در کل بانک‌های کشور بین 28 تا 60 میلیون تومان است."
                    });

                    scoreModel.FacilityZamenScore += 3;
                }
                else if (totalAmTashilatZamen <= 1200000000)
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "2",
                        ScoreDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت در کل بانک‌های کشور بین 60 تا 120 میلیون تومان است."
                    });

                    scoreModel.FacilityZamenScore += 2;
                }
                else if (totalAmTashilatZamen <= 2150000000)
                {

                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "1",
                        ScoreDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت در کل بانک‌های کشور بین 120 تا 215 میلیون تومان است."
                    });

                    scoreModel.FacilityZamenScore += 1;
                }
                else
                {


                    facilityZamenReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "مجموع مبالغ تسهیلات ضمانت شده در حال بازپرداخت در کل بانک‌های کشور بیش از 215 میلیون تومان است."
                    });

                    scoreModel.FacilityZamenScore += 0;
                }
            }

            res.Score += scoreModel.FacilityZamenScore;
            #endregion FacilityZamen


            //CreditReportList
            #region CreditReportList

            if (bouncedChequeReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = bouncedChequeReportList,
                    CreditReportName = "وضعیت چک برگشتی",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.ChequeBouncedScore + " (سقف امتیاز وضعیت چک برگشتی ۳۴۵ است)"
                });
            }
            if (clearedChequeReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = clearedChequeReportList,
                    CreditReportName = "وضعیت چک رفع سوءاثر شده ",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.ChequeClearedScore + " (سقف امتیاز وضعیت چک رفع سوءاثر شده ۱۳۰ است)"
                });
            }
            if (facilityAsliReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = facilityAsliReportList,
                    CreditReportName = "وضعیت تسهیلات شخص",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.FacilityAsliScore + " (سقف امتیاز وضعیت تسهیلات شخص 263 است)"
                });
            }
            if (facilityTasviehReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = facilityTasviehReportList,
                    CreditReportName = "وضعیت تسهیلات تسویه شده",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.FacilityTasviehScore + " (سقف امتیاز وضعیت تعهدات تسویه شده 118 است)"
                });
            }
            if (facilityZamenReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = facilityZamenReportList,
                    CreditReportName = "وضعیت تسهیلات ضمانت شده",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.FacilityZamenScore + " (سقف امتیاز وضعیت تسهیلات ضمانت شده ۸۴ است)"
                });
            }

            #endregion CreditReportList

            return res;
        }


        public static ScoreResultModel CalculateLegalScoreV2(FullCheckInfo chq, FullFacilityInfo fac)
        {

            var res = new ScoreResultModel
            {
                Score = 0,
                DecisionStatus = false,
                ScoreModelXml = null,
                ListOfCreditReportList = new List<CreditReportFull>(),
            };
            var scoreModel = new ScoreLegalModelV2
            {
                ChequeScore = 0,
                FacilityScore = 0,

            };


            //Cheque
            #region Cheque
            var chequeReportList = new List<CreditReportModel>();
            var chqCount = chq.CheckCount ?? 0;
            var chqAmount = chq.CheckAmountSum ?? 0;
            if (chqAmount <= 0 || chqCount <= 0)
            {
                scoreModel.ChequeScore = 500;

                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت زمانی چک‌های برگشتی",
                    ScoreValue = "500",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک ‌برگشتی رفع‌ سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک ‌برگشتی رفع‌ سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک ‌برگشتی رفع‌ سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک ‌برگشتی رفع‌ سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
                chequeReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                    ScoreValue = "-",
                    ScoreDescription = "مشتری چک برگشتی رفع سوء اثر نشده ندارد."
                });
            }
            else
            {

                var lastDate = chq.CheckInformationItems.OrderByDescending(o => o.ReturnedDate).First().ReturnedDate;
                var threeMonth = DateTime.Now.AddDays(-3 * 30);
                var sixMonth = DateTime.Now.AddDays(-6 * 30);
                var oneYear = DateTime.Now.AddDays(-1 * 365);
                var twoYear = DateTime.Now.AddDays(-2 * 365);
                var inSixMonCount = chq.CheckInformationItems.Count(o => o.ReturnedDate > sixMonth && o.AMCHQ != null);
                long inSixMonAmount = 0;
                if (inSixMonCount > 0)
                {
                    inSixMonAmount = chq.CheckInformationItems.Where(o => o.ReturnedDate > sixMonth && o.AMCHQ != null).Sum(o => (long)o.AMCHQ);
                }
                var olderSixMonToTwoyCount = chq.CheckInformationItems.Count(o => o.ReturnedDate <= sixMonth && o.ReturnedDate > twoYear && o.AMCHQ != null);
                long olderSixMonToTwoyAmount = 0;
                if (olderSixMonToTwoyCount > 0)
                {
                    olderSixMonToTwoyAmount = chq.CheckInformationItems.Where(o => o.ReturnedDate <= sixMonth && o.ReturnedDate > twoYear && o.AMCHQ != null).Sum(o => (long)o.AMCHQ);
                }
                var olderTwoyCount = chq.CheckInformationItems.Count(o => o.ReturnedDate <= twoYear && o.AMCHQ != null);
                long olderTwoyAmount = 0;
                if (olderTwoyCount > 0)
                {
                    olderTwoyAmount = chq.CheckInformationItems.Where(o => o.ReturnedDate <= twoYear && o.AMCHQ != null).Sum(o => (long)o.AMCHQ);
                }
                var countOfAccounts = chq.CheckInformationItems.Select(x => x.ACCNTNO).Distinct().Count();

                if (lastDate > sixMonth)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت زمانی چک‌های برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در طول سه ماه گذشته بوده است."
                    });

                    scoreModel.ChequeScore += 0;
                }
                else if (lastDate > sixMonth)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت زمانی چک‌های برگشتی",
                        ScoreValue = "18",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه سه تا شش ماه گذشته بوده است."
                    });

                    scoreModel.ChequeScore += 18;
                }
                else if (lastDate > oneYear)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت زمانی چک‌های برگشتی",
                        ScoreValue = "46",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه شش ماه تا یک سال گذشته بوده است."
                    });

                    scoreModel.ChequeScore += 46;
                }
                else if (lastDate > twoYear)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت زمانی چک‌های برگشتی",
                        ScoreValue = "72",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری در بازه یک سال تا دو سال گذشته بوده است."
                    });

                    scoreModel.ChequeScore += 72;
                }
                else
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت زمانی چک‌های برگشتی",
                        ScoreValue = "93",
                        ScoreDescription = "آخرین چک برگشتی رفع سوء اثر نشده مشتری قبل از دو سال گذشته بوده است."
                    });

                    scoreModel.ChequeScore += 93;
                }

                if (inSixMonAmount <= 0)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "72",
                        ScoreDescription = "مشتری در شش ماه اخیر چک برگشتی رفع سوء اثر نشده ندارد."
                    });

                    scoreModel.ChequeScore += 72;
                }
                else if (inSixMonAmount <= 1000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "61",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، کمتر از 100 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 61;
                }
                else if (inSixMonAmount <= 2000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "54",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بین 100 تا 200 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 54;
                }
                else if (inSixMonAmount <= 5000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "50",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بین 200 تا 500 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 50;
                }
                else if (inSixMonAmount <= 10000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "36",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بین 500 تا 1,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 36;
                }
                else if (inSixMonAmount <= 50000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "14",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بین 1,000 تا 5,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 14;
                }
                else if (inSixMonAmount <= 100000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "7",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بین 5,000 تا 10,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 7;
                }
                else if (inSixMonAmount <= 500000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "3",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بین 10,000 تا 50,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 3;
                }
                else
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "0",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بیش از 50,000 میلیون تومان است. "
                    });

                    scoreModel.ChequeScore += 0;
                }

                if (inSixMonCount <= 0)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "82",
                        ScoreDescription = "مشتری در شش ماه اخیر چک برگشتی رفع سوء اثر نشده ندارد."
                    });

                    scoreModel.ChequeScore += 82;
                }
                else if (inSixMonCount <= 1)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "66",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، 1 است."
                    });

                    scoreModel.ChequeScore += 66;
                }
                else if (inSixMonCount <= 3)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "41",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، 2 یا 3 است."
                    });

                    scoreModel.ChequeScore += 41;
                }
                else if (inSixMonCount <= 6)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "16",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، 3 تا 6 است."
                    });

                    scoreModel.ChequeScore += 16;
                }
                else if (inSixMonCount <= 8)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "8",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، 6 تا 8 است."
                    });

                    scoreModel.ChequeScore += 8;
                }
                else
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی در شش ماه اخیر",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در شش ماه اخیر، بیش از 8 است."
                    });

                    scoreModel.ChequeScore += 0;
                }

                if (olderSixMonToTwoyAmount <= 0)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "48",
                        ScoreDescription = "مشتری در طول شش ماه تا دو سال اخیر چک برگشتی رفع سوء اثر نشده ندارد."
                    });

                    scoreModel.ChequeScore += 48;
                }
                else if (olderSixMonToTwoyAmount <= 1000000000)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "41",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، کمتر از 100 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 41;
                }
                else if (olderSixMonToTwoyAmount <= 2000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "36",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بین 100 تا 200 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 36;
                }
                else if (olderSixMonToTwoyAmount <= 5000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "33",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بین 200 تا 500 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 33;
                }
                else if (olderSixMonToTwoyAmount <= 10000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "24",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بین 500 تا 1,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 24;
                }
                else if (olderSixMonToTwoyAmount <= 50000000000)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "9",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بین 1,000 تا 5,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 9;
                }
                else if (olderSixMonToTwoyAmount <= 100000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "5",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بین 5,000 تا 10,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 5;
                }
                else if (olderSixMonToTwoyAmount <= 500000000000)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "2",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بین 10,000 تا 50,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 2;
                }
                else
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "0",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بیش از 50,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 0;
                }

                if (olderSixMonToTwoyCount <= 0)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "55",
                        ScoreDescription = "مشتری در طول شش ماه تا دو سال اخیر چک برگشتی رفع سوء اثر نشده ندارد."
                    });

                    scoreModel.ChequeScore += 55;
                }
                else if (olderSixMonToTwoyCount <= 1)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "44",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، 1 است."
                    });

                    scoreModel.ChequeScore += 44;
                }
                else if (olderSixMonToTwoyCount <= 3)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "27",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، 2 یا 3 است."
                    });

                    scoreModel.ChequeScore += 27;
                }
                else if (olderSixMonToTwoyCount <= 6)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "11",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، 3 تا 6 است."
                    });

                    scoreModel.ChequeScore += 11;
                }
                else if (olderSixMonToTwoyCount <= 8)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "5",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، 6 تا 8 است."
                    });

                    scoreModel.ChequeScore += 5;
                }
                else
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  بین ۶ تا ۲۴ ماه اخیر",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری در طول شش ماه تا دو سال اخیر، بیش از 8 است."
                    });

                    scoreModel.ChequeScore += 0;
                }

                if (olderTwoyAmount <= 0)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "22",
                        ScoreDescription = "مشتری قبل از دو سال اخیر چک برگشتی رفع سوء اثر نشده ندارد."
                    });

                    scoreModel.ChequeScore += 22;
                }
                else if (olderTwoyAmount <= 1000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "19",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، کمتر از 100 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 19;
                }
                else if (olderTwoyAmount <= 2000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "17",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بین 100 تا 200 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 17;
                }
                else if (olderTwoyAmount <= 5000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "15",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بین 200 تا 500 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 15;
                }
                else if (olderTwoyAmount <= 10000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "11",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بین 500 تا 1,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 11;
                }
                else if (olderTwoyAmount <= 50000000000)
                {


                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "7",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بین 1,000 تا 5,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 7;
                }
                else if (olderTwoyAmount <= 100000000000)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "4",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بین 5,000 تا 10,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 4;
                }
                else if (olderTwoyAmount <= 500000000000)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "1",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بین 10,000 تا 50,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 1;
                }
                else
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ چک های برگشتی قبل از دو سال اخیر",
                        ScoreValue = "0",
                        ScoreDescription = "مجموع مبالغ چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بیش از 50,000 میلیون تومان است."
                    });

                    scoreModel.ChequeScore += 0;
                }

                if (olderTwoyCount <= 0)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                        ScoreValue = "25",
                        ScoreDescription = "مشتری قبل از دو سال اخیر چک برگشتی رفع سوء اثر نشده ندارد."
                    });

                    scoreModel.ChequeScore += 25;
                }
                else if (olderTwoyCount <= 1)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                        ScoreValue = "20",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، 1 است."
                    });

                    scoreModel.ChequeScore += 20;
                }
                else if (olderTwoyCount <= 3)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                        ScoreValue = "13",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، 2 یا 3 است."
                    });

                    scoreModel.ChequeScore += 13;
                }
                else if (olderTwoyCount <= 6)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                        ScoreValue = "5",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، 3 تا 6 است."
                    });

                    scoreModel.ChequeScore += 5;
                }
                else if (olderTwoyCount <= 8)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                        ScoreValue = "2",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، 6 تا 8 است."
                    });

                    scoreModel.ChequeScore += 2;
                }
                else
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد چک های برگشتی  قبل از دو سال اخیر",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد چک های برگشتی رفع سوء اثر نشده مشتری قبل از دو سال اخیر، بیش از 8 است."
                    });

                    scoreModel.ChequeScore += 0;
                }

                if (countOfAccounts <= 1)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "103",
                        ScoreDescription = "مشتری تنها از یک حساب چک برگشتی دارد."
                    });

                    scoreModel.ChequeScore += 103;
                }
                else if (countOfAccounts <= 2)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "75",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده، 2 است."
                    });

                    scoreModel.ChequeScore += 75;
                }
                else if (countOfAccounts <= 3)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "55",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده، 3 است."
                    });

                    scoreModel.ChequeScore += 55;
                }
                else if (countOfAccounts <= 5)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "35",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده، 4 یا 5 است."
                    });

                    scoreModel.ChequeScore += 35;
                }
                else if (countOfAccounts <= 7)
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "15",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده، 6 یا 7 است."
                    });

                    scoreModel.ChequeScore += 15;
                }
                else
                {

                    chequeReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد حساب‌های دارای چک برگشتی",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد حساب‌های مشتری که از آن‌ها چک برگشتی صادر شده، بیش از 7 است."
                    });

                    scoreModel.ChequeScore += 0;
                }

            }
            res.Score += scoreModel.ChequeScore;

            #endregion Cheque


            //Facility
            #region Facility
            var lstFacItm = new List<FFacilityInformationItem>();
            var facilityReportList = new List<CreditReportModel>();
            var numberOfFacilities = 0;
            var totalAmTashilat = 0L;
            var totalAmOriginal = 0L;
            var totalAmBedehi = 0L;
            var totalAmMashkuk = 0L;
            var totalAmMoavagh = 0L;
            var totalAmSarResid = 0L;
            if (fac.FacilityInformationItems.Any())
            {
                foreach (var fcccc in fac.FacilityInformationItems.ToList())
                {
                    try
                    {
                        //changed to 1000000
                        if (long.Parse(fcccc.AmBedehiKol) >= 1000000 && long.Parse(fcccc.AmOriginal) >= 1000000)
                        {
                            lstFacItm.Add(fcccc);
                            numberOfFacilities += 1;
                            totalAmTashilat += long.Parse(fcccc.AmBenefit) + long.Parse(fcccc.AmOriginal);
                            totalAmOriginal += long.Parse(fcccc.AmOriginal);
                            totalAmBedehi += long.Parse(fcccc.AmBedehiKol);
                            totalAmMashkuk += long.Parse(fcccc.AmMashkuk);
                            totalAmMoavagh += long.Parse(fcccc.AmMoavagh);
                            totalAmSarResid += long.Parse(fcccc.AmSarResid);
                        }
                    }
                    catch
                    {
                        //fac.FacilityInformationItems.Remove(fcccc);
                    }
                }
            }
            //changed to 1000000
            if (numberOfFacilities <= 0 || totalAmOriginal <= 1000000)
            {
                scoreModel.FacilityScore = 140;


                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بدترین وضعیت در باز پرداخت وام‌ها",
                    ScoreValue = "140",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "اقساط باقیمانده از وام‌ها",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
                facilityReportList.Add(new CreditReportModel
                {
                    VariableDescription = "بیشینه مبلغ وام دریافتی",
                    ScoreValue = "-",
                    ScoreDescription = "هیچ وامی در نظام بانکی کشور یافت نشد."
                });
            }
            else
            {
                //var badFac = lstFacItm.Where(o => o.AmMashkuk != "0" || o.AmMoavagh != "0" || o.AmSarResid != "0").OrderByDescending(o => o.AmMashkuk).ThenByDescending(o => o.AmMoavagh).ThenByDescending(o => o.AmSarResid).ToList();

                if (totalAmMashkuk > 0)
                {


                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در باز پرداخت وام‌ها",
                        ScoreValue = "0",
                        ScoreDescription = "بیشترین تاخیر در پرداخت اقساط بیش از 18 بوده است."
                    });

                    scoreModel.FacilityScore += 0;
                }
                else if (totalAmMoavagh > 0)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در باز پرداخت وام‌ها",
                        ScoreValue = "40",
                        ScoreDescription = "بیشترین تاخیر در پرداخت اقساط بین 6 تا 18 ماه بوده است."
                    });

                    scoreModel.FacilityScore += 40;
                }
                else if (totalAmSarResid > 0)
                {


                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در باز پرداخت وام‌ها",
                        ScoreValue = "86",
                        ScoreDescription = "بیشترین تاخیر در پرداخت اقساط بین 2 تا 6 ماه بوده است."
                    });

                    scoreModel.FacilityScore += 86;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بدترین وضعیت در باز پرداخت وام‌ها",
                        ScoreValue = "133",
                        ScoreDescription = "در حال حاضر، اقساط تمامی وام‌ها پرداخت شده است."
                        //ScoreDescription = "اقساط تمامی وام‌ها تا کنون به موقع پرداخت شده است."
                    });

                    scoreModel.FacilityScore += 133;
                }


                if (numberOfFacilities <= 1)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، 1 است."
                    });

                    scoreModel.FacilityScore += 0;
                }
                else if (numberOfFacilities <= 3)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "18",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، 2 یا 3 است."
                    });

                    scoreModel.FacilityScore += 18;
                }
                else if (numberOfFacilities <= 5)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "42",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، 4 یا 5 است."
                    });

                    scoreModel.FacilityScore += 42;
                }
                else if (numberOfFacilities <= 6)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "54",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، 6 است."
                    });

                    scoreModel.FacilityScore += 54;
                }
                else if (numberOfFacilities <= 10)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "60",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، 6 تا 10 است."
                    });

                    scoreModel.FacilityScore += 60;
                }
                else if (numberOfFacilities <= 14)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "36",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، 10 تا 14 است."
                    });

                    scoreModel.FacilityScore += 36;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌های در حال پرداخت",
                        ScoreValue = "2",
                        ScoreDescription = "تعداد وام‌های در حال پرداخت، بیش از 14 است."
                    });

                    scoreModel.FacilityScore += 2;
                }


                if (totalAmTashilat <= 10000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، کمتر از 1 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 0;
                }
                else if (totalAmTashilat <= 100000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "26",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بین 1 تا 10 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 26;
                }
                else if (totalAmTashilat <= 250000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "42",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بین 10 تا 25 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 42;
                }
                else if (totalAmTashilat <= 500000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "58",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بین 25 تا 50 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 58;
                }
                else if (totalAmTashilat <= 1000000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "64",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بین 50 تا 100 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 64;
                }
                else if (totalAmTashilat <= 2000000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "54",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بین 100 تا 200 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 54;
                }
                else if (totalAmTashilat <= 4000000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "36",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بین 200 تا 400 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 36;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "مجموع مبالغ وام‌های در حال پرداخت",
                        ScoreValue = "6",
                        ScoreDescription = "مجموع مبالغ وام‌های در حال پرداخت در کل بانک‌های کشور، بیش از 400 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 6;
                }

                var pMande = (100 * (double)totalAmBedehi) / (double)totalAmTashilat;
                if (pMande <= 25)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از وام‌ها",
                        ScoreValue = "127",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، کمتر از 25 درصد باقیمانده است."
                    });

                    scoreModel.FacilityScore += 127;
                }
                else if (pMande <= 50)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از وام‌ها",
                        ScoreValue = "102",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بین 25 تا 50 درصد باقیمانده است."
                    });

                    scoreModel.FacilityScore += 102;
                }
                else if (pMande <= 75)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از وام‌ها",
                        ScoreValue = "68",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بین 50 تا 75 درصد باقیمانده است."
                    });

                    scoreModel.FacilityScore += 68;
                }
                else if (pMande <= 100)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از وام‌ها",
                        ScoreValue = "18",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بین 75 تا 100 درصد باقیمانده است."
                    });

                    scoreModel.FacilityScore += 18;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "اقساط باقیمانده از وام‌ها",
                        ScoreValue = "0",
                        ScoreDescription = "از اقساط وام‌ها در کل بانک‌های کشور، بیش از 100 درصد باقیمانده است."
                    });

                    scoreModel.FacilityScore += 0;
                }

                var pMbDtoAll = (100 * (double)(totalAmMashkuk + totalAmMoavagh + totalAmSarResid)) / (double)totalAmTashilat;
                if (pMbDtoAll <= 0)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "27",
                        ScoreDescription = "در حال حاضر، اقساط پرداخت نشده وجود ندارد."
                        //ScoreDescription = "پایبندی مشتری به تعهدات خود بالا است."
                    });

                    scoreModel.FacilityScore += 27;
                }
                else if (pMbDtoAll <= 5)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "21",
                        ScoreDescription = "موعد پرداختِ زیر 5 درصد از اقساط گذشته است و هنوز پرداخت نشده‌اند."
                    });

                    scoreModel.FacilityScore += 21;
                }
                else if (pMbDtoAll <= 20)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "18",
                        ScoreDescription = "موعد پرداختِ 5 تا 20 درصد از اقساط گذشته است و هنوز پرداخت نشده‌اند."
                    });

                    scoreModel.FacilityScore += 18;
                }
                else if (pMbDtoAll <= 40)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "13",
                        ScoreDescription = "موعد پرداختِ 20 تا 40 درصد از اقساط گذشته است و هنوز پرداخت نشده‌اند."
                    });

                    scoreModel.FacilityScore += 13;
                }
                else if (pMbDtoAll <= 80)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "8",
                        ScoreDescription = "موعد پرداختِ 40 تا 80 درصد از اقساط گذشته است و هنوز پرداخت نشده‌اند."
                    });

                    scoreModel.FacilityScore += 8;
                }
                else if (pMbDtoAll <= 90)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "3",
                        ScoreDescription = "موعد پرداختِ 80 تا 90 درصد از اقساط گذشته است و هنوز پرداخت نشده‌اند."
                    });

                    scoreModel.FacilityScore += 3;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت مبالغ وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "موعد پرداختِ بیش از 90 درصد از اقساط گذشته است و هنوز پرداخت نشده‌اند."
                    });

                    scoreModel.FacilityScore += 0;
                }


                var cTbDdP =
                    fac.FacilityInformationItems.Count(
                        o =>
                            (o.AmMashkuk != null && o.AmMashkuk != "0") ||
                            (o.AmMoavagh != null && o.AmMoavagh != "0") ||
                            (o.AmSarResid != null && o.AmSarResid != "0"));

                if (cTbDdP <= 0)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "30",
                        ScoreDescription = "در حال حاضر، اقساط پرداخت نشده وجود ندارد."
                        //ScoreDescription = "پایبندی مشتری به تعهدات خود بالا است."
                    });

                    scoreModel.FacilityScore += 30;
                }
                else if (cTbDdP <= 1)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "24",
                        ScoreDescription = "در پرداخت 1 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityScore += 24;
                }
                else if (cTbDdP <= 3)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "20",
                        ScoreDescription = "در پرداخت 2 یا 3 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityScore += 20;
                }
                else if (cTbDdP <= 5)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "17",
                        ScoreDescription = "در پرداخت 4 یا 5 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityScore += 17;
                }
                else if (cTbDdP <= 6)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "12",
                        ScoreDescription = "در پرداخت 6 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityScore += 12;
                }
                else if (cTbDdP <= 10)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "7",
                        ScoreDescription = "در پرداخت 6 تا 10 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityScore += 7;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "وضعیت تعداد وام‌ها با دیرکرد در باز پرداخت",
                        ScoreValue = "0",
                        ScoreDescription = "در پرداخت بیش از 10 مورد از وام‌ها تاخیر مشاهده شده است."
                    });

                    scoreModel.FacilityScore += 0;
                }
                var orgAmList = new List<long>();
                foreach (var itm in fac.FacilityInformationItems)
                {
                    var vl = Int64.Parse(itm.AmOriginal);
                    orgAmList.Add(vl);
                }
                var maxValFac = orgAmList.OrderByDescending(o => o).First();
                if (maxValFac <= 10000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "0",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، کمتر از 1 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 0;
                }
                else if (maxValFac <= 20000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "13",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، 1 تا 2 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 13;
                }
                else if (maxValFac <= 50000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "18",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، 2 تا 5 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 18;
                }
                else if (maxValFac <= 100000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "24",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، 5 تا 10 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 24;
                }
                else if (maxValFac <= 200000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "34",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، 10 تا 20 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 34;
                }
                else if (maxValFac <= 500000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "40",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، 20 تا 50 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 40;
                }
                else if (maxValFac <= 2000000000000)
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "48",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، 50 تا 200 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 48;
                }
                else
                {

                    facilityReportList.Add(new CreditReportModel
                    {
                        VariableDescription = "بیشینه مبلغ وام دریافتی",
                        ScoreValue = "58",
                        ScoreDescription = "بیشینه مبلغ وام دریافتی مشتری، بیش از 200 میلیارد تومان است."
                    });

                    scoreModel.FacilityScore += 58;
                }

            }

            res.Score += scoreModel.FacilityScore;


            #endregion Facility

            //CreditReportList
            #region CreditReportList

            if (chequeReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = chequeReportList,
                    CreditReportName = "وضعیت چک برگشتی",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.ChequeScore + " (سقف امتیاز وضعیت چک برگشتی 500 است)"
                });
            }
            if (facilityReportList.Any())
            {
                res.ListOfCreditReportList.Add(new CreditReportFull
                {
                    CreditReportList = facilityReportList,
                    CreditReportName = "وضعیت تسهیلات بانکی",
                    ScoreSum = "مجموع امتیاز : " + scoreModel.FacilityScore + " (سقف امتیاز وضعیت تسهیلات بانکی 500 است)"
                });
            }
            #endregion CreditReportList





            return res;
        }

        public class ScoreResultModel
        {
            public int Score { get; set; }

            public bool DecisionStatus { get; set; }

            public string ScoreModelXml { get; set; }

            public List<CreditReportFull> ListOfCreditReportList { get; set; }
        }
        public class CreditReportFull
        {

            public string CreditReportName { get; set; }


            public List<CreditReportModel> CreditReportList { get; set; }


            public string ScoreSum { get; set; }
        }
        public class CreditReportModel
        {
            public string VariableDescription { get; set; }

            public string ScoreValue { get; set; }
            public string ScoreDescription { get; set; }

        }
        public class ScoreModelV02
        {
            public int ChequeBouncedScore { get; set; }
            public int ChequeClearedScore { get; set; }

            public int FacilityAsliScore { get; set; }
            public int FacilityTasviehScore { get; set; }
            public int FacilityZamenScore { get; set; }

            public int HistoryRecordScore { get; set; }
        }

        private static RecordsModel GetRecordsData(string nationalCode)
        {
            using (var clProx = new RecordsServiceClient())
            {
                return clProx.GetRecords(nationalCode);
            }
        }
        public static DateTime? ToDateTimeMiladi(string persianDate)
        {

            if (persianDate == null)
            {
                return null;
                //throw new ArgumentNullException(nameof(persianDate));
            }
            if (!persianDate.Contains('/'))
            {
                if (!int.TryParse(persianDate.Substring(0, 4), out int year) ||
                    !int.TryParse(persianDate.Substring(4, 2), out int month) ||
                    !int.TryParse(persianDate.Substring(6, 2), out int day))
                {
                    return null;
                    //throw new ArgumentException("Invalid date format.", nameof(persianDate));
                }

                if (year < 1300 || year > 2100)
                {
                    return null;
                    //throw new ArgumentOutOfRangeException(nameof(persianDate), "Year must be between 1300 and 2100.");
                }

                // Calculate the Gregorian date.
                var persianCalendar = new PersianCalendar();
                return persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
            else
            {
                return persianDate.ToGeoDateTime();
            }

        }
        private static long StringToLong(string strVal)
        {
            try
            {
                if (strVal.Contains('.'))
                {
                    return Convert.ToInt64(double.Parse(strVal));
                }
                else
                {
                    return long.Parse(strVal);
                }
            }
            catch
            {
                return 0;
            }
        }
        public class ScoreLegalModelV2
        {
            public int ChequeScore { get; set; }
            public int FacilityScore { get; set; }
        }
        public static DateTime? ToGeoDateTime(this string persianDate)
        {
            try
            {
                return GetGregorianFromSlashStringJalali(persianDate);
            }
            catch
            {
                return null;
            }
        }
        public static DateTime GetGregorianFromSlashStringJalali(string dateTime)
        {
            var cal = new PersianCalendar();
            var dateDelim = dateTime.Trim().Split('/');

            try
            {
                return dateDelim[2].Length == 4 ? cal.ToDateTime(Convert.ToInt32(dateDelim[2]), Convert.ToInt32(dateDelim[1]), Convert.ToInt32(dateDelim[0]), 0, 0, 0, 0) : cal.ToDateTime(Convert.ToInt32(dateDelim[0]), Convert.ToInt32(dateDelim[1]), Convert.ToInt32(dateDelim[2]), 0, 0, 0, 0);
            }
            catch
            {
                throw new Exception("خطا در تبدیل تاریخ");
            }
        }


        [DataContract]
        public class UserDataCr24SvcModel
        {
            [DataMember]
            public string UserName { get; set; }

            [DataMember]
            public string UserFaFullName { get; set; }



            [DataMember]
            public List<string> Roles { get; set; }
        }
    }
    public class UserAuthService
    {
        public static UserDataCr24SvcModel GetUserData(string userName)
        {
            try
            {
                var res = new UserDataCr24SvcModel();
                using (var clProx = new AuthenticationServiceClient())
                {
                    if (clProx.VersionConfirm("Cr24Version2.6R") != "FUbVu+Jaj9ERGjh00XYcEPhF6cgyZI52+vWe0DkxASs=") { throw new Exception("Wrong Auth Version!"); }
                    var dat = clProx.UserGetData(userName);
                    res.UserFaFullName = dat.Name + " " + dat.LastName;
                    res.UserName = dat.UserName;
                    var rol = clProx.AcGetUserRoles(userName, ApplicationService.GetAppName()).Select(e => e.RoleName).ToList();
                    if (rol == null) return null;
                    res.Roles = rol;
                }
                return res;
            }
            catch
            {
                return null;
            }
        }


        private static string GetSecurityAuthKey()
        {
            var dt = DateTime.Now;
            if (dt > new DateTime(2021, 6, 1))
            {
                return "rFAFKfbWNMfy85Zk";
            }
            if (dt > new DateTime(2020, 6, 1))
            {
                return "nSx9cQW8fdgagSvV";
            }
            if (dt > new DateTime(2019, 6, 1))
            {
                return "nE2Ud5L4hbhC7b7y";
            }
            if (dt > new DateTime(2018, 6, 1))
            {
                return "925RmyVSQygKhS5g";
            }
            if (dt > new DateTime(2017, 6, 1))
            {
                return "gx8eeNVvtWbS2mpt";
            }
            return "19fztjf7it2mev81";
        }
    }
    public class ApplicationService
    {
        private static readonly string ThisAppName = ConfigurationManager.AppSettings.Get("ApplicationName");
        public static string GetAppName()
        {
            return ThisAppName;
        }
    }

}