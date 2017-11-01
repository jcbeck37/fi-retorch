using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Areas.Dashboard.Models;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : BaseController
    {
        // GET: Dashboard/Home
        public ActionResult Index()
        {
            DashboardModel model = new DashboardModel();
            model.StartDate = System.DateTime.Now.AddDays(-3);
            model.EndDate = System.DateTime.Now.AddMonths(1);
            
            model.Accounts = AccountList.GetDashboardAccounts(db, userKey).Items;

            // active bookmarks
            model.Bookmarks = new BookmarkList(db, userKey, true);

            return View(model);
        }

        public ActionResult Settings()
        {
            return View();
        }

        #region AJAX functions
        // gets transactions for all accounts within date range
        public ActionResult GetAccountTransactions(System.DateTime startDate, System.DateTime endDate)
        {
            TransactionQuerySettings settings = new TransactionQuerySettings();
            settings.StartDate = startDate;
            settings.EndDate = endDate;

            TransactionList list = TransactionList.GetDashboardTransactions(db, userKey, settings);
            //list.Items.ForEach(t => t.Amount = t.Amount * (t.IsCredit ? 1 : -1));
            list.Items.ForEach(t => t.EditLink = Url.Action("Edit", "Transactions", new { Id = t.Id }));
            list.Items.ForEach(t => t.PostLink = Url.Action("Post", "Transactions", new { Id = t.Id }));

            return Json(list.Items.ToArray(), JsonRequestBehavior.AllowGet);
        }

        // gets all active reminders for all accounts
        public ActionResult GetAccountReminders()
        {
            ReminderList list = ReminderList.GetDashboardReminders(db, userKey);
            list.Items.ForEach(r => r.EditLink = Url.Action("Edit", "Reminders", new { Id = r.Id }));
            list.Items.ForEach(r => r.GoLink = Url.Action("Create", "Transactions", new { reminderId = r.Id }));
            //list.Items.ForEach(t => t.Amount = t.Amount * (t.IsCredit ? 1 : -1));

            return Json(list.Items.ToArray(), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}