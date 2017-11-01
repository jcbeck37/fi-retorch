using System;
using System.Globalization;
using System.Net;
using System.Web.Mvc;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Configuration.Models;
using PagedList;
using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class RemindersController : BaseController
    {
        private const string DefaultRedirectController = "Reminders"; // ~/Dashboard/Home
        private const string TempDataKeyReminderQuerySettings = "ReminderQuerySettings";

        // GET: Dashboard/Reminders
        public ActionResult Index(string sort = null, int? page = 1, bool reset = false)
        {
            ReminderQuerySettings settings = new ReminderQuerySettings();
            if (TempData.ContainsKey(TempDataKeyReminderQuerySettings) && !reset)
                settings = (ReminderQuerySettings)TempData[TempDataKeyReminderQuerySettings];

            settings.Page = page;
            settings.Search = settings.Filter;
            settings.Sort = sort;

            TempData[TempDataKeyReminderQuerySettings] = settings;
            
            //ReminderList list = new ReminderList(db, userKey, settings);
            //list.Items.ForEach(t => t.Amount = t.Amount * (t.IsCredit ? 1 : -1));

            ReminderList model = InnerIndex(settings);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "AccountTypeId,AccountId,CategoryId,StartDate,EndDate,Search,Filter,Sort")] ReminderQuerySettings settings)
        {
            settings.Page = 1;
            settings.Search = settings.Search ?? settings.Filter;

            TempData[TempDataKeyReminderQuerySettings] = settings;

            ReminderList model = InnerIndex(settings);

            return View(model);
        }

        private ReminderList InnerIndex(ReminderQuerySettings settings)
        {
            settings.PageSize = PageSize;

            ReminderList model = new ReminderList(db, userKey, settings);
            model.Items.ForEach(t => t.Amount = t.Amount * (t.IsCredit ? 1 : -1));

            //create staticPageList, defining your viewModel, current page, page size and total number of pages.
            model.PagedItems = new StaticPagedList<ReminderModel>(model.Items, settings.Page.Value, settings.PageSize.Value, model.TotalItems);

            ViewBag.AccountTypes = new SelectList(new AccountTypeList(db, userKey).Items, "Id", "Name");
            ViewBag.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            ViewBag.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings() { Sort = "Name" }).Items, "Id", "Name");

            ViewBag.Sort = settings.Sort;
            ViewBag.Filter = settings.Search;
            ViewBag.NameSortParm = settings.Sort == "Name" ? "NameDesc" : "Name";
            ViewBag.DateSortParm = String.IsNullOrEmpty(settings.Sort) ? "DisplayDate" : "";

            return model;
        }

        // GET: Dashboard/Reminders/Create
        public ActionResult Create()
        {
            ReminderModel model = new ReminderModel();
            model.NextDate = System.DateTime.Now;
            model.IsCredit = true;

            model.PositiveText = "Positive";
            model.NegativeText = "Negative";

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");
            //model.Types = new SelectList(new ReminderTypeList(db, true).Items, "Id", "Name");
            model.Schedules = new SelectList(new ReminderScheduleList(db, true).Items, "Id", "Name");

            if (Request.IsAjaxRequest())
                return PartialView("_Create", model);
            else
                return View(model);
        }

        // POST: Dashboard/Reminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountId,CategoryId,ReminderScheduleId,NextDate,PositiveText,NegativeText,IsCredit,Amount,AmountFormatted,Rate,RateFormatted,Name,LastDate")] ReminderModel model)
        {
            decimal amount;
            if (string.IsNullOrEmpty(model.AmountFormatted))
                amount = 0;
            else if (!decimal.TryParse(model.AmountFormatted, NumberStyles.Currency, CultureInfo.CurrentCulture, out amount))
                amount = 0;

            model.Amount = amount;
            
            decimal rate;
            if (string.IsNullOrEmpty(model.RateFormatted))
                rate = 0;
            else if (!decimal.TryParse(model.RateFormatted.Replace("%", ""), NumberStyles.Float, CultureInfo.CurrentCulture, out rate))
                rate = 0;

            if (rate > 0)
                model.Rate = rate;

            if (ModelState.IsValid)
            {
                ReminderModel.Save(model, db, userKey, true);

                if (Request.IsAjaxRequest())
                {
                    ReminderModel result = ReminderModel.Get(db, userKey, model.Id);
                    result.EditLink = Url.Action("Edit", "Reminders", new { Id = result.Id });
                    result.GoLink = Url.Action("Create", "Transactions", new { reminderId = result.Id });

                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                else
                    return RedirectToAction("Index", DefaultRedirectController);
            }

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");
            //model.Types = new SelectList(new ReminderTypeList(db, true).Items, "Id", "Name");
            model.Schedules = new SelectList(new ReminderScheduleList(db, true).Items, "Id", "Name");
            
            return View(model);
        }

        // GET: Dashboard/Reminders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ReminderModel model = ReminderModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");
            //model.Types = new SelectList(new ReminderTypeList(db, true).Items, "Id", "Name");
            model.Schedules = new SelectList(new ReminderScheduleList(db, true).Items, "Id", "Name");

            if (Request.IsAjaxRequest())
                return PartialView("_Edit", model);
            else
                return View(model);
        }

        // POST: Dashboard/Reminders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,CategoryId,ReminderScheduleId,NextDate,PositiveText,NegativeText,IsCredit,Amount,AmountFormatted,Rate,RateFormatted,Name,LastDate")] ReminderModel model)
        {
            decimal amount;
            if (string.IsNullOrEmpty(model.AmountFormatted))
                amount = 0;
            else if (!decimal.TryParse(model.AmountFormatted, NumberStyles.Currency, CultureInfo.CurrentCulture, out amount))
                amount = 0;

            model.Amount = amount;

            decimal rate;
            if (string.IsNullOrEmpty(model.RateFormatted))
                rate = 0;
            else if (!decimal.TryParse(model.RateFormatted.Replace("%",""), NumberStyles.Float, CultureInfo.CurrentCulture, out rate))
                rate = 0;

            if (rate > 0)
                model.Rate = rate;


            if (ModelState.IsValid)
            {
                ReminderModel.Save(model, db, userKey, false);

                if (Request.IsAjaxRequest())
                {
                    ReminderModel result = ReminderModel.Get(db, userKey, model.Id);
                    result.EditLink = Url.Action("Edit", "Reminders", new { Id = result.Id });
                    result.GoLink = Url.Action("Create", "Transactions", new { reminderId = result.Id });

                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                else
                    return RedirectToAction("Index", DefaultRedirectController);
            }

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");
            //model.Types = new SelectList(new ReminderTypeList(db, true).Items, "Id", "Name");
            model.Schedules = new SelectList(new ReminderScheduleList(db, true).Items, "Id", "Name");

            return View(model);
        }

        // GET: Dashboard/Reminders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ReminderModel model = ReminderModel.Get(db, userKey, id.Value);
            if (model == null)
                return HttpNotFound();
            if (model.Rate.HasValue)
                model.Rate = model.Rate / 100;

            if (Request.IsAjaxRequest())
                return PartialView("_Delete", model);
            else
                return View(model);
        }

        // POST: Dashboard/Reminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string RedirectController)
        {
            ReminderModel.Delete(db, userKey, id);

            if (Request.IsAjaxRequest())
                return Json(new { deleted = true }, JsonRequestBehavior.DenyGet);
            else
                return RedirectToAction("Index", String.IsNullOrEmpty(RedirectController) ? DefaultRedirectController : RedirectController);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
