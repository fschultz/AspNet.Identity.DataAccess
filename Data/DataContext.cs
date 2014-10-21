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

namespace AspNet.Identity.DataAccess.Data {
    using Maps;
    using System.Collections.Generic;
    using System.Linq;
    using Telerik.OpenAccess.FetchOptimization;
    using Telerik.OpenAccess.Metadata.Fluent;
    using Telerik.OpenAccess.Metadata;
    using Telerik.OpenAccess;

    public class DataContext : OpenAccessContext {
        private const int DatabaseVersion = 1;
        private static string _connectionStringName = "AspNetIdentity";
        private static readonly MetadataContainer MetadataContainer = new DataMetadataSource().GetModel();
        private static readonly BackendConfiguration BackendConfiguration = new BackendConfiguration();

        public DataContext() : base(ConnectionStringName, BackendConfiguration, MetadataContainer) {
        }

        public DataContext(string connectionStringName) : base(connectionStringName, BackendConfiguration, MetadataContainer) {
            FetchStrategy = new FetchStrategy {
                MaxFetchDepth = 1
            };
        }

        protected override string CacheKey {
            get { return "AspNetIdentity"; }
        }

        public IQueryable<IdentityRole> Roles {
            get { return GetAll<IdentityRole>(); }
        }

        public IQueryable<IdentityUserClaim> UserClaims {
            get { return GetAll<IdentityUserClaim>(); }
        }

        public IQueryable<IdentityUserLogin> UserLogins {
            get { return GetAll<IdentityUserLogin>(); }
        }

        public IQueryable<IdentityUser> Users {
            get { return GetAll<IdentityUser>(); }
        }

        public IQueryable<IdentityVersion> Versions {
            get { return GetAll<IdentityVersion>(); }
        }

        public IQueryable<IdentityUser> UsersWithIncludes {
            get {
                return Users
                    .Include(u => u.Claims)
                    .Include(u => u.Logins)
                    .Include(u => u.Roles);
            }
        }

        public IQueryable<IdentityRole> RolesWithIncludes {
            get {
                return Roles
                    .Include(r => r.Users);
            }
        }

        public static string ConnectionStringName {
            get { return _connectionStringName; }
            set { _connectionStringName = value; }
        }

        #region Scheme synchronization

        private static bool _isSchemaInSync;

        internal static void SynchronizeSchema() {
            if (_isSchemaInSync) {
                return;
            }

            var context = new DataContext();
            context.UpdateSchema();

            _isSchemaInSync = true;
        }

        internal void UpdateSchema() {
            var schemaHandler = GetSchemaHandler();
            string script;

            if (schemaHandler.DatabaseExists()) {
                var currentVersion = GetCurrentVersion();
                if (currentVersion >= DatabaseVersion) {
                    return;
                }
                script = schemaHandler.CreateUpdateDDLScript(null);
            }
            else {
                schemaHandler.CreateDatabase();
                script = schemaHandler.CreateDDLScript();
            }


            if (string.IsNullOrEmpty(script)) {
                return;
            }

            schemaHandler.ForceExecuteDDLScript(script);
            SetDatabaseVersion();
        }

        private void SetDatabaseVersion() {
            var version = Versions.FirstOrDefault() ?? new IdentityVersion();
            version.DatabaseVersion = DatabaseVersion;
            AttachCopy(version);
            SaveChanges();
        }

        private int GetCurrentVersion() {
            try {
                var version = Versions.FirstOrDefault();
                if (version == null) {
                    return 0;
                }
                return version.DatabaseVersion;
            }
            catch {
                return 0;
            }
        }

        #endregion
    }

    public class DataMetadataSource : FluentMetadataSource {
        protected override IList<MappingConfiguration> PrepareMapping() {
            var configurations = new List<MappingConfiguration> {
                new IdentityVersionMap(),
                new IdentityRoleMap(),
                new IdentityUserClaimMap(),
                new IdentityUserLoginMap(),
                new IdentityUserMap()
            };

            return configurations;
        }
    }
}
