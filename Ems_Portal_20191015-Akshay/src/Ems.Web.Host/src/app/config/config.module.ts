import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';

import { VendorChargeStatusesComponent } from './billing/vendorChargeStatuses/vendorChargeStatuses.component';
import { ViewVendorChargeStatusModalComponent } from './billing/vendorChargeStatuses/view-vendorChargeStatus-modal.component';
import { CreateOrEditVendorChargeStatusModalComponent } from './billing/vendorChargeStatuses/create-or-edit-vendorChargeStatus-modal.component';

import { WorkOrderStatusesComponent } from './support/workOrderStatuses/workOrderStatuses.component';
import { ViewWorkOrderStatusModalComponent } from './support/workOrderStatuses/view-workOrderStatus-modal.component';
import { CreateOrEditWorkOrderStatusModalComponent } from './support/workOrderStatuses/create-or-edit-workOrderStatus-modal.component';

import { WorkOrderActionsComponent } from './support/workOrderActions/workOrderActions.component';
import { ViewWorkOrderActionModalComponent } from './support/workOrderActions/view-workOrderAction-modal.component';
import { CreateOrEditWorkOrderActionModalComponent } from './support/workOrderActions/create-or-edit-workOrderAction-modal.component';


import { CustomerInvoiceStatusesComponent } from './billing/customerInvoiceStatuses/customerInvoiceStatuses.component';
import { ViewCustomerInvoiceStatusModalComponent } from './billing/customerInvoiceStatuses/view-customerInvoiceStatus-modal.component';
import { CreateOrEditCustomerInvoiceStatusModalComponent } from './billing/customerInvoiceStatuses/create-or-edit-customerInvoiceStatus-modal.component';


import { CustomerTypesComponent } from './customers/customerTypes/customerTypes.component';
import { ViewCustomerTypeModalComponent } from './customers/customerTypes/view-customerType-modal.component';
import { CreateOrEditCustomerTypeModalComponent } from './customers/customerTypes/create-or-edit-customerType-modal.component';

import { AssetTypesComponent } from './assets/assetTypes/assetTypes.component';
import { ViewAssetTypeModalComponent } from './assets/assetTypes/view-assetType-modal.component';
import { CreateOrEditAssetTypeModalComponent } from './assets/assetTypes/create-or-edit-assetType-modal.component';

import { BillingEventTypesComponent } from './billing/billingEventTypes/billingEventTypes.component';
import { ViewBillingEventTypeModalComponent } from './billing/billingEventTypes/view-billingEventType-modal.component';
import { CreateOrEditBillingEventTypeModalComponent } from './billing/billingEventTypes/create-or-edit-billingEventType-modal.component';

import { AssetStatusesComponent } from './assets/assetStatuses/assetStatuses.component';
import { ViewAssetStatusModalComponent } from './assets/assetStatuses/view-assetStatus-modal.component';
import { CreateOrEditAssetStatusModalComponent } from './assets/assetStatuses/create-or-edit-assetStatus-modal.component';

import { ConsumableTypesComponent } from './support/consumableTypes/consumableTypes.component';
import { ViewConsumableTypeModalComponent } from './support/consumableTypes/view-consumableType-modal.component';
import { CreateOrEditConsumableTypeModalComponent } from './support/consumableTypes/create-or-edit-consumableType-modal.component';

import { AssetClassesComponent } from './assets/assetClasses/assetClasses.component';
import { ViewAssetClassModalComponent } from './assets/assetClasses/view-assetClass-modal.component';
import { CreateOrEditAssetClassModalComponent } from './assets/assetClasses/create-or-edit-assetClass-modal.component';
import { AssetClassAssetTypeLookupTableModalComponent } from './assets/assetClasses/assetClass-assetType-lookup-table-modal.component';

import { WorkOrderTypesComponent } from './support/workOrderTypes/workOrderTypes.component';
import { ViewWorkOrderTypeModalComponent } from './support/workOrderTypes/view-workOrderType-modal.component';
import { CreateOrEditWorkOrderTypeModalComponent } from './support/workOrderTypes/create-or-edit-workOrderType-modal.component';

import { WorkOrderPrioritiesComponent } from './support/workOrderPriorities/workOrderPriorities.component';
import { ViewWorkOrderPriorityModalComponent } from './support/workOrderPriorities/view-workOrderPriority-modal.component';
import { CreateOrEditWorkOrderPriorityModalComponent } from './support/workOrderPriorities/create-or-edit-workOrderPriority-modal.component';

import { RfqTypesComponent } from './quotations/rfqTypes/rfqTypes.component';
import { ViewRfqTypeModalComponent } from './quotations/rfqTypes/view-rfqType-modal.component';
import { CreateOrEditRfqTypeModalComponent } from './quotations/rfqTypes/create-or-edit-rfqType-modal.component';

import { IncidentTypesComponent } from './support/incidentTypes/incidentTypes.component';
import { ViewIncidentTypeModalComponent } from './support/incidentTypes/view-incidentType-modal.component';
import { CreateOrEditIncidentTypeModalComponent } from './support/incidentTypes/create-or-edit-incidentType-modal.component';

import { IncidentStatusesComponent } from './support/incidentStatuses/incidentStatuses.component';
import { ViewIncidentStatusModalComponent } from './support/incidentStatuses/view-incidentStatus-modal.component';
import { CreateOrEditIncidentStatusModalComponent } from './support/incidentStatuses/create-or-edit-incidentStatus-modal.component';

import { IncidentPrioritiesComponent } from './support/incidentPriorities/incidentPriorities.component';
import { ViewIncidentPriorityModalComponent } from './support/incidentPriorities/view-incidentPriority-modal.component';
import { CreateOrEditIncidentPriorityModalComponent } from './support/incidentPriorities/create-or-edit-incidentPriority-modal.component';

import { BillingRuleTypesComponent } from './billing/billingRuleTypes/billingRuleTypes.component';
import { ViewBillingRuleTypeModalComponent } from './billing/billingRuleTypes/view-billingRuleType-modal.component';
import { CreateOrEditBillingRuleTypeModalComponent } from './billing/billingRuleTypes/create-or-edit-billingRuleType-modal.component';

import { SupportTypesComponent } from './support/supportTypes/supportTypes.component';
import { ViewSupportTypeModalComponent } from './support/supportTypes/view-supportType-modal.component';
import { CreateOrEditSupportTypeModalComponent } from './support/supportTypes/create-or-edit-supportType-modal.component';

import { QuotationStatusesComponent } from './quotations/quotationStatuses/quotationStatuses.component';
import { ViewQuotationStatusModalComponent } from './quotations/quotationStatuses/view-quotationStatus-modal.component';
import { CreateOrEditQuotationStatusModalComponent } from './quotations/quotationStatuses/create-or-edit-quotationStatus-modal.component';

import { ItemTypesComponent } from './quotations/itemTypes/itemTypes.component';
import { ViewItemTypeModalComponent } from './quotations/itemTypes/view-itemType-modal.component';
import { CreateOrEditItemTypeModalComponent } from './quotations/itemTypes/create-or-edit-itemType-modal.component';
import { ItemTypeLookupTableModalComponent } from './quotations/itemTypes/itemType-lookup-table-modal.component';

import { CurrenciesComponent } from './billing/currencies/currencies.component';
import { ViewCurrencyModalComponent } from './billing/currencies/view-currency-modal.component';
import { CreateOrEditCurrencyModalComponent } from './billing/currencies/create-or-edit-currency-modal.component';

import { SsicCodesComponent } from './organizations/ssicCodes/ssicCodes.component';
import { ViewSsicCodeModalComponent } from './organizations/ssicCodes/view-ssicCode-modal.component';
import { CreateOrEditSsicCodeModalComponent } from './organizations/ssicCodes/create-or-edit-ssicCode-modal.component';

import { UomsComponent } from './metrics/uoms/uoms.component';
import { ViewUomModalComponent } from './metrics/uoms/view-uom-modal.component';
import { CreateOrEditUomModalComponent } from './metrics/uoms/create-or-edit-uom-modal.component';
import { UomLookupTableModalComponent } from './metrics/uoms/uom-lookup-table-modal.component';


import { UtilsModule } from '@shared/utils/utils.module';
//import { AddMemberModalComponent } from 'app/admin/organization-units/add-member-modal.component';
//import { AddRoleModalComponent } from 'app/admin/organization-units/add-role-modal.component';
//import { FileUploadModule } from 'ng2-file-upload';
import { ModalModule, PopoverModule, TabsModule, TooltipModule, BsDropdownModule } from 'ngx-bootstrap';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { EditorModule } from 'primeng/editor';
//import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { InputMaskModule } from 'primeng/inputmask';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
//import { TreeModule } from 'primeng/tree';
//import { DragDropModule } from 'primeng/dragdrop';
//import { TreeDragDropService } from 'primeng/api';
import { ContextMenuModule } from 'primeng/contextmenu'

import { ConfigRoutingModule } from './config-routing.module';

import { NgxChartsModule } from '@swimlane/ngx-charts';
//import { CountoModule } from 'angular2-counto';
import { TextMaskModule } from 'angular2-text-mask';
//import { ImageCropperModule } from 'ngx-image-cropper';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { DropdownModule } from 'primeng/dropdown';

// Metronic
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    // suppressScrollX: true
};

@NgModule({
    imports: [
        ConfigRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        //FileUploadModule,
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TooltipModule.forRoot(),
        PopoverModule.forRoot(),
        BsDropdownModule.forRoot(),
        BsDatepickerModule.forRoot(),
        UtilsModule,
        AppCommonModule,
        TableModule,
        //TreeModule,
        //DragDropModule,
        ContextMenuModule,
        PaginatorModule,
        //PrimeNgFileUploadModule,
        AutoCompleteModule,
        EditorModule,
        InputMaskModule,
        NgxChartsModule,
        //CountoModule,
        TextMaskModule,
        //ImageCropperModule,
        PerfectScrollbarModule,
        DropdownModule
    ],
    declarations: [
        VendorChargeStatusesComponent,
		ViewVendorChargeStatusModalComponent,		CreateOrEditVendorChargeStatusModalComponent,
        WorkOrderStatusesComponent,
        ViewWorkOrderStatusModalComponent, CreateOrEditWorkOrderStatusModalComponent,
        WorkOrderActionsComponent,
        ViewWorkOrderActionModalComponent, CreateOrEditWorkOrderActionModalComponent,
        CustomerInvoiceStatusesComponent,
		ViewCustomerInvoiceStatusModalComponent,		CreateOrEditCustomerInvoiceStatusModalComponent,
        ViewAssetTypeModalComponent, CreateOrEditAssetTypeModalComponent,
        AssetClassesComponent,
        ViewAssetClassModalComponent, CreateOrEditAssetClassModalComponent,
        AssetClassAssetTypeLookupTableModalComponent, AssetClassesComponent,
        ViewAssetClassModalComponent, CreateOrEditAssetClassModalComponent,
        BillingEventTypesComponent,
        ViewBillingEventTypeModalComponent, CreateOrEditBillingEventTypeModalComponent,
        AssetStatusesComponent,
        ViewAssetStatusModalComponent, CreateOrEditAssetStatusModalComponent,
        ConsumableTypesComponent,
        ViewConsumableTypeModalComponent, CreateOrEditConsumableTypeModalComponent,
        AssetTypesComponent,
		CustomerTypesComponent,
        ViewCustomerTypeModalComponent,		
        CreateOrEditCustomerTypeModalComponent,
        //ConfigRoutingModule,
  
		WorkOrderTypesComponent,
		ViewWorkOrderTypeModalComponent,		CreateOrEditWorkOrderTypeModalComponent,
		WorkOrderPrioritiesComponent,
		ViewWorkOrderPriorityModalComponent,		CreateOrEditWorkOrderPriorityModalComponent,

		
		RfqTypesComponent,
		ViewRfqTypeModalComponent,		CreateOrEditRfqTypeModalComponent,
		
		IncidentTypesComponent,
		ViewIncidentTypeModalComponent,		CreateOrEditIncidentTypeModalComponent,
		IncidentStatusesComponent,
		ViewIncidentStatusModalComponent,		CreateOrEditIncidentStatusModalComponent,
		IncidentPrioritiesComponent,
		ViewIncidentPriorityModalComponent,		CreateOrEditIncidentPriorityModalComponent,
		
		BillingRuleTypesComponent,
		ViewBillingRuleTypeModalComponent,		CreateOrEditBillingRuleTypeModalComponent,
		SupportTypesComponent,
		ViewSupportTypeModalComponent,		CreateOrEditSupportTypeModalComponent,
		QuotationStatusesComponent,
		ViewQuotationStatusModalComponent,		CreateOrEditQuotationStatusModalComponent,
		ItemTypesComponent,
        ViewItemTypeModalComponent, CreateOrEditItemTypeModalComponent, ItemTypeLookupTableModalComponent,
		
		CurrenciesComponent,
		ViewCurrencyModalComponent,		CreateOrEditCurrencyModalComponent,

		SsicCodesComponent,
		ViewSsicCodeModalComponent,		CreateOrEditSsicCodeModalComponent,
        UomsComponent,
        UomLookupTableModalComponent,
		ViewUomModalComponent,		CreateOrEditUomModalComponent,

		IncidentTypesComponent,
		ViewIncidentTypeModalComponent,		CreateOrEditIncidentTypeModalComponent,

        //AddMemberModalComponent,
        //AddRoleModalComponent,
        
    ],
    exports: [
        //AddMemberModalComponent,
        //AddRoleModalComponent
    ],
    providers: [
        //TreeDragDropService,
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG }
    ]
})
export class ConfigModule { }
