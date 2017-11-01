using fi.retorch.com.Data;

namespace fi.retorch.com.Areas.Dashboard.EntityModels
{
    public class TransactionEntity
    {
        public Transaction Transaction { get; set; }
        public Account Account { get; set; }
        public AccountType AccountType { get; set; }
        public Category Category { get; set; }
    }
}