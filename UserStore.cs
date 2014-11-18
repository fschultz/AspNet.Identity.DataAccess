#region License and copyright notice
/* 
 * ASP.NET Identity provider for Telerik Data Access
 * 
 * Copyright (c) Fredrik Schultz and Contributors
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

namespace AspNet.Identity.DataAccess {
    using Data;
    using Microsoft.AspNet.Identity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System;

    public class UserStore : 
        IUserRoleStore<IdentityUser, Guid> ,
        IUserClaimStore<IdentityUser, Guid>,
        IUserPasswordStore<IdentityUser, Guid>,
        IUserSecurityStampStore<IdentityUser, Guid>,
        IUserEmailStore<IdentityUser, Guid>,
        IUserPhoneNumberStore<IdentityUser, Guid>,
        IUserTwoFactorStore<IdentityUser, Guid>,
        IUserLockoutStore<IdentityUser, Guid>,
        IUserLoginStore<IdentityUser, Guid>,
        IQueryableUserStore<IdentityUser, Guid> {
        private DataContext _context;
        private readonly bool _isDisposable;

        public UserStore() {
            _context = new DataContext();
            _isDisposable = true;
        }

        public UserStore(DataContext context) {
            _context = context;
            _isDisposable = false;
        }

        static UserStore() {
            DataContext.SynchronizeSchema();
        }


        #region IUserClaimStore

        public Task<IList<Claim>> GetClaimsAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            IList<Claim> claims = _context.UserClaims.Where(c => c.UserId == user.Id).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();

            return Task.FromResult(claims);
        }

        public Task AddClaimAsync(IdentityUser user, Claim claim) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (claim == null) {
                throw new ArgumentNullException("claim");
            }

            var alreadyHasClaim = GetUserClaim(user, claim).Any();

            if (!alreadyHasClaim) {
                _context.Add(new IdentityUserClaim(user.Id, claim));
                _context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(IdentityUser user, Claim claim) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (claim == null) {
                throw new ArgumentNullException("claim");
            }

            var userClaim = GetUserClaim(user, claim);
            _context.Delete(userClaim);
            _context.SaveChanges();

            return Task.FromResult(0);
        }

        #endregion


        #region IUserEmailStore

        public Task SetEmailAsync(IdentityUser user, string email) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task<IdentityUser> FindByEmailAsync(string email) {
            if (email == null) {
                throw new ArgumentNullException("email");
            }

            var user = _context.UsersWithIncludes.FirstOrDefault(u => u.Email == email);

            return Task.FromResult(user);
        }

        #endregion


        #region IUserLockoutStore

        public Task<DateTimeOffset> GetLockoutEndDateAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if(user.LockoutEndDateUtc.HasValue){
                return Task.FromResult(new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)));
            }

            return Task.FromResult(new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset lockoutEnd) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (lockoutEnd == DateTimeOffset.MinValue) {
                user.LockoutEndDateUtc = null;
            }
            else {
                user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            }

            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;

            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(IdentityUser user, bool enabled) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        #endregion


        #region IUserLoginStore

        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (login == null) {
                throw new ArgumentNullException("login");
            }

            user.Logins.Add(new IdentityUserLogin {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(IdentityUser user, UserLoginInfo login) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (login == null) {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var loginEntity = user.Logins.SingleOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (loginEntity != null) {
                user.Logins.Remove(loginEntity);
            }

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            IList<UserLoginInfo> result = user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();

            return Task.FromResult(result);
        }

        public Task<IdentityUser> FindAsync(UserLoginInfo login) {
            if (login == null) {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var userLogin = _context.UserLogins.FirstOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (userLogin == null) {
                return Task.FromResult<IdentityUser>(null);
            }

            var user = _context.UsersWithIncludes.FirstOrDefault(u => u.Id.Equals(userLogin.UserId));

            return Task.FromResult(user);
        }

        #endregion


        #region IUserPasswordStore

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var hasPassword = !string.IsNullOrEmpty(user.PasswordHash);

            return Task.FromResult(hasPassword);
        }

        #endregion


        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(IdentityUser user, string phoneNumber) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            
            user.PhoneNumber = phoneNumber;
            
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(IdentityUser user, bool confirmed) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        #endregion


        #region IUserRoleStore

        public Task AddToRoleAsync(IdentityUser user, string roleName) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            
            if (string.IsNullOrEmpty(roleName)) {
                throw new ArgumentException("roleName");
            }

            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role != null && user.Roles.All(r => r.Id != role.Id)) {
                user.Roles.Add(role);
                _context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName)) {
                throw new ArgumentNullException("roleName");
            }
            
            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role != null && user.Roles.Any(r => r.Id == role.Id)) {
                user.Roles.Remove(role);
                _context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var roles = user.Roles.Select(r => r.Name).ToList();

            return Task.FromResult<IList<string>>(roles);
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName)) {
                throw new ArgumentNullException("roleName");
            }
            
            var isInRole = user.Roles.Any(r => r.Name == roleName);

            return Task.FromResult(isInRole);
        }

        #endregion


        #region IUserSecurityStampStore

        public Task SetSecurityStampAsync(IdentityUser user, string stamp) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion


        #region IUserStore

        public void Dispose() {
            if (_context == null) {
                return;
            }

            if (_isDisposable) {
                _context.Dispose();
            }

            _context = null;
        }

        public Task CreateAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            if (user.Created == DateTime.MinValue) {
                user.Created = DateTime.Now;
            }

            user.Updated = DateTime.Now;

            _context.Add(user);
            _context.SaveChanges();

            return Task.FromResult(0);
        }

        public Task UpdateAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.Updated = DateTime.Now;

            _context.AttachCopy(user);
            _context.SaveChanges();

            return Task.FromResult(0);
        }

        public Task DeleteAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            _context.Delete(user);

            return Task.FromResult(0);
        }

        public Task<IdentityUser> FindByIdAsync(Guid userId) {
            if (userId == null) {
                throw new ArgumentNullException("userId");
            }

            var user = _context.UsersWithIncludes.FirstOrDefault(u => u.Id == userId);
            
            return Task.FromResult(user);
        }

        public Task<IdentityUser> FindByNameAsync(string userName) {
            if (string.IsNullOrEmpty(userName)) {
                throw new ArgumentNullException("userName");
            }

            var user = _context.UsersWithIncludes.FirstOrDefault(u => u.UserName == userName);
            
            return Task.FromResult(user);
        }

        #endregion


        #region IUserTwoFactorStore

        public Task SetTwoFactorEnabledAsync(IdentityUser user, bool enabled) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            user.TwoFactorEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(IdentityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion


        #region IQueryableUserStore

        public IQueryable<IdentityUser> Users {
            get { return _context.Users; }
        }

        #endregion


        #region Private functions

        private IQueryable<IdentityUserClaim> GetUserClaim(IUser<Guid> user, Claim claim) {
            return _context.UserClaims.Where(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value && c.UserId == user.Id);
        }

        #endregion
    }
}
