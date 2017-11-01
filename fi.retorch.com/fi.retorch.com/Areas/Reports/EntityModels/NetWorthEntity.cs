using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Data;

namespace fi.retorch.com.Areas.Reports.EntityModels
{
    public class NetWorthEntity
    {
        public Account Account { get; set; }
        public AccountType Type { get; set; }

        public AccountEntity ConvertNetWorthToAccount()
        {
            AccountEntity account = new AccountEntity();

            account.Account = Account;
            account.Type = Type;

            return account;
        }
    }

    public class NetWorthAccountTypeEntity
    {
        public AccountType AccountType { get; set; }
        public decimal? AccountOpeningTotal { get; set; }
        public decimal? AccountCurrentTotal { get; set; }
        public decimal? TransactionTotal { get; set; }
    }

    public class MonthlyEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Account Account { get; set; }
        public AccountType Type { get; set; }
        public decimal? TransactionTotal { get; set; }
    }

    public class AnnualEntity
    {
        public int Year { get; set; }
        public Account Account { get; set; }
        public AccountType Type { get; set; }
        public decimal? TransactionTotal { get; set; }
    }
}