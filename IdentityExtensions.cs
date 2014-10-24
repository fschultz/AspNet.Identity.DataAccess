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
    using System.ComponentModel;
    using System.Security.Principal;
    using Microsoft.AspNet.Identity;

    public static class IdentityExtensions {
        public static T GetUserIdGeneric<T>(this IIdentity identity) {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(identity.GetUserId());
        }
    }
}
