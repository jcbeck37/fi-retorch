using fi.retorch.com.Data;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace fi.retorch.com.Models
{ 
    public partial class ApplicationRole : IdentityRole
    {
        //public string Id { get; set; }
        //public string Name { get; set; }
        public static Role ConvertRoleToEntity(ApplicationRole model)
        {
            Role entity = new Role();

            entity.Id = model.Id;
            entity.Name = model.Name;

            return entity;
        }

        public static ApplicationRole ConvertEntityToRole(Role entity)
        {
            ApplicationRole role = new ApplicationRole();

            role.Id = entity.Id;
            role.Name = entity.Name;

            return role;
        }
    }
}