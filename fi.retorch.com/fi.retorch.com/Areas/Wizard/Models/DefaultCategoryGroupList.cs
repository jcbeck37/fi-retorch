using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Wizard.EntityModels;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Wizard.Models
{
    public class DefaultCategoryGroupList
    {
        public List<DefaultCategoryGroupModel> CategoryGroups { get; set; }

        // exists for model binding
        public DefaultCategoryGroupList()
        {

        }

        public DefaultCategoryGroupList(DefaultEntities db)
        {
            CategoryGroups = DefaultCategoryGroupEntityList.GetDefaultCategoryGroups(db);
        }
    }

    public class DefaultCategoryGroupModel
    {
        [Display(Name = "Create")]
        public bool IsChecked { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string Name { get; set; }

        [Display(Name = "Transfer Type")]
        public int TransferType { get; set; }

        public CategoryGroupModel ConvertDefaultToModel()
        {
            CategoryGroupModel model = new CategoryGroupModel();

            model.Name = Name;
            model.TransferType = TransferType;

            return model;
        }
    }
}