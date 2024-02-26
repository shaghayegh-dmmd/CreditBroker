using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using CreditBrokerMvc.Enumerations;
using CreditBrokerMvc.Helper;
using CreditBrokerMvc.Models;
using CreditBrokerMvc.ServiceReference1;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using static CreditBrokerMvc.Models.ReportModelStr;

namespace CreditBrokerMvc.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.EducationLevelOfManager = Enum.GetValues(typeof(EducationLevelOfManager)).Cast<EducationLevelOfManager>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = EnumHelper.GetDescription(e)
            });

            ViewBag.OwnerShipOfLocation = Enum.GetValues(typeof(OwnerShipOfLocation)).Cast<OwnerShipOfLocation>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = EnumHelper.GetDescription(e)
            });
            ViewBag.EzharHesabres = Enum.GetValues(typeof(EzharHesabres)).Cast<EzharHesabres>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = EnumHelper.GetDescription(e)
            });
            ViewBag.TarkibSahamdaran = Enum.GetValues(typeof(TarkibSahamdaran)).Cast<TarkibSahamdaran>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = EnumHelper.GetDescription(e)
            });

            return View();
        }

        [HttpPost]
        //    [ValidateAntiForgeryToken]
        public bool Create(ServiceReference1.RequestModel RequestModel)
        {
            try
            {
                RequestModel.UserName = "a.mashali";
                using (var serviceCl = new ServiceReference1.BrokerServicesClient())
                {
                    var data = serviceCl.Credit(RequestModel);

                    if (data.IsServiceOk)
                    {
                        var res = new ResultModelDTO();
                        res.CompanyName = data.CompanyName;
                        res.CompanyNationalCode = data.CompanyNationalCode;
                        res.TotalScore = data.TotalScore;

                        res.Finance = data.CreditBrokerModelList[0];
                        res.ManagerScore = data.CreditBrokerModelList[1];
                        res.BrokerPerformance = data.CreditBrokerModelList[2];
                        res.BussinessVariable = data.CreditBrokerModelList[3];
                        res.Hesabress = data.CreditBrokerModelList[4];

                        res.DateNow = HelperInfra.GetJalaliFromDateTimeGregorian(DateTime.Now);

                        HttpRuntime.Cache.Insert("CreditBroker" + RequestModel.CompanyNationalCode, res, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult ShowCreditStimul(string CompanyNationalCode)
        {
            try
            {
                var report = new StiReport();
                report.Load(Server.MapPath("~/Reports/Report.mrt"));
                Stimulsoft.Base.StiFontCollection.AddFontFile(Server.MapPath("~/fonts/ttf/IRANSansWeb.ttf"));
                report.Compile();

                if (HttpContext.Cache["CreditBroker" + CompanyNationalCode] != null)
                {
                    var data = (ResultModelDTO)HttpContext.Cache["CreditBroker" + CompanyNationalCode];
                    ReportModelStr dataStr = new ReportModelStr();
                    dataStr.DateNow = HelperInfra.EnglishNumbersToPersian(HelperInfra.GetJalaliFromDateTimeGregorian(DateTime.Now));
                    dataStr.TotalScore = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", data.TotalScore));
                    dataStr.CompanyName = data.CompanyName;
                    dataStr.CompanyNationalCode = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", data.CompanyNationalCode));
                    dataStr.Finance = new CreditBrokerModelStr();
                    dataStr.Finance.SumScore = data.Finance.SumScore;
                    dataStr.Finance.CreditName = data.Finance.CreditName;
                    dataStr.Finance.statuses = new List<StatusStr>();

                    foreach (var item in data.Finance.statuses)
                    {
                        dataStr.Finance.statuses.Add(new StatusStr
                        {
                            Description = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.Description))
                            ,
                            ScoreValue = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.ScoreValue))

                        });
                    }
                    dataStr.Hesabress = new CreditBrokerModelStr();
                    dataStr.Hesabress.statuses = new List<StatusStr>();
                    foreach (var item in data.Hesabress.statuses)
                    {
                        dataStr.Hesabress.statuses.Add(new StatusStr
                        {
                            Description = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.Description))
                            ,
                            ScoreValue = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.ScoreValue))

                        });
                    }
                    dataStr.BrokerPerformance = new CreditBrokerModelStr();
                    dataStr.BrokerPerformance.statuses = new List<StatusStr>();
                    foreach (var item in data.BrokerPerformance.statuses)
                    {
                        dataStr.BrokerPerformance.statuses.Add(new StatusStr
                        {
                            Description = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.Description))
                            ,
                            ScoreValue = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.ScoreValue))

                        });
                    }
                    dataStr.BussinessVariable = new CreditBrokerModelStr();
                    dataStr.BussinessVariable.statuses = new List<StatusStr>();
                    foreach (var item in data.BussinessVariable.statuses)
                    {
                        dataStr.BussinessVariable.statuses.Add(new StatusStr
                        {
                            Description = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.Description))
                            ,
                            ScoreValue = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.ScoreValue))
                        });
                    }
                    dataStr.ManagerScore = new CreditBrokerModelStr();
                    dataStr.ManagerScore.statuses = new List<StatusStr>();
                    foreach (var item in data.ManagerScore.statuses)
                    {
                        dataStr.ManagerScore.statuses.Add(new StatusStr
                        {
                            Description = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.Description))
                            ,
                            ScoreValue = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", item.ScoreValue))
                        });
                    }
                    Stimulsoft.Report.StiOptions.Engine.DefaultPaperSize = PaperKind.A4;
                    report.RegBusinessObject("data", dataStr);
                    return StiMvcViewer.GetReportResult(report);

                }
                else
                {
                    return RedirectToAction("Error");
                }
            }


            catch (Exception ex)
            { return null; }
        }

        //if (HttpContext.Cache[""CreditBroker" + nationalCode] != null)
        //   {
        //       var data = (BankCreditInfo)HttpContext.Cache["CreditBroker" + nationalCode];


        public ActionResult PreviewScore(string CompanyNationalCode)
        {

            return View();
        }
        public ActionResult viewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }
        public bool CheckNationalCode(string CompanyNationalCode)
        {
            using (var serviceCl = new ServiceReference1.BrokerServicesClient())
            {
                var data = serviceCl.CheckCreditToday(CompanyNationalCode);

                if (data.IsServiceOk)
                {
                    var res = new ResultModelDTO();
                    res.CompanyName = data.CompanyName;
                    res.CompanyNationalCode = HelperInfra.EnglishNumbersToPersian(string.Format("{0:n0}", long.Parse(data.CompanyNationalCode)));
                    res.TotalScore = data.TotalScore;

                    res.Finance = data.CreditBrokerModelList[0];
                    res.ManagerScore = data.CreditBrokerModelList[1];
                    res.BrokerPerformance = data.CreditBrokerModelList[2];
                    res.BussinessVariable = data.CreditBrokerModelList[3];
                    res.Hesabress = data.CreditBrokerModelList[4];

                    res.DateNow = HelperInfra.GetJalaliFromDateTimeGregorian(DateTime.Now);

                    HttpRuntime.Cache.Insert("CreditBroker" + CompanyNationalCode, res, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}