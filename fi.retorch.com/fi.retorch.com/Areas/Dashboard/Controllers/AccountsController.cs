using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Globalization;
using PagedList;
using fi.retorch.com.Models;
using fi.retorch.com.Areas.Dashboard.EntityModels;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class AccountsController : BaseController
    {
        private const string TempDataKeyAccountQuerySettings = "AccountQuerySettings";

        // GET: Dashboard/Accounts
        public ActionResult Index(string sort = null, int? page = 1, bool reset = false)
        {
            AccountQuerySettings settings = new AccountQuerySettings();
            if (TempData.ContainsKey(TempDataKeyAccountQuerySettings) && !reset)
                settings = (AccountQuerySettings)TempData[TempDataKeyAccountQuerySettings];
            
            settings.Page = page;
            settings.Sort = sort;
            settings.Search = settings.Filter;

            TempData[TempDataKeyAccountQuerySettings] = settings;

            AccountList model = InnerIndex(settings);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "TypeId,IsDisplayed,IsClosed,Search,Filter,Sort")] AccountQuerySettings settings)
        {
            settings.Page = 1;
            settings.Search = settings.Search ?? settings.Filter;

            TempData[TempDataKeyAccountQuerySettings] = settings;

            AccountList model = InnerIndex(settings);

            return View(model);
        }

        private AccountList InnerIndex(AccountQuerySettings settings)
        {
            settings.PageSize = PageSize;

            AccountList model = new AccountList(db, userKey, settings);
            model.Items.ForEach(t => t.CurrentBalance = t.CurrentBalance * (t.IsDebt ? -1 : 1));
            model.PagedItems = new StaticPagedList<AccountModel>(model.Items, settings.Page.Value, settings.PageSize.Value, model.TotalItems);

            ViewBag.AccountTypes = new SelectList(new AccountTypeList(db, userKey).Items, "Id", "Name");

            List<SelectListItem> displayOptions = new List<SelectListItem>();
            displayOptions.Add(new SelectListItem() { Text = "No", Value = "false" });
            displayOptions.Add(new SelectListItem() { Text = "Yes", Value = "true" });
            ViewBag.DisplayOptions = new SelectList(displayOptions, "Value", "Text");
            ViewBag.OpenOptions = new SelectList(displayOptions, "Value", "Text");

            ViewBag.Sort = settings.Sort;
            ViewBag.Filter = settings.Search;
            ViewBag.NameSortParm = settings.Sort == "Name" ? "NameDesc" : "Name";
            ViewBag.DateSortParm = System.String.IsNullOrEmpty(settings.Sort) ? "DisplayDate" : "";

            return model;
        }

        #region CRUD
        // GET: Dashboard/Accounts/Create
        public ActionResult Create()
        {
            AccountModel model = new AccountModel();
            model.DateOpened = System.DateTime.Now;
            model.IsDisplayed = true;
            model.IsClosed = false;

            model.Types = new SelectList(AccountTypeList.ConvertDataToList(db.AccountTypes.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");

            var categories = from c in db.Categories
                             join cg in db.CategoryGroups on c.GroupId equals cg.Id
                             where cg.UserId == userKey
                             where c.IsActive
                             select c;
            
            model.Categories = MultipleSelectListModel.ConvertDataToModel(categories.OrderBy(c => c.Name).ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).ToList(),
                new List<MultipleSelectData>());

            return View(model);
        }

        // POST: Dashboard/Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TypeId,TypeName,Name,DateOpened,OpeningBalanceFormatted,IsDisplayed,DisplayOrder,IsClosed,Categories")] AccountModel model)
        {
            decimal balance;
            if (string.IsNullOrEmpty(model.OpeningBalanceFormatted))
                balance = 0;
            else if (!decimal.TryParse(model.OpeningBalanceFormatted, NumberStyles.Currency, CultureInfo.CurrentCulture, out balance))
                balance = 0;

            model.OpeningBalance = balance;

            if (ModelState.IsValid)
            {
                int nextDisplayOrder = 1;
                if (model.IsDisplayed)
                {
                    Data.Account topAccount = db.Accounts.Where(a => a.TypeId == model.TypeId && a.Id != model.Id && a.UserId == userKey && a.IsDisplayed == true).OrderByDescending(a => a.DisplayOrder).FirstOrDefault();
                    if (topAccount != null)
                        nextDisplayOrder = topAccount.DisplayOrder + 1;
                }
                AccountModel.Save(model, db, userKey, true, nextDisplayOrder);
                
                return RedirectToAction("Index");
            }

            model.Types = new SelectList(AccountTypeList.ConvertDataToList(db.AccountTypes.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");

            var categories = from c in db.Categories
                             join cg in db.CategoryGroups on c.GroupId equals cg.Id
                             where cg.UserId == userKey
                             where c.IsActive
                             select c;
            
            model.Categories = MultipleSelectListModel.ConvertDataToModel(categories.OrderBy(c => c.Name).ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).ToList(),
                new List<MultipleSelectData>());

            //var categories = from c in db.Categories
            //                 join cg in db.CategoryGroups on c.GroupId equals cg.Id
            //                 where cg.UserId == userKey
            //                 select c;
            //var selected = model.Categories.ToList().Where(c => c.IsChecked).Select(c => new { Id = c.CategoryId, IsActive = true });
            //var selectList = selected.ToList().Select(s => AccountCategoryActive.ConvertDataToModel(s.Id, s.IsActive)).ToList();
            //model.Categories = AccountCategoryListModel.ConvertDataToModel(categories.ToList(), selectList);

            return View(model);
        }

        // GET: Dashboard/Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            AccountModel model = AccountModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();

            model.Types = new SelectList(AccountTypeList.ConvertDataToList(db.AccountTypes.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name", model.TypeId);
            
            //model.Categories = AccountCategoryListModel.ConvertDataToModel(categories.ToList(), selectList);


            var categories = from c in db.Categories
                             join cg in db.CategoryGroups on c.GroupId equals cg.Id
                             where cg.UserId == userKey
                             where c.IsActive
                             select c;
            var selected = from ac in db.AccountCategories
                           join a in db.Accounts on ac.AccountId equals a.Id
                           where a.Id == model.Id && a.UserId == userKey
                           select new { Id = ac.CategoryId, IsActive = ac.IsActive };

            model.Categories = MultipleSelectListModel.ConvertDataToModel(categories.OrderBy(c => c.Name).ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).ToList(),
                selected.ToList().Select(s => MultipleSelectData.ConvertDataToModel(s.Id, s.IsActive)).ToList());

            return View(model);
        }

        // POST: Dashboard/Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypeId,TypeName,Name,DateOpened,OpeningBalanceFormatted,IsDisplayed,DisplayOrder,IsClosed,Categories")] AccountModel model)
        {
            decimal balance;
            if (string.IsNullOrEmpty(model.OpeningBalanceFormatted))
                balance = 0;
            else if (!decimal.TryParse(model.OpeningBalanceFormatted, NumberStyles.Currency, CultureInfo.CurrentCulture, out balance))
                balance = 0;

            model.OpeningBalance = balance;

            if (ModelState.IsValid)
            {
                AccountModel.Save(model, db, userKey, false);

                return RedirectToAction("Index");
            }

            model.Types = new SelectList(AccountTypeList.ConvertDataToList(db.AccountTypes.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name", model.TypeId);

            var categories = from c in db.Categories
                             join cg in db.CategoryGroups on c.GroupId equals cg.Id
                             where cg.UserId == userKey
                             select c;
            var selected = model.Categories.ToList().Where(c => c.IsChecked).Select(c => new { Id = c.SelectionId, IsActive = true });

            model.Categories = MultipleSelectListModel.ConvertDataToModel(categories.ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).ToList(),
                selected.ToList().Select(s => MultipleSelectData.ConvertDataToModel(s.Id, s.IsActive)).ToList());

            return View(model);
        }

        // GET: Dashboard/Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            AccountModel model = AccountModel.Get(db, userKey, id.Value);
            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Dashboard/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountModel.Delete(db, userKey, id);

            return RedirectToAction("Index");
        }
        #endregion

        #region AJAX functions
        // TODO move all of these into Entity/Models; pass models in AJAX, not data
        public ActionResult GetAccountCategories(int? Id = null, int? CategoryId = null)
        {
            var categories = from ac in db.AccountCategories
                             join c in db.Categories on ac.CategoryId equals c.Id
                             where c.UserId == userKey && ac.AccountId == Id && (c.IsActive == true || c.Id == CategoryId)
                             select new { Id = c.Id, Value = c.Name, IsActive = c.IsActive };
            categories = categories.OrderBy(c => c.Value);
            return Json(categories.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccountType(int? Id = null)
        {
            var type = from at in db.AccountTypes
                             join a in db.Accounts on at.Id equals a.TypeId
                             where a.Id == Id && a.UserId == userKey
                             select at;
            return Json(type.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccountsByType(int Id, bool? isClosed = null)
        {
            var query = from a in db.Accounts
                        join at in db.AccountTypes on a.TypeId equals at.Id
                        where a.UserId == userKey && at.Id == Id
                            && (isClosed == null || a.IsClosed == isClosed.Value)
                        select new AccountEntity
                        {
                            Account = a,
                            Type = at
                        };
            AccountList list = new AccountList();
            list.Items = new List<AccountModel>();
            query.ToList().ForEach(q => list.Items.Add(AccountModel.ConvertEntityToModel(q)));

            list.Items = list.Items.OrderBy(l => l.IsClosed).ThenBy(l => l.Name).ToList();

            return Json(list.Items, JsonRequestBehavior.AllowGet);
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
