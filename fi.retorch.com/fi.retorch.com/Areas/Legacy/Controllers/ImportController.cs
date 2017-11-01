using fi.retorch.com.Areas.Dashboard.Code.Enums;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Legacy.Code.Base;
using fi.retorch.com.Areas.Legacy.Models;
using fi.retorch.com.Data;
using fi.retorch.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Legacy.Controllers
{
    public class ImportController : BaseController
    {

        // legacy user_id
        private bool hasUserId { get { return Request.Cookies["user_id"] != null; } }
        private int UserId
        {
            get
            {
                return int.Parse(Request.Cookies["user_id"].Value);
            }
            set
            {
                Response.Cookies["user_id"].Value = value.ToString();
            }
        }

        // GET: Dashboard/Import
        public ActionResult Index()
        {
            LoginModel model = new LoginModel();
            model.Authorized = hasUserId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            bool isValid = false;
            int id = model.Validate(legacyDb);

            if (id > 0)
            {
                UserId = id;
                isValid = true;
            }

            return Json(isValid, JsonRequestBehavior.DenyGet);
        }
    
        public ActionResult Bookmarks()
        {
            Dictionary<int, int> results = new Dictionary<int, int>();

            // import accounts and return list
            if (hasUserId)
            {
                LegacyBookmarkList importData = new LegacyBookmarkList(legacyDb, UserId);
                foreach (LegacyBookmarkModel importBookmark in importData.Items)
                {
                    // create replacement
                    BookmarkModel newBookmark = importBookmark.ConvertLegacyToModel();

                    // save it to db
                    newBookmark.Save(newBookmark, db, userKey, true);

                    // map new id to old
                    results.Add(importBookmark.Id, newBookmark.Id);

                }
            }

            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Categories()
        {
            int defaultGroupId = 0;
            Dictionary<int, int> results = new Dictionary<int, int>();

            // import categories and return list
            if (hasUserId)
            {
                CategoryGroupModel defaultCategoryGroup = CategoryGroupModel.FindByName(db, userKey, "Household Expenses");
                if (defaultCategoryGroup == null)
                {
                    defaultCategoryGroup = new CategoryGroupModel();
                    defaultCategoryGroup.TransferType = (int)TransferTypeEnum.Expense;
                    defaultCategoryGroup.Name = "Categories";

                    CategoryGroupModel.Save(defaultCategoryGroup, db, userKey, true);
                }

                defaultGroupId = defaultCategoryGroup.Id;
                Session["DefaultGroupId"] = defaultGroupId;
                
                LegacyCategoryList importData = new LegacyCategoryList(legacyDb, UserId);
                foreach (LegacyCategoryModel importCategory in importData.Items)
                {
                    // create replacement
                    CategoryModel newCategory = importCategory.ConvertLegacyToModel(defaultGroupId);

                    // save it to db
                    CategoryModel.Save(newCategory, db, userKey, true);

                    // map new id to old
                    results.Add(importCategory.Id, newCategory.Id);

                }

                Session["CategoryMappings"] = results;
            }

            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Accounts()
        {
            Dictionary<int, int> results = new Dictionary<int, int>();

            // import accounts and return list
            if (hasUserId)
            {
                LegacyAccountList importData = new LegacyAccountList(legacyDb, UserId);
                foreach (LegacyAccountModel importAccount in importData.Items)
                {
                    // create replacement
                    AccountModel newAccount = importAccount.ConvertLegacyToModel(db, userKey);

                    // save it to db
                    AccountModel.Save(newAccount, db, userKey, true, newAccount.DisplayOrder);

                    // map new id to old
                    results.Add(importAccount.Id, newAccount.Id);

                }

                Session["AccountMappings"] = results;
            }

            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountCategories()
        {
            Dictionary<int, int> CategoryMappings = (Dictionary <int, int>)Session["CategoryMappings"];
            Dictionary<int, int> AccountMappings = (Dictionary <int, int>)Session["AccountMappings"];

            List<Tuple<int, int>> results = new List<Tuple<int, int>>();

            var query = from ac in legacyDb.act_account_categories
                                                         join a in legacyDb.act_accounts on ac.account_id equals a.account_id
                                                         where a.user_id == UserId
                                                         select ac;
            List<act_account_categories> actCategories = query.Distinct().ToList();

            foreach (act_account_categories actCategory in actCategories)
            {
                AccountCategory data = new AccountCategory();

                if (AccountMappings.Keys.Contains(actCategory.account_id) && CategoryMappings.Keys.Contains(actCategory.category_id))
                {
                    data.AccountId = AccountMappings[actCategory.account_id];
                    data.CategoryId = CategoryMappings[actCategory.category_id];
                    data.IsActive = true;

                    db.AccountCategories.Add(data);
                    int result = db.SaveChanges();

                    if (result > 0)
                    {
                        results.Add(new Tuple<int, int>(data.AccountId, data.CategoryId));
                    }
                }
            }
            
            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reminders()
        {
            int defaultCategoryId = 0;
            Dictionary<int, int> results = new Dictionary<int, int>();

            // import Reminders and return list
            if (hasUserId)
            {
                Dictionary<int, int> AccountMappings = (Dictionary<int, int>)Session["AccountMappings"];
                Dictionary<int, int> CategoryMappings = (Dictionary<int, int>)Session["CategoryMappings"];

                CategoryModel defaultCategory = new CategoryModel();
                defaultCategory.Accounts = new List<MultipleSelectModel>();
                defaultCategory.GroupId = int.Parse(Session["DefaultGroupId"].ToString());
                defaultCategory.Name = "Uncategorized";
                defaultCategory.IsActive = true;

                CategoryModel.Save(defaultCategory, db, userKey, true);
                defaultCategoryId = defaultCategory.Id;
                Session["DefaultCategoryId"] = defaultCategoryId;

                LegacyReminderList importData = new LegacyReminderList(legacyDb, UserId);
                foreach (LegacyReminderModel importReminder in importData.Items)
                {
                    if (AccountMappings.ContainsKey(importReminder.AccountId))
                    {
                        // create replacement
                        ReminderModel newReminder = importReminder.ConvertLegacyToModel(defaultCategoryId, AccountMappings, CategoryMappings);

                        // save it to db
                        ReminderModel.Save(newReminder, db, userKey, true, false);

                        // map new id to old
                        results.Add(importReminder.Id, newReminder.Id);

                    }
                }

                db.SaveChanges();
            }
            
            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Transactions()
        {
            int defaultCategoryId = 0;
            Dictionary<int, int> results = new Dictionary<int, int>();

            // import Transactions and return list
            if (hasUserId)
            {
                Dictionary<int, int> AccountMappings = (Dictionary <int, int>)Session["AccountMappings"];
                Dictionary<int, int> CategoryMappings = (Dictionary<int, int>)Session["CategoryMappings"];
                
                defaultCategoryId = (int)Session["DefaultCategoryId"];

                LegacyTransactionList importData = new LegacyTransactionList(legacyDb, UserId);
                foreach (LegacyTransactionModel importTransaction in importData.Items)
                {
                    if (AccountMappings.ContainsKey(importTransaction.AccountId))
                    {
                        // create replacement
                        TransactionModel newTransaction = importTransaction.ConvertLegacyToModel(defaultCategoryId, AccountMappings, CategoryMappings);

                        // save it to db
                        TransactionModel.Save(newTransaction, db, userKey, true, false);

                        // map new id to old
                        results.Add(importTransaction.Id, newTransaction.Id);

                    }
                }

                db.SaveChanges();
            }

            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }
    }
}