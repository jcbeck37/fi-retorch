namespace fi.retorch.com.Areas.Reports.EntityModels
{
    public class BudgetReviewEntity
    {
    }

    public class CategoryEntity
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public int? TransferType { get; set; }
        public string Name { get; set; }
    }

    public class CategoryMonthEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public CategoryEntity Category { get; set; }
        public decimal? Total { get; set; }
    }

    public class CategoryYearEntity
    {
        public int Year { get; set; }
        public CategoryEntity Category { get; set; }
        public decimal? Total { get; set; }
    }
}