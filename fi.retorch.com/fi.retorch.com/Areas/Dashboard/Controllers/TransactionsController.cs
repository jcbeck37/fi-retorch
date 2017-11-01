using System;
using System.Net;
using System.Web.Mvc;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Dashboard.Code.Base;
using PagedList;
using System.Globalization;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class TransactionsController : BaseController
    {
        private const string DefaultRedirectController = "Home"; // ~/Dashboard/Home
        private const string TempDataKeyTransactionQuerySettings = "TransactionQuerySettings";

        #region Transactions CRUD
        // GET: Dashboard/Transactions
        public ActionResult Index(string sort = null, int? page = 1, bool reset = false)
        {
            TransactionQuerySettings settings = new TransactionQuerySettings();
            if (TempData.ContainsKey(TempDataKeyTransactionQuerySettings) && !reset)
                settings = (TransactionQuerySettings)TempData[TempDataKeyTransactionQuerySettings];
            
            settings.Page = page;
            settings.Search = settings.Search ?? settings.Filter;
            settings.Sort = sort;

            TempData[TempDataKeyTransactionQuerySettings] = settings;

            TransactionList model = InnerIndex(settings);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "AccountTypeId,AccountId,CategoryId,StartDate,EndDate,Search,Filter,Sort")] TransactionQuerySettings settings)
        {
            settings.Page = 1;
            settings.Search = settings.Search ?? settings.Filter;

            TempData[TempDataKeyTransactionQuerySettings] = settings;

            TransactionList model = InnerIndex(settings);

            return View(model);
        }

        private TransactionList InnerIndex(TransactionQuerySettings settings)
        {
            settings.PageSize = PageSize;

            TransactionList model = new TransactionList(db, userKey, settings);
            model.Items.ForEach(t => t.Amount = t.Amount * (t.IsCredit ? 1 : -1) * (t.IsDebt ? -1 : 1));

            //create staticPageList, defining your viewModel, current page, page size and total number of pages.
            model.PagedItems = new StaticPagedList<TransactionModel>(model.Items, settings.Page.Value, settings.PageSize.Value, model.TotalItems);

            ViewBag.AccountTypes = new SelectList(new AccountTypeList(db, userKey).Items, "Id", "Name");
            ViewBag.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name", AccountsToInclude = (settings.AccountId.HasValue ? new int[] { settings.AccountId.Value } : null) }).Items, "Id", "Name");
            ViewBag.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings() { Sort = "Name" }).Items, "Id", "Name");

            ViewBag.Sort = settings.Sort;
            ViewBag.Filter = settings.Search;
            ViewBag.NameSortParm = settings.Sort == "Name" ? "NameDesc" : "Name";
            ViewBag.DateSortParm = String.IsNullOrEmpty(settings.Sort) ? "DisplayDate" : "";

            return model;
        }

        // GET: Dashboard/Transactions/Create
        public ActionResult Create(string c = DefaultRedirectController, int? reminderId = null, DateTime? reminderDate = null, string amount = null)
        {
            TransactionModel model = new TransactionModel();
            model.RedirectController = c;

            // defaults
            model.DisplayDate = System.DateTime.Now;
            model.IsCredit = true;
            model.IsPosted = false;

            // if you're processing a reminder
            if (reminderId.HasValue)
            {
                ReminderModel reminder = ReminderModel.Get(db, userKey, reminderId.Value);

                if (reminder != null)
                {
                    model.AccountId = reminder.AccountId;
                    model.CategoryId = reminder.CategoryId;
                    model.DisplayDate = reminderDate.HasValue ? reminderDate.Value : reminder.NextDate;
                    model.IsCredit = reminder.IsCredit;
                    model.Amount = reminder.Amount;
                    if (amount != null)
                        model.Amount = decimal.Parse(amount);
                    model.Name = reminder.Name;

                    model.ReminderId = reminder.Id;
                }
            }

            model.PositiveText = "Positive";
            model.NegativeText = "Negative";

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");

            if (Request.IsAjaxRequest())
                return PartialView("_Create", model);
            else
                return View(model);
        }

        // POST: Dashboard/Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,AccountName,CategoryId,CategoryName,DisplayDate,IsCredit,AmountFormatted,Name,IsPosted,DatePosted,RedirectController,ReminderId")] TransactionModel model)
        {
            decimal amount;
            if (string.IsNullOrEmpty(model.AmountFormatted))
                amount = 0;
            else if (!decimal.TryParse(model.AmountFormatted, NumberStyles.Currency, CultureInfo.CurrentCulture, out amount))
                amount = 0;

            model.Amount = amount;

            if (ModelState.IsValid)
            {
                TransactionModel.Save(model, db, userKey, true);

                if (Request.IsAjaxRequest())
                {
                    TransactionModel result = TransactionModel.Get(db, userKey, model.Id);
                    result.EditLink = Url.Action("Edit", "Transactions", new { Id = result.Id });
                    result.PostLink = Url.Action("Post", "Transactions", new { Id = result.Id });

                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                else
                    return RedirectToAction("Index", String.IsNullOrEmpty(model.RedirectController) ? DefaultRedirectController : model.RedirectController);
            }

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");

            return View(model);
        }

        // GET: Dashboard/Transactions/Edit/5
        public ActionResult Edit(int? id, string c = DefaultRedirectController)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            TransactionModel model = TransactionModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();

            model.RedirectController = c;
            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name", AccountsToInclude = new int[] { model.AccountId } }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");

            if (Request.IsAjaxRequest())
                return PartialView("_Edit", model);
            else
                return View(model);
        }

        // POST: Dashboard/Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,AccountName,CategoryId,CategoryName,DisplayDate,IsCredit,Amount,AmountFormatted,Name,IsPosted,DatePosted,Sequence,RedirectController")] TransactionModel model)
        {
            decimal amount;
            if (string.IsNullOrEmpty(model.AmountFormatted))
                amount = 0;
            else if (!decimal.TryParse(model.AmountFormatted, NumberStyles.Currency, CultureInfo.CurrentCulture, out amount))
                amount = 0;

            model.Amount = amount;

            if (ModelState.IsValid)
            {
                TransactionModel.Save(model, db, userKey, false);

                if (Request.IsAjaxRequest())
                {
                    TransactionModel result = TransactionModel.Get(db, userKey, model.Id);
                    result.EditLink = Url.Action("Edit", "Transactions", new { Id = result.Id });
                    result.PostLink = Url.Action("Post", "Transactions", new { Id = result.Id });

                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                else
                    return RedirectToAction("Index", String.IsNullOrEmpty(model.RedirectController) ? DefaultRedirectController : model.RedirectController);
            }

            model.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name", AccountsToInclude = new int[] { model.AccountId } }).Items, "Id", "Name");
            model.Categories = new SelectList(new CategoryList(db, userKey, new CategoryQuerySettings()).Items, "Id", "Name");

            return View(model);
        }

        [HttpPost]
        public ActionResult Post(int Id)
        {
            TransactionModel.Post(db, userKey, Id);

            TransactionModel result = TransactionModel.Get(db, userKey, Id);
            result.EditLink = Url.Action("Edit", "Transactions", new { Id = result.Id });

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        // GET: Dashboard/Transactions/Delete/5
        public ActionResult Delete(int? id, string c = DefaultRedirectController)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            TransactionModel model = TransactionModel.Get(db, userKey, id.Value);
            if (model == null)
                return HttpNotFound();

            model.RedirectController = c;

            if (Request.IsAjaxRequest())
                return PartialView("_Delete", model);
            else
                return View(model);
        }

        // POST: Dashboard/Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string RedirectController)
        {
            TransactionModel.Delete(db, userKey, id);

            if (Request.IsAjaxRequest())
                return Json(new { deleted = true }, JsonRequestBehavior.DenyGet);
            else
                return RedirectToAction("Index", String.IsNullOrEmpty(RedirectController) ? DefaultRedirectController : RedirectController);
        }
        #endregion

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
