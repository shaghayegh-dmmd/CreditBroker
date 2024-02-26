using CreditBroker.InquiryService;
using CreditBroker.Models.DataBaseModel;
using CreditBrokerWCF.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CreditBroker.Helper
{
    public class Infrastracture
    {
        public static ResultModel CheckCreditToday(string companyNationalCode)
        {
            var res = new ResultModel
            {
                IsServiceOk = false,
                CreditBrokerModelList = new List<CreditBrokerModel>(),
                TotalScore = 0,
            };
            using (var db = new CreditContext())
            {
                long nA = long.Parse(companyNationalCode);
                var oldData = db.CreditBrokers.FirstOrDefault(o => o.CompanyNationalCode == nA && o.ReqDate > DateTime.Today);
                if (oldData != null)
                {
                    res = JsonConvert.DeserializeObject<ResultModel>(oldData.ResultData);
                    res.IsServiceOk = true;
                }
                return res;
            }
        }

        public static ResultModel GetCredit(RequestModel requestModel)
        {
            try
            {
                long nationalCode = long.Parse(requestModel.CompanyNationalCode);
                var desc = "شناسه ملی: " + nationalCode;

                var res = new ResultModel
                {
                    IsServiceOk = false,
                    CreditBrokerModelList = new List<CreditBrokerModel>(),
                    CompanyName = requestModel.CompanyName,
                    CompanyNationalCode = requestModel.CompanyNationalCode,
                    TotalScore = 0,
                };

                using (var db = new CreditContext())
                {
                    //var oldData = db.CreditBrokers.FirstOrDefault(o => o.CompanyNationalCode == nationalCode && o.ReqDate > DateTime.Today  /*&& o.RequestData == reqData*/);
                    //if (oldData == null)
                    //{
                    var stopWatchCsResponseTime = new Stopwatch();
                    stopWatchCsResponseTime.Start();
                    var legalSocre = string.Empty;
                    var managerScore1 = string.Empty;
                    var managerScore2 = string.Empty;
                    var managerScore3 = string.Empty;
                    try
                    {
                        legalSocre = GetLegalScore(requestModel.CompanyNationalCode);
                        managerScore1 = GetRealScore(requestModel.CreditCompanyAndManager.ManagerNationalCode1);
                        if (!string.IsNullOrEmpty(requestModel.CreditCompanyAndManager.ManagerNationalCode2))
                        {
                            managerScore2 = GetRealScore(requestModel.CreditCompanyAndManager.ManagerNationalCode2);
                        }
                        if (!string.IsNullOrEmpty(requestModel.CreditCompanyAndManager.ManagerNationalCode2))
                        {
                            managerScore3 = GetRealScore(requestModel.CreditCompanyAndManager.ManagerNationalCode3);
                        }
                    }
                    catch { }

                    if (legalSocre == null || managerScore1 == null)
                    {
                        return res;
                    }

                    var scoreResult = CalculateScore(requestModel, legalSocre, managerScore1, managerScore2, managerScore3);
                    res.IsServiceOk = true;
                    res.CreditBrokerModelList = scoreResult.CreditBrokerModelList;

                    res.TotalScore = scoreResult.TotalScore;
                    desc += " - " + "امتیاز: " + scoreResult.TotalScore;

                    stopWatchCsResponseTime.Stop();
                    if (stopWatchCsResponseTime.Elapsed.TotalSeconds > 120)
                    {
                        throw new Exception("TimeOut !");
                    }

                    db.CreditBrokers.Add(new Models.DataBaseModel.CreditBroker
                    {
                        CompanyNationalCode = nationalCode,
                        Description = desc,
                        ReqDate = DateTime.Now,
                        //ResponseTime = stopWatchCsResponseTime.Elapsed,
                        Guid = Guid.NewGuid(),

                        ReqestData = JsonConvert.SerializeObject(requestModel),
                        ResultData = JsonConvert.SerializeObject(res),
                        User = requestModel.UserName
                    });
                    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    res = JsonConvert.DeserializeObject<ResultModel>(oldData.ResultData);
                    //}
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string GetRealScore(string managerNationalCode)
        {
            SamaChequesResponseResultWithJson chq = null;
            CbiSamatResponseResultWithJson fac = null;
            try
            {

                using (var serviceCl = new InquiryServiceClient())
                {
                    chq = serviceCl.GetSamaChequesResult(managerNationalCode);
                    serviceCl.Close();
                }
                using (var serviceCl = new InquiryServiceClient())
                {
                    fac = serviceCl.GetCbiSamatResult(managerNationalCode);
                    serviceCl.Close();
                }
                var data = CalcHelper.CalculateActualScoreV02(managerNationalCode, chq, fac);
                return data.Score.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetLegalScore(string nationalCode)
        {
            try
            {
                FullCheckInfo cheq;
                FullFacilityInfo fac;
                using (var serviceCl = new InquiryServiceClient())
                {
                    cheq = serviceCl.GetTodayFullLegalCheckInfo(nationalCode, "", "", "");
                    serviceCl.Close();
                }

                using (var serviceCl = new InquiryServiceClient())
                {
                    fac = serviceCl.GetTodayFullLegalFacilityInfo(nationalCode, "", "", "");
                    serviceCl.Close();
                }
                return CalcHelper.CalculateLegalScoreV2(cheq, fac).Score.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ResultModel CalculateScore(RequestModel requestModel, string legalSocre, string managerScore1, string managerScore2, string managerScore3)
        {
            var res = new ResultModel
            {
                TotalScore = 0,
                CompanyName = requestModel.CompanyName,
                CompanyNationalCode = requestModel.CompanyNationalCode,
                CreditBrokerModelList = new List<CreditBrokerModel>(),
            };

            #region NesbateMali

            double FinancialRatioScore = 0;
            var FinancialRatioReport = new List<Status>();
            ////
            var hashiesood = (requestModel.FinancialRatio.NetProfit / requestModel.FinancialRatio.SumDaramadAmaliaty) * 100;
            if (hashiesood <= 0)
            {
                FinancialRatioScore = -3;
                FinancialRatioReport.Add(new Status
                {
                    Description = " حاشیه سود (زیان) خالص منفی است",
                    ScoreValue = "-3 امتیاز",
                });
            }
            else if (hashiesood <= 20)
            {
                FinancialRatioScore += 18;
                FinancialRatioReport.Add(new Status
                {
                    Description = " حاشیه سود (زیان) خالص بین 0 تا 20 درصداست",
                    ScoreValue = "18 امتیاز",
                });
            }
            else if (hashiesood <= 40)
            {
                FinancialRatioScore += 46;
                FinancialRatioReport.Add(new Status
                {
                    Description = " حاشیه سود (زیان) خالص بین 20 تا 40 درصداست",
                    ScoreValue = "46 امتیاز",
                });
            }
            else if (hashiesood <= 60)
            {
                FinancialRatioScore += 73;
                FinancialRatioReport.Add(new Status
                {
                    Description = " حاشیه سود (زیان) خالص بین 40 تا 60 درصداست",
                    ScoreValue = "73 امتیاز",
                });
            }
            else
            {
                FinancialRatioScore += 84;
                FinancialRatioReport.Add(new Status
                {
                    Description = " حاشیه سود (زیان) خالص بیش از 60 درصداست",
                    ScoreValue = "84 امتیاز",
                });
            }
            long nesbatBedehi = requestModel.FinancialRatio.TotalDebt / requestModel.FinancialRatio.SumDaraei;


            if (nesbatBedehi <= 0.5)
            {
                FinancialRatioScore += 51;
                FinancialRatioReport.Add(new Status
                {
                    Description = " نسبت بدهی کمتر از 0.5 است",
                    ScoreValue = "51 امتیاز",
                });
            }
            else if (nesbatBedehi <= 0.7)
            {
                FinancialRatioScore += 40;
                FinancialRatioReport.Add(new Status
                {
                    Description = " نسبت بدهی بین 0.5 تا 0.7 است",
                    ScoreValue = "40 امتیاز",
                });
            }
            else if (nesbatBedehi <= 0.8)
            {
                FinancialRatioScore += 27;
                FinancialRatioReport.Add(new Status
                {
                    Description = " نسبت بدهی بین 0.7 تا 0.8 است",
                    ScoreValue = "27 امتیاز",
                });
            }
            else if (nesbatBedehi <= 0.9)
            {
                FinancialRatioScore += 12;
                FinancialRatioReport.Add(new Status
                {
                    Description = " نسبت بدهی بین 0.8 تا 0.9 است",
                    ScoreValue = "12 امتیاز",
                });
            }
            else
            {
                FinancialRatioScore += -14;
                FinancialRatioReport.Add(new Status
                {
                    Description = " نسبت بدهی بیش از 0.9 است",
                    ScoreValue = "-14 امتیاز",
                });
            }

            var bazdehJameDaraee = ((requestModel.FinancialRatio.NetProfit + requestModel.FinancialRatio.FinancialCost) / requestModel.FinancialRatio.SumDaraei) * 100;
            if (bazdehJameDaraee <= 5)
            {
                FinancialRatioScore += 0;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده جمع دارایی ها بین 1 تا 5 درصد است",
                    ScoreValue = "0 امتیاز",
                });
            }
            else if (bazdehJameDaraee <= 10)
            {
                FinancialRatioScore += 7;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده جمع دارایی ها بین 5 تا 10 درصد است",
                    ScoreValue = "7 امتیاز",
                });
            }
            else if (bazdehJameDaraee <= 15)
            {
                FinancialRatioScore += 14;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده جمع دارایی ها بین 10 تا 15 درصد است",
                    ScoreValue = "14 امتیاز ",
                });
            }
            else if (bazdehJameDaraee <= 20)
            {
                FinancialRatioScore += 19;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده جمع دارایی ها بین 15 تا 20 درصد است",
                    ScoreValue = "19 امتیاز ",
                });
            }
            else
            {
                FinancialRatioScore += 22;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده جمع دارایی ها بیشتر از 20 درصد است",
                    ScoreValue = "22 امتیاز ",
                });
            }

            var bazdehhoghoghsahebsaham = requestModel.FinancialRatio.NetProfit / requestModel.FinancialRatio.HoghogheSahebanSaham;

            if (bazdehhoghoghsahebsaham <= -0.18)
            {
                FinancialRatioScore += -2;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده حقوق صاحبان سهام کمتر از -0.18 است",
                    ScoreValue = "-2 امتیاز",
                });
            }
            else if (bazdehhoghoghsahebsaham <= 0)
            {
                FinancialRatioScore += 0;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده حقوق صاحبان سهام بین -0.18 تا 0 است",
                    ScoreValue = "0 امتیاز",
                });
            }
            else if (bazdehhoghoghsahebsaham <= 0.2)
            {
                FinancialRatioScore += 5;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده حقوق صاحبان سهام بین 0 تا 0.2 است",
                    ScoreValue = "5 امتیاز",
                });
            }

            else if (bazdehhoghoghsahebsaham <= 0.37)
            {
                FinancialRatioScore += 9;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده حقوق صاحبان سهام بین 0.2 تا 0.37 است",
                    ScoreValue = "9 امتیاز",
                });
            }
            else
            {
                FinancialRatioScore += 13;
                FinancialRatioReport.Add(new Status
                {
                    Description = " بازده حقوق صاحبان سهام بیش از 0.37 است",
                    ScoreValue = "13 امتیاز",
                });
            }

            var ezharnazarReport = new List<Status>();
            if (requestModel.FinancialRatio.EzharHesabres == 100)
            {
                ezharnazarReport.Add(new Status
                {
                    Description = " نظر حسابرس مطلوب(مقبول) است",
                    ScoreValue = "100%",
                });
            }
            else if (requestModel.FinancialRatio.EzharHesabres == 67)
            {
                FinancialRatioScore *= 0.67;
                ezharnazarReport.Add(new Status
                {
                    Description = " نظر حسابرس مشروط است",
                    ScoreValue = "67%",
                });
            }
            else if (requestModel.FinancialRatio.EzharHesabres == 46)
            {
                FinancialRatioScore *=  0.46;
                ezharnazarReport.Add(new Status
                {
                    Description = " عدم اظهار نظر حسابرس ",
                    ScoreValue = "46%",
                });
            }
            else if (requestModel.FinancialRatio.EzharHesabres == 24)
            {
                FinancialRatioScore *= 0.24;
                ezharnazarReport.Add(new Status
                {
                    Description = "  نظر حسابرس مردود(منفی یا رد) است ",
                    ScoreValue = "24%",
                });
            }
            #endregion

            #region Etebar Sanji Sherkat Va Modiran

            var CreditCompanyAndManagerScore = 0;
            var CreditCompanyAndManagerReport = new List<Status>();

            var etebarsanjihoghoghi =Math.Ceiling((decimal) (int.Parse(legalSocre) * 0.085));

            CreditCompanyAndManagerScore += (int)etebarsanjihoghoghi;

            CreditCompanyAndManagerReport.Add(new Status
            {
                Description = "اعتبار سنجی حقوقی",
                ScoreValue = etebarsanjihoghoghi + " امتیاز"
            });

            var etebarsanjihaghighi = Math.Ceiling((decimal)(int.Parse(managerScore1) * 0.045));
            var avgActual = etebarsanjihaghighi;

            CreditCompanyAndManagerReport.Add(new Status
            {
                Description = "اعتبار سنجی مدیر اجرایی1",
                ScoreValue = (int)etebarsanjihaghighi + " امتیاز"
            });

            if (!string.IsNullOrEmpty(managerScore2))
            {
                CreditCompanyAndManagerReport.Add(new Status
                {
                    Description = "اعتبار سنجی مدیر اجرایی2",
                    ScoreValue = Math.Ceiling((decimal)(int.Parse(managerScore2) * 0.045)) + " امتیاز"
                });

                etebarsanjihaghighi += Math.Ceiling((decimal)(int.Parse(managerScore2) * 0.045));
                avgActual = etebarsanjihaghighi / 2;

                if (!string.IsNullOrEmpty(managerScore3))
                {
                    CreditCompanyAndManagerReport.Add(new Status
                    {
                        Description = "اعتبار سنجی مدیر اجرایی3",
                        ScoreValue = Math.Ceiling((decimal)(int.Parse(managerScore3) * 0.045)) + " امتیاز"
                    });
                    etebarsanjihaghighi += Math.Ceiling((decimal)(int.Parse(managerScore3) * 0.045));
                    avgActual = etebarsanjihaghighi / 3;
                }
            }

            CreditCompanyAndManagerScore += (int) Math.Ceiling(avgActual);

            #endregion

            #region Amalkard Kargozari 

            int BrokerPerformanceScore = 0;
            var BrokerPerformanceReport = new List<Status>();

            if (requestModel.BrokerPerformanceVariables.BrokerRank <= 15)
            {
                BrokerPerformanceScore += 121;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "رتبه کارگزاری 1 تا 15 است",
                    ScoreValue = "121 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.BrokerRank <= 35)
            {
                BrokerPerformanceScore += 64;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "رتبه کارگزاری 16 تا 35 است",
                    ScoreValue = "64 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.BrokerRank <= 70)
            {
                BrokerPerformanceScore += 32;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "رتبه کارگزاری 36 تا 70 است",
                    ScoreValue = "32 امتیاز"
                });
            }

            else if (requestModel.BrokerPerformanceVariables.BrokerRank <= 90)
            {
                BrokerPerformanceScore += 24;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "رتبه کارگزاری 71 تا 90 است",
                    ScoreValue = "24 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 17;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "رتبه کارگزاری بیش از 91است",
                    ScoreValue = "17 امتیاز"
                });
            }

            if (requestModel.BrokerPerformanceVariables.SarmayehSabti <= 50000)
            {
                BrokerPerformanceScore += 11;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "سرمایه ثبتی کمتر از 50 میلیارد ریال است",
                    ScoreValue = "11 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.SarmayehSabti <= 100000)
            {
                BrokerPerformanceScore += 27;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "سرمایه ثبتی بین 50 تا 100 میلیارد ریال است",
                    ScoreValue = "27 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.SarmayehSabti <= 200000)
            {
                BrokerPerformanceScore += 60;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "سرمایه ثبتی بین 100 تا 200 میلیارد ریال است",
                    ScoreValue = "60 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 97;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "سرمایه ثبتی بیش از 200 میلیارد ریال است",
                    ScoreValue = "97 امتیاز"
                });
            }

            if (requestModel.BrokerPerformanceVariables.TarkibSahamdaran == 46)
            {
                BrokerPerformanceScore += 46;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "عمده سهامداران حقوقی شامل نهاد های مالی هستند ",
                    ScoreValue = "46 امتیاز"
                });
            }
            if (requestModel.BrokerPerformanceVariables.TarkibSahamdaran == 27)
            {
                BrokerPerformanceScore += 27;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "عمده سهامداران شامل سایر اشخاص حقوقی هستند ",
                    ScoreValue = "27 امتیاز"
                });
            }
            if (requestModel.BrokerPerformanceVariables.TarkibSahamdaran == 10)
            {
                BrokerPerformanceScore += 10;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "عمده سهامداران اشخاص حقیقی هستند ",
                    ScoreValue = "10 امتیاز"
                });
            }
            if (requestModel.BrokerPerformanceVariables.SumOfFacilityToCustomer == null || requestModel.BrokerPerformanceVariables.SumOfReceiveFacility == null || requestModel.BrokerPerformanceVariables.SumOfReceiveFacility==0)
            {
                BrokerPerformanceScore += 0;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "فاقد اطلاعات ",
                    ScoreValue = "0 امتیاز"
                });
            }
            else
            {
                var darsadeSarfeTashilat = (requestModel.BrokerPerformanceVariables.SumOfFacilityToCustomer / requestModel.BrokerPerformanceVariables.SumOfReceiveFacility) * 100;

                if (darsadeSarfeTashilat <= 50)
                {
                    BrokerPerformanceScore += 21;
                    BrokerPerformanceReport.Add(new Status
                    {
                        Description = "کمتر از 50 درصد از تسهیلات صرف مشتریان می‌ شود ",
                        ScoreValue = "21 امتیاز"
                    });
                }
                else if (darsadeSarfeTashilat <= 70)
                {
                    BrokerPerformanceScore += 60;
                    BrokerPerformanceReport.Add(new Status
                    {
                        Description = "بین 50 تا 70 درصد تسهیلات صرف مشتریان می شود ",
                        ScoreValue = "60 امتیاز"
                    });
                }
                else if (darsadeSarfeTashilat > 70)
                {
                    BrokerPerformanceScore += 81;
                    BrokerPerformanceReport.Add(new Status
                    {
                        Description = "بیش از 70 درصد تسهیلات صرف مشتریان می شود ",
                        ScoreValue = "81 امتیاز"
                    });
                }
            }


            var nerkhRoshdedarAmad = ((requestModel.BrokerPerformanceVariables.LastYearIncomeReported / requestModel.BrokerPerformanceVariables.PreviousLastYearIncomeReported) - 1) * 100;
            if (nerkhRoshdedarAmad <= -29)
            {
                BrokerPerformanceScore += 0;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "نرخ رشد درآمد کمتر از 29 - درصد است",
                    ScoreValue = "0 امتیاز"
                });
            }
            else if (nerkhRoshdedarAmad <= 4)
            {
                BrokerPerformanceScore += 6;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "نرخ رشد بین -29 تا 4 درصد است",
                    ScoreValue = "6 امتیاز"
                });
            }
            else if (nerkhRoshdedarAmad <= 22)
            {
                BrokerPerformanceScore += 11;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "نرخ رشد بین 4 تا 22 درصد است",
                    ScoreValue = "11 امتیاز"
                });
            }
            else if (nerkhRoshdedarAmad <= 57)
            {
                BrokerPerformanceScore += 17;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "نرخ رشد بین 22 تا 57 درصد است",
                    ScoreValue = "17 امتیاز"
                });
            }
            else if (nerkhRoshdedarAmad <= 292)
            {
                BrokerPerformanceScore += 30;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "نرخ رشد بین 57 تا 292 درصد است",
                    ScoreValue = "30 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 67;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "نرخ رشد بیش از 292 درصد است",
                    ScoreValue = "67 امتیاز"
                });
            }

            if (requestModel.BrokerPerformanceVariables.NumberOfPersonnel <= 40)
            {
                BrokerPerformanceScore += 4;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد پرسنل کمتر از 40 نفر است",
                    ScoreValue = "4 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.NumberOfPersonnel <= 100)
            {
                BrokerPerformanceScore += 9;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد پرسنل بین 41 تا 100 نفر است",
                    ScoreValue = "9 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.NumberOfPersonnel <= 200)
            {
                BrokerPerformanceScore += 15;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد پرسنل بین 100 تا 200 نفر است",
                    ScoreValue = "15 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.NumberOfPersonnel <= 500)
            {
                BrokerPerformanceScore += 29;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد پرسنل بین 200 تا 500 نفر است",
                    ScoreValue = "29 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 57;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد پرسنل بیش از 500 نفر است",
                    ScoreValue = "57 امتیاز"
                });
            }

            if (requestModel.BrokerPerformanceVariables.NumberOfbranch <= 5)
            {
                BrokerPerformanceScore += 4;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد شعب کمتر از 5 عدداست",
                    ScoreValue = "4 امتیاز"
                });
            }

            else if (requestModel.BrokerPerformanceVariables.NumberOfbranch <= 10)
            {
                BrokerPerformanceScore += 7;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد شعب بین 6تا 10 عدداست",
                    ScoreValue = "7 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.NumberOfbranch <= 20)
            {
                BrokerPerformanceScore += 13;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد شعب بین 11 تا 20 عدداست",
                    ScoreValue = "13 امتیاز"
                });
            }
            else if (requestModel.BrokerPerformanceVariables.NumberOfbranch <= 50)
            {
                BrokerPerformanceScore += 23;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد شعب بین 21 تا 50 عدداست",
                    ScoreValue = "23 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 48;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "تعداد شعب بیش از 51 عدداست",
                    ScoreValue = "48 امتیاز"
                });
            }

            if (requestModel.BrokerPerformanceVariables.EmtiazCommitteePayeshRisk <= 3)
            {
                BrokerPerformanceScore += 0;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "امتیاز کمیته پایش کمتر از 3 است",
                    ScoreValue = "0 امتیاز"
                });
            }
          
            else if (requestModel.BrokerPerformanceVariables.EmtiazCommitteePayeshRisk <= 4.5)
            {
                BrokerPerformanceScore += 20;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "امتیاز کمیته پایش بین 3 تا 4.5 است",
                    ScoreValue = "20 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 39;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "امتیاز کمیته پایش بیش از 4.5 است",
                    ScoreValue = "39 امتیاز"
                });
            }

            long MablagheAstane = 0;
            if (requestModel.BrokerPerformanceVariables.BrokerRank <= 15) MablagheAstane = 71104;
            else if (requestModel.BrokerPerformanceVariables.BrokerRank <= 35) MablagheAstane = 17260;
            else if (requestModel.BrokerPerformanceVariables.BrokerRank <= 70) MablagheAstane = 24683;
            else if (requestModel.BrokerPerformanceVariables.BrokerRank <= 90) MablagheAstane = 30021;
            else MablagheAstane = 80298;

            if (requestModel.BrokerPerformanceVariables.AmountOfValueAtRisk < MablagheAstane)
            {
                BrokerPerformanceScore += 34;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "مبلغ ارزش در ریسک  " + requestModel.BrokerPerformanceVariables.AmountOfValueAtRisk + "میلیون ریال  می باشد",
                    ScoreValue = "34 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 0;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "مبلغ ارزش در ریسک  " + requestModel.BrokerPerformanceVariables.AmountOfValueAtRisk + "میلیون ریال  می باشد",
                    ScoreValue = "0 امتیاز"
                });
            }

            if (requestModel.BrokerPerformanceVariables.MojavezSabadGardani)
            {
                BrokerPerformanceScore += 19;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "مجوز سبد گردانی دارد",
                    ScoreValue = "19 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 0;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "مجوز سبد گردانی ندارد",
                    ScoreValue = "0 امتیاز"
                });
            }
            if (requestModel.BrokerPerformanceVariables.MojavezMoamelatKala)
            {
                BrokerPerformanceScore += 15;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "مجوز معاملات کالا (در همه بخش ها) دارد",
                    ScoreValue = "15 امتیاز"
                });
            }
            else
            {
                BrokerPerformanceScore += 0;
                BrokerPerformanceReport.Add(new Status
                {
                    Description = "مجوز معاملات کالا (در همه بخش ها) ندارد",
                    ScoreValue = "0 امتیاز"
                });
            }

            #endregion

            #region bussiness Variable

            int bussinessVariableScore = 0;
            var bussinessVariableReport = new List<Status>();

            if (requestModel.BusinessVaribales.CompanyActivityTime <= 3)
            {
                bussinessVariableScore += 2;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مدت فعالیت کمتر از 3 سال است",
                    ScoreValue = "2 امتیاز"

                });

            }
            else if (requestModel.BusinessVaribales.CompanyActivityTime <= 7)
            {
                bussinessVariableScore += 4;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مدت فعالیت بین 3 تا 7 سال است",
                    ScoreValue = "4 امتیاز"
                });
            }

            else if (requestModel.BusinessVaribales.CompanyActivityTime <= 12)
            {
                bussinessVariableScore += 8;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مدت فعالیت بین 7 تا 12 سال است",
                    ScoreValue = "8 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.CompanyActivityTime <= 20)
            {
                bussinessVariableScore += 13;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مدت فعالیت بین 12 تا 20 سال است",
                    ScoreValue = "13 امتیاز"
                });
            }
            else
            {
                bussinessVariableScore += 29;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مدت فعالیت بیشتر از 20 سال است",
                    ScoreValue = "29 امتیاز"
                });
            }

            if (requestModel.BusinessVaribales.OwnerShipOfLocation == 6)
            {
                bussinessVariableScore += 6;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مالکیت محل کار استیجاری است",
                    ScoreValue = "6 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.OwnerShipOfLocation == 17)
            {
                bussinessVariableScore += 17;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مالکیت محل کار رهنی است",
                    ScoreValue = "17 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.OwnerShipOfLocation == 23)
            {
                bussinessVariableScore += 23;
                bussinessVariableReport.Add(new Status
                {
                    Description = "مالکیت محل کار شخصی است",
                    ScoreValue = "23 امتیاز"
                });
            }

            if (requestModel.BusinessVaribales.ExperinceOfManager <= 1)
            {
                bussinessVariableScore += 0;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تجربه کاری مرتبط مدیر اجرایی کمتر از 1 سال است",
                    ScoreValue = "0 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.ExperinceOfManager <= 5)
            {
                bussinessVariableScore += 1;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تجربه کاری مرتبط مدیر اجرایی بین 1 تا 5 سال است",
                    ScoreValue = "1 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.ExperinceOfManager <= 10)
            {
                bussinessVariableScore += 3;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تجربه کاری مرتبط مدیر اجرایی بین 5 تا 10 سال است",
                    ScoreValue = "3 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.ExperinceOfManager <= 20)
            {
                bussinessVariableScore += 7;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تجربه کاری مرتبط مدیر اجرایی بین 10 تا 20 سال است",
                    ScoreValue = "7 امتیاز"
                });
            }
            else
            {
                bussinessVariableScore += 11;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تجربه کاری مرتبط مدیر اجرایی بیش از 20 سال است",
                    ScoreValue = "11 امتیاز"
                });
            }

            if (requestModel.BusinessVaribales.EducationLevelOfManager == 0)
            {
                bussinessVariableScore += 0;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی زیر دیپلم و دیپلم است",
                    ScoreValue = "0 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.EducationLevelOfManager == 1)
            {
                bussinessVariableScore += 1;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی فوق دیپلم است",
                    ScoreValue = "1 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.EducationLevelOfManager == 2)
            {
                bussinessVariableScore += 2;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی لیسانس است",
                    ScoreValue = "2 امتیاز"
                });
            }
            else if (requestModel.BusinessVaribales.EducationLevelOfManager == 4)
            {
                bussinessVariableScore += 4;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی فوق لیسانس است",
                    ScoreValue = "4 امتیاز"
                });
            }
            else
            {
                bussinessVariableScore += 8;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی دکتری و بالاتراست",
                    ScoreValue = "8 امتیاز"
                });
            }

            if (requestModel.BusinessVaribales.RelatedEducationOfManager)
            {
                bussinessVariableScore += 5;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی با فعالیت مرتبط است",
                    ScoreValue = "5 امتیاز"
                });
            }
            if (!requestModel.BusinessVaribales.RelatedEducationOfManager)
            {
                bussinessVariableScore += 0;
                bussinessVariableReport.Add(new Status
                {
                    Description = "تحصیلات مدیر اجرایی با فعالیت غیر مرتبط است",
                    ScoreValue = "0 امتیاز"
                });
            }


            #endregion

            int financeScoreInt =(int) Math.Ceiling(FinancialRatioScore);
            if (FinancialRatioReport.Any())
            {
                res.CreditBrokerModelList.Add(new CreditBrokerModel
                {
                    statuses = FinancialRatioReport,
                    CreditName = "وضعیت  نسبت های مالی",
                    SumScore = "مجموع امتیاز : " + financeScoreInt + " (سقف امتیاز وضعیت نسبت های مالی 170 است)"
                });
            }

            if (CreditCompanyAndManagerReport.Any())
            {
                res.CreditBrokerModelList.Add(new CreditBrokerModel
                {
                    statuses = CreditCompanyAndManagerReport,
                    CreditName = "وضعیت اعتباری مدیران اجرایی",
                    SumScore = "مجموع امتیاز : " + CreditCompanyAndManagerScore + " (سقف امتیاز وضعیت اعتباری مدیران اجرایی 130 است)"
                });
            }
            if (BrokerPerformanceReport.Any())
            {
                res.CreditBrokerModelList.Add(new CreditBrokerModel
                {
                    statuses = BrokerPerformanceReport,
                    CreditName = "وضعیت متغییر های عملکرد کارگزاری",
                    SumScore = "مجموع امتیاز : " + BrokerPerformanceScore + " (سقف امتیاز وضعیت متغییر های عملکرد کارگزاری 624 است)"
                });
            }
            if (bussinessVariableReport.Any())
            {
                res.CreditBrokerModelList.Add(new CreditBrokerModel
                {
                    statuses = bussinessVariableReport,
                    CreditName = "وضعیت متغییرهای کسب و کار",
                    SumScore = "مجموع امتیاز : " + bussinessVariableScore + " (سقف امتیاز وضعیت متغییرهای کسب و کار 76 است)"
                });
            }
            if (ezharnazarReport.Any())
            {
                res.CreditBrokerModelList.Add(new CreditBrokerModel
                {
                    statuses = ezharnazarReport,
                    CreditName = "وضعیت اظهار نظر حسابرس",
                    // SumScore = "مجموع امتیاز : " + bussinessVariableScore + " (سقف امتیاز وضعیت متغییرهای کسب و کار 76 است)"
                });
            }
            res.TotalScore = financeScoreInt + BrokerPerformanceScore + bussinessVariableScore + CreditCompanyAndManagerScore;

            return res;
        }
    }
}