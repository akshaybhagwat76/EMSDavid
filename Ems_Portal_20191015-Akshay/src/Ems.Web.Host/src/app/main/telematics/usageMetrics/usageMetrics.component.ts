import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UsageMetricsServiceProxy, UsageMetricDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditUsageMetricModalComponent } from './create-or-edit-usageMetric-modal.component';
import { ViewUsageMetricModalComponent } from './view-usageMetric-modal.component';
import { ViewUsageMetricRecordModalComponent } from '../usageMetricRecords/view-usageMetricRecord-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './usageMetrics.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class UsageMetricsComponent extends AppComponentBase {

    @ViewChild('createOrEditUsageMetricModal', { static: true }) createOrEditUsageMetricModal: CreateOrEditUsageMetricModalComponent;
    @ViewChild('viewUsageMetricModal', { static: true }) viewUsageMetricModal: ViewUsageMetricModalComponent;
    @ViewChild('viewUsageMetricRecordModal', { static: true }) viewUsageMetricRecordModal: ViewUsageMetricRecordModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    metricFilter = '';
    descriptionFilter = '';
    leaseItemItemFilter = '';
    assetReferenceFilter = '';
    uomUnitOfMeasurementFilter = '';

    _entityTypeFullName = 'Ems.Telematics.UsageMetric';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _usageMetricsServiceProxy: UsageMetricsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getUsageMetrics(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._usageMetricsServiceProxy.getAll(
            this.filterText,
            this.metricFilter,
            this.descriptionFilter,
            this.leaseItemItemFilter,
            this.assetReferenceFilter,
            this.uomUnitOfMeasurementFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createUsageMetric(): void {
        this.createOrEditUsageMetricModal.show(true, null);
    }

    showHistory(usageMetric: UsageMetricDto): void {
        this.entityTypeHistoryModal.show({
            entityId: usageMetric.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteUsageMetric(usageMetric: UsageMetricDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._usageMetricsServiceProxy.delete(usageMetric.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._usageMetricsServiceProxy.getUsageMetricsToExcel(
            this.filterText,
            this.metricFilter,
            this.descriptionFilter,
            this.leaseItemItemFilter,
            this.assetReferenceFilter,
            this.uomUnitOfMeasurementFilter
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }
}
