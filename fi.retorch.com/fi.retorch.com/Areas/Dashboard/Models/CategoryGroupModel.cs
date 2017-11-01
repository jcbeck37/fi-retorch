using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.Enums;
using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace fi.retorch.com.Areas.Dashboard.Models
{
    public class CategoryGroupList : BaseModel
    { 
        private Entities Database { get; set; }

        public List<CategoryGroupModel> Items { get; set; }

        public CategoryGroupList()
        {
        }

        public CategoryGroupList(Entities database, string UserKey)
        {
            Database = database;
            Items = new List<CategoryGroupModel>();

            List<CategoryGroup> data = Database.CategoryGroups.Where(cg => cg.UserId == UserKey).ToList();
            data.ForEach(d => Items.Add(CategoryGroupModel.ConvertDataToModel(d)));
        }

        public static List<CategoryGroupModel> ConvertDataToList(List<CategoryGroup> data)
        {
            List<CategoryGroupModel> list = new List<CategoryGroupModel>();
            data.ForEach(d => list.Add(CategoryGroupModel.ConvertDataToModel(d)));

            return list;
        }
    }

    public class CategoryGroupModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Group")]
        public string Name { get; set; }
        
        [Display(Name = "Transfer Type")]
        public int TransferType { get; set; }
        public string TransferTypeText { get; set; }
        
        public static CategoryGroupModel ConvertDataToModel(CategoryGroup data) {
            CategoryGroupModel model = null;

            if (data != null)
            {
                model = new CategoryGroupModel();

                model.Id = data.Id;
                model.Name = data.Name;
                model.TransferType = data.TransferType;
                model.TransferTypeText = ((TransferTypeEnum)data.TransferType).ToFriendlyString();
            }

            return model;
        }

        public static CategoryGroupModel Get(Entities db, string userKey, int id)
        {
            CategoryGroup data = db.CategoryGroups.Where(cg => cg.Id == id && cg.UserId == userKey).FirstOrDefault();

            return ConvertDataToModel(data);
        }

        public static CategoryGroupModel FindByName(Entities db, string userKey, string name)
        {
            CategoryGroup data = db.CategoryGroups.Where(cg => cg.Name == name && cg.UserId == userKey).FirstOrDefault();

            return ConvertDataToModel(data);
        }

        public static void Save(CategoryGroupModel model, Entities db, string userKey, bool isNew = false)
        {
            CategoryGroup data = null;

            if (isNew)
            {
                data = new CategoryGroup();
                data.UserId = userKey;
                data.DateCreated = DateTime.Now;

                data.Name = model.Name;
                data.TransferType = model.TransferType;

                db.CategoryGroups.Add(data);
                db.SaveChanges();

                model.Id = data.Id;
            }
            else
            {
                data = db.CategoryGroups.Where(cg => cg.Id == model.Id && cg.UserId == userKey).FirstOrDefault();

                data.Name = model.Name;
                data.TransferType = model.TransferType;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public static void Delete(Entities db, string userKey, int id)
        {
            CategoryGroup data = db.CategoryGroups.Where(cg => cg.Id == id && cg.UserId == userKey).FirstOrDefault();
            db.CategoryGroups.Remove(data);
            db.SaveChanges();
        }
    }
}