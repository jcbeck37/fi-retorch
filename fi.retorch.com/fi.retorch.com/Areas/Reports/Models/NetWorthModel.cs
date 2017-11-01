using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Reports.EntityModels;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Reports.Models
{
    public class NetWorthModel
    {
    }

    public class NetWorthReportModel
    {
        public List<NetWorthAccountTypeModel> AccountTypes { get; set; }
        public List<AccountModel> Accounts { get; set; }

        public static NetWorthReportModel GetInitialData(Entities db, string userKey, DateTime startDate, DateTime endDate)
        {
            NetWorthReportModel baseReport = new NetWorthReportModel();

            // gather all applicable accounts for start date values
            var typeQuery = from typ in db.AccountTypes
                            where typ.UserId == userKey //&& typ.Id == 7
                            join act in db.Accounts on typ.Id equals act.TypeId
                            where act.UserId == userKey
                                && (act.DateClosed == null || act.DateClosed >= startDate)
                                && act.DateOpened <= endDate
                            group new
                            {
                                Account = act
                            } by new
                            {
                                typ
                            } into typeList
                            select new NetWorthAccountTypeEntity
                            {
                                AccountType = typeList.Key.typ,
                                AccountCurrentTotal = typeList.Select(x => x.Account.DateClosed == null || x.Account.DateClosed > endDate ? x.Account : null).Distinct().Select(x => x == null ? 0 : x.CurrentBalance).Sum(),
                            };

            baseReport.AccountTypes = new List<NetWorthAccountTypeModel>();
            typeQuery.ToList().ForEach(q => baseReport.AccountTypes.Add(NetWorthAccountTypeModel.ConvertEntityToModel(q)));

            // any accounts that opened with a balance or closed with a balance between start and finish
            var accountChangeQuery = from typ in db.AccountTypes
                                     where typ.UserId == userKey //&& typ.Id == 7
                                     join act in db.Accounts on typ.Id equals act.TypeId
                                     where act.UserId == userKey
                                        && ((act.DateOpened >= startDate && act.DateOpened <= endDate && act.OpeningBalance != 0)
                                        || (act.DateClosed >= startDate && act.DateClosed <= endDate && act.CurrentBalance != 0))
                                     select new AccountEntity
                                     {
                                         Account = act,
                                         Type = typ
                                     };

            baseReport.Accounts = new List<AccountModel>();
            accountChangeQuery.ToList().ForEach(q => baseReport.Accounts.Add(AccountModel.ConvertEntityToModel(q)));

            return baseReport;
        }

        public class NetWorthAccountTypeModel
        {
            public AccountTypeModel AccountType { get; set; }
            public decimal? EndingBalance { get; set; }

            public static NetWorthAccountTypeModel ConvertEntityToModel(NetWorthAccountTypeEntity entity)
            {
                NetWorthAccountTypeModel model = new NetWorthAccountTypeModel();

                model.AccountType = AccountTypeModel.ConvertDataToModel(entity.AccountType);
                model.EndingBalance = entity.AccountCurrentTotal * (entity.AccountType.IsDebt ? -1 : 1);

                return model;
            }
        }
    }
}