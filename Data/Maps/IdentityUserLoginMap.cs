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

    public class IdentityUserLoginMap : MappingConfiguration<IdentityUserLogin> {
        public IdentityUserLoginMap() {
            MapType(x => new { }).WithConcurencyControl(OptimisticConcurrencyControlStrategy.Changed).ToTable("AspNetUserLogins");
            HasProperty(x => x.LoginProvider).IsIdentity().ToColumn("LoginProvider").IsNotNullable().WithVariableLength(128);
            HasProperty(x => x.ProviderKey).IsIdentity().ToColumn("ProviderKey").IsNotNullable().WithVariableLength().HasLength(128);
            HasProperty(x => x.UserId).IsIdentity().ToColumn("UserId").IsNotNullable();

            HasAssociation(x => x.User).WithOpposite(x => x.Logins).ToColumn("UserId").HasConstraint((x, y) => x.UserId == y.Id).IsRequired();
        }
    }
}
