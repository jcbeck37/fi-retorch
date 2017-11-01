using fi.retorch.com.Areas.Reports.Code.Base;
using fi.retorch.com.Areas.Reports.Models;
using System;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Reports.Controllers
{
    public class BudgetReviewController : BaseController
    {
        // GET: Reports/BudgetReview
        public ActionResult Monthly(DateTime? startDate, DateTime? endDate)
        {
            BudgetReviewMonthlyModel model = new BudgetReviewMonthlyModel(startDate, endDate);

            return View(model);
        }

        public ActionResult Annual(DateTime? startDate, DateTime? endDate)
        {
            BudgetReviewAnnualModel model = new BudgetReviewAnnualModel(startDate, endDate);

            return View(model);
        }

        [HttpGet]
        public ActionResult GetMonthlyBudgetReview([Bind(Include = "StartDate,EndDate")] BudgetReviewMonthlyModel settings)
        {
            if (settings == null)
                settings = new BudgetReviewMonthlyModel();

            BudgetReviewMonthlyReport model = new BudgetReviewMonthlyReport(db, userKey, settings.StartDate.Value, settings.EndDate.Value);

            return Json(model.Items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAnnualBudgetReview([Bind(Include = "StartDate,EndDate")] BudgetReviewAnnualModel settings)
        {
            if (settings == null)
                settings = new BudgetReviewAnnualModel();

            BudgetReviewAnnualReport model = new BudgetReviewAnnualReport(db, userKey, settings.StartDate.Value, settings.EndDate.Value);

            return Json(model.Items, JsonRequestBehavior.AllowGet);
        }
    }
}