import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EstimatesServiceProxy, EstimateDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditEstimateModalComponent } from './create-or-edit-estimate-modal.component';
import { ViewEstimateModalComponent } from './view-estimate-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './estimates.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class EstimatesComponent extends AppComponentBase {

    @ViewChild('createOrEditEstimateModal', { static: true }) createOrEditEstimateModal: CreateOrEditEstimateModalComponent;
    @ViewChild('viewEstimateModalComponent', { static: true }) viewEstimateModal: ViewEstimateModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    quotationId: number;
    workOrderId: number;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    titleFilter = '';
    descriptionFilter = '';
    maxStartDateFilter: moment.Moment;
    minStartDateFilter: moment.Moment;
    maxEndDateFilter: moment.Moment;
    minEndDateFilter: moment.Moment;
    maxTotalTaxFilter: number;
    maxTotalTaxFilterEmpty: number;
    minTotalTaxFilter: number;
    minTotalTaxFilterEmpty: number;
    maxTotalPriceFilter: number;
    maxTotalPriceFilterEmpty: number;
    minTotalPriceFilter: number;
    minTotalPriceFilterEmpty: number;
    maxTotalDiscountFilter: number;
    maxTotalDiscountFilterEmpty: number;
    minTotalDiscountFilter: number;
    minTotalDiscountFilterEmpty: number;
    maxTotalChargeFilter: number;
    maxTotalChargeFilterEmpty: number;
    minTotalChargeFilter: number;
    minTotalChargeFilterEmpty: number;
    maxVersionFilter: number;
    maxVersionFilterEmpty: number;
    minVersionFilter: number;
    minVersionFilterEmpty: number;
    remarkFilter = '';
    maxRequoteRefIdFilter: number;
    maxRequoteRefIdFilterEmpty: number;
    minRequoteRefIdFilter: number;
    minRequoteRefIdFilterEmpty: number;
    quotationLoc8GUIDFilter = '';
    maxAcknowledgedByFilter: number;
    maxAcknowledgedByFilterEmpty: number;
    minAcknowledgedByFilter: number;
    minAcknowledgedByFilterEmpty: number;
    maxAcknowledgedAtFilter: moment.Moment;
    minAcknowledgedAtFilter: moment.Moment;
    quotationTitleFilter = '';
    workOrderSubjectFilter = '';
    customerNameFilter = '';
    estimateStatusStatusFilter = '';


    _entityTypeFullName = 'Ems.Support.Estimate';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _estimatesServiceProxy: EstimatesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
        this.quotationId = this._activatedRoute.snapshot.queryParams['quotationId'];
        this.workOrderId = this._activatedRoute.snapshot.queryParams['workOrderId'];
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getEstimates(event?: LazyLoadEvent, createEstimate?: boolean) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._estimatesServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.titleFilter,
            this.descriptionFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.maxTotalTaxFilter == null ? this.maxTotalTaxFilterEmpty : this.maxTotalTaxFilter,
            this.minTotalTaxFilter == null ? this.minTotalTaxFilterEmpty : this.minTotalTaxFilter,
            this.maxTotalPriceFilter == null ? this.maxTotalPriceFilterEmpty : this.maxTotalPriceFilter,
            this.minTotalPriceFilter == null ? this.minTotalPriceFilterEmpty : this.minTotalPriceFilter,
            this.maxTotalDiscountFilter == null ? this.maxTotalDiscountFilterEmpty : this.maxTotalDiscountFilter,
            this.minTotalDiscountFilter == null ? this.minTotalDiscountFilterEmpty : this.minTotalDiscountFilter,
            this.maxTotalChargeFilter == null ? this.maxTotalChargeFilterEmpty : this.maxTotalChargeFilter,
            this.minTotalChargeFilter == null ? this.minTotalChargeFilterEmpty : this.minTotalChargeFilter,
            this.maxVersionFilter == null ? this.maxVersionFilterEmpty : this.maxVersionFilter,
            this.minVersionFilter == null ? this.minVersionFilterEmpty : this.minVersionFilter,
            this.remarkFilter,
            this.maxRequoteRefIdFilter == null ? this.maxRequoteRefIdFilterEmpty : this.maxRequoteRefIdFilter,
            this.minRequoteRefIdFilter == null ? this.minRequoteRefIdFilterEmpty : this.minRequoteRefIdFilter,
            this.quotationLoc8GUIDFilter,
            this.maxAcknowledgedByFilter == null ? this.maxAcknowledgedByFilterEmpty : this.maxAcknowledgedByFilter,
            this.minAcknowledgedByFilter == null ? this.minAcknowledgedByFilterEmpty : this.minAcknowledgedByFilter,
            this.maxAcknowledgedAtFilter,
            this.minAcknowledgedAtFilter,
            this.customerNameFilter,
            this.workOrderSubjectFilter,
            this.quotationTitleFilter,
            this.estimateStatusStatusFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();

            if (createEstimate && this.quotationId > 0) {
                this.createOrEditEstimateModal.show(0, this.quotationId);
            }

            if (createEstimate && this.workOrderId > 0) {
                this.createOrEditEstimateModal.show(0, 0, this.workOrderId);
            }
        });
    }

    clearSearch() {
        this.filterText = this.referenceFilter = '';
        this.titleFilter = this.descriptionFilter = '';
        this.maxStartDateFilter = this.minStartDateFilter = null;
        this.maxEndDateFilter = this.minEndDateFilter = null;
        this.maxTotalTaxFilter = this.minTotalTaxFilter = null;
        this.maxTotalPriceFilter = this.minTotalPriceFilter = null;
        this.maxTotalDiscountFilter = this.minTotalDiscountFilter = null;
        this.maxTotalChargeFilter = this.minTotalChargeFilter = null;
        this.maxVersionFilter = this.minVersionFilter = null;
        this.maxRequoteRefIdFilter = this.minRequoteRefIdFilter = null;
        this.remarkFilter = this.quotationLoc8GUIDFilter = '';
        this.maxAcknowledgedByFilter = this.minAcknowledgedByFilter = null;
        this.maxAcknowledgedAtFilter = this.minAcknowledgedAtFilter = null;
        this.customerNameFilter = this.workOrderSubjectFilter = '';
        this.quotationTitleFilter = this.estimateStatusStatusFilter = '';

        this.getEstimates();
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createEstimate(): void {
        this.createOrEditEstimateModal.show();
    }

    showHistory(estimate: EstimateDto): void {
        this.entityTypeHistoryModal.show({
            entityId: estimate.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteEstimate(estimate: EstimateDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._estimatesServiceProxy.delete(estimate.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._estimatesServiceProxy.getEstimatesToExcel(
            this.filterText,
            this.referenceFilter,
            this.titleFilter,
            this.descriptionFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.maxTotalTaxFilter == null ? this.maxTotalTaxFilterEmpty : this.maxTotalTaxFilter,
            this.minTotalTaxFilter == null ? this.minTotalTaxFilterEmpty : this.minTotalTaxFilter,
            this.maxTotalPriceFilter == null ? this.maxTotalPriceFilterEmpty : this.maxTotalPriceFilter,
            this.minTotalPriceFilter == null ? this.minTotalPriceFilterEmpty : this.minTotalPriceFilter,
            this.maxTotalDiscountFilter == null ? this.maxTotalDiscountFilterEmpty : this.maxTotalDiscountFilter,
            this.minTotalDiscountFilter == null ? this.minTotalDiscountFilterEmpty : this.minTotalDiscountFilter,
            this.maxTotalChargeFilter == null ? this.maxTotalChargeFilterEmpty : this.maxTotalChargeFilter,
            this.minTotalChargeFilter == null ? this.minTotalChargeFilterEmpty : this.minTotalChargeFilter,
            this.maxVersionFilter == null ? this.maxVersionFilterEmpty : this.maxVersionFilter,
            this.minVersionFilter == null ? this.minVersionFilterEmpty : this.minVersionFilter,
            this.remarkFilter,
            this.maxRequoteRefIdFilter == null ? this.maxRequoteRefIdFilterEmpty : this.maxRequoteRefIdFilter,
            this.minRequoteRefIdFilter == null ? this.minRequoteRefIdFilterEmpty : this.minRequoteRefIdFilter,
            this.quotationLoc8GUIDFilter,
            this.maxAcknowledgedByFilter == null ? this.maxAcknowledgedByFilterEmpty : this.maxAcknowledgedByFilter,
            this.minAcknowledgedByFilter == null ? this.minAcknowledgedByFilterEmpty : this.minAcknowledgedByFilter,
            this.maxAcknowledgedAtFilter,
            this.minAcknowledgedAtFilter,
            this.customerNameFilter,
            this.workOrderSubjectFilter,
            this.quotationTitleFilter,
            this.estimateStatusStatusFilter
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }

    viewEstimate(estimateId): void {
        this._router.navigate(['app/main/support/estimates/viewEstimate'], { queryParams: { estimateId: estimateId } });
    }

    generatePDF(estimateId): void {
        this._router.navigate(['app/main/support/estimates/estimatePDF'], { queryParams: { estimateId: estimateId } });
    }

    createCustomerInvoice(estimateId): void {
        this._router.navigate(['app/main/billing/customerInvoices'], { queryParams: { estimateId: estimateId } });
    }
}
