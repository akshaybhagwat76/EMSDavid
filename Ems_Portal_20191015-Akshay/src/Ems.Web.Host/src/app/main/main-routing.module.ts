import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { InventoryItemsComponent } from './assets/inventoryItems/inventoryItems.component';
import { AssetPartStatusesComponent } from './assets/assetPartStatuses/assetPartStatuses.component';
import { AssetPartsComponent } from './assets/assetParts/assetParts.component';
import { AssetPartTypesComponent } from './assets/assetPartTypes/assetPartTypes.component';
import { AgedReceivablesPeriodsComponent } from './finance/agedReceivablesPeriods/agedReceivablesPeriods.component';

import { CustomerInvoiceDetailsComponent } from './billing/customerInvoices/customerInvoiceDetails.component';
import { CustomerInvoicesComponent } from './billing/customerInvoices/customerInvoices.component';
import { ViewCustomerInvoiceComponent } from './billing/customerInvoices/viewCustomerInvoice.component';
import { EstimatesComponent } from './support/estimates/estimates.component';
import { ViewEstimateComponent } from './support/estimates/viewEstimate.component';
import { EstimateStatusesComponent } from './support/estimateStatuses/estimateStatuses.component';
import { AttachmentsComponent } from './storage/attachments/attachments.component';
import { EstimatePdfComponent } from './support/estimates/estimate-pdf.component';

import { ViewQuotationComponent } from './support/quotations/viewQuotation.component';
import { QuotationPdfComponent } from './support/quotations/quotation-pdf.component';
import { ViewWorkOrderComponent } from './support/workOrders/viewWorkOrder.component';
import { WorkOrdersComponent } from './support/workOrders/workOrders.component';
import { RfqsComponent } from './quotations/rfqs/rfqs.component';
import { IncidentsComponent } from './support/incidents/incidents.component';
import { ViewIncidentComponent } from './support/incidents/viewIncident.component';
import { BillingEventDetailsComponent } from './billing/billingEventDetails/billingEventDetails.component';
import { BillingEventsComponent } from './billing/billingEvents/billingEvents.component';
import { VendorChargesComponent } from './vendors/vendorCharges/vendorCharges.component';
import { BillingRulesComponent } from './billing/billingRules/billingRules.component';
import { UsageMetricRecordsComponent } from './telematics/usageMetricRecords/usageMetricRecords.component';
import { UsageMetricsComponent } from './telematics/usageMetrics/usageMetrics.component';
//import { LeaseItemsComponent } from './assets/leaseItems/leaseItems.component';
import { AssetOwnershipsComponent } from './assets/assetOwnerships/assetOwnerships.component';
import { AssetsComponent } from './assets/assets/assets.component';
import { ViewAssetComponent } from './assets/assets/viewAsset.component';
import { LeaseAgreementsComponent } from './assets/leaseAgreements/leaseAgreements.component';
import { ViewLeaseAgreementComponent } from './assets/leaseAgreements/viewLeaseAgreement.component';
import { QuotationsComponent } from './support/quotations/quotations.component';
import { VendorChargeDetailsComponent } from './vendors/vendorChargeDetails/vendorChargeDetails.component';

//import { WorkOrderUpdatesComponent } from './support/workOrderUpdates/workOrderUpdates.component';
import { SupportContractsComponent } from './support/supportContracts/supportContracts.component';
import { ViewSupportContractComponent } from './support/supportContracts/viewSupportContract.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { InvoicePDFComponent } from './billing/customerInvoices/invoice-pdf.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'assets/inventoryItems', component: InventoryItemsComponent, data: { permission: 'Pages.InventoryItems' }  },
                    { path: 'assets/assetPartStatuses', component: AssetPartStatusesComponent, data: { permission: 'Pages.Main.AssetPartStatuses' }  },
                    { path: 'assets/assetParts', component: AssetPartsComponent, data: { permission: 'Pages.Main.AssetParts' }  },
                    { path: 'assets/assetPartTypes', component: AssetPartTypesComponent, data: { permission: 'Pages.AssetPartTypes' }  },
                    { path: 'finance/agedReceivablesPeriods', component: AgedReceivablesPeriodsComponent, data: { permission: 'Pages.AgedReceivablesPeriods' }  },
                    { path: 'storage/attachments', component: AttachmentsComponent, data: { permission: 'Pages.Main.Attachments' }  },
                    { path: 'billing/customerInvoiceDetails', component: CustomerInvoiceDetailsComponent, data: { permission: 'Pages.Main.CustomerInvoices' }  },
                    { path: 'support/estimateStatuses', component: EstimateStatusesComponent, data: { permission: 'Pages.Configuration.EstimateStatuses' }  },
                    { path: 'storage/attachments', component: AttachmentsComponent, data: { permission: 'Pages.Main.Attachments' }  },
                    { path: 'support/workOrders/:workOrderId', component: ViewWorkOrderComponent, data: { permission: 'Pages.Main.WorkOrders.View' }  },
                    { path: 'support/workOrders', component: WorkOrdersComponent, data: { permission: 'Pages.Main.WorkOrders' } },
                    { path: 'support/workOrders/:incidentId', component: WorkOrdersComponent, data: { permission: 'Pages.Main.WorkOrders' } },
                    { path: 'support/supportContracts', component: SupportContractsComponent, data: { permission: 'Pages.Main.SupportContracts' }  },
                    { path: 'support/supportContracts/:supportContractId', component: ViewSupportContractComponent, data: { permission: 'Pages.Main.SupportContracts.View' } },
                    { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Main.Dashboard' } },
                    { path: 'quotations/rfqs', component: RfqsComponent, data: { permission: 'Pages.Main.Rfqs' } },
                    { path: 'support/incidents', component: IncidentsComponent, data: { permission: 'Pages.Main.Incidents' } },
                    { path: 'support/incidents/:incidentId', component: ViewIncidentComponent, data: { permission: 'Pages.Main.Incidents.View' }  },
                    { path: 'billing/billingEventDetails', component: BillingEventDetailsComponent, data: { permission: 'Pages.Main.BillingEvents' } },
                    { path: 'billing/billingEvents', component: BillingEventsComponent, data: { permission: 'Pages.Main.BillingEvents' } },
                    { path: 'vendors/vendorCharges', component: VendorChargesComponent, data: { permission: 'Pages.Main.VendorCharges' } },
                    { path: 'billing/billingRules', component: BillingRulesComponent, data: { permission: 'Pages.Main.BillingRules' } },
                    { path: 'telematics/usageMetricRecords', component: UsageMetricRecordsComponent, data: { permission: 'Pages.Main.UsageMetricRecords' } },
                    { path: 'telematics/usageMetrics', component: UsageMetricsComponent, data: { permission: 'Pages.Main.UsageMetrics' } },
                    { path: 'assets/assetOwnerships', component: AssetOwnershipsComponent, data: { permission: 'Pages.Main.AssetOwnerships' } },
                    { path: 'assets/assets', component: AssetsComponent, data: { permission: 'Pages.Main.Assets' } },
                    { path: 'assets/assets/:assetId', component: ViewAssetComponent, data: { permission: 'Pages.Main.Assets.View' } },
                    { path: 'assets/leaseAgreements', component: LeaseAgreementsComponent, data: { permission: 'Pages.Main.LeaseAgreements' } },
                    { path: 'assets/leaseAgreements/:leaseAgreementId', component: ViewLeaseAgreementComponent, data: { permission: 'Pages.Main.LeaseAgreements.View' } },
                    {
                        path: 'support/quotations',
                        children: [
                        
                            { path: 'viewQuotation', component: ViewQuotationComponent, data: { permission: 'Pages.Main.Quotations.View' } },
                            { path: 'quotationPDF', component: QuotationPdfComponent, data: { permission: 'Pages.Main.Quotations' } },
                            { path: '', component: QuotationsComponent, data: { permission: 'Pages.Main.Quotations' } }
                        ]
                    },
                    {
                        path: 'support/estimates',
                        children: [

                            { path: 'viewEstimate', component: ViewEstimateComponent, data: { permission: 'Pages.Main.Estimates.View' } },
                            { path: 'estimatePDF', component: EstimatePdfComponent, data: { permission: 'Pages.Main.Estimates' } },
                            { path: '', component: EstimatesComponent, data: { permission: 'Pages.Main.Estimates' } }
                        ]
                    },
                    {
                        path: 'billing/customerInvoices',
                        children: [

                            { path: 'viewCustomerInvoice', component: ViewCustomerInvoiceComponent, data: { permission: 'Pages.Main.CustomerInvoices.View' } },
                            { path: 'invoicePDF', component: InvoicePDFComponent, data: { permission: 'Pages.Main.CustomerInvoices.View' } },
                            { path: '', component: CustomerInvoicesComponent, data: { permission: 'Pages.Main.CustomerInvoices' } }
                        ]
                    },

                    { path: 'vendors/vendorChargeDetails', component: VendorChargeDetailsComponent, data: { permission: 'Pages.Main.VendorCharges' } },

                    //{ path: 'billing/customerInvoices', component: CustomerInvoicesComponent, data: { permission: 'Pages.Main.CustomerInvoices' } },
                    //{ path: 'billing/customerInvoices/:customerInvoiceId', component: ViewCustomerInvoiceComponent, data: { permission: 'Pages.Main.CustomerInvoices.View' } },
                    //{ path: 'support/estimates', component: EstimatesComponent, data: { permission: 'Pages.Main.Estimates' } },
                    //{ path: 'support/estimates/:estimateId', component: ViewEstimateComponent, data: { permission: 'Pages.Main.Estimates.View' } },
                    //{ path: 'support/estimates/:quotationId', component: EstimatesComponent, data: { permission: 'Pages.Main.Estimates' } },
                    //{ path: 'support/estimates/:workOrderId', component: EstimatesComponent, data: { permission: 'Pages.Main.Estimates' } },
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
