using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Reports.EntityModels;
using fi.retorch.com.Data;
using LinqKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace fi.retorch.com.Areas.Reports.Models
{
    public class NetWorthInstantModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public AccountModel Account { get; set; }
        public AccountTypeModel Type { get; set; }
        public decimal CurrentBalance { get; set; }

        public static List<NetWorthInstantModel> GetInstantNetWorth(Entities db, string userKey, DateTime date)
        {
            var data = from a in db.Accounts.AsExpandable()
                       join at in db.AccountTypes on a.TypeId equals at.Id
                       where a.UserId == userKey
                       select new NetWorthEntity
                       {
                           Account = a,
                           Type = at
                       };

            var predicate = PredicateBuilder.New<NetWorthEntity>(true);
            predicate = predicate.And(p => p.Account.DateOpened <= date && (!p.Account.IsClosed || p.Account.DateClosed >= date));
            data = data.Where(predicate);

            return ConvertDataToModel(data.ToList(), date);
        }

        public static List<NetWorthInstantModel> ConvertDataToModel(List<NetWorthEntity> data, DateTime date)
        {
            List<NetWorthInstantModel> model = new List<NetWorthInstantModel>();

            data.ForEach(d => model.Add(ConvertAccountToNetWorth(d, date)));
            return model;
        }

        public static NetWorthInstantModel ConvertAccountToNetWorth(NetWorthEntity data, DateTime date)
        {
            NetWorthInstantModel model = new NetWorthInstantModel();

            AccountEntity account = data.ConvertNetWorthToAccount();
            model.Date = date;
            model.Account = AccountModel.ConvertEntityToModel(account);
            model.Type = AccountTypeModel.ConvertDataToModel(data.Type);
            model.CurrentBalance = model.Account.CurrentBalance.Value;

            return model;
        }
    }
}