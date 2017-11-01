using fi.retorch.com.Areas.Reports.EntityModels;
using fi.retorch.com.Data;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Reports.Models
{
    public class BudgetReviewAnnualModel : BudgetReviewModel
    {
        public BudgetReviewAnnualModel()
        {
            // default values
            EndDate = DateTime.Now;

            StartDate = EndDate.Value.AddDays((-1 * EndDate.Value.DayOfYear)).AddYears(-1); // first day of previous year
        }

        public BudgetReviewAnnualModel(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            if (!EndDate.HasValue)
            {
                // start with today
                EndDate = DateTime.Now;
                
                // last day of current month (go to next month and remove all the days)
                EndDate.Value.AddMonths(1);
                int daysToRemove = EndDate.Value.Day;
                EndDate.Value.AddDays(-1 * daysToRemove);
            }

            if (!StartDate.HasValue)
            {
                StartDate = EndDate.Value.AddYears(-2).AddDays((-1 * EndDate.Value.Day) + 1); // show previous two (full) years
            }
        }
    }

    public class BudgetReviewAnnualReport
    { 
        public List<CategoryYearTotal> Items { get; set; }

        public BudgetReviewAnnualReport(Entities db, string userKey, DateTime startDate, DateTime endDate)
        {
            Items = new List<CategoryYearTotal>();

            // gather data
            var query = 
                        from trns in db.Transactions //on cat.Id equals trns.CategoryId
                        where trns.DatePosted != null && trns.DisplayDate >= startDate && trns.DisplayDate <= endDate
                        join act in db.Accounts on trns.AccountId equals act.Id
                        where act.UserId == userKey
                        join typ in db.AccountTypes on act.TypeId equals typ.Id
                        join cat1 in db.Categories on trns.CategoryId equals cat1.Id into cat2
                        from cat in cat2.DefaultIfEmpty()
                        where cat == null || cat.UserId == userKey
                        join jgrp in db.CategoryGroups on cat.GroupId equals jgrp.Id into tmpgrp
                        from grp in tmpgrp.DefaultIfEmpty()
                        group new
                        {
                            trns.Amount,
                            trns.IsCredit,
                            typ.IsDebt
                        } by new {
                            trns.DisplayDate.Year,
                            Ctgry = cat == null ? null : cat,
                            Grp = grp == null ? null : grp
                        } into trnsGroup
                        select new CategoryYearEntity
                        {
                            Year = trnsGroup.Key.Year,
                            Category = trnsGroup.Key.Ctgry == null ? null : new CategoryEntity {
                                Id = trnsGroup.Key.Ctgry.Id,
                                GroupId = trnsGroup.Key.Grp == null ? (int?)null : trnsGroup.Key.Grp.Id,
                                GroupName = trnsGroup.Key.Grp == null ? null : trnsGroup.Key.Grp.Name,
                                TransferType = trnsGroup.Key.Grp == null ? (int?)null : trnsGroup.Key.Grp.TransferType,
                                Name = trnsGroup.Key.Ctgry.Name
                            },
                            Total = trnsGroup.Select(x => x.Amount * (x.IsDebt ? -1 : 1) *(x.IsCredit  ? 1 : -1)).Sum()
                        };

            // convert to list of category/month totals
            query.ToList().ForEach(q => Items.Add(CategoryYearTotal.ConvertEntityToModel(q)));

            // sort by transfer type, group/category name
            Items = Items.OrderBy(q => q.Category != null ?  q.Category.TransferType : 0)
                .ThenBy(q => q.Category != null ? q.Category.GroupName : "")
                .ThenBy(q => q.Category != null ? q.Category.Name : "").ToList();
        }
    }

    public class CategoryYearTotal : CategoryTotalModel
    {
        public int Year { get; set; }

        public static CategoryYearTotal ConvertEntityToModel(CategoryYearEntity entity)
        {
            CategoryYearTotal model = new CategoryYearTotal();

            if (entity.Category != null)
            {
                CategoryTotalModel baseModel = CategoryTotalModel.ConvertEntityToModel(entity.Category);

                model.Category = baseModel.Category;
            }
            model.Year = entity.Year;
            model.Total = entity.Total;

            return model;
        }
    }
}