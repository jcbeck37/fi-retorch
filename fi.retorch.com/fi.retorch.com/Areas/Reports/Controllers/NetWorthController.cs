using fi.retorch.com.Areas.Reports.Code.Base;
using fi.retorch.com.Areas.Reports.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Reports.Controllers
{
    public class NetWorthController : BaseController
    {
        // GET: Reports/NetWorth
        public ActionResult Index()
        {
            NetWorthInstantModel model = new NetWorthInstantModel();
            model.Date = DateTime.Now;

            return View(model);
        }

        public ActionResult Monthly()
        {
            NetWorthMonthlyModel model = new NetWorthMonthlyModel();
            model.StartDate = DateTime.Now.AddYears(-2);

            return View(model);
        }

        public ActionResult Annual()
        {
            NetWorthAnnualSettingsModel model = new NetWorthAnnualSettingsModel();
            model.StartDate = DateTime.Now.AddYears(-10);

            return View(model);
        }

        #region AJAX calls
        public ActionResult GetAccountBalances(DateTime startDate)
        {
            List<NetWorthInstantModel> data = NetWorthInstantModel.GetInstantNetWorth(db, userKey, startDate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransactionTotals(DateTime startDate, DateTime? endDate = null)
        {
            List<TransactionTotalModel> data = TransactionTotalModel.GetTransactionTotal(db, userKey, startDate, endDate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMonthlyData(DateTime startDate, DateTime? endDate = null)
        {
            MonthlyItemList data = MonthlyItemList.GetReport(db, userKey, startDate, endDate.Value);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAnnualData(DateTime startDate, DateTime? endDate = null)
        {
            NetWorthAnnualReportModel data = NetWorthAnnualReportModel.GetReport(db, userKey, startDate, endDate.Value);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}