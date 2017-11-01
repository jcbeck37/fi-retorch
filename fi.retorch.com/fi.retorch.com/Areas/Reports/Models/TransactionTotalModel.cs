using fi.retorch.com.Areas.Reports.EntityModels;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Reports.Models
{
    public class TransactionTotalModel
    {
        public int AccountId { get; set; }
        public decimal Total { get; set; }
        
        public static List<TransactionTotalModel> GetTransactionTotal(Entities db, string userKey, DateTime startDate, DateTime? endDate = null)
        {
            List<TransactionTotalModel> model = new List<TransactionTotalModel>();

            var data = from trns in db.Transactions
                       where trns.DisplayDate > startDate && trns.DatePosted != null && (endDate == null || trns.DisplayDate <= endDate.Value)
                       join act in db.Accounts on trns.AccountId equals act.Id into a
                       from allA in a.DefaultIfEmpty()
                       join at in db.AccountTypes on allA.TypeId equals at.Id
                       where allA.UserId == userKey
                       group trns by new { AccountId = trns.AccountId } into set
                       select new TransactionTotalEntity
                       {
                           AccountId = set.Select(x => x.AccountId).FirstOrDefault(),
                           Total = set.Select(x => x.Amount * (x.IsCredit ? 1 : -1)).Sum()
                       };

            data.ToList().ForEach(d => model.Add(ConvertEntityToModel(d)));

            return model;
        }

        public static TransactionTotalModel ConvertEntityToModel(TransactionTotalEntity entity)
        {
            TransactionTotalModel model = new TransactionTotalModel();

            if (entity != null)
            {
                model.AccountId = entity.AccountId;
                model.Total = entity.Total;
            }

            return model;
        }

        public static TransactionTotalModel GetTransactionTotal(Entities db, string userKey, DateTime date, int accountId)
        {
            var data = from trns in db.Transactions
                       where trns.DisplayDate > date && trns.DatePosted != null
                       join act in db.Accounts on trns.AccountId equals act.Id into a
                       from allA in a.DefaultIfEmpty()
                       join at in db.AccountTypes on allA.TypeId equals at.Id
                       where allA.UserId == userKey && allA.Id == accountId
                       group trns by allA.Id into set
                       select new TransactionTotalEntity
                       {
                           AccountId = set.Select(x => x.AccountId).FirstOrDefault(),
                           Total = set.Select(x => x.Amount * (x.IsCredit ? 1 : -1)).Sum()
                       };

            return ConvertEntityToModel(data.FirstOrDefault());
        }
    }
}