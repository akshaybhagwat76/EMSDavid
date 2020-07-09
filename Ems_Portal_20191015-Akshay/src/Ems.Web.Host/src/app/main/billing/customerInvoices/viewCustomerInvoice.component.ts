import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import * as _ from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { CreateOrEditCustomerInvoiceDetailModalComponent } from './create-or-edit-customerInvoiceDetail-modal.component';
import { ViewCustomerInvoiceDetailModalComponent } from './view-customerInvoiceDetail-modal.component';
import { CustomerInvoicesServiceProxy, CustomerInvoiceDto, EstimateDto, WorkOrderDto, CustomerInvoiceDetailsServiceProxy, CustomerInvoiceDetailDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerInvoiceModalComponent } from './create-or-edit-customerInvoice-modal.component';
import * as moment from 'moment';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';

@Component({
    selector: 'viewCustomerInvoice',
    templateUrl: './viewCustomerInvoice.component.html',
    animations: [appModuleAnimation()]
})
export class ViewCustomerInvoiceComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditCustomerInvoiceModal', { static: true }) createOrEditCustomerInvoiceModal: CreateOrEditCustomerInvoiceModalComponent;
    @ViewChild('createOrEditCustomerInvoiceDetailModal', { static: true }) createOrEditCustomerInvoiceDetailModal: CreateOrEditCustomerInvoiceDetailModalComponent;
    @ViewChild('viewCustomerInvoiceDetailModal', { static: true }) viewCustomerInvoiceDetailModal: ViewCustomerInvoiceDetailModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table
    @ViewChild('dataTable2', { static: true }) dataTable2: Table; // For the 3rd table
    @ViewChild('paginator2', { static: true }) paginator2: Paginator; // For the 3rd table

    primengTableHelper1: PrimengTableHelper;
    primengTableHelper2: PrimengTableHelper;

    advancedFiltersAreShown = false;

    customerInvoiceId: number;
    customerInvoice: CustomerInvoiceDto;
    estimate: EstimateDto;
    workOrder: WorkOrderDto;

    customerName = "";
    workOrderSubject = "";
    estimateTitle = "";
    currencyCode = "";
    billingRuleName = "";
    billingEventPurpose = "";
    invoiceStatusStatus = "";

    eQuotationTitle = "";
    eWorkOrderSubject = "";
    eCustomerName = "";
    eEstimateStatusStatus = "";

    woWorkOrderStatus = "";
    woWorkOrderPriority = "";
    woWorkOrderType = "";
    woVendorName = "";
    woIncidentDescription = "";
    woSupportItemDescription = "";
    woAssetDisplayName = "";
    woUserName = "";
    woCustomerName = "";

    _entityTypeFullName = 'Ems.Billing.CustomerInvoice';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _customerInvoicesServiceProxy: CustomerInvoicesServiceProxy,
        private _customerInvoiceDetailsServiceProxy: CustomerInvoiceDetailsServiceProxy,
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
        this.primengTableHelper2 = new PrimengTableHelper(); // For the 3rd table
    }

    active = false;
    saving = false;

    ngOnInit(): void {

        this.customerInvoiceId = this._activatedRoute.snapshot.queryParams['customerInvoiceId'];
        this.customerInvoice = new CustomerInvoiceDto();
        this.estimate = new EstimateDto();
        this.workOrder = new WorkOrderDto();

        //this.getCustomerInvoice();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getCustomerInvoice(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._customerInvoicesServiceProxy.getCustomerInvoiceForView(
            this.customerInvoiceId,
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSorting(this.dataTable)
        ).subscribe((customerInvoiceResult) => {

            if (customerInvoiceResult.customerInvoice == null) {
                this.primengTableHelper.hideLoadingIndicator();
                this.close();
            }
            else {
                this.customerInvoice = customerInvoiceResult.customerInvoice;
                this.customerInvoiceId = customerInvoiceResult.customerInvoice.id;
                this.customerName = customerInvoiceResult.customerName;
                this.workOrderSubject = customerInvoiceResult.workOrderSubject;
                this.estimateTitle = customerInvoiceResult.estimateTitle;
                this.currencyCode = customerInvoiceResult.currencyCode;
                this.billingRuleName = customerInvoiceResult.billingRuleName;
                this.billingEventPurpose = customerInvoiceResult.billingEventPurpose;
                this.invoiceStatusStatus = customerInvoiceResult.invoiceStatusStatus;

                this.primengTableHelper.totalRecordsCount = customerInvoiceResult.customerInvoiceDetails.totalCount;
                this.primengTableHelper.records = customerInvoiceResult.customerInvoiceDetails.items;
                this.primengTableHelper.hideLoadingIndicator();

                this.getEstimateInfo();
                this.getWorkOrderInfo();
            }
        });
    }

    getEstimateInfo(event?: LazyLoadEvent) {
        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._customerInvoicesServiceProxy.getCustomerInvoiceEstimateForView(
            this.customerInvoiceId,
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event),
            this.primengTableHelper1.getSorting(this.dataTable1)
        ).subscribe((eResult) => {

            if (eResult.estimate == null) {
                this.estimate = null;
                this.primengTableHelper1.hideLoadingIndicator();
            }
            else {
                this.estimate = eResult.estimate.estimate;
                this.eQuotationTitle = eResult.estimate.quotationTitle;
                this.eWorkOrderSubject = eResult.estimate.workOrderSubject;
                this.eCustomerName = eResult.estimate.customerName;
                this.eEstimateStatusStatus = eResult.estimate.estimateStatusStatus;

                this.primengTableHelper1.totalRecordsCount = eResult.estimateDetails.totalCount;
                this.primengTableHelper1.records = eResult.estimateDetails.items;
                this.primengTableHelper1.hideLoadingIndicator();
            }
        });
    }

    getWorkOrderInfo(event?: LazyLoadEvent) {
        if (this.primengTableHelper2.shouldResetPaging(event)) {
            this.paginator2.changePage(0);
            return;
        }

        this.primengTableHelper2.showLoadingIndicator();

        this._customerInvoicesServiceProxy.getCustomerInvoiceWorkOrderForView(
            this.customerInvoiceId,
            this.primengTableHelper2.getSkipCount(this.paginator2, event),
            this.primengTableHelper2.getMaxResultCount(this.paginator2, event),
            this.primengTableHelper2.getSorting(this.dataTable2)
        ).subscribe((woResult) => {
            if (woResult.workOrder == null) {
                this.workOrder = null;
                this.primengTableHelper2.hideLoadingIndicator();
            }
            else {
                this.workOrder = woResult.workOrder.workOrder;
                this.woWorkOrderStatus = woResult.workOrder.workOrderStatusStatus;
                this.woWorkOrderPriority = woResult.workOrder.workOrderPriorityPriority;
                this.woWorkOrderType = woResult.workOrder.workOrderTypeType;
                this.woVendorName = woResult.workOrder.vendorName;
                this.woIncidentDescription = woResult.workOrder.incidentDescription;
                this.woSupportItemDescription = woResult.workOrder.supportItemDescription;
                this.woAssetDisplayName = woResult.workOrder.assetOwnershipAssetDisplayName;
                this.woUserName = woResult.workOrder.userName;
                this.woCustomerName = woResult.workOrder.customerName;

                this.primengTableHelper2.totalRecordsCount = woResult.workOrderUpdates.totalCount;
                this.primengTableHelper2.records = woResult.workOrderUpdates.items;
                this.primengTableHelper2.hideLoadingIndicator();
            }
        });
    }


    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createCustomerInvoiceDetail(): void {
        this.createOrEditCustomerInvoiceDetailModal.show(null, this.customerInvoiceId);
    }

    deleteCustomerInvoiceDetail(customerInvoiceDetail: CustomerInvoiceDetailDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._customerInvoiceDetailsServiceProxy.delete(customerInvoiceDetail.id)
                        .subscribe(() => {
                            this._customerInvoicesServiceProxy.updateCustomerInvoicePrices(customerInvoiceDetail.customerInvoiceId)
                                .subscribe(() => {
                                    this.reloadPage();
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                });
                        });
                }
            }
        );
    }

    showHistory(customerInvoice: CustomerInvoiceDto): void {
        this.entityTypeHistoryModal.show({
            entityId: customerInvoice.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteCustomerInvoice(customerInvoice: CustomerInvoiceDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._customerInvoicesServiceProxy.delete(customerInvoice.id)
                        .subscribe(() => {
                            this.close();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }


    close(): void {
        this._router.navigate(['app/main/billing/customerInvoices']);
    }
}
