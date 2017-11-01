using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Wizard.EntityModels;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Wizard.Models
{
    public class DefaultCategoryList
    {
        public List<DefaultCategoryModel> Categories { get; set; }

        // exists for model binding
        public DefaultCategoryList()
        {

        }

        public DefaultCategoryList(DefaultEntities defaultDb, Entities db, string userKey)
        {
            Categories = DefaultCategoryEntityList.GetDefaultCategories(defaultDb, db, userKey);
            
            Categories.ForEach(c =>
            {
                c.Groups = new SelectList(CategoryGroupList.ConvertDataToList(db.CategoryGroups.Where(cg => cg.UserId == userKey).ToList()), "Id", "Name", c.GroupId);
            });
        }
    }

    public class DefaultCategoryModel
    {
        public IEnumerable<SelectListItem> Groups { get; set; }

        [Display(Name = "Group")]
        public int? GroupId { get; set; }

        [Display(Name = "Create")]
        public bool IsChecked { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Name { get; set; }
        
        public CategoryModel ConvertDefaultToModel()
        {
            CategoryModel model = new CategoryModel();

            model.GroupId = GroupId;
            model.Name = Name;
            model.IsActive = true;

            return model;
        }
    }
}