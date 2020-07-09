/// <reference path="organizations/customers/view-customer-modal.component.ts" />
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { XeroInvoicesComponent } from './billing/xeroInvoices/xeroInvoices.component';
import { ViewXeroInvoiceModalComponent } from './billing/xeroInvoices/view-xeroInvoice-modal.component';
import { CreateOrEditXeroInvoiceModalComponent } from './billing/xeroInvoices/create-or-edit-xeroInvoice-modal.component';
import { XeroInvoiceCustomerInvoiceLookupTableModalComponent } from './billing/xeroInvoices/xeroInvoice-customerInvoice-lookup-table-modal.component';

import { CrossTenantPermissionsComponent } from './authorization/crossTenantPermissions/crossTenantPermissions.component';
import { ViewCrossTenantPermissionModalComponent } from './authorization/crossTenantPermissions/view-crossTenantPermission-modal.component';
import { CreateOrEditCrossTenantPermissionModalComponent } from './authorization/crossTenantPermissions/create-or-edit-crossTenantPermission-modal.component';

//import { QuotationsComponent } from './quotations/quotations/quotations.component';
//import { ViewQuotationModalComponent } from './quotations/quotations/view-quotation-modal.component';
//import { CreateOrEditQuotationModalComponent } from './quotations/quotations/create-or-edit-quotation-modal.component';
//import { QuotationSupportContractLookupTableModalComponent } from './quotations/quotations/quotation-supportContract-lookup-table-modal.component';
//import { QuotationQuotationStatusLookupTableModalComponent } from './quotations/quotations/quotation-quotationStatus-lookup-table-modal.component';


import { AssetOwnersComponent } from './organizations/assetOwners/assetOwners.component';
import { ViewAssetOwnerComponent } from './organizations/assetOwners/viewAssetOwner.component';
import { CreateOrEditAssetOwnerModalComponent } from './organizations/assetOwners/create-or-edit-assetOwner-modal.component';
import { AssetOwnerCurrencyLookupTableModalComponent } from './organizations/assetOwners/assetOwner-currency-lookup-table-modal.component';
import { AssetOwnerSsicCodeLookupTableModalComponent } from './organizations/assetOwners/assetOwner-ssicCode-lookup-table-modal.component';
import { ViewAssetOwnerAddressModalComponent } from './organizations/assetOwners/view-assetOwner-address-modal.component';
import { CreateOrEditAssetOwnerAddressModalComponent } from './organizations/assetOwners/create-or-edit-assetOwner-address-modal.component';

import { VendorsComponent } from './organizations/vendors/vendors.component';
import { ViewVendorComponent } from './organizations/vendors/viewVendor.component';
import { ViewVendorAddressModalComponent } from './organizations/vendors/view-vendor-address-modal.component';
import { CreateOrEditVendorModalComponent } from './organizations/vendors/create-or-edit-vendor-modal.component';
import { CreateOrEditVendorAddressModalComponent } from './organizations/vendors/create-or-edit-vendor-address-modal.component';
import { VendorSsicCodeLookupTableModalComponent } from './organizations/vendors/vendor-ssicCode-lookup-table-modal.component';
import { VendorCurrencyLookupTableModalComponent } from './organizations/vendors/vendor-currency-lookup-table-modal.component';

import { CreateOrEditCustomerAddressModalComponent } from './organizations/customers/create-or-edit-customer-address-modal.component';
import { ViewCustomerAddressModalComponent } from './organizations/customers/view-customer-address-modal.component';

import { ViewCustomerComponent } from './organizations/customers/viewCustomer.component';
import { CustomersComponent } from './organizations/customers/customers.component';

import { ViewCustomerModalComponent } from './organizations/customers/view-customer-modal.component';

import { CreateOrEditCustomerModalComponent } from './organizations/customers/create-or-edit-customer-modal.component';

import { CustomerCustomerTypeLookupTableModalComponent } from './organizations/customers/customer-customerType-lookup-table-modal.component';
import { CustomerCurrencyLookupTableModalComponent } from './organizations/customers/customer-currency-lookup-table-modal.component';

import { ContactsComponent } from './organizations/contacts/contacts.component';
import { ViewContactModalComponent } from './organizations/contacts/view-contact-modal.component';
import { CreateOrEditContactModalComponent } from './organizations/contacts/create-or-edit-contact-modal.component';
import { ContactUserLookupTableModalComponent } from './organizations/contacts/contact-user-lookup-table-modal.component';
import { ContactVendorLookupTableModalComponent } from './organizations/contacts/contact-vendor-lookup-table-modal.component';
import { ContactAssetOwnerLookupTableModalComponent } from './organizations/contacts/contact-assetOwner-lookup-table-modal.component';
import { ContactCustomerLookupTableModalComponent } from './organizations/contacts/contact-customer-lookup-table-modal.component';

import { CustomerGroupMembershipsComponent } from './organizations/customerGroupMemberships/customerGroupMemberships.component';
import { ViewCustomerGroupMembershipModalComponent } from './organizations/customerGroupMemberships/view-customerGroupMembership-modal.component';
import { CreateOrEditCustomerGroupMembershipModalComponent } from './organizations/customerGroupMemberships/create-or-edit-customerGroupMembership-modal.component';
import { CustomerGroupMembershipCustomerGroupLookupTableModalComponent } from './organizations/customerGroupMemberships/customerGroupMembership-customerGroup-lookup-table-modal.component';
import { CustomerGroupMembershipCustomerLookupTableModalComponent } from './organizations/customerGroupMemberships/customerGroupMembership-customer-lookup-table-modal.component';

import { CustomerGroupsComponent } from './organizations/customerGroups/customerGroups.component';
import { ViewCustomerGroupModalComponent } from './organizations/customerGroups/view-customerGroup-modal.component';
import { CreateOrEditCustomerGroupModalComponent } from './organizations/customerGroups/create-or-edit-customerGroup-modal.component';

import { UtilsModule } from '@shared/utils/utils.module';
import { AddMemberModalComponent } from 'app/admin/organization-units/add-member-modal.component';
import { AddRoleModalComponent } from 'app/admin/organization-units/add-role-modal.component';
import { FileUploadModule } from 'ng2-file-upload';
import { ModalModule, PopoverModule, TabsModule, TooltipModule, BsDropdownModule } from 'ngx-bootstrap';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { EditorModule } from 'primeng/editor';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { InputMaskModule } from 'primeng/inputmask';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TreeModule } from 'primeng/tree';
import { DragDropModule } from 'primeng/dragdrop';
import { TreeDragDropService } from 'primeng/api'; 
import { ContextMenuModule } from 'primeng/contextmenu';
import { AdminRoutingModule } from './admin-routing.module';
import { AuditLogDetailModalComponent } from './audit-logs/audit-log-detail-modal.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { HostDashboardComponent } from './dashboard/host-dashboard.component';
import { DemoUiComponentsComponent } from './demo-ui-components/demo-ui-components.component';
import { DemoUiDateTimeComponent } from './demo-ui-components/demo-ui-date-time.component';
import { DemoUiEditorComponent } from './demo-ui-components/demo-ui-editor.component';
import { DemoUiFileUploadComponent } from './demo-ui-components/demo-ui-file-upload.component';
import { DemoUiInputMaskComponent } from './demo-ui-components/demo-ui-input-mask.component';
import { DemoUiSelectionComponent } from './demo-ui-components/demo-ui-selection.component';
import { CreateEditionModalComponent } from './editions/create-edition-modal.component';
import { EditEditionModalComponent } from './editions/edit-edition-modal.component';
import { MoveTenantsToAnotherEditionModalComponent } from './editions/move-tenants-to-another-edition-modal.component';
import { EditionsComponent } from './editions/editions.component';
import { InstallComponent } from './install/install.component';
import { CreateOrEditLanguageModalComponent } from './languages/create-or-edit-language-modal.component';
import { EditTextModalComponent } from './languages/edit-text-modal.component';
import { LanguageTextsComponent } from './languages/language-texts.component';
import { LanguagesComponent } from './languages/languages.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { CreateOrEditUnitModalComponent } from './organization-units/create-or-edit-unit-modal.component';
import { OrganizationTreeComponent } from './organization-units/organization-tree.component';
import { OrganizationUnitMembersComponent } from './organization-units/organization-unit-members.component';
import { OrganizationUnitRolesComponent } from './organization-units/organization-unit-roles.component';
import { OrganizationUnitsComponent } from './organization-units/organization-units.component';
import { CreateOrEditRoleModalComponent } from './roles/create-or-edit-role-modal.component';
import { RolesComponent } from './roles/roles.component';
import { HostSettingsComponent } from './settings/host-settings.component';
import { TenantSettingsComponent } from './settings/tenant-settings.component';
import { EditionComboComponent } from './shared/edition-combo.component';
import { FeatureTreeComponent } from './shared/feature-tree.component';
import { OrganizationUnitsTreeComponent } from './shared/organization-unit-tree.component';
import { PermissionComboComponent } from './shared/permission-combo.component';
import { PermissionTreeComponent } from './shared/permission-tree.component';
import { RoleComboComponent } from './shared/role-combo.component';
import { InvoiceComponent } from './subscription-management/invoice/invoice.component';
import { SubscriptionManagementComponent } from './subscription-management/subscription-management.component';
import { CreateTenantModalComponent } from './tenants/create-tenant-modal.component';
import { EditTenantModalComponent } from './tenants/edit-tenant-modal.component';
import { TenantFeaturesModalComponent } from './tenants/tenant-features-modal.component';
import { TenantsComponent } from './tenants/tenants.component';
import { UiCustomizationComponent } from './ui-customization/ui-customization.component';
import { DefaultThemeUiSettingsComponent } from './ui-customization/default-theme-ui-settings.component';
import { Theme2ThemeUiSettingsComponent } from './ui-customization/theme2-theme-ui-settings.component';
import { Theme3ThemeUiSettingsComponent } from './ui-customization/theme3-theme-ui-settings.component';
import { Theme4ThemeUiSettingsComponent } from './ui-customization/theme4-theme-ui-settings.component';
import { Theme5ThemeUiSettingsComponent } from './ui-customization/theme5-theme-ui-settings.component';
import { Theme6ThemeUiSettingsComponent } from './ui-customization/theme6-theme-ui-settings.component';
import { Theme7ThemeUiSettingsComponent } from './ui-customization/theme7-theme-ui-settings.component';
import { Theme8ThemeUiSettingsComponent } from './ui-customization/theme8-theme-ui-settings.component';
import { Theme9ThemeUiSettingsComponent } from './ui-customization/theme9-theme-ui-settings.component';
import { Theme10ThemeUiSettingsComponent } from './ui-customization/theme10-theme-ui-settings.component';
import { Theme11ThemeUiSettingsComponent } from './ui-customization/theme11-theme-ui-settings.component';
import { Theme12ThemeUiSettingsComponent } from './ui-customization/theme12-theme-ui-settings.component';
import { CreateOrEditUserModalComponent } from './users/create-or-edit-user-modal.component';
import { EditUserPermissionsModalComponent } from './users/edit-user-permissions-modal.component';
import { ImpersonationService } from './users/impersonation.service';
import { UsersComponent } from './users/users.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { CountoModule } from 'angular2-counto';
import { TextMaskModule } from 'angular2-text-mask';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { DropdownModule } from 'primeng/dropdown';

// Metronic
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { PermissionTreeModalComponent } from './shared/permission-tree-modal.component';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    // suppressScrollX: true
};

@NgModule({
    imports: [
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        FileUploadModule,
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TooltipModule.forRoot(),
        PopoverModule.forRoot(),
        BsDropdownModule.forRoot(),
        BsDatepickerModule.forRoot(),
        AdminRoutingModule,
        UtilsModule,
        AppCommonModule,
        TableModule,
        TreeModule,
        DragDropModule,
        ContextMenuModule,
        PaginatorModule,
        PrimeNgFileUploadModule,
        AutoCompleteModule,
        EditorModule,
        InputMaskModule,
        NgxChartsModule,
        CountoModule,
        TextMaskModule,
        ImageCropperModule,
        PerfectScrollbarModule,
        DropdownModule
    ],
    declarations: [
		XeroInvoicesComponent,
		ViewXeroInvoiceModalComponent,		CreateOrEditXeroInvoiceModalComponent,
    XeroInvoiceCustomerInvoiceLookupTableModalComponent,
		CrossTenantPermissionsComponent,
		ViewCrossTenantPermissionModalComponent,		CreateOrEditCrossTenantPermissionModalComponent,
		/*QuotationsComponent,
		ViewQuotationModalComponent,		CreateOrEditQuotationModalComponent,
    QuotationSupportContractLookupTableModalComponent,
    QuotationQuotationStatusLookupTableModalComponent,
*/
        VendorsComponent,
        ViewVendorComponent,		CreateOrEditVendorModalComponent, CreateOrEditVendorAddressModalComponent,
        VendorSsicCodeLookupTableModalComponent,ViewVendorAddressModalComponent,
        VendorCurrencyLookupTableModalComponent,
        CustomersComponent, ViewCustomerComponent,ViewCustomerAddressModalComponent, CreateOrEditCustomerAddressModalComponent,
        CreateOrEditCustomerModalComponent, ViewCustomerModalComponent,
        
        ContactsComponent, ViewAssetOwnerAddressModalComponent, CreateOrEditAssetOwnerAddressModalComponent,
        ViewContactModalComponent,		CreateOrEditContactModalComponent,
        ContactUserLookupTableModalComponent,
        ContactVendorLookupTableModalComponent,
        ContactAssetOwnerLookupTableModalComponent,
        ContactCustomerLookupTableModalComponent,

        AssetOwnersComponent,
        ViewAssetOwnerComponent,		CreateOrEditAssetOwnerModalComponent,
        AssetOwnerCurrencyLookupTableModalComponent,
        AssetOwnerSsicCodeLookupTableModalComponent,
        CustomersComponent,
        CreateOrEditCustomerModalComponent,
        CustomerCurrencyLookupTableModalComponent,

        CustomerGroupMembershipsComponent,
        ViewCustomerGroupMembershipModalComponent,		CreateOrEditCustomerGroupMembershipModalComponent,
        CustomerGroupMembershipCustomerGroupLookupTableModalComponent,
        CustomerGroupMembershipCustomerLookupTableModalComponent,
        CustomerGroupsComponent,
        ViewCustomerGroupModalComponent,		CreateOrEditCustomerGroupModalComponent,
        CustomersComponent,
        CreateOrEditCustomerModalComponent,
        CustomerCustomerTypeLookupTableModalComponent,
        UsersComponent,
        PermissionComboComponent,
        RoleComboComponent,
        CreateOrEditUserModalComponent,
        EditUserPermissionsModalComponent,
        PermissionTreeComponent,
        FeatureTreeComponent,
        OrganizationUnitsTreeComponent,
        RolesComponent,
        CreateOrEditRoleModalComponent,
        AuditLogsComponent,
        AuditLogDetailModalComponent,
        HostSettingsComponent,
        InstallComponent,
        MaintenanceComponent,
        EditionsComponent,
        CreateEditionModalComponent,
        EditEditionModalComponent,
        MoveTenantsToAnotherEditionModalComponent,
        LanguagesComponent,
        LanguageTextsComponent,
        CreateOrEditLanguageModalComponent,
        TenantsComponent,
        CreateTenantModalComponent,
        EditTenantModalComponent,
        TenantFeaturesModalComponent,
        CreateOrEditLanguageModalComponent,
        EditTextModalComponent,
        OrganizationUnitsComponent,
        OrganizationTreeComponent,
        OrganizationUnitMembersComponent,
        OrganizationUnitRolesComponent,
        CreateOrEditUnitModalComponent,
        TenantSettingsComponent,
        HostDashboardComponent,
        EditionComboComponent,
        InvoiceComponent,
        SubscriptionManagementComponent,
        AddMemberModalComponent,
        AddRoleModalComponent,
        DemoUiComponentsComponent,
        DemoUiDateTimeComponent,
        DemoUiSelectionComponent,
        DemoUiFileUploadComponent,
        DemoUiInputMaskComponent,
        DemoUiEditorComponent,
        UiCustomizationComponent,
        DefaultThemeUiSettingsComponent,
        Theme2ThemeUiSettingsComponent,
        Theme3ThemeUiSettingsComponent,
        Theme4ThemeUiSettingsComponent,
        Theme5ThemeUiSettingsComponent,
        Theme6ThemeUiSettingsComponent,
        Theme7ThemeUiSettingsComponent,
        Theme8ThemeUiSettingsComponent,
        Theme9ThemeUiSettingsComponent,
        Theme10ThemeUiSettingsComponent,
        Theme12ThemeUiSettingsComponent,
        Theme11ThemeUiSettingsComponent,
        PermissionTreeModalComponent
    ],
    exports: [
        AddMemberModalComponent,
        AddRoleModalComponent
    ],
    providers: [
        ImpersonationService,
        TreeDragDropService,
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG }
    ]
})
export class AdminModule { }
