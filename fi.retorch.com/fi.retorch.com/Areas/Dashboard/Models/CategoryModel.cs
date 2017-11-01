using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Data;
using fi.retorch.com.Models;
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
    public class CategoryList
    {
        public int TotalItems { get; set; }
        public List<CategoryModel> Items { get; set; }
        public IPagedList<CategoryModel> PagedItems { get; set; }
        public CategoryQuerySettings Settings { get; set; }

        public CategoryList()
        {
        }

        public CategoryList(Entities db, string userKey, CategoryQuerySettings settings) // int? groupId = null, int? pageSize = null, int? page = null, string sort = null)
        {
            Settings = settings;
            if (Settings == null)
                Settings = new CategoryQuerySettings();

            Items = new List<CategoryModel>();

            // get all matching 
            var data = from c in db.Categories.AsExpandable()
                       where c.UserId == userKey
                       join cg in db.CategoryGroups on c.GroupId equals cg.Id into cgj
                       from cgr in cgj.DefaultIfEmpty()
                       select new CategoryEntity { Category = c, Group = cgr };

            if (Settings.GroupId != null || Settings.IsActive != null)
            {
                var predicate = PredicateBuilder.New<CategoryEntity>(true);

                if (Settings.GroupId != null)
                    predicate = predicate.And(p => p.Category.GroupId == Settings.GroupId);
                if (Settings.IsActive != null)
                    predicate = predicate.And(p => p.Category.IsActive == Settings.IsActive);

                data = data.Where(predicate);
            }

            if (Settings.Search != null)
            {
                var predicate = PredicateBuilder.New<CategoryEntity>(false);
                
                predicate = predicate.Or(p => p.Category.Name.Contains(Settings.Search));
                predicate = predicate.Or(p => p.Group.Name.Contains(Settings.Search));

                data = data.Where(predicate);
            }

            // sort records
            if (string.IsNullOrEmpty(Settings.Sort))
                Settings.Sort = "Name";
            switch (Settings.Sort)
            {
                case "Name":
                    data = data.OrderBy(t => t.Category.Name);
                    break;
                case "NameDesc":
                    data = data.OrderByDescending(t => t.Category.Name);
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

            data.ToList().ForEach(d => Items.Add(CategoryModel.ConvertDataToModel(d)));
        }
    }

    public class CategoryModel : BaseModel
    {
        public int Id { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }

        //[Required]
        [Display(Name = "Group")]
        public int? GroupId { get; set; }
        
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Name { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        
        public List<MultipleSelectModel> Accounts { get; set; }

        public CategoryModel() {}

        public static CategoryModel ConvertDataToModel(CategoryEntity data)
        {
            CategoryModel model = null;

            if (data != null)
            {
                model = new CategoryModel();
                
                model.Id = data.Category.Id;
                model.GroupId = data.Category.GroupId;
                model.GroupName = data.Group != null ? data.Group.Name : null;
                model.Name = data.Category.Name;
                model.IsActive = data.Category.IsActive;
            }

            return model;
        }

        public static CategoryModel Get(Entities db, string userKey, int id)
        {
            var data = from c in db.Categories.AsExpandable()
                       where c.UserId == userKey && c.Id == id
                       join cg in db.CategoryGroups on c.GroupId equals cg.Id into cgj
                       from cgr in cgj.DefaultIfEmpty()
                       select new CategoryEntity { Category = c, Group = cgr };

            return ConvertDataToModel(data.FirstOrDefault());
        }

        public static CategoryModel FindByName(Entities db, string userKey, string name)
        {
            var data = from c in db.Categories.AsExpandable()
                       where c.UserId == userKey && c.Name == name
                       join cg in db.CategoryGroups on c.GroupId equals cg.Id into cgj
                       from cgr in cgj.DefaultIfEmpty()
                       select new CategoryEntity { Category = c, Group = cgr };

            return ConvertDataToModel(data.FirstOrDefault());
        }

        public static void Save(CategoryModel model, Entities db, string userKey, bool isNew = false)
        {
            Category data = null;

            if (isNew)
            {
                data = new Category();
                data.UserId = userKey;
                data.DateCreated = DateTime.Now;

                data.GroupId = model.GroupId;
                data.Name = model.Name;
                data.IsActive = model.IsActive;

                db.Categories.Add(data);
                db.SaveChanges();

                model.Id = data.Id;
            }
            else
            {
                data = db.Categories.Where(c => c.Id == model.Id && c.UserId == userKey).FirstOrDefault();

                data.GroupId = model.GroupId;
                data.Name = model.Name;
                data.IsActive = model.IsActive;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }

            
            // handle accounts
            var existing = from ac in db.AccountCategories
                           where ac.CategoryId == data.Id
                           select ac;
            List<MultipleSelectModel> newAccounts = new List<MultipleSelectModel>();
            if (model.Accounts != null)
                newAccounts = model.Accounts.Where(c => c.IsChecked && !existing.ToList().Exists(e => e.AccountId == c.SelectionId)).ToList();

            foreach (MultipleSelectModel act in newAccounts)
            {
                AccountCategory dbActCat = new AccountCategory();

                dbActCat.AccountId = act.SelectionId;
                dbActCat.CategoryId = data.Id;
                dbActCat.IsActive = true;

                db.AccountCategories.Add(dbActCat);
            }
            foreach (AccountCategory act in existing)
            {
                MultipleSelectModel modelAct = new MultipleSelectModel();
                
                if (model.Accounts != null)
                    modelAct = model.Accounts.FirstOrDefault(c => c.SelectionId == act.AccountId);

                bool newValue = modelAct != null ? modelAct.IsChecked : false; // deleted account does not exist, make inactive
                if (act.IsActive != newValue)
                {
                    act.IsActive = newValue;
                    db.Entry(act).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        public static void Delete(Entities db, int id)
        {
            Category data = db.Categories.Where(cg => cg.Id == id).FirstOrDefault();
            db.Categories.Remove(data);
            db.SaveChanges();
        }
    }
}