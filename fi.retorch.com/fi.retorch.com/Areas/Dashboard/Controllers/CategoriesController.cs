using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Dashboard.Code.Base;
using PagedList;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Models;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class CategoriesController : BaseController
    {
        private const string TempDataKeyCategoryQuerySettings = "CategoryQuerySettings";

        // GET: Dashboard/Categories
        public ActionResult Index(string sort = null, int? page = 1, bool reset = false)
        {
            CategoryQuerySettings settings = new CategoryQuerySettings();
            if (TempData.ContainsKey(TempDataKeyCategoryQuerySettings) && !reset)
                settings = (CategoryQuerySettings)TempData[TempDataKeyCategoryQuerySettings];

            settings.Page = page;
            settings.Search = settings.Filter;
            settings.Sort = sort;

            TempData[TempDataKeyCategoryQuerySettings] = settings;

            //CategoryList list = new CategoryList(db, userKey, settings);
            //IPagedList<CategoryModel> pagedItems = new StaticPagedList<CategoryModel>(list.Items, page.Value, PageSize, list.TotalItems);

            CategoryList model = InnerIndex(settings);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "GroupId,IsActive,Search,Filter,Sort")] CategoryQuerySettings settings)
        {
            settings.Page = 1;
            settings.Search = settings.Search ?? settings.Filter;

            TempData[TempDataKeyCategoryQuerySettings] = settings;

            CategoryList model = InnerIndex(settings);

            return View(model);
        }

        private CategoryList InnerIndex(CategoryQuerySettings settings)
        {
            settings.PageSize = PageSize;

            CategoryList model = new CategoryList(db, userKey, settings);

            //create staticPageList, defining your viewModel, current page, page size and total number of pages.
            model.PagedItems = new StaticPagedList<CategoryModel>(model.Items, settings.Page.Value, settings.PageSize.Value, model.TotalItems);
            
            ViewBag.CategoryGroups = new SelectList(new CategoryGroupList(db, userKey).Items, "Id", "Name");

            List<SelectListItem> displayOptions = new List<SelectListItem>();
            displayOptions.Add(new SelectListItem() { Text = "No", Value = "false" });
            displayOptions.Add(new SelectListItem() { Text = "Yes", Value = "true" });
            ViewBag.IsActiveOptions = new SelectList(displayOptions, "Value", "Text");

            ViewBag.Sort = settings.Sort;
            ViewBag.Filter = settings.Search;
            ViewBag.NameSortParm = settings.Sort == "Name" ? "NameDesc" : "Name";
            ViewBag.DateSortParm = string.IsNullOrEmpty(settings.Sort) ? "DisplayDate" : "";

            return model;
        }

        // GET: Dashboard/Categories/Create
        public ActionResult Create()
        {
            CategoryModel model = new CategoryModel();
            model.Groups = new SelectList(CategoryGroupList.ConvertDataToList(db.CategoryGroups.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");

            var accounts = from a in db.Accounts
                             where a.UserId == userKey // and c.IsClosed == false
                             select a;

            model.Accounts = MultipleSelectListModel.ConvertDataToModel(accounts.ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).OrderBy(s => s.Name).ToList(),
                new List<MultipleSelectData>());

            return View(model);
        }

        // POST: Dashboard/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GroupId,Name,IsActive,Accounts")] CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                CategoryModel.Save(model, db, userKey, true);
                
                return RedirectToAction("Index");
            }

            model.Groups = new SelectList(CategoryGroupList.ConvertDataToList(db.CategoryGroups.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");
            return View(model);
        }

        // GET: Dashboard/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            CategoryModel model = CategoryModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();

            model.Groups = new SelectList(CategoryGroupList.ConvertDataToList(db.CategoryGroups.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");

            var accounts = from a in db.Accounts
                           where a.UserId == userKey // and c.IsClosed == false
                           select a;

            var selected = from ac in db.AccountCategories
                           join c in db.Categories on ac.CategoryId equals c.Id
                           where c.Id == model.Id
                           select new { Id = ac.AccountId, IsActive = ac.IsActive };

            model.Accounts = MultipleSelectListModel.ConvertDataToModel(accounts.ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).OrderBy(s => s.Name).ToList(),
                selected.ToList().Select(s => MultipleSelectData.ConvertDataToModel(s.Id, s.IsActive)).ToList());

            return View(model);
        }

        // POST: Dashboard/Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GroupId,Name,IsActive,Accounts")] CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                CategoryModel.Save(model, db, userKey, false);

                return RedirectToAction("Index");
            }

            model.Groups = new SelectList(CategoryGroupList.ConvertDataToList(db.CategoryGroups.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");

            var accounts = from a in db.Accounts
                           where a.UserId == userKey // and c.IsClosed == false
                           select a;

            var selected = model.Accounts.ToList().Where(c => c.IsChecked).Select(c => new { Id = c.SelectionId, IsActive = true });

            model.Accounts = MultipleSelectListModel.ConvertDataToModel(accounts.ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name }).OrderBy(s => s.Name).ToList(),
                selected.ToList().Select(s => MultipleSelectData.ConvertDataToModel(s.Id, s.IsActive)).ToList());

            return View(model);
        }

        // GET: Dashboard/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            CategoryModel model = CategoryModel.Get(db, userKey, id.Value);
            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Dashboard/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryModel.Delete(db, id);

            return RedirectToAction("Index");
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
