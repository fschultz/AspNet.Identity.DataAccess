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

    public class DataContext : DataContext<IdentityUser> {
        public DataContext(OpenAccessContextBase otherContext) : base(otherContext) {}
        public DataContext() {}
        public DataContext(string connectionStringName) : base(connectionStringName) {}
    }

    public class DataContext<TUser> : OpenAccessContext where TUser : IdentityUser {
        private const int DatabaseVersion = 2;
        private static string _connectionStringName = "AspNetIdentity";
        private static MetadataContainer _metadataContainer = new DataMetadataSource().GetModel();
        private static BackendConfiguration _backendConfiguration = new BackendConfiguration();

        public DataContext(OpenAccessContextBase otherContext) : base(otherContext) {
        }

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

        public IQueryable<TUser> Users {
            get { return GetAll<TUser>(); }
        }

        public IQueryable<IdentityVersion> Versions {
            get { return GetAll<IdentityVersion>(); }
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

        public static MetadataContainer MetadataContainer {
            get { return _metadataContainer; }
            set { _metadataContainer = value; }
        }

        public static BackendConfiguration BackendConfiguration {
            get { return _backendConfiguration; }
            set { _backendConfiguration = value; }
        }

        public static DataContext<TUser> Create() {
            return new DataContext<TUser>();
        }

        #region Scheme synchronization

        private static bool _isSchemaInSync;

        internal static void SynchronizeSchema() {
            if (_isSchemaInSync) {
                return;
            }

            var context = new DataContext<TUser>();
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
                return version == null ? 0 : version.DatabaseVersion;
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
