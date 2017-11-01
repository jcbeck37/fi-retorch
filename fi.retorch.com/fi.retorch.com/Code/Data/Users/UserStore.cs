using fi.retorch.com.Data;
using fi.retorch.com.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace fi.retorch.com.Code.Data.Users
{
    public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>, IUserLockoutStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>, IUserLoginStore<ApplicationUser>,
        IRoleStore<ApplicationRole>, IUserRoleStore<ApplicationUser, string>
    {
        private AuthEntities database;

        public UserStore()
        {
            this.database = new AuthEntities();
        }

        public void Dispose()
        {
            this.database.Dispose();
        }

        public Task CreateAsync(ApplicationUser ApplicationUser)
        {
            User user = new User();

            user.Id = ApplicationUser.Id;
            user.Email = ApplicationUser.UserName;
            user.PasswordHash = ApplicationUser.PasswordHash;

            this.database.Users.Add(user);
            return this.database.SaveChangesAsync();
        }

        public Task CreateAsync(ApplicationUser ApplicationUser, string password)
        {
            User user = new User();

            user.Email = ApplicationUser.UserName;

            //user.PasswordSalt = Utilities.Authentication.GeneratePasswordSalt();
            //user.PasswordHash = Utilities.Authentication.GeneratePasswordHash(password, user.PasswordSalt);

            this.database.Users.Add(user);
            return this.database.SaveChangesAsync();
        }
        
        public Task UpdateAsync(ApplicationUser user)
        {
            // TODO
            //throw new NotImplementedException();
            // called from adding role but doesn't need to do anything at this time

            return Task.FromResult(0);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userKey)
        {
            User dbUser = await this.database.Users.Where(c => c.Id == userKey).FirstOrDefaultAsync();
            return ApplicationUser.ConvertDBUser(dbUser);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            ApplicationUser appUser = null;

            User dbUser = await this.database.Users.Where(c => c.Email == userName).FirstOrDefaultAsync();
            if (dbUser != null)
                appUser = ApplicationUser.ConvertDBUser(dbUser);

            return appUser;
        }

        #region UserPasswordStore
        public Task SetPasswordHashAsync(ApplicationUser ApplicationUser, string passwordHash)
        {
            ApplicationUser.PasswordHash = passwordHash;

            User user = this.database.Users.Where(u => u.Id == ApplicationUser.Id).FirstOrDefault();

            if (user != null)
            {
                user.PasswordHash = ApplicationUser.PasswordHash;
                this.database.Entry(user).State = EntityState.Modified;
                this.database.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult<bool>(!String.IsNullOrEmpty(user.PasswordHash));
        }
        #endregion

        #region PhoneNumberStore
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            return Task.FromResult<string>(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region EmailStore
        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult<string>(user.Email);

        //    string email = null;
        //    User dbUser = this.database.Users.Where(c => c.Id == user.Id).FirstOrDefault();
        //    if (dbUser != null)
        //        email = dbUser.Email;
        //    return Task.FromResult<string>(email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult<bool>(true);

            // TODO: Send confirmation email to verify user upon registration
            //return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            ApplicationUser userFound = null;

            User dbUser =  this.database.Users.Where(c => c.Email == email).FirstOrDefault();
            if (dbUser != null)
                userFound = ApplicationUser.ConvertDBUser(dbUser);

            return Task.FromResult<ApplicationUser>(userFound);
        }
        #endregion

        #region LockoutStore
        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew<bool>(() => false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region LoginStore
        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        // external logins
        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            List<UserLoginInfo> logins = new List<UserLoginInfo>();
            //UserLoginInfo userLogin = new UserLoginInfo(user.Email, user.PasswordHash);
            //logins.Add(userLogin);
            return Task.FromResult<IList<UserLoginInfo>>(logins);
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region RoleStore

        public Task CreateAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole, string>.FindByIdAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole, string>.FindByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region UserRoleStore

        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            Role role = this.database.Roles.Where(r => r.Name == roleName).FirstOrDefault();

            UserRole userRole = new UserRole();
            userRole.RoleId = role.Id;
            userRole.UserId = user.Id;
            this.database.UserRoles.Add(userRole);
            this.database.SaveChanges();

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            List<string> roles = new List<string>();

            var query = from ur in this.database.UserRoles
                        join r in this.database.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select r.Name;
            roles = query.ToList();

            return Task.FromResult<IList<string>>(roles);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            UserRole userRole = null;

            Role role = await this.database.Roles.Where(r => r.Name == roleName).FirstOrDefaultAsync();
            if (role != null)
                userRole = await this.database.UserRoles.Where(c => c.RoleId == role.Id && c.UserId == user.Id).FirstOrDefaultAsync();

            return (userRole != null);
        }
        #endregion
    }
}