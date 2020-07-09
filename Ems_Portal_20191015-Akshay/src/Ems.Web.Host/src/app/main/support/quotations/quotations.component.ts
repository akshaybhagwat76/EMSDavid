import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QuotationsServiceProxy, QuotationDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditQuotationModalComponent } from './create-or-edit-quotation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './quotations.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class QuotationsComponent extends AppComponentBase {

    @ViewChild('createOrEditQuotationModal', { static: true }) createOrEditQuotationModal: CreateOrEditQuotationModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

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
    isFinalFilter = -1;
    remarkFilter = '';
    maxRequoteRefIdFilter: number;
    maxRequoteRefIdFilterEmpty: number;
    minRequoteRefIdFilter: number;
    minRequoteRefIdFilterEmpty: number;
    quotationLoc8GUIDFilter = '';
    attachmentsFilter = '';
    acknowledgedByFilter = '';
    maxAcknowledgedAtFilter: moment.Moment;
    minAcknowledgedAtFilter: moment.Moment;
    supportContractTitleFilter = '';
    quotationStatusStatusFilter = '';
    rfqTitleFilter = '';
    workOrderSubjectFilter = '';
    assetReferenceFilter = '';
    assetClassClassFilter = '';
    supportTypeTypeFilter = '';
    supportItemDescriptionFilter = '';

    _entityTypeFullName = 'Ems.Quotations.Quotation';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _quotationsServiceProxy: QuotationsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
        this.workOrderId = this._activatedRoute.snapshot.queryParams['workOrderId'];
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getQuotations(event?: LazyLoadEvent, createQuotation?: boolean) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._quotationsServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.titleFilter,
            this.descriptionFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.isFinalFilter,
            this.remarkFilter,
            this.maxRequoteRefIdFilter == null ? this.maxRequoteRefIdFilterEmpty : this.maxRequoteRefIdFilter,
            this.minRequoteRefIdFilter == null ? this.minRequoteRefIdFilterEmpty : this.minRequoteRefIdFilter,
            this.quotationLoc8GUIDFilter,
            this.acknowledgedByFilter,
            this.maxAcknowledgedAtFilter,
            this.minAcknowledgedAtFilter,
            this.supportContractTitleFilter,
            this.quotationStatusStatusFilter,
            this.workOrderSubjectFilter,
            this.assetReferenceFilter,
            this.assetClassClassFilter,
            this.supportTypeTypeFilter,
            this.supportItemDescriptionFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();

            if (createQuotation && this.workOrderId > 0) {
                this.createOrEditQuotationModal.show(null, this.workOrderId);
            }
        });
    }

    clearSearch() {
        this.filterText = this.referenceFilter = '';
        this.titleFilter = this.descriptionFilter = '';
        this.maxStartDateFilter = this.minStartDateFilter = null;
        this.maxEndDateFilter = this.minEndDateFilter = null;
        this.isFinalFilter = -1;
        this.remarkFilter = '';
        this.minRequoteRefIdFilter = this.maxRequoteRefIdFilter = null;
        this.quotationLoc8GUIDFilter = this.acknowledgedByFilter = '';
        this.maxAcknowledgedAtFilter = this.minAcknowledgedAtFilter = null;
        this.supportContractTitleFilter = this.quotationStatusStatusFilter = '';
        this.workOrderSubjectFilter = this.assetReferenceFilter = '';
        this.assetClassClassFilter = this.supportTypeTypeFilter = '';
        this.supportItemDescriptionFilter = '';

        this.getQuotations();
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createQuotation(): void {
        this.createOrEditQuotationModal.show();
    }

    showHistory(quotation: QuotationDto): void {
        this.entityTypeHistoryModal.show({
            entityId: quotation.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteQuotation(quotation: QuotationDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._quotationsServiceProxy.delete(quotation.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._quotationsServiceProxy.getQuotationsToExcel(
            this.filterText,
            this.referenceFilter,
            this.titleFilter,
            this.descriptionFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.isFinalFilter,
            this.remarkFilter,
            this.maxRequoteRefIdFilter == null ? this.maxRequoteRefIdFilterEmpty : this.maxRequoteRefIdFilter,
            this.minRequoteRefIdFilter == null ? this.minRequoteRefIdFilterEmpty : this.minRequoteRefIdFilter,
            this.quotationLoc8GUIDFilter,
            this.acknowledgedByFilter,
            this.maxAcknowledgedAtFilter,
            this.minAcknowledgedAtFilter,
            this.supportContractTitleFilter,
            this.quotationStatusStatusFilter,
            this.workOrderSubjectFilter,
            this.assetReferenceFilter,
            this.assetClassClassFilter,
            this.supportTypeTypeFilter,
            this.supportItemDescriptionFilter
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }

    viewQuotation(quotationId): void {
        this._router.navigate(['app/main/support/quotations/viewQuotation'], { queryParams: { quotationId: quotationId } });
    }

    createEstimate(quotationId): void {
        this._router.navigate(['app/main/support/estimates'], { queryParams: { quotationId: quotationId } });
    }

    generatePDF(quotationId): void {
        this._router.navigate(['app/main/support/quotations/quotationPDF'], { queryParams: { quotationId: quotationId } });
    }
}
