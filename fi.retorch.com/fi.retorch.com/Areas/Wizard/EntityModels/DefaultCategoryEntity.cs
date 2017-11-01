using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Wizard.Models;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Wizard.EntityModels
{
    public class DefaultCategoryEntityList
    {
        public static List<DefaultCategoryModel> GetDefaultCategories(DefaultEntities defaultDb, Entities db, string userKey)
        {
            var query = from types in defaultDb.DefaultCategories
                        select types;

            List<DefaultCategoryModel> list = new List<DefaultCategoryModel>();
            query.ToList().ForEach(q => list.Add(DefaultCategoryEntity.ConvertDataToModel(db, userKey, q)));

            return list;
        }
    }

    public class DefaultCategoryEntity
    {
        public static DefaultCategoryModel ConvertDataToModel(Entities db, string userKey, DefaultCategory data)
        {
            DefaultCategoryModel model = new DefaultCategoryModel();

            CategoryGroupModel group = CategoryGroupModel.FindByName(db, userKey, data.GroupName);
            if (group != null)
                model.GroupId = group.Id;

            model.IsChecked = true;
            model.Name = data.Name;

            return model;
        }
    }
}