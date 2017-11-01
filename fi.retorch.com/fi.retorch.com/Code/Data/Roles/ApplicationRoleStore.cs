using fi.retorch.com.Data;
using fi.retorch.com.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace fi.retorch.com.Code.Data.Roles
{
    public class ApplicationRoleStore : IRoleStore<ApplicationRole>
    {
        private AuthEntities db;
        public ApplicationRoleStore() { }

        public ApplicationRoleStore(AuthEntities database)
        {
            db = database;
        }

        public Task CreateAsync(ApplicationRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("RoleIsRequired");
            }
            var roleEntity = ApplicationRole.ConvertRoleToEntity(role);
            db.Roles.Add(roleEntity);
            return db.SaveChangesAsync();

        }

        public Task DeleteAsync(ApplicationRole role)
        {
            var roleEntity = db.Roles.FirstOrDefault(x => x.Id == role.Id);
            if (roleEntity == null) throw new InvalidOperationException("No such role exists!");
            db.Roles.Remove(roleEntity);
            return db.SaveChangesAsync();
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId)
        {
            var role = db.Roles.FirstOrDefault(x => x.Id == roleId);

            var result = role == null
                ? null
                : ApplicationRole.ConvertEntityToRole(role);

            return Task.FromResult(result);
        }

        public Task<ApplicationRole> FindByNameAsync(string roleName)
        {

            var role = db.Roles.FirstOrDefault(x => x.Name == roleName);

            var result = role == null
                ? null
                : ApplicationRole.ConvertEntityToRole(role);

            return Task.FromResult(result);
        }

        public Task UpdateAsync(ApplicationRole role)
        {

            return db.SaveChangesAsync();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }    
}