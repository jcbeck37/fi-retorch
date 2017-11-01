using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Legacy.Models
{
    public class LegacyTransactionList
    {
        public List<LegacyTransactionModel> Items { get; set; }

        public LegacyTransactionList(LegacyEntities db, int userId)
        {
            Items = new List<LegacyTransactionModel>();

            // get all categories with at least on transaction assigned
            var data = from t in db.act_transactions
                       join a in db.act_accounts on t.account_id equals a.account_id
                       where a.user_id == userId // && cat.isActive == 1
                       select t;
            int sequence = 0;
            DateTime lastItem = DateTime.Now;
            data.Distinct().OrderBy(t => t.transaction_date).ThenBy(t => t.register_date).ToList().ForEach(d => Items.Add(LegacyTransactionModel.ConvertDataToModel(d, ref sequence, ref lastItem)));
        }
    }

    public class LegacyTransactionModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime DisplayDate { get; set; }
        public bool IsCredit { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? DatePosted { get; set; }
        public int Sequence { get; set; }

        public static LegacyTransactionModel ConvertDataToModel(act_transactions data, ref int sequence, ref DateTime lastItem)
        {
            LegacyTransactionModel model = new LegacyTransactionModel();

            if (DateTime.Compare(lastItem, data.transaction_date.Value) == 0)
                sequence++;
            else
                sequence = 0;

            model.Id = data.transaction_id;
            model.AccountId = data.account_id.Value;
            model.CategoryId = data.category_id;
            model.Name = data.transaction_name;
            model.Amount = (decimal)data.amount / 100;
            model.DisplayDate = data.transaction_date.Value;
            model.IsCredit = data.positive != 0;
            model.IsPosted = data.registered != 0;
            model.DatePosted = data.register_date.HasValue ? data.register_date.Value : (DateTime?)null;
            model.Sequence = sequence;

            lastItem = model.DisplayDate;

            return model;
        }

        public TransactionModel ConvertLegacyToModel(int defaultCategoryId, Dictionary<int, int> AccountMappings, Dictionary<int, int> CategoryMappings)
        {
            TransactionModel model = new TransactionModel();

            model.Accounts = new List<SelectListItem>();
            model.Categories = new List<SelectListItem>();

            //model.Id = Id;
            model.AccountId = AccountMappings[AccountId];
            model.CategoryId = CategoryId.HasValue ? CategoryMappings[CategoryId.Value] : defaultCategoryId;
            model.Name = Name;
            model.Amount = Amount;
            model.DisplayDate = DisplayDate;
            model.IsCredit = IsCredit;
            model.IsPosted = IsPosted;
            model.DatePosted = DatePosted;
            model.Sequence = Sequence;

            return model;
        }
    }
}