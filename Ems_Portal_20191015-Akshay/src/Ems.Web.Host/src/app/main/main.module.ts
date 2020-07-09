// <reference path="billing/billingeventtypes/create-or-edit-billingeventtype-modal.component.ts" />
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { InventoryItemsComponent } from './assets/inventoryItems/inventoryItems.component';
import { ViewInventoryItemModalComponent } from './assets/inventoryItems/view-inventoryItem-modal.component';
import { CreateOrEditInventoryItemModalComponent } from './assets/inventoryItems/create-or-edit-inventoryItem-modal.component';
import { InventoryItemItemTypeLookupTableModalComponent } from './assets/inventoryItems/inventoryItem-itemType-lookup-table-modal.component';
import { InventoryItemAssetLookupTableModalComponent } from './assets/inventoryItems/inventoryItem-asset-lookup-table-modal.component';

import { TreeModule } from 'primeng/tree';
import { DragDropModule } from 'primeng/dragdrop';
import { TreeDragDropService } from 'primeng/api';
import { ContextMenuModule } from 'primeng/contextmenu';


import { AssetPartTreeComponent } from './assets/assetParts//assetPart-tree.component';
import { AssetPartsComponent } from './assets/assetParts/assetParts.component';
import { ViewAssetPartModalComponent } from './assets/assetParts/view-assetPart-modal.component';
import { CreateOrEditAssetPartModalComponent } from './assets/assetParts/create-or-edit-assetPart-modal.component';
import { AssetPartAssetPartStatusLookupTableModalComponent } from './assets/assetParts/assetPart-assetPartStatus-lookup-table-modal.component';
import { AssetPartUsageMetricLookupTableModalComponent } from './assets/assetParts/assetPart-usageMetric-lookup-table-modal.component';
import { AssetPartAttachmentLookupTableModalComponent } from './assets/assetParts/assetPart-attachment-lookup-table-modal.component';
import { AssetPartAssetLookupTableModalComponent } from './assets/assetParts/assetPart-asset-lookup-table-modal.component';
import { AssetPartItemTypeLookupTableModalComponent } from './assets/assetParts/assetPart-itemType-lookup-table-modal.component';
import { AssetPartAssetPartLookupTableModalComponent } from './assets/assetParts/assetPart-assetPart-lookup-table-modal.component';
import { AssetPartAssetPartTypeLookupTableModalComponent } from './assets/assetParts/assetPart-assetPartType-lookup-table-modal.component';

import { AssetPartStatusesComponent } from './assets/assetPartStatuses/assetPartStatuses.component';
import { ViewAssetPartStatusModalComponent } from './assets/assetPartStatuses/view-assetPartStatus-modal.component';
import { CreateOrEditAssetPartStatusModalComponent } from './assets/assetPartStatuses/create-or-edit-assetPartStatus-modal.component';


import { AssetPartTypesComponent } from './assets/assetPartTypes/assetPartTypes.component';
import { ViewAssetPartTypeModalComponent } from './assets/assetPartTypes/view-assetPartType-modal.component';
import { CreateOrEditAssetPartTypeModalComponent } from './assets/assetPartTypes/create-or-edit-assetPartType-modal.component';

import { AgedReceivablesPeriodsComponent } from './finance/agedReceivablesPeriods/agedReceivablesPeriods.component';
import { ViewAgedReceivablesPeriodModalComponent } from './finance/agedReceivablesPeriods/view-agedReceivablesPeriod-modal.component';
import { CreateOrEditAgedReceivablesPeriodModalComponent } from './finance/agedReceivablesPeriods/create-or-edit-agedReceivablesPeriod-modal.component';

//import { QuotationsComponent } from './support/quotations/quotations.component';
//import { ViewQuotationModalComponent } from './support/quotations/view-quotation-modal.component';
//import { CreateOrEditQuotationModalComponent } from './support/quotations/create-or-edit-quotation-modal.component';
import { QuotationWorkOrderLookupTableModalComponent } from './support/quotations/quotation-workOrder-lookup-table-modal.component';

//import { EstimatesComponent } from './support/estimates/estimates.component';
//import { ViewEstimateModalComponent } from './support/estimates/view-estimate-modal.component';
//import { CreateOrEditEstimateModalComponent } from './support/estimates/create-or-edit-estimate-modal.component';
import { EstimateEstimateStatusLookupTableModalComponent } from './support/estimates/estimate-estimateStatus-lookup-table-modal.component';


//import { AttachmentsComponent } from './storage/attachments/attachments.component';
//import { ViewAttachmentModalComponent } from './storage/attachments/view-attachment-modal.component';
//import { CreateOrEditAttachmentModalComponent } from './storage/attachments/create-or-edit-attachment-modal.component';
import { AttachmentCustomerInvoiceLookupTableModalComponent } from './storage/attachments/attachment-customerInvoice-lookup-table-modal.component';

import { CustomerInvoiceDetailsComponent } from './billing/customerInvoices/customerInvoiceDetails.component';
import { ViewCustomerInvoiceDetailModalComponent } from './billing/customerInvoices/view-customerInvoiceDetail-modal.component';
import { CreateOrEditCustomerInvoiceDetailModalComponent } from './billing/customerInvoices/create-or-edit-customerInvoiceDetail-modal.component';
import { CustomerInvoiceDetailCustomerInvoiceLookupTableModalComponent } from './billing/customerInvoices/customerInvoiceDetail-customerInvoice-lookup-table-modal.component';
import { CustomerInvoiceDetailLeaseItemLookupTableModalComponent } from './billing/customerInvoices/customerInvoiceDetail-leaseItem-lookup-table-modal.component';

import { CustomerInvoicesComponent } from './billing/customerInvoices/customerInvoices.component';
import { ViewCustomerInvoiceComponent } from './billing/customerInvoices/viewCustomerInvoice.component';
import { ViewCustomerInvoiceModalComponent } from './billing/customerInvoices/view-customerInvoice-modal.component';
import { CreateOrEditCustomerInvoiceModalComponent } from './billing/customerInvoices/create-or-edit-customerInvoice-modal.component';
import { CustomerInvoiceCustomerLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-customer-lookup-table-modal.component';
import { CustomerInvoiceCurrencyLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-currency-lookup-table-modal.component';
import { CustomerInvoiceBillingRuleLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-billingRule-lookup-table-modal.component';
import { CustomerInvoiceBillingEventLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-billingEvent-lookup-table-modal.component';
import { CustomerInvoiceInvoiceStatusLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-invoiceStatus-lookup-table-modal.component';
import { CustomerInvoiceWorkOrderLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-workOrder-lookup-table-modal.component';
import { CustomerInvoiceEstimateLookupTableModalComponent } from './billing/customerInvoices/customerInvoice-estimate-lookup-table-modal.component';

import { EstimatesComponent } from './support/estimates/estimates.component';
import { ViewEstimateComponent } from './support/estimates/viewEstimate.component';
import { ViewEstimateModalComponent } from './support/estimates/view-estimate-modal.component';
import { CreateOrEditEstimateModalComponent } from './support/estimates/create-or-edit-estimate-modal.component';
//import { EstimateSupportContractLookupTableModalComponent } from './support/estimates/estimate-supportContract-lookup-table-modal.component';
//import { EstimateQuotationStatusLookupTableModalComponent } from './support/estimates/estimate-quotationStatus-lookup-table-modal.component';
import { EstimateQuotationLookupTableModalComponent } from './support/estimates/estimate-quotation-lookup-table-modal.component';
//import { EstimateRfqLookupTableModalComponent } from './support/estimates/estimate-rfq-lookup-table-modal.component';
import { CreateOrEditEstimateDetailModalComponent } from './support/estimates/create-or-edit-estimateDetail-modal.component';
import { ViewEstimateDetailModalComponent } from './support/estimates/view-estimateDetail-modal.component';
import { EstimateWorkOrderLookupTableModalComponent } from './support/estimates/estimate-workOrder-lookup-table-modal.component';
import { EstimateCustomerLookupTableModalComponent } from './support/estimates/estimate-customer-lookup-table-modal.component';
import { EstimatePdfComponent } from './support/estimates/estimate-pdf.component';


import { EstimateStatusesComponent } from './support/estimateStatuses/estimateStatuses.component';
import { ViewEstimateStatusModalComponent } from './support/estimateStatuses/view-estimateStatus-modal.component';
import { CreateOrEditEstimateStatusModalComponent } from './support/estimateStatuses/create-or-edit-estimateStatus-modal.component';

import { AttachmentsComponent } from './storage/attachments/attachments.component';
import { ViewAttachmentModalComponent } from './storage/attachments/view-attachment-modal.component';
import { CreateOrEditAttachmentModalComponent } from './storage/attachments/create-or-edit-attachment-modal.component';
import { AttachmentAssetLookupTableModalComponent } from './storage/attachments/attachment-asset-lookup-table-modal.component';
import { AttachmentIncidentLookupTableModalComponent } from './storage/attachments/attachment-incident-lookup-table-modal.component';
import { AttachmentLeaseAgreementLookupTableModalComponent } from './storage/attachments/attachment-leaseAgreement-lookup-table-modal.component';
import { AttachmentQuotationLookupTableModalComponent } from './storage/attachments/attachment-quotation-lookup-table-modal.component';
import { AttachmentSupportContractLookupTableModalComponent } from './storage/attachments/attachment-supportContract-lookup-table-modal.component';
import { AttachmentWorkOrderLookupTableModalComponent } from './storage/attachments/attachment-workOrder-lookup-table-modal.component';

import { FileUploadModule } from 'ng2-file-upload';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';

//import { WorkOrderUpdatesComponent } from './support/workOrders/workOrderUpdates.component';
//import { ViewWorkOrderUpdateModalComponent } from './support/workOrders/view-workOrderUpdate-modal.component';
//import { CreateOrEditWorkOrderUpdateModalComponent } from './support/workOrders/create-or-edit-workOrderUpdate-modal.component';
//import { WorkOrderUpdateWorkOrderLookupTableModalComponent } from './support/workOrders/workOrderUpdate-workOrder-lookup-table-modal.component';

import { SupportContractsComponent } from './support/supportContracts/supportContracts.component';
import { ViewSupportContractComponent } from './support/supportContracts/viewSupportContract.component';
import { ViewSupportContractModalComponent } from './support/supportContracts/view-supportContract-modal.component';
import { CreateOrEditSupportContractModalComponent } from './support/supportContracts/create-or-edit-supportContract-modal.component';
import { SupportContractVendorLookupTableModalComponent } from './support/supportContracts/supportContract-vendor-lookup-table-modal.component';
import { SupportContractAssetOwnerLookupTableModalComponent } from './support/supportContracts/supportContract-assetOwner-lookup-table-modal.component';

import { AutoCompleteModule } from 'primeng/autocomplete';
import { PaginatorModule } from 'primeng/paginator';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask';
//import { FileUploadModule } from 'primeng/fileupload';
import { TableModule } from 'primeng/table';

import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { ModalModule, TabsModule, TooltipModule, BsDropdownModule, PopoverModule } from 'ngx-bootstrap';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MainRoutingModule } from './main-routing.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';

//import { QuotationRfqLookupTableModalComponent } from './support/quotations/quotation-rfq-lookup-table-modal.component';

import { RfqsComponent } from './quotations/rfqs/rfqs.component';
import { ViewRfqModalComponent } from './quotations/rfqs/view-rfq-modal.component';
import { CreateOrEditRfqModalComponent } from './quotations/rfqs/create-or-edit-rfq-modal.component';
import { RfqRfqTypeLookupTableModalComponent } from './quotations/rfqs/rfq-rfqType-lookup-table-modal.component';
import { RfqAssetOwnerLookupTableModalComponent } from './quotations/rfqs/rfq-assetOwner-lookup-table-modal.component';
import { RfqCustomerLookupTableModalComponent } from './quotations/rfqs/rfq-customer-lookup-table-modal.component';
import { RfqAssetClassLookupTableModalComponent } from './quotations/rfqs/rfq-assetClass-lookup-table-modal.component';
import { RfqIncidentLookupTableModalComponent } from './quotations/rfqs/rfq-incident-lookup-table-modal.component';
import { RfqVendorLookupTableModalComponent } from './quotations/rfqs/rfq-vendor-lookup-table-modal.component';
import { RfqUserLookupTableModalComponent } from './quotations/rfqs/rfq-user-lookup-table-modal.component';
import { ViewIncidentUpdateModalComponent } from './support/incidents/view-incidentUpdate-modal.component';
import { CreateOrEditIncidentUpdateModalComponent } from './support/incidents/create-or-edit-incidentUpdate-modal.component';
import { IncidentUpdateIncidentLookupTableModalComponent } from './support/incidents/incidentUpdate-incident-lookup-table-modal.component';
import { IncidentUpdateUserLookupTableModalComponent } from './support/incidents/incidentUpdate-user-lookup-table-modal.component';

import { IncidentsComponent } from './support/incidents/incidents.component';
import { ViewIncidentComponent } from './support/incidents/viewIncident.component';
import { ViewIncidentModalComponent } from './support/incidents/view-incident-modal.component';
import { CreateOrEditIncidentModalComponent } from './support/incidents/create-or-edit-incident-modal.component';
import { IncidentIncidentPriorityLookupTableModalComponent } from './support/incidents/incident-incidentPriority-lookup-table-modal.component';
import { IncidentIncidentStatusLookupTableModalComponent } from './support/incidents/incident-incidentStatus-lookup-table-modal.component';
import { IncidentCustomerLookupTableModalComponent } from './support/incidents/incident-customer-lookup-table-modal.component';
import { IncidentAssetLookupTableModalComponent } from './support/incidents/incident-asset-lookup-table-modal.component';
import { IncidentSupportItemLookupTableModalComponent } from './support/incidents/incident-supportItem-lookup-table-modal.component';
import { IncidentIncidentTypeLookupTableModalComponent } from './support/incidents/incident-incidentType-lookup-table-modal.component';
import { IncidentUserLookupTableModalComponent } from './support/incidents/incident-user-lookup-table-modal.component';
import { IncidentLocationLookupTableModalComponent } from './support/incidents/incident-location-lookup-table-modal.component';

import { ViewSupportItemModalComponent } from './support/supportContracts/view-supportItem-modal.component';
import { CreateOrEditSupportItemModalComponent } from './support/supportContracts/create-or-edit-supportItem-modal.component';
import { SupportItemAssetLookupTableModalComponent } from './support/supportContracts/supportItem-asset-lookup-table-modal.component';
import { SupportItemAssetClassLookupTableModalComponent } from './support/supportContracts/supportItem-assetClass-lookup-table-modal.component';
import { SupportItemSupportContractLookupTableModalComponent } from './support/supportContracts/supportItem-supportContract-lookup-table-modal.component';
import { SupportItemConsumableTypeLookupTableModalComponent } from './support/supportContracts/supportItem-consumableType-lookup-table-modal.component';
import { SupportItemSupportTypeLookupTableModalComponent } from './support/supportContracts/supportItem-supportType-lookup-table-modal.component';

import { BillingEventDetailsComponent } from './billing/billingEventDetails/billingEventDetails.component';
import { ViewBillingEventDetailModalComponent } from './billing/billingEventDetails/view-billingEventDetail-modal.component';
import { CreateOrEditBillingEventDetailModalComponent } from './billing/billingEventDetails/create-or-edit-billingEventDetail-modal.component';
import { BillingEventDetailBillingRuleLookupTableModalComponent } from './billing/billingEventDetails/billingEventDetail-billingRule-lookup-table-modal.component';
import { BillingEventDetailLeaseItemLookupTableModalComponent } from './billing/billingEventDetails/billingEventDetail-leaseItem-lookup-table-modal.component';
import { BillingEventDetailBillingEventLookupTableModalComponent } from './billing/billingEventDetails/billingEventDetail-billingEvent-lookup-table-modal.component';

import { BillingEventsComponent } from './billing/billingEvents/billingEvents.component';
import { ViewBillingEventModalComponent } from './billing/billingEvents/view-billingEvent-modal.component';
import { CreateOrEditBillingEventModalComponent } from './billing/billingEvents/create-or-edit-billingEvent-modal.component';
import { BillingEventLeaseAgreementLookupTableModalComponent } from './billing/billingEvents/billingEvent-leaseAgreement-lookup-table-modal.component';
import { BillingEventVendorChargeLookupTableModalComponent } from './billing/billingEvents/billingEvent-vendorCharge-lookup-table-modal.component';
import { BillingEventBillingEventTypeLookupTableModalComponent } from './billing/billingEvents/billingEvent-billingEventType-lookup-table-modal.component';

import { BillingRulesComponent } from './billing/billingRules/billingRules.component';
import { ViewBillingRuleModalComponent } from './billing/billingRules/view-billingRule-modal.component';
import { CreateOrEditBillingRuleModalComponent } from './billing/billingRules/create-or-edit-billingRule-modal.component';
import { BillingRuleLeaseAgreementLookupTableModalComponent } from './billing/billingRules/billingRule-leaseAgreement-lookup-table-modal.component';
import { BillingRuleVendorLookupTableModalComponent } from './billing/billingRules/billingRule-vendor-lookup-table-modal.component';
import { BillingRuleLeaseItemLookupTableModalComponent } from './billing/billingRules/billingRule-leaseItem-lookup-table-modal.component';
import { BillingRuleCurrencyLookupTableModalComponent } from './billing/billingRules/billingRule-currency-lookup-table-modal.component';
import { BillingRuleBillingRuleTypeLookupTableModalComponent } from './billing/billingRules/billingRule-billingRuleType-lookup-table-modal.component';
import { BillingRuleUsageMetricLookupTableModalComponent } from './billing/billingRules/billingRule-usageMetric-lookup-table-modal.component';

//import { BillingEventTypesComponent } from './billing/billingEventTypes/billingEventTypes.component';
//import { CreateOrEditBillingEventTypeModalComponent } from './billing/billingEventTypes/create-or-edit-billingEventType-modal.component'
//import { ViewBillingEventTypeModalComponent } from './billing/billingEventTypes/view-billingEventType-modal.component'
import { UsageMetricRecordsComponent } from './telematics/usageMetricRecords/usageMetricRecords.component';
import { ViewUsageMetricRecordModalComponent } from './telematics/usageMetricRecords/view-usageMetricRecord-modal.component';
import { CreateOrEditUsageMetricRecordModalComponent } from './telematics/usageMetricRecords/create-or-edit-usageMetricRecord-modal.component';
import { UsageMetricRecordUsageMetricLookupTableModalComponent } from './telematics/usageMetricRecords/usageMetricRecord-usageMetric-lookup-table-modal.component';

import { UsageMetricsComponent } from './telematics/usageMetrics/usageMetrics.component';
import { ViewUsageMetricModalComponent } from './telematics/usageMetrics/view-usageMetric-modal.component';
import { CreateOrEditUsageMetricModalComponent } from './telematics/usageMetrics/create-or-edit-usageMetric-modal.component';
import { UsageMetricLeaseItemLookupTableModalComponent } from './telematics/usageMetrics/usageMetric-leaseItem-lookup-table-modal.component';
import { UsageMetricAssetLookupTableModalComponent } from './telematics/usageMetrics/usageMetric-asset-lookup-table-modal.component';

//import { LeaseItemsComponent } from './assets/leaseAgreements/leaseItems.component';
import { ViewLeaseItemModalComponent } from './assets/leaseAgreements/view-leaseItem-modal.component';

import { AssetOwnershipsComponent } from './assets/assetOwnerships/assetOwnerships.component';
import { ViewAssetOwnershipModalComponent } from './assets/assetOwnerships/view-assetOwnership-modal.component';
import { CreateOrEditAssetOwnershipModalComponent } from './assets/assetOwnerships/create-or-edit-assetOwnership-modal.component';
import { AssetOwnershipAssetLookupTableModalComponent } from './assets/assetOwnerships/assetOwnership-asset-lookup-table-modal.component';
import { AssetOwnershipAssetOwnerLookupTableModalComponent } from './assets/assetOwnerships/assetOwnership-assetOwner-lookup-table-modal.component';

import { AssetsComponent } from './assets/assets/assets.component';
import { ViewAssetModalComponent } from './assets/assets/view-asset-modal.component';
import { CreateOrEditAssetModalComponent } from './assets/assets/create-or-edit-asset-modal.component';
import { AssetAssetClassLookupTableModalComponent } from './assets/assets/asset-assetClass-lookup-table-modal.component';
import { AssetAssetStatusLookupTableModalComponent } from './assets/assets/asset-assetStatus-lookup-table-modal.component';
import { AssetLocationLookupTableModalComponent } from './assets/assets/asset-location-lookup-table-modal.component';
import { ViewAssetComponent } from './assets/assets/viewAsset.component';
import { CreateOrEditAssetNotesModalComponent } from './assets/assets/create-or-edit-assetNotes-modal.component';
import { ViewAssetNotesModalComponent } from './assets/assets/view-assetNotes-modal.component';

import { LeaseAgreementsComponent } from './assets/leaseAgreements/leaseAgreements.component';
import { ViewLeaseAgreementComponent } from './assets/leaseAgreements/viewLeaseAgreement.component';
import { ViewLeaseAgreementModalComponent } from './assets/leaseAgreements/view-leaseAgreement-modal.component';
import { CreateOrEditLeaseAgreementModalComponent } from './assets/leaseAgreements/create-or-edit-leaseAgreement-modal.component';
import { LeaseAgreementContactLookupTableModalComponent } from './assets/leaseAgreements/leaseAgreement-contact-lookup-table-modal.component';
import { LeaseAgreementAssetOwnerLookupTableModalComponent } from './assets/leaseAgreements/leaseAgreement-assetOwner-lookup-table-modal.component';
import { LeaseAgreementCustomerLookupTableModalComponent } from './assets/leaseAgreements/leaseAgreement-customer-lookup-table-modal.component';
import { CreateOrEditLeaseItemModalComponent } from './assets/leaseAgreements/create-or-edit-leaseItem-modal.component';
import { LeaseItemAssetClassLookupTableModalComponent } from './assets/leaseAgreements/leaseItem-assetClass-lookup-table-modal.component';
import { LeaseItemAssetLookupTableModalComponent } from './assets/leaseAgreements/leaseItem-asset-lookup-table-modal.component';
import { LeaseItemDepositUomLookupTableModalComponent } from './assets/leaseAgreements/leaseItem-deposit-uom-lookup-table-modal.component';
import { LeaseItemRentalUomLookupTableModalComponent } from './assets/leaseAgreements/leaseItem-rental-uom-lookup-table-modal.component';
import { LeaseItemLeaseAgreementLookupTableModalComponent } from './assets/leaseAgreements/leaseItem-leaseAgreement-lookup-table-modal.component';

import { QuotationsComponent } from './support/quotations/quotations.component';
import { ViewQuotationComponent } from './support/quotations/viewQuotation.component';
import { CreateQuotationWithDetailModalComponent } from './support/quotations/create-quotationWithDetail-modal.component';
import { CreateOrEditQuotationModalComponent } from './support/quotations/create-or-edit-quotation-modal.component';
import { QuotationSupportContractLookupTableModalComponent } from './support/quotations/quotation-supportContract-lookup-table-modal.component';
import { QuotationQuotationStatusLookupTableModalComponent } from './support/quotations/quotation-quotationStatus-lookup-table-modal.component';
import { QuotationPdfComponent } from './support/quotations/quotation-pdf.component';

import { VendorChargeDetailsComponent } from './vendors/vendorChargeDetails/vendorChargeDetails.component';
import { ViewVendorChargeDetailModalComponent } from './vendors/vendorChargeDetails/view-vendorChargeDetail-modal.component';
import { CreateOrEditVendorChargeDetailModalComponent } from './vendors/vendorChargeDetails/create-or-edit-vendorChargeDetail-modal.component';

import { ViewQuotationDetailModalComponent } from './support/quotations/view-quotationDetail-modal.component';
import { CreateOrEditQuotationDetailModalComponent } from './support/quotations/create-or-edit-quotationDetail-modal.component';
import { QuotationAssetLookupTableModalComponent } from './support/quotations/quotation-asset-lookup-table-modal.component';
import { QuotationAssetClassLookupTableModalComponent } from './support/quotations/quotation-assetClass-lookup-table-modal.component';
import { QuotationSupportTypeLookupTableModalComponent } from './support/quotations/quotation-supportType-lookup-table-modal.component';
import { QuotationSupportItemLookupTableModalComponent } from './support/quotations/quotation-supportItem-lookup-table-modal.component';
import { QuotationDetailWorkOrderLookupTableModalComponent } from './support/quotations/quotationDetail-workOrder-lookup-table-modal.component';

import { VendorChargesComponent } from './vendors/vendorCharges/vendorCharges.component';
import { ViewVendorChargeModalComponent } from './vendors/vendorCharges/view-vendorCharge-modal.component';
import { CreateOrEditVendorChargeModalComponent } from './vendors/vendorCharges/create-or-edit-vendorCharge-modal.component';
import { VendorChargeWorkOrderLookupTableModalComponent } from './vendors/vendorCharges/vendorCharge-workOrder-lookup-table-modal.component';
import { VendorChargeVendorLookupTableModalComponent } from './vendors/vendorCharges/vendorCharge-vendor-lookup-table-modal.component';
import { VendorChargeSupportContractLookupTableModalComponent } from './vendors/vendorCharges/vendorCharge-supportContract-lookup-table-modal.component';
import { VendorChargeVendorChargeStatusLookupTableModalComponent } from './vendors/vendorCharges/vendorCharge-vendorChargeStatus-lookup-table-modal.component';

import { WorkOrdersComponent } from './support/workOrders/workOrders.component';
import { ViewWorkOrderComponent } from './support/workOrders/viewWorkOrder.component';
import { CreateOrEditWorkOrderModalComponent } from './support/workOrders/create-or-edit-workOrder-modal.component';

import { WorkOrderWorkOrderPriorityLookupTableModalComponent } from './support/workOrders/workOrder-workOrderPriority-lookup-table-modal.component';
import { WorkOrderWorkOrderTypeLookupTableModalComponent } from './support/workOrders/workOrder-workOrderType-lookup-table-modal.component';
import { WorkOrderWorkOrderStatusLookupTableModalComponent } from './support/workOrders/workOrder-workOrderStatus-lookup-table-modal.component';
import { WorkOrderWorkOrderActionLookupTableModalComponent } from './support/workOrders/workOrder-workOrderAction-lookup-table-modal.component';
import { WorkOrderVendorLookupTableModalComponent } from './support/workOrders/workOrder-vendor-lookup-table-modal.component';
import { WorkOrderIncidentLookupTableModalComponent } from './support/workOrders/workOrder-incident-lookup-table-modal.component';
import { WorkOrderSupportItemLookupTableModalComponent } from './support/workOrders/workOrder-supportItem-lookup-table-modal.component';
//import { WorkOrderAssetOwnerLookupTableModalComponent } from './support/workOrders/workOrder-assetOwner-lookup-table-modal.component';
import { WorkOrderUserLookupTableModalComponent } from './support/workOrders/workOrder-user-lookup-table-modal.component';
import { WorkOrderCustomerLookupTableModalComponent } from './support/workOrders/workOrder-customer-lookup-table-modal.component';
import { WorkOrderAssetOwnershipLookupTableModalComponent } from './support/workOrders/workOrder-assetOwnership-lookup-table-modal.component';
import { WorkOrderLocationLookupTableModalComponent } from './support/workOrders/workOrder-location-lookup-table-modal.component';

import { DemoUiComponentsComponent } from './demo-ui-components/demo-ui-components.component';
import { DemoUiDateTimeComponent } from './demo-ui-components/demo-ui-date-time.component';
import { DemoUiEditorComponent } from './demo-ui-components/demo-ui-editor.component';
import { DemoUiFileUploadComponent } from './demo-ui-components/demo-ui-file-upload.component';
import { DemoUiInputMaskComponent } from './demo-ui-components/demo-ui-input-mask.component';
import { DemoUiSelectionComponent } from './demo-ui-components/demo-ui-selection.component';


import { ConsumableTypesComponent } from './support/consumableTypes/consumableTypes.component';
import { CreateOrEditConsumableTypeModalComponent } from './support/consumableTypes/create-or-edit-consumableType-modal.component';
import { ViewConsumableTypeModalComponent } from './support/consumableTypes/view-consumableType-modal.component';


import { ViewWorkOrderUpdateModalComponent } from './support/workOrders/view-workOrderUpdate-modal.component';
import { CreateOrEditWorkOrderUpdateModalComponent } from './support/workOrders/create-or-edit-workOrderUpdate-modal.component';

import { UomLookupTableModalComponent } from './metrics/uoms/uom-lookup-table-modal.component';
import { ItemTypeLookupTableModalComponent } from './quotations/itemTypes/itemType-lookup-table-modal.component';;
import { InvoicePDFComponent } from './billing/customerInvoices/invoice-pdf.component';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
    imports: [
        FileUploadModule,
        PrimeNgFileUploadModule,
        AutoCompleteModule,
        PaginatorModule,
        EditorModule,
        InputMaskModule, TableModule,
        TreeModule,
        DragDropModule,
        ContextMenuModule,
        CommonModule,
        FormsModule,
        ModalModule,
        TabsModule,
        TooltipModule,
        AppCommonModule,
        UtilsModule,
        MainRoutingModule,
        CountoModule,
        NgxChartsModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot()
    ],
    declarations: [
        AssetPartTreeComponent,
        InventoryItemsComponent,
        ViewInventoryItemModalComponent, CreateOrEditInventoryItemModalComponent,
        InventoryItemItemTypeLookupTableModalComponent,
        InventoryItemAssetLookupTableModalComponent,
        AssetPartsComponent,
        ViewAssetPartModalComponent, CreateOrEditAssetPartModalComponent,
        AssetPartAssetPartStatusLookupTableModalComponent,
        AssetPartUsageMetricLookupTableModalComponent,
        AssetPartAttachmentLookupTableModalComponent,
        AssetPartAssetLookupTableModalComponent,
        AssetPartAssetPartLookupTableModalComponent,
        AssetPartAssetPartTypeLookupTableModalComponent,
        AssetPartItemTypeLookupTableModalComponent,
        AssetPartStatusesComponent,
        AssetPartTypesComponent,
        ViewAssetPartStatusModalComponent, CreateOrEditAssetPartStatusModalComponent,

        ViewAssetPartTypeModalComponent, CreateOrEditAssetPartTypeModalComponent,
        AgedReceivablesPeriodsComponent,
        ViewAgedReceivablesPeriodModalComponent, CreateOrEditAgedReceivablesPeriodModalComponent,
        QuotationsComponent,
        //ViewQuotationModalComponent,		
        CreateOrEditQuotationModalComponent,
        QuotationWorkOrderLookupTableModalComponent,
        EstimatesComponent,
        ViewEstimateModalComponent, CreateOrEditEstimateModalComponent,
        EstimateEstimateStatusLookupTableModalComponent,

        AttachmentsComponent,
        ViewAttachmentModalComponent, CreateOrEditAttachmentModalComponent,
        AttachmentCustomerInvoiceLookupTableModalComponent,

        CustomerInvoiceDetailsComponent,
        ViewCustomerInvoiceDetailModalComponent, CreateOrEditCustomerInvoiceDetailModalComponent,
        CustomerInvoiceDetailLeaseItemLookupTableModalComponent,
        CustomerInvoiceWorkOrderLookupTableModalComponent,
        CustomerInvoiceEstimateLookupTableModalComponent,
        CustomerInvoiceDetailCustomerInvoiceLookupTableModalComponent,
        CustomerInvoicesComponent, ViewCustomerInvoiceComponent,
        ViewCustomerInvoiceModalComponent, CreateOrEditCustomerInvoiceModalComponent,
        CustomerInvoiceCustomerLookupTableModalComponent,
        CustomerInvoiceCurrencyLookupTableModalComponent,
        CustomerInvoiceBillingRuleLookupTableModalComponent,
        CustomerInvoiceBillingEventLookupTableModalComponent,
        CustomerInvoiceInvoiceStatusLookupTableModalComponent,
        EstimatesComponent, ViewEstimateComponent,
        ViewEstimateModalComponent, CreateOrEditEstimateModalComponent,
        //EstimateSupportContractLookupTableModalComponent,
        //EstimateQuotationStatusLookupTableModalComponent,
        EstimateQuotationLookupTableModalComponent,
        //EstimateRfqLookupTableModalComponent,
        CreateOrEditEstimateDetailModalComponent,
        ViewEstimateDetailModalComponent,
        EstimateWorkOrderLookupTableModalComponent,
        EstimateCustomerLookupTableModalComponent,
        EstimatePdfComponent,
        EstimateStatusesComponent,
        ViewEstimateStatusModalComponent, CreateOrEditEstimateStatusModalComponent,
        AttachmentsComponent,
        ViewAttachmentModalComponent, CreateOrEditAttachmentModalComponent,
        AttachmentAssetLookupTableModalComponent,
        AttachmentIncidentLookupTableModalComponent,
        AttachmentLeaseAgreementLookupTableModalComponent,
        AttachmentQuotationLookupTableModalComponent,
        AttachmentSupportContractLookupTableModalComponent,
        AttachmentWorkOrderLookupTableModalComponent,
        //WorkOrderUpdatesComponent,

        ViewWorkOrderUpdateModalComponent, CreateOrEditWorkOrderUpdateModalComponent,
        //WorkOrderUpdateWorkOrderLookupTableModalComponent,
        CreateQuotationWithDetailModalComponent,
        SupportContractsComponent, ViewSupportContractComponent,
        ViewSupportContractModalComponent, CreateOrEditSupportContractModalComponent,
        SupportContractVendorLookupTableModalComponent,
        SupportContractAssetOwnerLookupTableModalComponent,

        DashboardComponent,

        //WorkOrderUpdatesComponent,


        //  CreateOrEditWorkOrderUpdateModalComponent,
        //WorkOrderUpdateWorkOrderLookupTableModalComponent,

        SupportContractsComponent,
        ViewSupportContractModalComponent,
        CreateOrEditSupportContractModalComponent,
        SupportContractVendorLookupTableModalComponent,
        SupportContractAssetOwnerLookupTableModalComponent,
        DashboardComponent,
        ViewWorkOrderComponent,
        WorkOrdersComponent,
        CreateOrEditWorkOrderModalComponent,
        ViewQuotationDetailModalComponent,
        CreateOrEditQuotationDetailModalComponent,
        QuotationDetailWorkOrderLookupTableModalComponent,
        QuotationAssetLookupTableModalComponent,
        QuotationAssetClassLookupTableModalComponent,
        QuotationSupportTypeLookupTableModalComponent,
        QuotationSupportItemLookupTableModalComponent,
        VendorChargesComponent,
        ViewVendorChargeModalComponent,
        CreateOrEditVendorChargeModalComponent,
        VendorChargeWorkOrderLookupTableModalComponent,
        VendorChargeVendorChargeStatusLookupTableModalComponent,
        WorkOrderWorkOrderPriorityLookupTableModalComponent,
        WorkOrderWorkOrderTypeLookupTableModalComponent,
        WorkOrderWorkOrderStatusLookupTableModalComponent,
        WorkOrderWorkOrderActionLookupTableModalComponent,
        WorkOrderVendorLookupTableModalComponent,
        WorkOrderIncidentLookupTableModalComponent,
        WorkOrderSupportItemLookupTableModalComponent,
        WorkOrderAssetOwnershipLookupTableModalComponent,
        WorkOrderLocationLookupTableModalComponent,
        WorkOrderUserLookupTableModalComponent,
        WorkOrderCustomerLookupTableModalComponent,

        QuotationsComponent,
        ViewQuotationComponent,
        QuotationPdfComponent,
        CreateOrEditQuotationModalComponent,
        //QuotationRfqLookupTableModalComponent,
        RfqsComponent,
        ViewRfqModalComponent,
        CreateOrEditRfqModalComponent,
        RfqRfqTypeLookupTableModalComponent,
        RfqAssetOwnerLookupTableModalComponent,
        RfqCustomerLookupTableModalComponent,
        RfqAssetClassLookupTableModalComponent,
        RfqIncidentLookupTableModalComponent,
        RfqVendorLookupTableModalComponent,
        RfqUserLookupTableModalComponent,
        ViewIncidentUpdateModalComponent,
        CreateOrEditIncidentUpdateModalComponent,
        IncidentUpdateIncidentLookupTableModalComponent,
        IncidentUpdateUserLookupTableModalComponent,
        IncidentsComponent,
        ViewIncidentComponent,
        ViewIncidentModalComponent,
        CreateOrEditIncidentModalComponent,
        IncidentIncidentPriorityLookupTableModalComponent,
        IncidentIncidentStatusLookupTableModalComponent,
        IncidentCustomerLookupTableModalComponent,
        IncidentAssetLookupTableModalComponent,
        IncidentSupportItemLookupTableModalComponent,
        IncidentIncidentTypeLookupTableModalComponent,
        IncidentUserLookupTableModalComponent,
        IncidentLocationLookupTableModalComponent,
        ViewSupportItemModalComponent,
        CreateOrEditSupportItemModalComponent,
        SupportItemAssetLookupTableModalComponent,
        SupportItemAssetClassLookupTableModalComponent,
        SupportItemSupportContractLookupTableModalComponent,
        SupportItemConsumableTypeLookupTableModalComponent,
        SupportItemSupportTypeLookupTableModalComponent,
        BillingEventDetailsComponent,
        ViewBillingEventDetailModalComponent,
        CreateOrEditBillingEventDetailModalComponent,
        BillingEventDetailBillingRuleLookupTableModalComponent,
        BillingEventDetailLeaseItemLookupTableModalComponent,
        BillingEventDetailBillingEventLookupTableModalComponent,
        BillingEventsComponent,
        ViewBillingEventModalComponent,
        CreateOrEditBillingEventModalComponent,
        BillingEventLeaseAgreementLookupTableModalComponent,
        BillingEventVendorChargeLookupTableModalComponent,
        BillingEventBillingEventTypeLookupTableModalComponent,
        VendorChargeVendorLookupTableModalComponent,
        VendorChargeSupportContractLookupTableModalComponent,
        BillingRulesComponent,
        ViewBillingRuleModalComponent,
        CreateOrEditBillingRuleModalComponent,
        BillingRuleLeaseAgreementLookupTableModalComponent,
        BillingRuleVendorLookupTableModalComponent,
        BillingRuleLeaseItemLookupTableModalComponent,
        BillingRuleCurrencyLookupTableModalComponent,
        BillingRuleBillingRuleTypeLookupTableModalComponent,
        BillingRuleUsageMetricLookupTableModalComponent,
        //BillingEventTypesComponent,
        //CreateOrEditBillingEventTypeModalComponent,
        //ViewBillingEventTypeModalComponent,

        UsageMetricRecordsComponent,
        ViewUsageMetricRecordModalComponent,
        CreateOrEditUsageMetricRecordModalComponent,
        UsageMetricRecordUsageMetricLookupTableModalComponent,
        UsageMetricsComponent,
        ViewUsageMetricModalComponent,
        CreateOrEditUsageMetricModalComponent,
        UsageMetricLeaseItemLookupTableModalComponent,
        UsageMetricAssetLookupTableModalComponent,
        //LeaseItemsComponent,
        ViewLeaseItemModalComponent,
        CreateOrEditLeaseItemModalComponent,
        LeaseItemAssetClassLookupTableModalComponent,
        LeaseItemAssetLookupTableModalComponent,
        LeaseItemDepositUomLookupTableModalComponent,
        LeaseItemRentalUomLookupTableModalComponent,
        LeaseItemLeaseAgreementLookupTableModalComponent,
        AssetOwnershipsComponent,
        ViewAssetOwnershipModalComponent,
        CreateOrEditAssetOwnershipModalComponent,
        AssetOwnershipAssetLookupTableModalComponent,
        AssetOwnershipAssetOwnerLookupTableModalComponent,
        AssetsComponent,
        ViewAssetComponent,
        ViewAssetModalComponent,
        CreateOrEditAssetModalComponent,
        AssetAssetClassLookupTableModalComponent,
        AssetAssetStatusLookupTableModalComponent,
        AssetLocationLookupTableModalComponent,
        CreateOrEditAssetNotesModalComponent,
        ViewAssetNotesModalComponent,
        LeaseAgreementsComponent,
        ViewLeaseAgreementComponent,
        ViewLeaseAgreementModalComponent,
        CreateOrEditLeaseAgreementModalComponent,
        LeaseAgreementContactLookupTableModalComponent,
        LeaseAgreementAssetOwnerLookupTableModalComponent,
        LeaseAgreementCustomerLookupTableModalComponent,

        QuotationSupportContractLookupTableModalComponent,
        QuotationQuotationStatusLookupTableModalComponent,

        VendorChargeDetailsComponent,
        ViewVendorChargeDetailModalComponent,
        CreateOrEditVendorChargeDetailModalComponent,

        DemoUiComponentsComponent,
        DemoUiDateTimeComponent,
        DemoUiEditorComponent,
        DemoUiFileUploadComponent,
        DemoUiInputMaskComponent,
        DemoUiSelectionComponent,

        ConsumableTypesComponent,
        CreateOrEditConsumableTypeModalComponent,
        ViewConsumableTypeModalComponent,

        UomLookupTableModalComponent,
        ItemTypeLookupTableModalComponent,        InvoicePDFComponent
    ],
    providers: [
        TreeDragDropService,
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale }
    ]
})
export class MainModule { }
