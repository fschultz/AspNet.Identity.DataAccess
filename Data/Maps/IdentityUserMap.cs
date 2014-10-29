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

namespace AspNet.Identity.DataAccess.Data.Maps {
    using Telerik.OpenAccess;
    using Telerik.OpenAccess.Metadata.Fluent;

    class IdentityUserMap : MappingConfiguration<IdentityUser> {
        public IdentityUserMap() {
            MapType(x => new { }).WithConcurencyControl(OptimisticConcurrencyControlStrategy.Changed).ToTable("AspNetUsers");
            HasProperty(x => x.Id).IsIdentity().ToColumn("Id").IsNotNullable();
            HasProperty(x => x.Created).ToColumn("Created").IsNotNullable();
            HasProperty(x => x.Updated).ToColumn("Updated").IsNotNullable();
            HasProperty(x => x.FirstName).ToColumn("FirstName").IsNullable().WithVariableLength(100);
            HasProperty(x => x.SurName).ToColumn("SurName").IsNullable().WithVariableLength(100);
            HasProperty(x => x.External).ToColumn("External").IsNotNullable();
            HasProperty(x => x.Email).ToColumn("Email").IsNullable().WithVariableLength(256);
            HasProperty(x => x.EmailConfirmed).ToColumn("EmailConfirmed").IsNotNullable();
            HasProperty(x => x.PasswordHash).ToColumn("PasswordHash").IsNullable().WithInfiniteLength();
            HasProperty(x => x.SecurityStamp).ToColumn("SecurityStamp").IsNullable().WithInfiniteLength();
            HasProperty(x => x.PhoneNumber).ToColumn("PhoneNumber").IsNullable().WithInfiniteLength();
            HasProperty(x => x.PhoneNumberConfirmed).ToColumn("PhoneNumberConfirmed").IsNotNullable();
            HasProperty(x => x.TwoFactorEnabled).ToColumn("TwoFactorEnabled").IsNotNullable();
            HasProperty(x => x.LockoutEndDateUtc).ToColumn("LockoutEndDateUtc").IsNullable();
            HasProperty(x => x.LockoutEnabled).ToColumn("LockoutEnabled").IsNotNullable();
            HasProperty(x => x.AccessFailedCount).ToColumn("AccessFailedCount").IsNotNullable();
            HasProperty(x => x.UserName).ToColumn("UserName").IsNotNullable().WithVariableLength(256);
            HasProperty(x => x.ProviderId).ToColumn("ProviderId").IsNullable();
            HasProperty(x => x.LastLoginDate).ToColumn("LastLoginDate").IsNullable();
            
            HasAssociation(x => x.Logins).WithOpposite(x => x.User).ToColumn("UserId").HasConstraint((y, x) => x.UserId == y.Id);
            HasAssociation(x => x.Claims).WithOpposite(x => x.User).ToColumn("UserId").HasConstraint((y, x) => x.UserId == y.Id);
            HasAssociation(x => x.Roles).WithOpposite(x => x.Users).IsManaged();
        }
    }
}
