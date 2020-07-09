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
import { EstimatesServiceProxy, EstimateDto, WorkOrderDto, QuotationDto, EstimateDetailDto, EstimateDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditEstimateModalComponent } from './create-or-edit-estimate-modal.component';
import { ViewEstimateDetailModalComponent } from './view-estimateDetail-modal.component';
import { CreateOrEditEstimateDetailModalComponent } from './create-or-edit-estimateDetail-modal.component';
import * as moment from 'moment';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';

@Component({
    selector: 'viewEstimate',
    templateUrl: './viewEstimate.component.html',
    animations: [appModuleAnimation()]
})
export class ViewEstimateComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditEstimateModal', { static: true }) createOrEditEstimateModal: CreateOrEditEstimateModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('createOrEditEstimateDetailModal', { static: true }) createOrEditEstimateDetailModal: CreateOrEditEstimateDetailModalComponent;
    @ViewChild('viewEstimateDetailModalComponent', { static: true }) viewEstimateDetailModal: ViewEstimateDetailModalComponent;


    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table
    @ViewChild('dataTable2', { static: true }) dataTable2: Table; // For the 3rd table
    @ViewChild('paginator2', { static: true }) paginator2: Paginator; // For the 3rd table

    primengTableHelper1: PrimengTableHelper;
    primengTableHelper2: PrimengTableHelper;

    advancedFiltersAreShown = false;
    filterText = '';
    maxUpdatedAtFilter: moment.Moment;
    minUpdatedAtFilter: moment.Moment;
    updateFilter = '';
    maxUpdatedByUserIdFilter: number;
    maxUpdatedByUserIdFilterEmpty: number;
    minUpdatedByUserIdFilter: number;
    minUpdatedByUserIdFilterEmpty: number;
    workOrderSubjectFilter = '';

    _entityTypeFullName = 'Ems.Support.Estimate';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _estimatesServiceProxy: EstimatesServiceProxy,
        private _estimateDetailsServiceProxy: EstimateDetailsServiceProxy
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
        this.primengTableHelper2 = new PrimengTableHelper(); // For the 3rd table
    }

    active = false;
    saving = false;

    estimateId: number;
    estimate: EstimateDto;
    quotation: QuotationDto;
    workOrder: WorkOrderDto;

    estimateStatusStatus = '';
    quotationTitle = '';
    workOrderSubject = '';
    customerName = ''

    qAssetClassClass = '';
    qAssetReference = '';
    qQuotationStatusStatus = '';
    qSupportContractTitle = '';
    qSupportItemDescription = '';
    qSupportTypeType = '';
    qWorkOrderSubject = '';

    woWorkOrderStatus = '';
    woWorkOrderPriority = '';
    woWorkOrderType = '';
    woVendorName = '';
    woIncidentDescription = '';
    woSupportItemDescription = '';
    woAssetDisplayName = '';
    woUserName = '';
    woCustomerName = '';


    ngOnInit(): void {

        this.estimateId = this._activatedRoute.snapshot.queryParams['estimateId'];
        this.estimate = new EstimateDto();
        this.quotation = new QuotationDto();
        this.workOrder = new WorkOrderDto();

        this.getEstimate();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getEstimate(event?: LazyLoadEvent, loadEstimateDetails?: number) {
        this._estimatesServiceProxy.getEstimateForView(
            this.estimateId
        ).subscribe((estimateResult) => {

            if (estimateResult.estimate == null) {
                this.primengTableHelper.hideLoadingIndicator();
                this.close();
            }
            else {
                //this.estimateId = estimateResult.estimate.id;
                this.estimate = estimateResult.estimate;

                this.quotationTitle = estimateResult.quotationTitle;
                this.workOrderSubject = estimateResult.workOrderSubject;
                this.customerName = estimateResult.customerName;
                this.estimateStatusStatus = estimateResult.estimateStatusStatus;

                this.getEstimateDetails();
                this.getQuotationInfo();
                this.getWorkOrderInfo();
            }
        });
    }

    getEstimateDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._estimateDetailsServiceProxy.getAll(
            this.estimateId,
            '', '', '', '','',
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    getQuotationInfo(event?: LazyLoadEvent) {
        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._estimatesServiceProxy.getEstimateQuotationForView(
            this.estimateId,
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event),
            this.primengTableHelper1.getSorting(this.dataTable1)
        ).subscribe((qResult) => {

            if (qResult.quotation == null) {
                this.quotation = null;
                this.primengTableHelper1.hideLoadingIndicator();
            }
            else {
                this.quotation = qResult.quotation.quotation;
                this.qAssetClassClass = qResult.quotation.assetClassClass;
                this.qAssetReference = qResult.quotation.assetReference;
                this.qQuotationStatusStatus = qResult.quotation.quotationStatusStatus;
                this.qSupportContractTitle = qResult.quotation.supportContractTitle;
                this.qSupportItemDescription = qResult.quotation.supportItemDescription;
                this.qSupportTypeType = qResult.quotation.supportTypeType;
                this.qWorkOrderSubject = qResult.quotation.workOrderSubject;

                this.primengTableHelper1.totalRecordsCount = qResult.quotationDetails.totalCount;
                this.primengTableHelper1.records = qResult.quotationDetails.items;
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

        this._estimatesServiceProxy.getEstimateWorkOrderForView(
            this.estimateId,
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

    createEstimateDetail(): void {
        this.createOrEditEstimateDetailModal.show(null, this.estimateId);
    }

    deleteEstimateDetail(estimateDetail: EstimateDetailDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._estimateDetailsServiceProxy.delete(estimateDetail.id)
                        .subscribe(() => {
                            this._estimatesServiceProxy.updateEstimatePrices(estimateDetail.estimateId)
                                .subscribe(() => {
                                    this.reloadPage();
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                });
                        });
                }
            }
        );
    }

    deleteEstimate(estimate: EstimateDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._estimatesServiceProxy.delete(estimate.id)
                        .subscribe(() => {
                            this.close();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    showHistory(estimate: EstimateDto): void {
        this.entityTypeHistoryModal.show({
            entityId: estimate.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    generatePDF(estimateId): void {
        this._router.navigate(['app/main/support/estimates/estimatePDF'], { queryParams: { estimateId: estimateId } });
    }

    createCustomerInvoice(estimateId): void {
        this._router.navigate(['app/main/billing/customerInvoices'], { queryParams: { estimateId: estimateId } });
    }


    reloadPage(): void {
        this.getEstimate();
    }

    close(): void {
        this._router.navigate(['app/main/support/estimates']);
    }
}
