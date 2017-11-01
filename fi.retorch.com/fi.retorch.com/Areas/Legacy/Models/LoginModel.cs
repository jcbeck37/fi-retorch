using fi.retorch.com.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace fi.retorch.com.Areas.Legacy.Models
{
    public class LoginModel
    {
        public bool Authorized { get; set; }

        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int Validate(LegacyEntities db)
        {
            var data = db.ng_users.FirstOrDefault(u => u.username == Username && u.password == Password);
            if (data != null)
                return data.user_id;

            return 0;
        }
    }
}