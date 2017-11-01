using fi.retorch.com.Data;

namespace fi.retorch.com.Areas.Dashboard.EntityModels
{
    public class ReminderEntity
    {
        public Reminder Reminder { get; set; }
        public Account Account { get; set; }
        public AccountType AccountType { get; set; }
        public ReminderSchedule Schedule { get; set; }
        public Category Category { get; set; }
    }
}