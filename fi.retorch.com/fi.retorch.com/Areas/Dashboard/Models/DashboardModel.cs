using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fi.retorch.com.Areas.Dashboard.Models
{
    public class DashboardModel
    {
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public bool ShowBookmarks { get { return Bookmarks.Items.Any(); } }
        public BookmarkList Bookmarks { get; set; }

        public List<AccountModel> Accounts { get; set; }
    }
}