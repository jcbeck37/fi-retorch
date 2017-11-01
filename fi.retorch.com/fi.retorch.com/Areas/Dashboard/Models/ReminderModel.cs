using fi.retorch.com.Areas.Dashboard.Code.Enums;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Areas.Reports.Models;
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
    public class ReminderList
    {
        public int TotalItems { get; set; }
        public List<ReminderModel> Items { get; set; }
        public IPagedList<ReminderModel> PagedItems { get; set; }
        public ReminderQuerySettings Settings { get; set; }

        public ReminderList()
        {
        }

        public ReminderList(Entities db, string userKey, ReminderQuerySettings settings)
        {
            Settings = settings;
            if (Settings == null)
                Settings = new ReminderQuerySettings();

            Items = new List<ReminderModel>();

            // get all matching transactions
            var data = from r in db.Reminders.AsExpandable()
                       join a in db.Accounts.AsExpandable() on r.AccountId equals a.Id
                       join at in db.AccountTypes.AsExpandable() on a.TypeId equals at.Id
                       join s in db.ReminderSchedules on r.ScheduleId equals s.Id
                       join c in db.Categories on r.CategoryId equals c.Id into cj
                       from rc in cj.DefaultIfEmpty()
                       where a.UserId == userKey && a.IsDisplayed
                       select new ReminderEntity { Reminder = r, Account = a, AccountType = at, Schedule = s, Category = rc };

            if (Settings.AccountTypeId.HasValue || Settings.AccountId.HasValue || Settings.CategoryId.HasValue || Settings.StartDate != null || Settings.EndDate != null)
            {
                var predicate = PredicateBuilder.New<ReminderEntity>(true);

                if (Settings.AccountTypeId.HasValue)
                    predicate = predicate.And(p => p.Account.TypeId == Settings.AccountTypeId.Value);
                if (Settings.AccountId.HasValue)
                    predicate = predicate.And(p => p.Reminder.AccountId == Settings.AccountId.Value);
                if (Settings.CategoryId.HasValue)
                    predicate = predicate.And(p => p.Reminder.CategoryId == Settings.CategoryId.Value);

                if (Settings.StartDate != null)
                    predicate = predicate.And(p => p.Reminder.NextDate >= Settings.StartDate);
                if (Settings.EndDate != null)
                    predicate = predicate.And(p => p.Reminder.NextDate <= Settings.EndDate);

                data = data.Where(predicate);
            }

            if (Settings.Search != null)
            {
                var predicate = PredicateBuilder.New<ReminderEntity>(false);

                predicate = predicate.Or(p => p.Reminder.Name.Contains(Settings.Search));
                predicate = predicate.Or(p => p.Account.Name.Contains(Settings.Search));
                predicate = predicate.Or(p => p.Category.Name.Contains(Settings.Search));

                data = data.Where(predicate);
            }

            // sort records
            switch (Settings.Sort)
            {
                case "NextDateDesc":
                    data = data.OrderByDescending(t => t.Reminder.NextDate);
                    break;
                case "NextDate":
                default:
                    data = data.OrderBy(t => t.Reminder.NextDate);
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

            data.ToList().ForEach(d => Items.Add(ReminderModel.ConvertDataToModel(d)));
            Items.ForEach(r =>
            {
                DateTime previousDate = r.CalculateReminderDate(true);
                TransactionTotalModel transactions = TransactionTotalModel.GetTransactionTotal(db, userKey, previousDate, r.AccountId);
                r.PreviousBalance -= transactions.Total;
            });
        }

        public static ReminderList GetDashboardReminders(Entities db, string userKey)
        {
            ReminderList list = new ReminderList();
            list.Items = new List<ReminderModel>();

            // get all matching transactions
            var data = from r in db.Reminders
                       join a in db.Accounts on r.AccountId equals a.Id
                       where a.UserId == userKey && a.IsDisplayed
                       join s in db.ReminderSchedules on r.ScheduleId equals s.Id
                       select new ReminderEntity { Reminder = r, Account = a, Schedule = s };

            data.ToList().ForEach(d => list.Items.Add(ReminderModel.ConvertDataToModel(d)));

            return list;
        }
    }

    public class ReminderModel
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

        //public IEnumerable<SelectListItem> Types { get; set; }
        ////[Required]
        //[Display(Name = "Type")]
        //public int? ReminderTypeId { get; set; }
        //public string ReminderTypeName { get; set; }

        public IEnumerable<SelectListItem> Schedules { get; set; }
        [Required]
        [Display(Name = "Schedule")]
        public int ReminderScheduleId { get; set; }
        public string ReminderScheduleName { get; set; }

        [Required]
        [Display(Name = "Next Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime NextDate { get; set; }

        public string PositiveText { get; set; }
        public string NegativeText { get; set; }
        [Display(Name = "Credit")]
        public bool IsCredit { get; set; }

        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Amount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public string AmountFormatted { get; set; }

        [Display(Name = "Interest Rate")]
        [DisplayFormat(DataFormatString = "{0:P4}")]
        public decimal? Rate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P4}")]
        public string RateFormatted { get; set; }

        [Required]
        [Display(Name = "Reminder")]
        public string Name { get; set; }

        [Display(Name = "Final Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? LastDate { get; set; }

        public decimal? PreviousBalance { get; set; }
        public string EditLink { get; set; }
        public string GoLink { get; set; }
        #endregion

        public DateTime CalculateReminderDate(bool previous = false)
        {
            ReminderModel reminder = this; // just makes properties more obvious

            int directionMultipler = previous ? -1 : 1;

            DateTime nextDate = reminder.NextDate;
            DateTime calcDate = reminder.NextDate;

            switch ((ReminderScheduleEnum)reminder.ReminderScheduleId)
            {
                case ReminderScheduleEnum.Daily:
                    nextDate = calcDate.AddDays(directionMultipler * 1);
                    break;
                case ReminderScheduleEnum.Weekly:
                    nextDate = calcDate.AddDays(directionMultipler * 7);
                    break;
                case ReminderScheduleEnum.BiWeekly:
                    nextDate = calcDate.AddDays(directionMultipler * 14);
                    break;
                case ReminderScheduleEnum.SemiMonthly:
                    switch (nextDate.Day)
                    {
                        case 1:
                            if (previous)
                                nextDate = calcDate.AddMonths(-1).AddDays(14);
                            else
                                nextDate = calcDate.AddDays(14);
                            break;
                        case 15:
                        case 16:
                            // sets last day of the month
                            if (previous)
                            {
                                nextDate = calcDate.AddMonths(-1).AddDays(DateTime.DaysInMonth(nextDate.Year, nextDate.Month) - nextDate.Day);
                            }
                            else
                            {
                                nextDate = calcDate.AddDays(DateTime.DaysInMonth(nextDate.Year, nextDate.Month) - nextDate.Day);
                            }
                            break;
                        default:
                            nextDate = calcDate.AddDays(directionMultipler * 15);
                            break;
                    }
                    break;
                case ReminderScheduleEnum.Monthly:
                    nextDate = calcDate.AddMonths(directionMultipler * 1);
                    break;
                case ReminderScheduleEnum.Quarterly:
                    nextDate = calcDate.AddMonths(directionMultipler * 3);
                    break;
                case ReminderScheduleEnum.SemiAnnual:
                    nextDate = calcDate.AddMonths(directionMultipler * 6);
                    break;
                case ReminderScheduleEnum.Annual:
                    nextDate = calcDate.AddMonths(directionMultipler * 12);
                    break;
            }

            return nextDate;
        }

        public static ReminderModel ConvertDataToModel(ReminderEntity data)
        {
            ReminderModel model = null;

            if (data != null)
            {
                model = new ReminderModel();

                // account information
                model.AccountId = data.Reminder.AccountId;
                model.AccountName = data.Account != null ? data.Account.Name : null;

                model.IsDebt = data.AccountType != null? data.AccountType.IsDebt : false;
                model.PositiveText = data.AccountType != null && !string.IsNullOrEmpty(data.AccountType.PositiveText) ? data.AccountType.PositiveText : "Positive";
                model.NegativeText = data.AccountType != null && !string.IsNullOrEmpty(data.AccountType.NegativeText) ? data.AccountType.NegativeText : "Negative";

                // category information
                model.CategoryId = data.Reminder.CategoryId;
                model.CategoryName = data.Category != null ? data.Category.Name : null;

                // reminder type information
                //model.ReminderTypeId = data.Reminder.TypeId;
                //model.ReminderTypeName = data.Type != null ? data.Type.Name : null;

                // reminder schedule information
                model.ReminderScheduleId = data.Reminder.ScheduleId;
                model.ReminderScheduleName = data.Schedule != null ? data.Schedule.Name : null;

                // reminder information
                model.Id = data.Reminder.Id;
                model.Name = data.Reminder.Name;

                //model.NextDate = DateTime.Now;
                model.NextDate = data.Reminder.NextDate;
                model.IsCredit = data.Reminder.IsCredit;
                model.Amount = data.Reminder.Amount;
                model.AmountFormatted = data.Reminder.Amount.ToString();
                model.Rate = data.Reminder.Rate;
                model.LastDate = data.Reminder.LastDate;

                if (model.Rate.HasValue)
                    model.PreviousBalance = data.Account != null ? data.Account.CurrentBalance : 0;
            }

            return model;
        }

        public static ReminderModel Get(Entities db, string userKey, int id)
        {
            var data = from r in db.Reminders
                       where r.Id == id
                       join a in db.Accounts on r.AccountId equals a.Id
                       join at in db.AccountTypes on a.TypeId equals at.Id
                       join s in db.ReminderSchedules on r.ScheduleId equals s.Id
                       join c in db.Categories on r.CategoryId equals c.Id into cj
                       from rc in cj.DefaultIfEmpty()
                       where a.UserId == userKey && a.IsDisplayed
                       select new ReminderEntity { Reminder = r, Account = a, AccountType = at, Schedule = s, Category = rc };

            return ConvertDataToModel(data.FirstOrDefault());
        }

        public static void Save(ReminderModel model, Entities db, string userKey, bool isNew = false, bool commit = true)
        {
            Reminder data = null;

            if (isNew)
            {
                Account account = db.Accounts.FirstOrDefault(a => a.Id == model.AccountId && a.UserId == userKey);

                if (account != null)
                {
                    data = new Reminder();
                    //data.DateCreated = DateTime.Now;

                    data.AccountId = model.AccountId;
                    data.CategoryId = model.CategoryId;
                    //data.TypeId = model.ReminderTypeId;
                    data.ScheduleId = model.ReminderScheduleId;

                    data.NextDate = model.NextDate;
                    data.Name = model.Name;
                    data.IsCredit = model.IsCredit;
                    data.Amount = model.Amount.HasValue ? model.Amount.Value : (decimal?)null;
                    data.Rate = model.Rate.HasValue ? model.Rate.Value : (decimal?)null;
                    data.LastDate = model.LastDate;

                    db.Reminders.Add(data);
                    if (commit)
                    {
                        db.SaveChanges();
                        model.Id = data.Id;
                    }
                }
            }
            else
            {
                data = db.Reminders.Where(cg => cg.Id == model.Id).FirstOrDefault();

                // verifies transaction account against authenticated user
                if (data != null && db.Accounts.FirstOrDefault(a => a.Id == model.AccountId && a.UserId == userKey) != null)
                {
                    data.AccountId = model.AccountId;
                    data.CategoryId = model.CategoryId;
                    //data.TypeId = model.ReminderTypeId;
                    data.ScheduleId = model.ReminderScheduleId;

                    data.NextDate = model.NextDate;
                    data.Name = model.Name;
                    data.IsCredit = model.IsCredit;
                    data.Amount = model.Amount.HasValue ? model.Amount.Value : (decimal?)null;
                    data.Rate = model.Rate.HasValue ? model.Rate.Value : (decimal?)null;
                    data.LastDate = model.LastDate;

                    db.Entry(data).State = EntityState.Modified;
                    if (commit)
                        db.SaveChanges();
                }
            }
        }

        public static void Increment(Entities db, string userKey, ReminderModel model)
        {
            Reminder data = db.Reminders.Where(cg => cg.Id == model.Id).FirstOrDefault();

            // verifies transaction account against authenticated user
            if (data != null && db.Accounts.FirstOrDefault(a => a.Id == model.AccountId && a.UserId == userKey) != null)
            {
                data.NextDate = model.NextDate;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Entities db, string userKey, int id)
        {
            Reminder data = db.Reminders.Where(cg => cg.Id == id).FirstOrDefault();

            // verifies transaction account against authenticated user
            if (data != null && db.Accounts.FirstOrDefault(a => a.Id == data.AccountId && a.UserId == userKey) != null)
            {
                db.Reminders.Remove(data);
                db.SaveChanges();
            }
        }
    }
}