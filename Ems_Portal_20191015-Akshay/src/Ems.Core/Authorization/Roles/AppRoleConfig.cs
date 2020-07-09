using Abp.MultiTenancy;
using Abp.Zero.Configuration;

namespace Ems.Authorization.Roles
{
    public static class AppRoleConfig
    {
        public static void Configure(IRoleManagementConfig roleManagementConfig)
        {
            //Static host roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Host.Admin,
                    MultiTenancySides.Host,
                    grantAllPermissionsByDefault: true)
                );

            //Static tenant roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.Admin,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: true)
                );

            //roleManagementConfig.StaticRoles.Add(
            //    new StaticRoleDefinition(
            //        StaticRoleNames.Tenants.User,
            //        MultiTenancySides.Tenant,
            //        grantAllPermissionsByDefault: false)
            //    );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.AssetOwnerAdmin,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.CustomerAdmin,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.VendorAdmin,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.AssetOwnerUser,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.CustomerUser,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.VendorUser,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );
        }
    }
}
