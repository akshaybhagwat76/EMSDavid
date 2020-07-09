import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UsageMetricRecordsServiceProxy, UsageMetricRecordDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
//import { CreateOrEditUsageMetricRecordModalComponent } from './create-or-edit-usageMetricRecord-modal.component';
//import { ViewUsageMetricRecordModalComponent } from './view-usageMetricRecord-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './usageMetricRecords.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class UsageMetricRecordsComponent extends AppComponentBase {

    //@ViewChild('createOrEditUsageMetricRecordModal', { static: true }) createOrEditUsageMetricRecordModal: CreateOrEditUsageMetricRecordModalComponent;
    //@ViewChild('viewUsageMetricRecordModal', { static: true }) viewUsageMetricRecordModal: ViewUsageMetricRecordModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    maxStartTimeFilter: moment.Moment;
    minStartTimeFilter: moment.Moment;
    maxEndTimeFilter: moment.Moment;
    minEndTimeFilter: moment.Moment;
    maxUnitsConsumedFilter: number;
    maxUnitsConsumedFilterEmpty: number;
    minUnitsConsumedFilter: number;
    minUnitsConsumedFilterEmpty: number;
    usageMetricMetricFilter = '';


    constructor(
        injector: Injector,
        private _usageMetricRecordsServiceProxy: UsageMetricRecordsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getUsageMetricRecords(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._usageMetricRecordsServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.maxStartTimeFilter,
            this.minStartTimeFilter,
            this.maxEndTimeFilter,
            this.minEndTimeFilter,
            this.maxUnitsConsumedFilter == null ? this.maxUnitsConsumedFilterEmpty : this.maxUnitsConsumedFilter,
            this.minUnitsConsumedFilter == null ? this.minUnitsConsumedFilterEmpty : this.minUnitsConsumedFilter,
            this.usageMetricMetricFilter, 
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

    createUsageMetricRecord(): void {
        //this.createOrEditUsageMetricRecordModal.show();
    }

    deleteUsageMetricRecord(usageMetricRecord: UsageMetricRecordDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._usageMetricRecordsServiceProxy.delete(usageMetricRecord.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._usageMetricRecordsServiceProxy.getUsageMetricRecordsToExcel(
            this.filterText,
            this.referenceFilter,
            this.maxStartTimeFilter,
            this.minStartTimeFilter,
            this.maxEndTimeFilter,
            this.minEndTimeFilter,
            this.maxUnitsConsumedFilter == null ? this.maxUnitsConsumedFilterEmpty : this.maxUnitsConsumedFilter,
            this.minUnitsConsumedFilter == null ? this.minUnitsConsumedFilterEmpty : this.minUnitsConsumedFilter,
            this.usageMetricMetricFilter,
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }
}
