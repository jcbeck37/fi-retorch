using fi.retorch.com.Areas.Reports.EntityModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Reports.Models
{
    public class BudgetReviewModel
    {
        [Display(Name = "Start")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End")]
        public DateTime? EndDate { get; set; }
    }

    public class CategoryModel
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public int? TransferType { get; set; }
        public string Name { get; set; }
    }

    public class CategoryTotalModel
    {
        public CategoryModel Category { get; set; }
        public decimal? Total { get; set; }

        public static CategoryTotalModel ConvertEntityToModel(CategoryEntity entity)
        {
            CategoryTotalModel model = new CategoryTotalModel();
            
            if (entity != null)
            {
                model.Category = new CategoryModel();
                model.Category.Id = entity.Id;
                model.Category.GroupId = entity.GroupId;
                model.Category.GroupName = entity.GroupName;
                model.Category.TransferType = entity.TransferType;
                model.Category.Name = entity.Name;
            }

            return model;
        }
    }
}