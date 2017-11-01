using fi.retorch.com.Areas.Dashboard.Code.Enums;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Wizard.Code.Base;
using fi.retorch.com.Areas.Wizard.Models;
using fi.retorch.com.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Wizard.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Wizard/Home
        public ActionResult Index()
        {
            DefaultAccountTypeList model = new DefaultAccountTypeList(defaultDb);

            return View(model);
        }

        [HttpGet]
        public ActionResult Step1()
        {
            DefaultAccountTypeList model = new DefaultAccountTypeList(defaultDb);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step1(DefaultAccountTypeList model)
        {
            if (ModelState.IsValid)
            {
                model.AccountTypes.Where(t => t.IsChecked).ToList().ForEach(t => {
                    AccountTypeModel newItem = t.ConvertDefaultToModel();

                    // don't create duplicates
                    if (AccountTypeModel.FindByName(db, userKey, newItem.Name) == null)
                        AccountTypeModel.Save(t.ConvertDefaultToModel(), db, userKey, true);
                });

                // if none were created, create a default
                AccountTypeList list = new AccountTypeList(db, userKey);
                if (list.Items.Count == 0)
                {
                    AccountTypeModel newItem = new AccountTypeModel();
                    newItem.IsDebt = false;
                    newItem.Name = "All Accounts";
                    newItem.PositiveText = "Deposit";
                    newItem.NegativeText = "Withdrawal";
                    AccountTypeModel.Save(newItem, db, userKey, true);
                }

                return RedirectToAction("Step2");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Step2()
        {
            DefaultCategoryGroupList model = new DefaultCategoryGroupList(defaultDb);

            var types = new Dictionary<int, string>();
            foreach (TransferTypeEnum transferType in System.Enum.GetValues(typeof(TransferTypeEnum)))
            {
                types.Add((int)transferType, transferType.ToFriendlyString());
            }
            ViewBag.TransferTypes = types;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step2(DefaultCategoryGroupList model)
        {
            if (ModelState.IsValid)
            {
                model.CategoryGroups.Where(t => t.IsChecked).ToList().ForEach(t => {
                    CategoryGroupModel newItem = t.ConvertDefaultToModel();

                    // don't create duplicates
                    if (CategoryGroupModel.FindByName(db, userKey, newItem.Name) == null)
                        CategoryGroupModel.Save(t.ConvertDefaultToModel(), db, userKey, true);
                });
                
                // if none were created, create a default
                CategoryGroupList list = new CategoryGroupList(db, userKey);
                if (list.Items.Count == 0)
                {
                    CategoryGroupModel newItem = new CategoryGroupModel();
                    newItem.TransferType = (int)TransferTypeEnum.Expense;
                    newItem.Name = "All Categories";
                    CategoryGroupModel.Save(newItem, db, userKey, true);
                }

                return RedirectToAction("Step3");
            }

            var types = new Dictionary<int, string>();
            foreach (TransferTypeEnum transferType in System.Enum.GetValues(typeof(TransferTypeEnum)))
            {
                types.Add((int)transferType, transferType.ToFriendlyString());
            }
            ViewBag.TransferTypes = types;

            return View(model);
        }

        [HttpGet]
        public ActionResult Step3()
        {
            DefaultCategoryList model = new DefaultCategoryList(defaultDb, db, userKey);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step3(DefaultCategoryList model)
        {
            if (ModelState.IsValid)
            {
                model.Categories.Where(t => t.IsChecked).ToList().ForEach(t => {
                    CategoryModel newCtgry = t.ConvertDefaultToModel();

                    // don't create duplicates
                    if (CategoryModel.FindByName(db, userKey, newCtgry.Name) == null)
                        CategoryModel.Save(t.ConvertDefaultToModel(), db, userKey, true);
                });

                // if none were created, create a default
                CategoryList list = new CategoryList(db, userKey, new Dashboard.Code.QuerySettings.CategoryQuerySettings());
                if (list.Items.Count == 0)
                {
                    CategoryGroupList groupList = new CategoryGroupList(db, userKey);

                    CategoryModel newItem = new CategoryModel();
                    newItem.GroupId = groupList != null && groupList.Items != null && groupList.Items.Count > 0 ? groupList.Items.First().Id : (int?)null;
                    newItem.IsActive = true;
                    newItem.Name = "Uncategorized";
                    CategoryModel.Save(newItem, db, userKey, true);
                }

                return RedirectToAction("Step4");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Step4()
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

            List<MultipleSelectData> data = categories.OrderBy(c => c.Name).ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name, IsActive = true }).ToList();
            model.Categories = MultipleSelectListModel.ConvertDataToModel(data, data);

            return View(model);
        }

        [HttpPost]
        public ActionResult Step4([Bind(Include = "Id,TypeId,TypeName,Name,DateOpened,OpeningBalanceFormatted,IsDisplayed,DisplayOrder,IsClosed,Categories")] AccountModel model)
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

                return RedirectToAction("Index", "Home", new { Area = "Dashboard" });
            }

            model.Types = new SelectList(AccountTypeList.ConvertDataToList(db.AccountTypes.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name");

            var categories = from c in db.Categories
                             join cg in db.CategoryGroups on c.GroupId equals cg.Id
                             where cg.UserId == userKey
                             where c.IsActive
                             select c;

            List<MultipleSelectData> data = categories.OrderBy(c => c.Name).ToList().Select(c => new MultipleSelectData { Id = c.Id, Name = c.Name, IsActive = true }).ToList();
            model.Categories = MultipleSelectListModel.ConvertDataToModel(data, data);

            return View(model);
        }
    }
}