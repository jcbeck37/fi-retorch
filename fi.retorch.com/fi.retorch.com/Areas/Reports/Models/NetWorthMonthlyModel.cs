using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Reports.EntityModels;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace fi.retorch.com.Areas.Reports.Models
{
    public class NetWorthMonthlyModel
    {
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime StartDate { get; set; }
    }

    public class MonthlyItemList : NetWorthReportModel
    {
        public List<MonthlyItem> Items { get; set; }

        public static MonthlyItemList GetReport(Entities db, string userKey, DateTime startDate, DateTime endDate)
        {
            MonthlyItemList report = new MonthlyItemList();
            NetWorthReportModel baseReport = GetInitialData(db, userKey, startDate, endDate);
            report.AccountTypes = baseReport.AccountTypes;
            report.Accounts = baseReport.Accounts;
            
            // gather data
            var query = from typ in db.AccountTypes
                        where typ.UserId == userKey //&& typ.Id == 7
                        join act in db.Accounts on typ.Id equals act.TypeId
                        join trns in db.Transactions on act.Id equals trns.AccountId
                        where act.DateOpened <= endDate && (act.DateClosed == null || act.DateClosed >= startDate)
                        where trns.DisplayDate >= startDate && trns.DisplayDate <= endDate && trns.DatePosted != null
                        group new
                        {
                            Amount = trns == null ? 0 : trns.Amount,
                            IsCredit = trns == null ? true : trns.IsCredit
                        } by new
                        {
                            typ,
                            act,
                            trns.DisplayDate.Year,
                            trns.DisplayDate.Month
                        } into set
                        select new MonthlyEntity
                        {
                            Month = set.Key.Month,
                            Year = set.Key.Year,
                            Type = set.Key.typ,
                            Account = set.Key.act,
                            TransactionTotal = set.Select(x => x == null ? 0 : x.Amount * (set.Key.typ.IsDebt ? -1 : 1) * (x.IsCredit ? 1 : -1)).Sum()
                        };

            report.Items = new List<MonthlyItem>();
            query.ToList().ForEach(q => report.Items.Add(MonthlyItem.ConvertEntityToModel(q)));

            return report;
        }
    }

    public class MonthlyItem
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public AccountModel Account { get; set; }
        public AccountTypeModel Type { get; set; }
        public decimal? TransactionTotal { get; set; }

        public static MonthlyItem ConvertEntityToModel(MonthlyEntity entity)
        {
            MonthlyItem model = new MonthlyItem();

            model.Month = entity.Month;
            model.Year = entity.Year;
            model.Account = AccountModel.ConvertEntityToModel(new AccountEntity { Account = entity.Account, Type = entity.Type });
            model.Type = AccountTypeModel.ConvertDataToModel(entity.Type);
            model.TransactionTotal = entity.TransactionTotal;

            return model;
        }
    }
}