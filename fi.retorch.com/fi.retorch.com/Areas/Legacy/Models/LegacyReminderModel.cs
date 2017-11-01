using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Legacy.Models
{
    public class LegacyReminderList
    {
        public List<LegacyReminderModel> Items { get; set; }

        public LegacyReminderList(LegacyEntities db, int userId)
        {
            Items = new List<LegacyReminderModel>();

            // get all categories with at least on Reminder assigned
            var data = from t in db.act_reminders
                       join a in db.act_accounts on t.account_id equals a.account_id
                       where a.user_id == userId // && cat.isActive == 1
                       select t;
            data.Distinct().ToList().ForEach(d => Items.Add(LegacyReminderModel.ConvertDataToModel(d)));
        }
    }

    public class LegacyReminderModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? CategoryId { get; set; }
        public int ScheduleId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal? Rate { get; set; }
        public DateTime NextDate { get; set; }
        public bool IsCredit { get; set; }
        public DateTime? LastDate { get; set; }
        
        public static LegacyReminderModel ConvertDataToModel(act_reminders data)
        {
            LegacyReminderModel model = new LegacyReminderModel();

            model.Id = data.reminder_id;
            model.AccountId = data.account_id.Value;
            model.CategoryId = data.category_id;
            model.Name = data.reminder_name;
            model.Amount = (decimal)data.amount / 100;
            model.Rate = data.interest_rate.HasValue ? (decimal)data.interest_rate.Value * 100 : (decimal?)null;
            model.IsCredit = data.positive != 0;
            model.LastDate = data.end_date;

            /* OLD
            1	Annual (new: 6)
            2	Monthly (new: 5)
            3	Bi-weekly (new: 3)
            4	Weekly (new: 2)
            5	Quad-weekly (new: 2)
            */
            DateTime startDate = data.start_date.HasValue ? data.start_date.Value : (data.end_date.HasValue ? data.end_date.Value : data.last_posted.Value);
            DateTime? lastDate = data.last_posted;
            switch (data.schedule_id)
            {
                case 1: // annual
                    model.NextDate = lastDate.HasValue ? lastDate.Value.AddYears(1) : startDate;
                    model.ScheduleId = 7;
                    break;
                case 2: // monthly
                    model.NextDate = lastDate.HasValue ? lastDate.Value.AddMonths(1) : startDate;
                    model.ScheduleId = 5;
                    break;
                case 3: // bi-weekly
                    model.NextDate = lastDate.HasValue ? lastDate.Value.AddDays(14) : startDate;
                    model.ScheduleId = 3;
                    break;
                case 4: // weekly
                case 5: // quad-weekly
                    model.NextDate = lastDate.HasValue ? lastDate.Value.AddDays(7) : startDate;
                    model.ScheduleId = 2;
                    break;
            }
            /* NEW
            1   Daily
            2   Weekly
            3   Bi - weekly
            4   Semi - monthly
            5   Monthly
            6   Semi - Annual
            7   Annual
            */

            return model;
        }

        public ReminderModel ConvertLegacyToModel(int defaultCategoryId, Dictionary<int, int> AccountMappings, Dictionary<int, int> CategoryMappings)
        {
            //int defaultTypeId = 2; // recurring expense

            ReminderModel model = new ReminderModel();

            //model.Accounts = new List<SelectListItem>();
            //model.Categories = new List<SelectListItem>();

            //model.Id = Id;
            model.AccountId = AccountMappings[AccountId];
            model.CategoryId = CategoryId.HasValue ? CategoryMappings[CategoryId.Value] : defaultCategoryId;
            model.ReminderScheduleId = ScheduleId;
            //model.ReminderTypeId = defaultTypeId;
            model.Name = Name;
            model.Amount = Amount;
            model.IsCredit = IsCredit;
            model.NextDate = NextDate;
            model.LastDate = LastDate;

            return model;
        }
    }
}