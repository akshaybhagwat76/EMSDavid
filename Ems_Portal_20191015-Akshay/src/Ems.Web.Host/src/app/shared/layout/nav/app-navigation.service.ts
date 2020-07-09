import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { AppSessionService } from '@shared/common/session/app-session.service';

import { Injectable } from '@angular/core';
import { AppMenu } from './app-menu';
import { AppMenuItem } from './app-menu-item';
import { AbpSessionService } from 'abp-ng2-module/dist/src/session/abp-session.service';

@Injectable()
export class AppNavigationService {

    constructor(
        private _permissionCheckerService: PermissionCheckerService,
        private _appSessionService: AppSessionService
    ) {

    }

    getMenu(): AppMenu {
        return new AppMenu('MainMenu', 'MainMenu', [
            new AppMenuItem('Dashboard', 'Pages.Administration.Host.Dashboard', 'flaticon-line-graph', '/app/admin/hostDashboard'),
            new AppMenuItem('Dashboard', 'Pages.Main.Dashboard.Finance', 'flaticon-line-graph', '/app/main/dashboard'),
            new AppMenuItem('Organization', '', 'flaticon-map', '', [
                new AppMenuItem('Profile', 'Pages.Main.Customers.Profile', 'flaticon-profile-1', '/app/admin/organizations/customers/viewCustomer'),
                new AppMenuItem('Profile', 'Pages.Main.Vendors.Profile', 'flaticon-profile-1', '/app/admin/organizations/vendors/viewVendor'),
                new AppMenuItem('Profile', 'Pages.Main.AssetOwners.Profile', 'flaticon-profile-1', '/app/admin/organizations/assetOwners/viewAssetOwner'),
                new AppMenuItem('Asset Owners', 'Pages.Main.AssetOwners.Create', 'flaticon-truck', '/app/admin/organizations/assetOwners'),
                new AppMenuItem('Contacts', 'Pages.Main.Contacts', 'flaticon-network', '/app/admin/organizations/contacts'),
                new AppMenuItem('CustomerGroups', 'Pages.Main.CustomerGroups', 'flaticon-shapes', '/app/admin/organizations/customerGroups'),
                //new AppMenuItem('CustomerGroupMemberships', 'Pages.Main.CustomerGroupMemberships', 'flaticon-more', '/app/admin/organizations/customerGroupMemberships'),
                new AppMenuItem('Customers', 'Pages.Main.Customers.Create', '	flaticon-car', '/app/admin/organizations/customers'),
                new AppMenuItem('Structure', 'Pages.Administration.OrganizationUnits', 'flaticon-map', '/app/admin/organization-units'),
                new AppMenuItem('Users', 'Pages.Administration.Users', 'flaticon-users', '/app/admin/users'),
                new AppMenuItem('Vendors', 'Pages.Main.Vendors.Create', 'flaticon-user', '/app/admin/organizations/vendors')
            ]),

                    new AppMenuItem('Assets', '', 'flaticon-truck', '', [
                        new AppMenuItem('Assets', 'Pages.Main.Assets', 'flaticon-interface-4', '/app/main/assets/assets'),
                        //new AppMenuItem('AssetParts', 'Pages.Main.AssetParts', 'flaticon-more', '/app/main/assets/assetParts'),
                        new AppMenuItem('Inventory', 'Pages.InventoryItems', 'flaticon-more', '/app/main/assets/inventoryItems'),
                        new AppMenuItem('UsageMetrics', 'Pages.Main.UsageMetrics', 'flaticon-medical', '/app/main/telematics/usageMetrics'),
                    ]),

            new AppMenuItem('Support', '', 'flaticon-alert-2', '', [
                new AppMenuItem('Incidents', 'Pages.Main.Incidents', 'flaticon-exclamation-square', '/app/main/support/incidents'),
                new AppMenuItem('WorkOrders', 'Pages.Main.WorkOrders', 'flaticon-clipboard', '/app/main/support/workOrders'),
                new AppMenuItem('Vendor Quotations', 'Pages.Main.Quotations', 'flaticon-speech-bubble-1', '/app/main/support/quotations'),
                new AppMenuItem('Customer Estimates', 'Pages.Main.Estimates', 'flaticon-edit-1', '/app/main/support/estimates'),
                new AppMenuItem('VendorCharges', 'Pages.Main.VendorCharges', 'flaticon-web', '/app/main/vendors/vendorCharges'),
                new AppMenuItem('RFQs', 'Pages.Main.Rfqs', 'flaticon-paper-plane', '/app/main/quotations/rfqs'),
                new AppMenuItem('SupportContracts', 'Pages.Main.SupportContracts', 'flaticon-folder', '/app/main/support/supportContracts'),
                new AppMenuItem('SupportItems', 'Pages.Main.SupportItems', 'flaticon-folder', '/app/main/support/supportItems'),
            ]),
            
            new AppMenuItem('Finance', '', 'flaticon-graph', '', [
                //new AppMenuItem('AssetOwnerships', 'Pages.Main.AssetOwnerships', 'flaticon-more', '/app/admin/assets/assetOwnerships'),
                //new AppMenuItem('BillingEventDetails', 'Pages.Main.BillingEvents', 'flaticon-more', '/app/admin/billing/billingEventDetails'),
                new AppMenuItem('BillingEvents', 'Pages.Main.BillingEvents', 'flaticon-share', '/app/main/billing/billingEvents'),
                new AppMenuItem('BillingRules', 'Pages.Main.BillingRules', 'flaticon-signs-1', '/app/main/billing/billingRules'),
                new AppMenuItem('CustomerInvoices', 'Pages.Main.CustomerInvoices', 'flaticon-coins', '/app/main/billing/customerInvoices'),
                //new AppMenuItem('CustomerInvoiceDetails', 'Pages.Main.CustomerInvoices', 'flaticon-more', '/app/main/billing/customerInvoiceDetails'),
                //new AppMenuItem('LeaseItems', 'Pages.Main.LeaseItems', 'flaticon-more', '/app/admin/assets/leaseItems'),
                new AppMenuItem('LeaseAgreements', 'Pages.Main.LeaseAgreements', 'flaticon-tool', '/app/main/assets/leaseAgreements'),
                new AppMenuItem('Receivables', 'Pages.AgedReceivablesPeriods', 'flaticon-time', '/app/main/finance/agedReceivablesPeriods'),
                new AppMenuItem('VendorCharges', 'Pages.Main.VendorCharges', 'flaticon-coins', '/app/main/vendors/vendorCharges')
            ]),


           
            new AppMenuItem('Administration', '', 'flaticon-interface-8', '', [
                new AppMenuItem('Attachments', 'Pages.Main.Attachments.Admin', 'flaticon-attachment', '/app/main/storage/attachments'),
                new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', 'flaticon-folder-1', '/app/admin/auditLogs'),
                new AppMenuItem('CrossTenantPermissions', 'Pages.Administration.CrossTenantPermissions', 'flaticon-user-ok', '/app/admin/authorization/crossTenantPermissions'),
                new AppMenuItem('Editions', 'Pages.Administration.Editions', 'flaticon-app', '/app/admin/editions'),
                new AppMenuItem('Languages', 'Pages.Configuration.Languages', 'flaticon-tabs', '/app/admin/languages'),
                new AppMenuItem('Maintenance', 'Pages.Administration.Host.Maintenance', 'flaticon-lock', '/app/admin/maintenance'),
                new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
                new AppMenuItem('Subscription', 'Pages.Administration.Tenant.SubscriptionManagement', 'flaticon-refresh', '/app/admin/subscription-management'),
                new AppMenuItem('Settings', 'Pages.Administration.Host.Settings', 'flaticon-settings', '/app/admin/hostSettings'),
                new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/admin/tenantSettings'),
                new AppMenuItem('Tenants', 'Pages.Administration.Tenants', 'flaticon-list-3', '/app/admin/tenants'),
                new AppMenuItem('VisualSettings', 'Pages.Administration.UiCustomization', 'flaticon-medical', '/app/admin/ui-customization'),
                new AppMenuItem('XeroInvoices', 'Pages.Administration.XeroInvoices', 'flaticon-coins', '/app/admin/billing/xeroInvoices'),
                new AppMenuItem('DemoUiComponents', 'Pages.DemoUiComponents', 'flaticon-shapes', '/app/admin/demo-ui-components')
                
            ]),
            new AppMenuItem('Configuration', '', 'flaticon-cogwheel', '', [
                new AppMenuItem('AssetClasses', 'Pages.Configuration.AssetClasses.Edit', 'flaticon-more', '/app/config/assets/assetClasses'),
                new AppMenuItem('AssetPartStatuses', 'Pages.Main.AssetPartStatuses', 'flaticon-more', '/app/main/assets/assetPartStatuses'),
                new AppMenuItem('AssetPartTypes', 'Pages.AssetPartTypes', 'flaticon-more', '/app/main/assets/assetPartTypes'),
                new AppMenuItem('AssetStatuses', 'Pages.Configuration.AssetStatuses.Edit', 'flaticon-more', '/app/config/assets/assetStatuses'),
                new AppMenuItem('AssetTypes', 'Pages.Configuration.AssetTypes.Edit', 'flaticon-more', '/app/config/assets/assetTypes'),
                new AppMenuItem('AssetTypes', 'Pages.Administration.AssetTypes.Edit', 'flaticon-more', '/app/config/assets/assetTypes'),
                new AppMenuItem('BillingEventTypes', 'Pages.Configuration.BillingEventTypes.Edit', 'flaticon-more', '/app/config/billing/billingEventTypes'),
                new AppMenuItem('BillingRuleTypes', 'Pages.Configuration.BillingRuleTypes.Edit', 'flaticon-more', '/app/config/billing/billingRuleTypes'),
                new AppMenuItem('ConsumableTypes', 'Pages.Configuration.ConsumableTypes.Edit', 'flaticon-more', '/app/config/support/consumableTypes'),
                new AppMenuItem('Currencies', 'Pages.Configuration.Currencies.Edit', 'flaticon-more', '/app/config/billing/currencies'),
                new AppMenuItem('CustomerInvoiceStatuses', 'Pages.Configuration.CustomerInvoiceStatuses.Edit', 'flaticon-more', '/app/config/billing/customerInvoiceStatuses'),
                new AppMenuItem('CustomerTypes', 'Pages.Configuration.CustomerTypes.Edit', 'flaticon-more', '/app/config/customers/customerTypes'),
                new AppMenuItem('EstimateStatuses', 'Pages.Configuration.EstimateStatuses.Edit', 'flaticon-more', '/app/main/support/estimateStatuses'),
                new AppMenuItem('IncidentTypes', 'Pages.Configuration.IncidentTypes.Edit', 'flaticon-more', '/app/config/support/incidentTypes'),
                new AppMenuItem('IncidentStatuses', 'Pages.Configuration.IncidentStatuses.Edit', 'flaticon-more', '/app/config/support/incidentStatuses'),
                new AppMenuItem('IncidentPriorities', 'Pages.Configuration.IncidentPriorities.Edit', 'flaticon-more', '/app/config/support/incidentPriorities'),
                new AppMenuItem('ItemTypes', 'Pages.Configuration.ItemTypes.Edit', 'flaticon-more', '/app/config/quotations/itemTypes'),
                new AppMenuItem('QuotationStatuses', 'Pages.Configuration.QuotationStatuses.Edit', 'flaticon-more', '/app/config/quotations/quotationStatuses'),
                new AppMenuItem('RfqTypes', 'Pages.Configuration.RfqTypes.Edit', 'flaticon-more', '/app/config/quotations/rfqTypes'),
                new AppMenuItem('SupportTypes', 'Pages.Configuration.SupportTypes.Edit', 'flaticon-more', '/app/config/support/supportTypes'),
                new AppMenuItem('SsicCodes', 'Pages.Configuration.SsicCodes.Edit', 'flaticon-more', '/app/config/organizations/ssicCodes'),
                new AppMenuItem('Uoms', 'Pages.Configuration.Uoms.Edit', 'flaticon-more', '/app/config/metrics/uoms'),
                new AppMenuItem('VendorChargeStatuses', 'Pages.Configuration.VendorChargeStatuses.Edit', 'flaticon-more', '/app/config/billing/vendorChargeStatuses'),
                new AppMenuItem('WorkOrderPriorities', 'Pages.Configuration.WorkOrderPriorities.Edit', 'flaticon-more', '/app/config/support/workOrderPriorities'),
                new AppMenuItem('WorkOrderStatuses', 'Pages.Configuration.WorkOrderStatuses.Edit', 'flaticon-more', '/app/config/support/workOrderStatuses'),
                new AppMenuItem('WorkOrderActions', 'Pages.Configuration.WorkOrderActions.Edit', 'flaticon-more', '/app/config/support/workOrderActions'),
                new AppMenuItem('WorkOrderTypes', 'Pages.Configuration.WorkOrderTypes.Edit', 'flaticon-more', '/app/config/support/workOrderTypes')
            ]),
        ]);
    }
    
    checkChildMenuItemPermission(menuItem): boolean {

        for (let i = 0; i < menuItem.items.length; i++) {
            let subMenuItem = menuItem.items[i];

            if (subMenuItem.permissionName === '' || subMenuItem.permissionName === null || subMenuItem.permissionName && this._permissionCheckerService.isGranted(subMenuItem.permissionName)) {
                return true;
            } else if (subMenuItem.items && subMenuItem.items.length) {
                return this.checkChildMenuItemPermission(subMenuItem);
            }
        }

        return false;
    }

    showMenuItem(menuItem: AppMenuItem): boolean {
        if (menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' && this._appSessionService.tenant && !this._appSessionService.tenant.edition) {
            return false;
        }

        let hideMenuItem = false;

        if (menuItem.requiresAuthentication && !this._appSessionService.user) {
            hideMenuItem = true;
        }

        if (menuItem.permissionName && !this._permissionCheckerService.isGranted(menuItem.permissionName)) {
            hideMenuItem = true;
        }

        if (this._appSessionService.tenant || !abp.multiTenancy.ignoreFeatureCheckForHostUsers) {
            if (menuItem.hasFeatureDependency() && !menuItem.featureDependencySatisfied()) {
                hideMenuItem = true;
            }
        }

        if (!hideMenuItem && menuItem.items && menuItem.items.length) {
            return this.checkChildMenuItemPermission(menuItem);
        }

        return !hideMenuItem;
    }
}
