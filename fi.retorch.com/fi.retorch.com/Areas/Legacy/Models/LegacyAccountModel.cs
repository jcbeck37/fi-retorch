using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Data;
using fi.retorch.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Legacy.Models
{
    public class LegacyAccountList
    {
        public List<LegacyAccountModel> Items { get; set; }

        public LegacyAccountList(LegacyEntities db, int userId)
        {
            Items = new List<LegacyAccountModel>();

            // get all categories with at least on transaction assigned
            var data = from act in db.act_accounts
                       join t in db.act_transactions on act.account_id equals t.account_id
                       where act.user_id == userId // && cat.isActive == 1
                       select act;
            data.Distinct().ToList().ForEach(d => Items.Add(LegacyAccountModel.ConvertDataToModel(d)));
        }
    }

    public class LegacyAccountModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public decimal OpeningBalance { get; set; }
        public DateTime DateOpened { get; set; }
        public bool IsDisplayed { get; set; }
        public int DisplayOrder { get; set; }

        public static LegacyAccountModel ConvertDataToModel(act_accounts data)
        {
            LegacyAccountModel model = new LegacyAccountModel();

            model.Id = data.account_id;
            model.TypeId = data.type_id.Value;
            model.Name = data.account_name;
            model.OpeningBalance = (decimal)data.initial_balance / 100;
            model.DateOpened = data.start_date.HasValue ? data.start_date.Value : Convert.ToDateTime("2006-10-21 00:00:00.000");
            model.IsDisplayed = data.isActive != 0;
            model.DisplayOrder = data.dispOrder.HasValue ? data.dispOrder.Value : 0;

            return model;
        }

        public AccountModel ConvertLegacyToModel(Entities db, string userKey)
        {
            AccountModel model = new AccountModel();

            model.Categories = new List<MultipleSelectModel>();

            model.Name = Name;
            model.DateOpened = DateOpened;
            model.IsClosed = false;
            model.OpeningBalance = OpeningBalance;
            model.CurrentBalance = OpeningBalance;
            model.IsDisplayed = IsDisplayed;
            model.DisplayOrder = DisplayOrder;

            string typeName = "";
            switch (TypeId) // convert legacy type ID to name
            {
                case 1:
                    typeName = "Checking";
                    break;
                case 2:
                    typeName = "Loan";
                    break;
                case 3:
                    typeName = "Credit Card";
                    break;
                case 4:
                    typeName = "Other Assets";
                    break;
                case 5:
                    typeName = "Investment";
                    break;
                case 6:
                    typeName = "Retirement";
                    break;
                case 7:
                    typeName = "Real Estate";
                    break;
            }

            AccountTypeModel type = AccountTypeModel.FindByName(db, userKey, typeName);
            model.TypeId = type != null ? type.Id : 0;

            return model;
        }
    }
}