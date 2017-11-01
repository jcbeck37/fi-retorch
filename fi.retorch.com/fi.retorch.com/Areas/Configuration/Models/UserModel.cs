using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Configuration.Models
{
    public class UserListModel
    {
        public List<UserStatModel> Items { get; set; }

        public UserListModel(AuthEntities authDb, Entities db)
        {
            var query = from u in authDb.Users
                        select u;
            var users = query.ToList();

            var statsquery = from u in users
                             join a1 in db.Accounts on u.Id equals a1.UserId
                             into ua1
                             from ua in ua1.DefaultIfEmpty()
                             join t1 in db.Transactions on ua?.Id equals t1.AccountId
                             into t2
                             from t3 in t2.DefaultIfEmpty()
                             join r1 in db.Reminders on ua?.Id equals r1.AccountId into r2
                             from r3 in r2.DefaultIfEmpty()
                             join c1 in db.Categories on u.Id equals c1.UserId into c2
                             from c3 in c2.DefaultIfEmpty()
                             group new
                             {
                                 act = ua,
                                 trn = t3,
                                 rmd = r3,
                                 cat = c3
                             } by new
                             {
                                 usr = u
                             } into userList
                             select new UserStatEntity
                             {
                                 User = userList.Key.usr,
                                 AccountCount = userList.Where(ul => ul.act != null).Select(ul => ul.act.Id).Distinct().Count(),
                                 TransactionCount = userList.Where(ul => ul.trn != null).Select(ul => ul.trn.Id).Distinct().Count(),
                                 ReminderCount = userList.Where(ul => ul.rmd != null).Select(ul => ul.rmd.Id).Distinct().Count(),
                                 CategoryCount = userList.Where(ul => ul.cat != null).Select(ul => ul.cat.Id).Distinct().Count()
                             };
            Items = new List<UserStatModel>();
            statsquery.ToList().ForEach(q => Items.Add(new UserStatModel(q)));
        }
    }

    public class UserStatEntity
    {
        public User User { get; set; }
        public int AccountCount { get; set; }
        public int TransactionCount { get; set; }
        public int ReminderCount { get; set; }
        public int CategoryCount { get; set; }
    }

    public class UserStatModel
    {
        public string Email { get; set; }
        public int Accounts { get; set; }
        public int Transactions { get; set; }
        public int Reminders { get; set; }
        public int Categories { get; set; }
        public DateTime? LastTransaction { get; set; }

        public UserStatModel(UserStatEntity data)
        {
            Email = data.User.Email;
            Accounts = data.AccountCount;
            Transactions = data.TransactionCount;
            Reminders = data.ReminderCount;
            Categories = data.CategoryCount;
        }
    }
}