using fi.retorch.com.Areas.Dashboard.Code.Enums;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Data;
using LinqKit;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Dashboard.Models
{
    public class TransactionList
    {
        public List<TransactionModel> Items { get; set; }
        public int TotalItems { get; set; }
        public IPagedList<TransactionModel> PagedItems { get; set; }
        public TransactionQuerySettings Settings { get; set; }

        public TransactionList()
        {
        }

        public TransactionList(Entities db, string userKey, TransactionQuerySettings settings)
        {
            Settings = settings;
            if (Settings == null)
                Settings = new TransactionQuerySettings();

            Items = new List<TransactionModel>();

            // get all matching transactions
            var data = from t in db.Transactions.AsExpandable()
                       join a in db.Accounts.AsExpandable() on t.AccountId equals a.Id
                       where a.UserId == userKey
                       join at in db.AccountTypes on a.TypeId equals at.Id
                       join c in db.Categories.AsExpandable() on t.CategoryId equals c.Id into cj
                       from rc in cj.DefaultIfEmpty()
                       select new TransactionEntity { Transaction = t, Account = a, AccountType = at, Category = rc };

            if (Settings.AccountTypeId.HasValue || Settings.AccountId.HasValue || Settings.CategoryId.HasValue || Settings.StartDate != null || Settings.EndDate != null)
            {
                var predicate = PredicateBuilder.New<TransactionEntity>(true);

                if (Settings.AccountTypeId.HasValue)
                    predicate = predicate.And(p => p.Account.TypeId == Settings.AccountTypeId.Value);
                if (Settings.AccountId.HasValue)
                    predicate = predicate.And(p => p.Transaction.AccountId == Settings.AccountId.Value);
                if (Settings.CategoryId.HasValue)
                    predicate = predicate.And(p => p.Transaction.CategoryId == Settings.CategoryId.Value);

                if (Settings.StartDate != null)
                    predicate = predicate.And(p => p.Transaction.DisplayDate >= Settings.StartDate);
                if (Settings.EndDate != null)
                    predicate = predicate.And(p => p.Transaction.DisplayDate <= Settings.EndDate);

                data = data.Where(predicate);
            }

            if (Settings.Search != null)
            {
                var predicate = PredicateBuilder.New<TransactionEntity>(false);

                predicate = predicate.Or(p => p.Transaction.Name.Contains(Settings.Search));
                predicate = predicate.Or(p => p.Account.Name.Contains(Settings.Search));
                predicate = predicate.Or(p => p.Category.Name.Contains(Settings.Search));

                data = data.Where(predicate);
            }

            // sort records
            switch (Settings.Sort)
            {
                case "Amount":
                    data = data.OrderBy(t => t.Transaction.Amount);
                    break;
                case "AmountDesc":
                    data = data.OrderByDescending(t => t.Transaction.Amount);
                    break;
                case "Name":
                    data = data.OrderBy(t => t.Transaction.Name);
                    break;
                case "NameDesc":
                    data = data.OrderByDescending(t => t.Transaction.Name);
                    break;
                case "DisplayDate":
                    data = data.OrderBy(t => t.Transaction.DisplayDate);
                    break;
                default:
                case "DisplayDateDesc":
                    data = data.OrderByDescending(t => t.Transaction.DisplayDate);
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

            data.ToList().ForEach(d => Items.Add(TransactionModel.ConvertDataToModel(d)));
        }


        public static TransactionList GetDashboardTransactions(Entities db, string userKey, TransactionQuerySettings settings)
        {
            TransactionList list = new TransactionList();
            list.Items = new List<TransactionModel>();

            // get all matching transactions
            var data = from t in db.Transactions.AsExpandable()
                       join a in db.Accounts.AsExpandable() on t.AccountId equals a.Id
                       where a.UserId == userKey && a.IsDisplayed == true && a.IsClosed == false
                       select new TransactionEntity { Transaction = t, Account = a };

            if (settings.StartDate != null || settings.EndDate != null)
            {
                var predicate = PredicateBuilder.New<TransactionEntity>(true);
                
                // always show non-posted transactions from the past (but not the future)
                if (settings.StartDate != null)
                    predicate = predicate.And(p => p.Transaction.DatePosted == null || p.Transaction.DisplayDate >= settings.StartDate);
                if (settings.EndDate != null)
                    predicate = predicate.And(p => p.Transaction.DisplayDate <= settings.EndDate);

                data = data.Where(predicate);
            }

            data.ToList().ForEach(d => list.Items.Add(TransactionModel.ConvertDataToModel(d)));

            return list;
        }
    }

    public class TransactionModel
    {
        #region Properties
        public int Id { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
        [Required]
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public bool IsDebt { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DisplayDate { get; set; }

        public string PositiveText { get; set; }
        public string NegativeText { get; set; }
        [Display(Name = "Credit")]
        public bool IsCredit { get; set; }

        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Amount { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public string AmountFormatted { get; set; }

        [Required]
        [Display(Name = "Transaction")]
        public string Name { get; set; }

        [Display(Name = "Posted To Account")]
        public bool IsPosted { get; set; }

        [Display(Name = "Date Posted")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DatePosted { get; set; }

        public int Sequence { get; set; }

        public string RedirectController { get; set; }
        public string EditLink { get; set; }
        public string PostLink { get; set; }
        public int? ReminderId { get; set; }
        #endregion

        public static TransactionModel ConvertDataToModel(TransactionEntity data)
        {
            TransactionModel model = null;

            if (data != null)
            {
                model = new TransactionModel();

                model.Id = data.Transaction.Id;
                model.AccountId = data.Transaction.AccountId;

                model.AccountName = data.Account != null ? data.Account.Name : null;

                model.IsDebt = data.AccountType != null ? data.AccountType.IsDebt : false;
                model.PositiveText = data.AccountType != null && data.AccountType.PositiveText != null ? data.AccountType.PositiveText : "Positive";
                model.NegativeText = data.AccountType != null && data.AccountType.NegativeText != null ? data.AccountType.NegativeText : "Negative";

                model.CategoryId = data.Transaction.CategoryId;

                model.CategoryName = data.Category != null ? data.Category.Name : null;

                model.Name = data.Transaction.Name;

                //model.DisplayDate = DateTime.Now;
                model.DisplayDate = data.Transaction.DisplayDate;
                model.Amount = data.Transaction.Amount;
                model.AmountFormatted = data.Transaction.Amount.ToString();
                model.IsCredit = data.Transaction.IsCredit;
                model.IsPosted = data.Transaction.DatePosted.HasValue ? true : false;
                model.DatePosted = data.Transaction.DatePosted;
                model.Sequence = data.Transaction.Sequence;
            }

            return model;
        }

        public static TransactionModel Get(Entities db, string userKey, int id)
        {
            var data = from t in db.Transactions
                       where t.Id == id
                       join a in db.Accounts on t.AccountId equals a.Id
                       where a.UserId == userKey
                       join at in db.AccountTypes on a.TypeId equals at.Id
                       join c in db.Categories on t.CategoryId equals c.Id into cj
                       from rc in cj.DefaultIfEmpty()
                       select new TransactionEntity { Transaction = t, Account = a, AccountType = at, Category = rc };

            return ConvertDataToModel(data.FirstOrDefault());
        }

        public static void Save(TransactionModel model, Entities db, string userKey, bool isNew = false, bool commit = true)
        {
            Transaction data = null;

            if (isNew)
            {
                Account account = db.Accounts.FirstOrDefault(a => a.Id == model.AccountId && a.UserId == userKey);

                if (account != null)
                {
                    data = new Transaction();
                    data.DateCreated = DateTime.Now;

                    data.AccountId = model.AccountId;
                    data.CategoryId = model.CategoryId;

                    data.DisplayDate = model.DisplayDate;
                    data.IsCredit = model.IsCredit;
                    data.Amount = model.Amount.Value;
                    data.Name = model.Name;
                    //data.Sequence = model.Sequence;

                    InnerPost(db, userKey, account, data, model.IsPosted, model.Amount.Value, model.IsCredit, commit, model.DatePosted);
                    
                    db.Transactions.Add(data);
                    if (commit)
                    {
                        db.SaveChanges();
                        model.Id = data.Id;
                    }
                }
            }
            else
            {
                int? previousAccountId = null;
                data = db.Transactions.Where(cg => cg.Id == model.Id).FirstOrDefault();
                if (model.AccountId != data.AccountId)
                    previousAccountId = data.AccountId;

                // verifies transaction account against authenticated user
                if (data != null && db.Accounts.FirstOrDefault(a => a.Id == model.AccountId && a.UserId == userKey) != null)
                {
                    // if reversing IsPosted, use db info
                    data.AccountId = model.AccountId;
                    data.CategoryId = model.CategoryId;

                    data.DisplayDate = model.DisplayDate;
                    data.Name = model.Name;

                    // load selected account from UI
                    Account account = db.Accounts.FirstOrDefault(a => a.Id == model.AccountId && a.UserId == userKey);

                    InnerPost(db, userKey, account, data, model.IsPosted, model.Amount.Value, model.IsCredit, commit, null, previousAccountId);
                    
                    data.IsCredit = model.IsCredit;
                    data.Amount = model.Amount.Value;
                    data.Sequence = model.Sequence;

                    db.Entry(data).State = EntityState.Modified;
                    if (commit)
                        db.SaveChanges();
                }
            }

            if (model.ReminderId.HasValue)
            {
                // set NextDate
                ReminderModel reminder = ReminderModel.Get(db, userKey, model.ReminderId.Value);

                // use transaction actual date to set next reminder (if selected)
                //DateTime nextDate = model.DisplayDate;
                reminder.NextDate = reminder.CalculateReminderDate(false);

                ReminderModel.Increment(db, userKey, reminder);
            }
        }

        public static void Post(Entities db, string userKey, int Id)
        {
            Transaction data = null;

            data = db.Transactions.Where(cg => cg.Id == Id).FirstOrDefault();

            Account account = db.Accounts.FirstOrDefault(a => a.Id == data.AccountId && a.UserId == userKey);
            InnerPost(db, userKey, account, data, true, data.Amount, data.IsCredit, true, null);

            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
        }

        private static void InnerPost(Entities db, string userKey, Account account, Transaction data, bool isPosted, decimal amount, bool isCredit, bool commit, DateTime? posted, int? previousAccountId = null)
        {
            Account previousAccount = null;
            decimal oldAmount = data.Amount;
            bool oldIsCredit = data.IsCredit;
            decimal netPrevious = oldAmount * (oldIsCredit ? 1 : -1);

            if (previousAccountId.HasValue)
                previousAccount = db.Accounts.FirstOrDefault(a => a.Id == previousAccountId.Value && a.UserId == userKey);

            decimal newAmount = amount * (isCredit ? 1 : -1);

            if (isPosted && data.DatePosted == null)
            {
                // post transaction (for import, we set date posted, but not anywhere else)
                data.DatePosted = posted.HasValue ? posted.Value : DateTime.Now;

                AccountModel.UpdateCurrentBalance(db, userKey, account, CurrentBalanceUpdateAction.AddTransaction, 0, newAmount, commit);
            }
            else if (!isPosted && data.DatePosted != null)
            {
                // reversing posted transaction
                data.DatePosted = null;

                if (previousAccount != null)
                    AccountModel.UpdateCurrentBalance(db, userKey, previousAccount, CurrentBalanceUpdateAction.RemoveTransaction, netPrevious, 0, commit);
                else
                    AccountModel.UpdateCurrentBalance(db, userKey, account, CurrentBalanceUpdateAction.RemoveTransaction, netPrevious, 0, commit);
            }
            else if (data.DatePosted != null && (amount != oldAmount || isCredit != oldIsCredit || previousAccountId.HasValue))
            {
                // editing a posted transaction amount
                if (previousAccount != null)
                {
                    // reverse old amount from previous account
                    AccountModel.UpdateCurrentBalance(db, userKey, previousAccount, CurrentBalanceUpdateAction.RemoveTransaction, netPrevious, 0, commit);

                    // post new amount to selected account
                    AccountModel.UpdateCurrentBalance(db, userKey, account, CurrentBalanceUpdateAction.AddTransaction, 0, newAmount, commit);
                }
                else
                {
                    AccountModel.UpdateCurrentBalance(db, userKey, account, CurrentBalanceUpdateAction.UpdateTransaction, netPrevious, newAmount, commit);
                }
            }
        }

        public static void Delete(Entities db, string userKey, int id)
        {
            Transaction data = db.Transactions.Where(cg => cg.Id == id).FirstOrDefault();

            // verifies transaction account against authenticated user
            if (data != null && db.Accounts.FirstOrDefault(a => a.Id == data.AccountId && a.UserId == userKey) != null)
            {
                if (data.DatePosted != null)
                {
                    decimal netPrevious = data.Amount * (data.IsCredit ? 1 : -1);
                    Account account = db.Accounts.FirstOrDefault(a => a.Id == data.AccountId && a.UserId == userKey);
                    AccountModel.UpdateCurrentBalance(db, userKey, account, CurrentBalanceUpdateAction.RemoveTransaction, netPrevious, 0);
                }
                db.Transactions.Remove(data);
                db.SaveChanges();
            }
        }
    }
}