using System.Collections.Generic;

namespace Ems.Authorization.Roles
{
    

    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string Admin = "Admin";

            public const string User = "User";

            public const string VendorAdmin = "Vendor Admin";

            public const string AssetOwnerAdmin = "Asset Owner Admin";

            public const string CustomerAdmin = "Customer Admin";

            public const string VendorUser = "Vendor User";

            public const string AssetOwnerUser = "Asset Owner User";

            public const string CustomerUser = "Customer User";

            public static List<RoleInfo> RoleHierarchy = new List<RoleInfo>()
            {
                new RoleInfo(){RoleName = "Admin", TenantType = "H", Level = 100},
                new RoleInfo(){RoleName = "Super Admin", TenantType = "V", Level = 95},
                new RoleInfo(){RoleName = "User", TenantType = "H", Level = 90},
                new RoleInfo(){RoleName = "Admin", TenantType = "V", Level = 100},
                new RoleInfo(){RoleName = "Vendor Super Admin", TenantType = "V", Level = 95},
                new RoleInfo(){RoleName = "Vendor Admin", TenantType = "V", Level = 90},
                new RoleInfo(){RoleName = "Vendor User", TenantType = "V", Level = 80},
                new RoleInfo(){RoleName = "Admin", TenantType = "C", Level = 100},
                new RoleInfo(){RoleName = "Customer Super Admin", TenantType = "C", Level = 95},
                new RoleInfo(){RoleName = "Customer Admin", TenantType = "C", Level = 90},
                new RoleInfo(){RoleName = "Customer User", TenantType = "C", Level = 80},
                new RoleInfo(){RoleName = "Admin", TenantType = "A", Level = 100},
                new RoleInfo(){RoleName = "Asset Owner Super Admin", TenantType = "A", Level = 95},
                new RoleInfo(){RoleName = "Asset Owner Admin", TenantType = "A", Level = 90},
                new RoleInfo(){RoleName = "Asset Owner User", TenantType = "A", Level = 80}
            };
        }
    }

    public class RoleInfo
    {
        public string RoleName { get; set; }
        public string TenantType { get; set; }
        public int Level { get; set; }
    }
}