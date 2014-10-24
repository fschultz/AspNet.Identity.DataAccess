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
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using System;

    public class IdentityUser : IUser<Guid> {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public Guid ProviderId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Phone { get; set; }
        public bool External { get; set; }

        public IList<IdentityUserClaim> Claims { get; set; }
        public IList<IdentityUserLogin> Logins { get; set; }
        public IList<IdentityRole> Roles { get; set; }

        public IdentityUser() {
            Id = Guid.NewGuid();
            
            Claims = new List<IdentityUserClaim>();
            Logins = new List<IdentityUserLogin>();
            Roles = new List<IdentityRole>();
        }
        
        public IdentityUser(string userName) : this() {
            UserName = userName;
        }

        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser, Guid> manager) {
            var claimsIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            return claimsIdentity;
        }

    }
}
