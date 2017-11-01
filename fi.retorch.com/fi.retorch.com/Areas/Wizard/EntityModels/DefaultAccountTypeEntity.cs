using fi.retorch.com.Areas.Wizard.Models;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Wizard.EntityModels
{
    public class DefaultAccountTypeEntityList
    {
        public static List<DefaultAccountTypeModel> GetDefaultAccountTypes(DefaultEntities db)
        {
            var query = from types in db.DefaultAccountTypes
                        select types;

            List<DefaultAccountTypeModel> list = new List<DefaultAccountTypeModel>();
            query.ToList().ForEach(q => list.Add(DefaultAccountTypeEntity.ConvertDataToModel(q)));

            return list;
        }
    }

    public class DefaultAccountTypeEntity
    {
        public static DefaultAccountTypeModel ConvertDataToModel(DefaultAccountType data)
        {
            DefaultAccountTypeModel model = new DefaultAccountTypeModel();

            model.IsChecked = true;
            model.Name = data.Name;
            model.IsDebt = data.IsDebt;
            model.PositiveText = data.PositiveText;
            model.NegativeText = data.NegativeText;

            return model;
        }
    }
}