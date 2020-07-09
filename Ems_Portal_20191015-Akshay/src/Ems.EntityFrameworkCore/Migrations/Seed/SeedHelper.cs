using System;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Ems.EntityFrameworkCore;
using Ems.Migrations.Seed.Host;
using Ems.Migrations.Seed.Tenants;
using System.Collections.Generic;
using System.Linq;

namespace Ems.Migrations.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<EmsDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(EmsDbContext context)
        {
            context.SuppressAutoSetTenantId = true;
            List<string> validTenantTypes = "H,A,C,V".Split(',').ToList();

            bool createTestData = false;
            bool createSeedData = false;

            //Deal with newly reted tenants
           // List<AbpTenant> newTenants = context.Tenants.Where(t => t.TenantType == "new-A") 

           new TenantRoleAndUserBuilder(context, 74, "A").Create();

            //Host seed
            new InitialHostDbBuilder(context).Create();

            //Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();
            new TenantRoleAndUserBuilder(context, 1, null).Create();


            // Create the default Config data for EMS/AEL
            new DefaultConfigCreator(context).Create();

            // Create Test Data - TENANTS, ROLES and USERS
            if (createTestData)
            {
                new TestDataCreator(context).CreateTenants();
                var tenants = context.Tenants.Where(t => t.TenantType != null).ToListAsync().Result;
                foreach (var tenant in tenants)
                {
                    if(!validTenantTypes.Contains(tenant.TenantType)) { throw new Exception($"'{tenant.TenantType}' is not a valid Tenant Type!"); }
                    new TenantRoleAndUserBuilder(context, tenant.Id, tenant.TenantType).CreateSeedRolesAndUsers();
                }
            }

            // Create AEL Data - TENANTS, ROLES and USERS
            if (createSeedData)
            {
                new SeedDataCreator(context).CreateTenants();
                var tenants = context.Tenants.Where(t => t.TenantType !=null).ToListAsync().Result;
                foreach (var tenant in tenants)
                {
                    if (!validTenantTypes.Contains(tenant.TenantType)) { throw new Exception($"'{tenant.TenantType}' is not a valid Tenant Type!"); }
                    new TenantRoleAndUserBuilder(context, tenant.Id, tenant.TenantType).CreateSeedRolesAndUsers();
                }
            }

            // Create Test Data - SUPPORT CONTRACTS, LEASE AGREEMENTS, WORKORDER, ETC...
            if (createTestData){new TestDataCreator(context).CreateData();}

            // Create AEL Data - SUPPORT CONTRACTS, LEASE AGREEMENTS, WORKORDER, ETC...
            if (createSeedData){new SeedDataCreator(context).CreateData();}
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}
