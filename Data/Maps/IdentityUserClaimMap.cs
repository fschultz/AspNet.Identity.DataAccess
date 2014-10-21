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

    public class IdentityUserClaimMap : MappingConfiguration<IdentityUserClaim> {
        public IdentityUserClaimMap() {
            MapType(x => new { }).WithConcurencyControl(OptimisticConcurrencyControlStrategy.Changed).ToTable("AspNetUserClaims");
            HasProperty(x => x.Id).IsIdentity().ToColumn("Id").IsNotNullable();
            HasProperty(x => x.UserId).ToColumn("UserId").IsNotNullable();
            HasProperty(x => x.ClaimType).ToColumn("ClaimType").IsNullable().WithInfiniteLength();
            HasProperty(x => x.ClaimValue).ToColumn("ClaimValue").IsNullable().WithInfiniteLength();
            
            HasAssociation(x => x.User).WithOpposite(x => x.Claims).ToColumn("UserId").HasConstraint((x, y) => x.UserId == y.Id).IsRequired();
        }
    }
}
