using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Data;
using fi.retorch.com.Models;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Legacy.Models
{
    public class LegacyCategoryList
    {
        public List<LegacyCategoryModel> Items { get; set; }

        public LegacyCategoryList(LegacyEntities db, int userId)
        {
            Items = new List<LegacyCategoryModel>();

            // get all categories with at least on transaction assigned
            var data = from cat in db.act_categories
                       join t in db.act_transactions on cat.category_id equals t.category_id
                       where cat.user_id == userId // && cat.isActive == 1
                       select cat;
            data.Distinct().ToList().ForEach(d => Items.Add(LegacyCategoryModel.ConvertDataToModel(d)));
        }
    }

    public class LegacyCategoryModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public LegacyCategoryModel()
        {

        }

        public LegacyCategoryModel(LegacyEntities db, int id)
        {
            act_categories data = db.act_categories.Find(id);

            Id = data.category_id;
            Name = data.category_name;
            IsActive = data.isActive != 0;
        }

        public static LegacyCategoryModel ConvertDataToModel(act_categories data)
        {
            LegacyCategoryModel model = new LegacyCategoryModel();

            model.Id = data.category_id;
            model.Name = data.category_name;
            model.IsActive = data.isActive != 0;

            return model;
        }

        public CategoryModel ConvertLegacyToModel(int defaultCategoryGroupId)
        {
            CategoryModel model = new CategoryModel();

            model.GroupId = defaultCategoryGroupId;
            model.Name = Name;
            model.Accounts = new List<MultipleSelectModel>();
            model.IsActive = IsActive;

            return model;
        }
    }
}