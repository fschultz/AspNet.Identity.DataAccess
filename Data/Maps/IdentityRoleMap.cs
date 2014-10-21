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

    class IdentityRoleMap : MappingConfiguration<IdentityRole> {
        public IdentityRoleMap() {
            MapType(x => new { }).WithConcurencyControl(OptimisticConcurrencyControlStrategy.Changed).ToTable("AspNetRoles");
            HasProperty(x => x.Id).IsIdentity().ToColumn("Id").IsNotNullable();
            HasProperty(x => x.Name).ToColumn("Name").IsNotNullable().WithVariableLength(256);

            HasAssociation(x => x.Users).WithOpposite(x => x.Roles).IsManaged().MapJoinTable("AspNetUserRoles", (x, y) => new { RoleId = x.Id, UserId = y.Id }).CreatePrimaryKeyFromForeignKeys();
        }
    }
}
