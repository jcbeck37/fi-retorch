using fi.retorch.com.Areas.Dashboard.Code.Base;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Dashboard.Code.QuerySettings
{
    public class AccountQuerySettings : BaseQuerySettings
    {
        [Display(Name = "Account Type")]
        public int? TypeId { get; set; }

        [Display(Name = "Display on Dashboard")]
        public bool? IsDisplayed { get; set; }

        [Display(Name = "Closed")]
        public bool? IsClosed { get; set; }

        public int[] AccountsToInclude { get; set; }

        public AccountQuerySettings()
        {
            Page = 1;
            Sort = "DisplayOrder";
            //IsDisplayed = true;
            IsClosed = false;
        }
    }
}