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
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity;

    public class IdentityRole : IRole<Guid> {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<IdentityUser> Users { get; set; }

        public IdentityRole() {
            Id = Guid.NewGuid();
            Users = new List<IdentityUser>();
        }

        public IdentityRole(string name) : this() {
            Name = name;
        }

        public IdentityRole(string name, Guid id) {
            Id = id;
            Name = name;
            Users = new List<IdentityUser>();
        }
    }
}
