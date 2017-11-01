using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace fi.retorch.com.Areas.Dashboard.Models
{
    public class AccountTypeList : BaseModel
    {
        public List<AccountTypeModel> Items { get; set; }

        public AccountTypeList()
        {
        }

        public AccountTypeList(Entities db, string UserKey)
        {
            Items = new List<AccountTypeModel>();

            List<AccountType> data = db.AccountTypes.Where(cg => cg.UserId == UserKey).ToList();
            data.ForEach(d => Items.Add(AccountTypeModel.ConvertDataToModel(d)));
        }

        public static List<AccountTypeModel> ConvertDataToList(List<AccountType> data)
        {
            List<AccountTypeModel> list = new List<AccountTypeModel>();
            data.ForEach(d => list.Add(AccountTypeModel.ConvertDataToModel(d)));

            return list;
        }
    }

    public class AccountTypeModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string Name { get; set; }

        [Display(Name = "Debt/Liability")]
        public bool IsDebt { get; set; }

        [Display(Name = "Credits Label")]
        public string PositiveText { get; set; }

        [Display(Name = "Debits Label")]
        public string NegativeText { get; set; }

        public static AccountTypeModel ConvertDataToModel(AccountType data)
        {
            AccountTypeModel model = null;

            if (data != null)
            {
                model = new AccountTypeModel();

                model.Id = data.Id;
                model.Name = data.Name;
                model.IsDebt = data.IsDebt;
                model.PositiveText = data.PositiveText;
                model.NegativeText = data.NegativeText;
            }

            return model;
        }

        public static AccountTypeModel Get(Entities db, string userKey, int id)
        {
            AccountType data = db.AccountTypes.Where(cg => cg.Id == id && cg.UserId == userKey).FirstOrDefault();

            return ConvertDataToModel(data);
        }

        public static AccountTypeModel FindByName(Entities db, string userKey, string name)
        {
            AccountType data = db.AccountTypes.Where(at => at.UserId == userKey && at.Name == name).FirstOrDefault();

            return ConvertDataToModel(data);
        }

        public static void Save(AccountTypeModel model, Entities db, string userKey, bool isNew = false)
        {
            AccountType data = null;

            if (isNew)
            {
                data = new AccountType();
                data.UserId = userKey;
                data.DateCreated = DateTime.Now;

                data.Name = model.Name;
                data.IsDebt = model.IsDebt;
                data.PositiveText = model.PositiveText;
                data.NegativeText = model.NegativeText;

                db.AccountTypes.Add(data);
            }
            else
            {
                data = db.AccountTypes.Where(cg => cg.Id == model.Id && cg.UserId == userKey).FirstOrDefault();

                data.Name = model.Name;
                data.IsDebt = model.IsDebt;
                data.PositiveText = model.PositiveText;
                data.NegativeText = model.NegativeText;

                db.Entry(data).State = EntityState.Modified;
            }
            
            db.SaveChanges();
        }

        public static void Delete(Entities db, string userKey, int id)
        {
            AccountType data = db.AccountTypes.Where(cg => cg.Id == id && cg.UserId == userKey).FirstOrDefault();
            db.AccountTypes.Remove(data);
            db.SaveChanges();
        }
    }
}