using fi.retorch.com.Areas.Dashboard.Code.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Dashboard.Code.QuerySettings
{
    public class TransactionQuerySettings : BaseQuerySettings
    {
        [Display(Name = "Account Type")]
        public int? AccountTypeId { get; set; }

        [Display(Name = "Account")]
        public int? AccountId { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EndDate { get; set; }

        public TransactionQuerySettings()
        {
            Page = 1;
            Sort = "DisplayDateDesc";
        }
    }
}