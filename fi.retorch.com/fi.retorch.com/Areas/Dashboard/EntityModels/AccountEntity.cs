using fi.retorch.com.Data;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Dashboard.EntityModels
{
    public class AccountEntity
    {
        public Account Account { get; set; }
        public AccountType Type { get; set; }

        public static List<AccountCategoryJson> GetAccountCategories(Entities db, string userKey, int accountId, int? categoryId = null)
        {
            // only get Categories that are active, and Account Categories for the Account that are currently enabled, plus any exception passed in
            var categories = from ac in db.AccountCategories
                             join c in db.Categories on new { c = ac.CategoryId, a = ac.AccountId } equals new { c = c.Id, a = accountId }
                             where c.UserId == userKey && c.IsActive == true
                                && (ac.IsActive || (categoryId.HasValue && c.Id == categoryId.Value))
                             select new AccountCategoryJson { Id = c.Id, Value = c.Name, IsActive = ac.IsActive };
            return categories.OrderBy(c => c.Value).ToList();
        }
    }

    public class AccountCategoryJson
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}