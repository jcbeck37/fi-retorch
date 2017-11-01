using fi.retorch.com.Areas.Wizard.Models;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Wizard.EntityModels
{
    public class DefaultCategoryGroupEntityList
    {
        public static List<DefaultCategoryGroupModel> GetDefaultCategoryGroups(DefaultEntities db)
        {
            var query = from types in db.DefaultCategoryGroups
                        select types;

            List<DefaultCategoryGroupModel> list = new List<DefaultCategoryGroupModel>();
            query.ToList().ForEach(q => list.Add(DefaultCategoryGroupEntity.ConvertDataToModel(q)));

            return list;
        }
    }
    public class DefaultCategoryGroupEntity
    {
        public static DefaultCategoryGroupModel ConvertDataToModel(DefaultCategoryGroup data)
        {
            DefaultCategoryGroupModel model = new DefaultCategoryGroupModel();

            model.IsChecked = true;
            model.Name = data.Name;
            model.TransferType = data.TransferType;

            return model;
        }
    }
}