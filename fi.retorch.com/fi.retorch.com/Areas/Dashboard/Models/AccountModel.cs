using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.Enums;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Data;
using fi.retorch.com.Models;
using LinqKit;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Dashboard.Models
{
    public class AccountList
    {
        public int TotalItems { get; set; }
        public List<AccountModel> Items { get; set; }
        public IPagedList<AccountModel> PagedItems { get; set; }
        public AccountQuerySettings Settings { get; set; }

        public AccountList()
        {
        }

        public AccountList(Entities db, string userKey, AccountQuerySettings settings)
        {
            Settings = settings;
            if (Settings == null)
                if (Settings == null)
                    Settings = new AccountQuerySettings();

            Items = new List<AccountModel>();

            var data = from a in db.Accounts.AsExpandable()
                       join at in db.AccountTypes.AsExpandable() on a.TypeId equals at.Id
                       where a.UserId == userKey
                       select new AccountEntity { Account = a, Type = at };

            var data2 = data;
            if (Settings.AccountsToInclude != null && Settings.AccountsToInclude.Any())
            {
                var predicate = PredicateBuilder.New<AccountEntity>(false);

                foreach (int AccountId in Settings.AccountsToInclude)
                    predicate = predicate.Or(p => p.Account.Id == AccountId);

                data2 = data.Where(predicate);
            }

            if (Settings.TypeId != null || Settings.IsDisplayed != null || Settings.IsClosed != null)
            {
                var predicate = PredicateBuilder.New<AccountEntity>(true);
                if (Settings.TypeId != null)
                    predicate = predicate.And(p => p.Account.TypeId == Settings.TypeId);
                if (Settings.IsDisplayed != null)
                    predicate = predicate.And(p => p.Account.IsDisplayed == Settings.IsDisplayed);
                if (Settings.IsClosed != null)
                    predicate = predicate.And(p => p.Account.IsClosed == Settings.IsClosed);
                data = data.Where(predicate);
            }

            if (Settings.Search != null)
            {
                var predicate = PredicateBuilder.New<AccountEntity>(false);

                predicate = predicate.Or(p => p.Account.Name.Contains(this.Settings.Search));
                predicate = predicate.Or(p => p.Type.Name.Contains(this.Settings.Search));

                data = data.Where(predicate);
            }

            if (Settings.AccountsToInclude != null && Settings.AccountsToInclude.Any())
            {
                data = data.Union(data2);
            }

            switch (Settings.Sort)
            {
                case "DisplayOrder":
                    data = data.OrderBy(d => d.Account.DisplayOrder);
                    break;
                case "DisplayOrderDesc":
                    data = data.OrderByDescending(d => d.Account.DisplayOrder);
                    break;
                case "NameDesc":
                    data = data.OrderByDescending(d => d.Account.Name);
                    break;
                default:
                case "Name":
                    data = data.OrderBy(d => d.Account.Name);
                    break;
            }

            if (Settings.PageSize != null)
            {
                // save information for paging
                TotalItems = data.Count();

                if (Settings.Page == null)
                    Settings.Page = 1;

                data = data.Skip(Settings.PageSize.Value * (Settings.Page.Value - 1)).Take(Settings.PageSize.Value);
            }

            data.ToList().ForEach(d => Items.Add(AccountModel.ConvertEntityToModel(d)));
        }

        public static AccountList GetDashboardAccounts(Entities db, string userKey)
        {
            AccountList accounts = new AccountList();
            accounts.Items = new List<AccountModel>();

            var data = from a in db.Accounts.AsExpandable()
                       join at in db.AccountTypes.AsExpandable() on a.TypeId equals at.Id
                       where a.UserId == userKey
                       select new AccountEntity { Account = a, Type = at };

            var predicate = PredicateBuilder.New<AccountEntity>(true);
            predicate = predicate.And(p => p.Account.IsDisplayed == true);
            predicate = predicate.And(p => p.Account.IsClosed == false);
            data = data.Where(predicate);


            data = data.OrderBy(d => d.Account.DisplayOrder);

            data.ToList().ForEach(d => accounts.Items.Add(AccountModel.ConvertEntityToModel(d)));

            return accounts;
        }
    }

    public class AccountModel : BaseModel
    {
        public int Id { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int TypeId { get; set; }
        
        public string TypeName { get; set; }
        public bool IsDebt { get; set; }

        [Required]
        [Display(Name = "Account")]
        public string Name { get; set; }

        [Display(Name = "Opening Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOpened { get; set; }

        [Display(Name = "Initial Balance")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? OpeningBalance { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public string OpeningBalanceFormatted { get; set; }

        [Display(Name = "Balance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? CurrentBalance { get; set; }

        [Display(Name = "Display on Dashboard")]
        public bool IsDisplayed { get; set; }

        public int DisplayOrder { get; set; }

        [Display(Name = "Account Closed")]
        public bool IsClosed { get; set; }

        [Display(Name = "Date Closed")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateClosed { get; set; }

        public List<MultipleSelectModel> Categories { get; set; }

        public static AccountModel ConvertEntityToModel(AccountEntity data)
        {
            AccountModel model = null;

            if (data != null)
            {
                model = new AccountModel();
                
                model.Id = data.Account.Id;
                model.TypeId = data.Account.TypeId;
                model.TypeName = data.Type != null ? data.Type.Name : null;
                model.IsDebt = data.Type != null ? data.Type.IsDebt : false;
                model.Name = data.Account.Name;
                
                //model.DateOpened = DateTime.Now;
                model.DateOpened = data.Account.DateOpened;
                model.OpeningBalance = data.Account.OpeningBalance;
                model.OpeningBalanceFormatted = data.Account.OpeningBalance.ToString();
                model.CurrentBalance = data.Account.CurrentBalance;
                model.IsClosed = data.Account.IsClosed;
                model.DateClosed = data.Account.DateClosed;
                model.IsDisplayed = data.Account.IsDisplayed;
                model.DisplayOrder = data.Account.DisplayOrder;
            }

            return model;
        }

        public static AccountModel Get(Entities db, string userKey, int id)
        {
            var data = from a in db.Accounts
                       where a.Id == id
                       join at in db.AccountTypes on a.TypeId equals at.Id
                       where a.UserId == userKey
                       //where !a.IsClosed
                       select new AccountEntity { Account = a, Type = at };

            return ConvertEntityToModel(data.FirstOrDefault());
        }

        public static void Save(AccountModel model, Entities db, string userKey, bool isNew = false, int? nextDisplayOrder = null)
        {
            Account data = null;

            if (isNew)
            {
                data = new Account();

                data.UserId = userKey;
                data.TypeId = model.TypeId;
                data.Name = model.Name;

                //data.DateOpened = DateTime.Now;
                data.DateOpened = model.DateOpened;
                data.OpeningBalance = model.OpeningBalance.Value;
                data.CurrentBalance = model.OpeningBalance.Value;
                data.IsClosed = model.IsClosed;
                data.IsDisplayed = model.IsDisplayed;

                if (model.IsDisplayed && nextDisplayOrder.HasValue)
                    data.DisplayOrder = nextDisplayOrder.HasValue ? nextDisplayOrder.Value : model.DisplayOrder;

                db.Accounts.Add(data);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }

                model.Id = data.Id;
            }
            else
            {
                data = db.Accounts.Where(cg => cg.Id == model.Id && cg.UserId == userKey).FirstOrDefault();
                // in case this changes, update current
                decimal oldOpeningBalance = data.OpeningBalance;

                data.TypeId = model.TypeId;
                data.Name = model.Name;

                data.DateOpened = model.DateOpened;
                data.OpeningBalance = model.OpeningBalance.Value;
                data.IsClosed = model.IsClosed;
                data.IsDisplayed = model.IsDisplayed;

                if (model.IsDisplayed && nextDisplayOrder.HasValue)
                    data.DisplayOrder = nextDisplayOrder.Value;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                if (data.OpeningBalance != oldOpeningBalance)
                    AccountModel.UpdateCurrentBalance(db, userKey, data, CurrentBalanceUpdateAction.UpdateOpeningBalance, oldOpeningBalance, data.OpeningBalance);
            }


            // handle categories
            var existing = from ac in db.AccountCategories
                           where ac.AccountId == data.Id
                           select ac;
            List<MultipleSelectModel> newCategories = new List<MultipleSelectModel>();
            if (model.Categories != null)
                newCategories = model.Categories.Where(c => c.IsChecked && !existing.ToList().Exists(e => e.CategoryId == c.SelectionId)).ToList();

            foreach(MultipleSelectModel cat in newCategories)
            {
                AccountCategory dbCat = new AccountCategory();

                dbCat.AccountId = data.Id;
                dbCat.CategoryId = cat.SelectionId;
                dbCat.IsActive = true;

                db.AccountCategories.Add(dbCat);
            }
            foreach(AccountCategory cat in existing)
            {
                MultipleSelectModel modelCat = null;
                if (model.Categories != null)
                    modelCat = model.Categories.FirstOrDefault(c => c.SelectionId == cat.CategoryId);

                bool newValue = modelCat != null ? modelCat.IsChecked : false; // deleted category does not exist, make inactive
                if (cat.IsActive != newValue)
                {
                    cat.IsActive = newValue;
                    db.Entry(cat).State = EntityState.Modified;
                }
            }
            db.SaveChanges();

        }

        public static void UpdateCurrentBalance(Entities db, string userKey, Account account, CurrentBalanceUpdateAction action, decimal oldAmount, decimal newAmount, bool commit = true)
        {
            account.CurrentBalance = account.CurrentBalance - oldAmount + newAmount;
            // account.LastModified = DateTime.Now;
            db.Entry(account).State = EntityState.Modified;
            if (commit)
                db.SaveChanges();
        }

        public static void Delete(Entities db, string userKey, int id)
        {
            Account data = db.Accounts.Where(cg => cg.Id == id && cg.UserId == userKey).FirstOrDefault();
            db.Accounts.Remove(data);
            db.SaveChanges();
        }
    }
}