import { NgModule } from '@angular/core';

import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { XeroInvoicesComponent } from './billing/xeroInvoices/xeroInvoices.component';
import { CrossTenantPermissionsComponent } from './authorization/crossTenantPermissions/crossTenantPermissions.component';
//import { QuotationsComponent } from './quotations/quotations/quotations.component';
//import { WorkOrdersComponent } from './support/workOrders/workOrders.component';
//import { VendorChargesComponent } from './vendors/vendorCharges/vendorCharges.component';

import { VendorsComponent } from './organizations/vendors/vendors.component';
import { ContactsComponent } from './organizations/contacts/contacts.component';
import { CustomersComponent } from './organizations/customers/customers.component';
import { CustomerGroupMembershipsComponent } from './organizations/customerGroupMemberships/customerGroupMemberships.component';
import { CustomerGroupsComponent } from './organizations/customerGroups/customerGroups.component';
import { AssetOwnersComponent } from './organizations/assetOwners/assetOwners.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { HostDashboardComponent } from './dashboard/host-dashboard.component';
import { DemoUiComponentsComponent } from './demo-ui-components/demo-ui-components.component';
import { EditionsComponent } from './editions/editions.component';
import { InstallComponent } from './install/install.component';
import { LanguageTextsComponent } from './languages/language-texts.component';
import { LanguagesComponent } from './languages/languages.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { OrganizationUnitsComponent } from './organization-units/organization-units.component';
import { RolesComponent } from './roles/roles.component';
import { HostSettingsComponent } from './settings/host-settings.component';
import { TenantSettingsComponent } from './settings/tenant-settings.component';
import { InvoiceComponent } from './subscription-management/invoice/invoice.component';
import { SubscriptionManagementComponent } from './subscription-management/subscription-management.component';
import { TenantsComponent } from './tenants/tenants.component';
import { UiCustomizationComponent } from './ui-customization/ui-customization.component';
import { UsersComponent } from './users/users.component';
import { ViewCustomerComponent } from './organizations/customers/viewCustomer.component';
import { ViewVendorComponent } from './organizations/vendors/viewVendor.component';
import { ViewAssetOwnerComponent } from './organizations/assetOwners/viewAssetOwner.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'billing/xeroInvoices', component: XeroInvoicesComponent, data: { permission: 'Pages.Administration.XeroInvoices' }  },
                    { path: 'authorization/crossTenantPermissions', component: CrossTenantPermissionsComponent, data: { permission: 'Pages.Administration.CrossTenantPermissions' }  },
                    //{ path: 'quotations/quotations', component: QuotationsComponent, data: { permission: 'Pages.Main.Quotations' }  },
                    //{ path: 'support/workOrders', component: WorkOrdersComponent, data: { permission: 'Pages.Administration.WorkOrders' }  },
                    //{ path: 'vendors/vendorCharges', component: VendorChargesComponent, data: { permission: 'Pages.Main.VendorCharges' }  },
                    { path: 'organizations/vendors/:vendorId', component: ViewVendorComponent, data: { permission: 'Pages.Main.Vendors.Profile' }  },
                    { path: 'organizations/vendors', component: VendorsComponent, data: { permission: 'Pages.Main.Vendors' }  },
                    { path: 'organizations/customerGroupMemberships', component: CustomerGroupMembershipsComponent, data: { permission: 'Pages.Main.CustomerGroupMemberships' }  },
                    { path: 'organizations/customerGroups', component: CustomerGroupsComponent, data: { permission: 'Pages.Main.CustomerGroups' }  },
                    { path: 'organizations/assetOwners/:assetOwnerId', component: ViewAssetOwnerComponent, data: { permission: 'Pages.Main.AssetOwners.Profile' }  },
                    { path: 'organizations/assetOwners', component: AssetOwnersComponent, data: { permission: 'Pages.Main.AssetOwners' }  },
                    { path: 'organizations/contacts', component: ContactsComponent, data: { permission: 'Pages.Main.Contacts' }  },
                    { path: 'organizations/customers/:customerId', component: ViewCustomerComponent, data: { permission: 'Pages.Main.Customers.Profile' }  },
                    { path: 'organizations/customers', component: CustomersComponent, data: { permission: 'Pages.Main.Customers' }  },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Administration.Users' } },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Administration.Roles' } },
                    { path: 'auditLogs', component: AuditLogsComponent, data: { permission: 'Pages.Administration.AuditLogs' } },
                    { path: 'maintenance', component: MaintenanceComponent, data: { permission: 'Pages.Administration.Host.Maintenance' } },
                    { path: 'hostSettings', component: HostSettingsComponent, data: { permission: 'Pages.Administration.Host.Settings' } },
                    { path: 'editions', component: EditionsComponent, data: { permission: 'Pages.Administration.Editions' } },
                    { path: 'languages', component: LanguagesComponent, data: { permission: 'Pages.Configuration.Languages' } },
                    { path: 'languages/:name/texts', component: LanguageTextsComponent, data: { permission: 'Pages.Configuration.Languages.ChangeTexts' } },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Administration.Tenants' } },
                    { path: 'organization-units', component: OrganizationUnitsComponent, data: { permission: 'Pages.Administration.OrganizationUnits' } },
                    { path: 'subscription-management', component: SubscriptionManagementComponent, data: { permission: 'Pages.Administration.Tenant.SubscriptionManagement' } },
                    { path: 'invoice/:paymentId', component: InvoiceComponent, data: { permission: 'Pages.Administration.Tenant.SubscriptionManagement' } },
                    { path: 'tenantSettings', component: TenantSettingsComponent, data: { permission: 'Pages.Administration.Tenant.Settings' } },
                    { path: 'hostDashboard', component: HostDashboardComponent, data: { permission: 'Pages.Administration.Host.Dashboard' } },
                    { path: 'demo-ui-components', component: DemoUiComponentsComponent, data: { permission: 'Pages.DemoUiComponents' } },
                    { path: 'install', component: InstallComponent },
                    { path: 'ui-customization', component: UiCustomizationComponent }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AdminRoutingModule {

    constructor(
        private router: Router
    ) {
        router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                window.scroll(0, 0);
            }
        });
    }
}
