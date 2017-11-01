using fi.retorch.com.Areas.Dashboard.Code.Base;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Dashboard.Code.QuerySettings
{
	public class CategoryQuerySettings : BaseQuerySettings
    {
        [Display(Name = "Group")]
        public int? GroupId { get; set; }

        [Display(Name = "Active")]
        public bool? IsActive { get; set; }
    }
}