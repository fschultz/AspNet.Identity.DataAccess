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
    using System.Linq;
    using System.Threading.Tasks;
    using System;
    using Telerik.OpenAccess;

    public class RoleStore : RoleStore<IdentityRole> {
        public RoleStore() {}
        public RoleStore(OpenAccessContext context) : base(context) {}
    }


    public class RoleStore<TRole> :
        IQueryableRoleStore<TRole, Guid>
        where TRole : IdentityRole {
        private OpenAccessContext _context;
        private readonly bool _isDisposable;

        public RoleStore() {
            _context = new DataContext();
            _isDisposable = true;
        }

        public RoleStore(OpenAccessContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            _context = context;
            _isDisposable = false;
        }

        static RoleStore() {
            DataContext.SynchronizeSchema();
        }

        #region IRoleStore

        public void Dispose() {
            if (_context == null) {
                return;
            }

            if (_isDisposable) {
                _context.Dispose();
            }

            _context = null;
        }

        public Task CreateAsync(TRole role) {
            if (role == null) {
                throw new ArgumentNullException("role");
            }

            _context.Add(role);
            _context.SaveChanges();

            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(TRole role) {
            if (role == null) {
                throw new ArgumentNullException("role");
            }

            _context.AttachCopy(role);
            _context.SaveChanges();

            return Task.FromResult<Object>(null);
        }

        public Task DeleteAsync(TRole role) {
            if (role == null) {
                throw new ArgumentNullException("role");
            }

            _context.Delete(role);
            _context.SaveChanges();

            return Task.FromResult<Object>(null);
        }

        public Task<TRole> FindByIdAsync(Guid roleId) {
            var role = Roles.FirstOrDefault(r => r.Id.Equals(roleId));
            return Task.FromResult(role);
        }

        public Task<TRole> FindByNameAsync(string roleName) {
            var role = Roles.FirstOrDefault(r => r.Name == roleName);
            return Task.FromResult(role);
        }

        #endregion


        #region IQueryableRoleStore

        public IQueryable<TRole> Roles {
            get { return _context.GetAll<TRole>(); }
        }

        #endregion
    }
}
