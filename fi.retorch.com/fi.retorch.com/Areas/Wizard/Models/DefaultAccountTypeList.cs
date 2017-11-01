using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Wizard.EntityModels;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Wizard.Models
{
    public class DefaultAccountTypeList
    {
        public List<DefaultAccountTypeModel> AccountTypes { get; set; }

        // exists for model binding
        public DefaultAccountTypeList()
        {

        }

        public DefaultAccountTypeList(DefaultEntities db)
        {
            AccountTypes = DefaultAccountTypeEntityList.GetDefaultAccountTypes(db);
        }
    }

    public class DefaultAccountTypeModel
    {
        [Display(Name = "Create")]
        public bool IsChecked { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string Name { get; set; }

        [Display(Name = "Debt/Liability")]
        public bool IsDebt { get; set; }

        [Display(Name = "Credits Label")]
        public string PositiveText { get; set; }

        [Display(Name = "Debits Label")]
        public string NegativeText { get; set; }

        public AccountTypeModel ConvertDefaultToModel()
        {
            AccountTypeModel model = new AccountTypeModel();

            model.Name = Name;
            model.IsDebt = IsDebt;
            model.PositiveText = PositiveText;
            model.NegativeText = NegativeText;

            return model;
        }
    }
}