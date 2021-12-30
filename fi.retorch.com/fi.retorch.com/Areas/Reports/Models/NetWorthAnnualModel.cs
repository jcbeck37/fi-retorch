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
    public class NetWorthAnnualSettingsModel
    {
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime StartDate { get; set; }
    }

    public class NetWorthAnnualReportModel : NetWorthReportModel
    {
        public List<AnnualItem> Items { get; set; }

        public static NetWorthAnnualReportModel GetReport(Entities db, string userKey, DateTime startDate, DateTime endDate)
        {            
            NetWorthAnnualReportModel report = new NetWorthAnnualReportModel();
            NetWorthReportModel baseReport = GetInitialData(db, userKey, startDate, endDate);
            report.AccountTypes = baseReport.AccountTypes;
            report.Accounts = baseReport.Accounts;

            // gather data
            var query = from typ in db.AccountTypes
                        where typ.UserId == userKey // && typ.Id == 3
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
                            trns.DisplayDate.Year
                        } into set
                        select new AnnualEntity
                        {
                            Year = set.Key.Year,
                            Type = set.Key.typ,
                            Account = set.Key.act,
                            TransactionTotal = set.Select(x => x == null ? 0 : x.Amount * (set.Key.typ.IsDebt ? -1 : 1) * (x.IsCredit ? 1 : -1)).Sum()
                        };
            
            report.Items = new List<AnnualItem>();
            query.ToList().ForEach(q => report.Items.Add(AnnualItem.ConvertEntityToModel(q)));

            return report;
        }
    }

    public class AnnualItem
    {
        public int Year { get; set; }
        public AccountModel Account { get; set; }
        public AccountTypeModel Type { get; set; }
        public decimal? TransactionTotal { get; set; }

        public static AnnualItem ConvertEntityToModel(AnnualEntity entity)
        {
            AnnualItem model = new AnnualItem();
            
            model.Year = entity.Year;
            model.Account = AccountModel.ConvertEntityToModel(new AccountEntity { Account = entity.Account, Type = entity.Type });
            model.Type = AccountTypeModel.ConvertDataToModel(entity.Type);
            model.TransactionTotal = entity.TransactionTotal;

            return model;
        }
    }
}