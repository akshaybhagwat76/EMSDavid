import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AgedReceivablesPeriodsServiceProxy, AgedReceivablesPeriodDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAgedReceivablesPeriodModalComponent } from './create-or-edit-agedReceivablesPeriod-modal.component';
import { ViewAgedReceivablesPeriodModalComponent } from './view-agedReceivablesPeriod-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './agedReceivablesPeriods.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AgedReceivablesPeriodsComponent extends AppComponentBase {

    @ViewChild('createOrEditAgedReceivablesPeriodModal', { static: true }) createOrEditAgedReceivablesPeriodModal: CreateOrEditAgedReceivablesPeriodModalComponent;
    @ViewChild('viewAgedReceivablesPeriodModalComponent', { static: true }) viewAgedReceivablesPeriodModal: ViewAgedReceivablesPeriodModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    maxPeriodFilter : moment.Moment;
		minPeriodFilter : moment.Moment;
    maxCurrentFilter : number;
		maxCurrentFilterEmpty : number;
		minCurrentFilter : number;
		minCurrentFilterEmpty : number;
    maxOver30Filter : number;
		maxOver30FilterEmpty : number;
		minOver30Filter : number;
		minOver30FilterEmpty : number;
    maxOver60Filter : number;
		maxOver60FilterEmpty : number;
		minOver60Filter : number;
		minOver60FilterEmpty : number;
    maxOver90Filter : number;
		maxOver90FilterEmpty : number;
		minOver90Filter : number;
		minOver90FilterEmpty : number;
    maxOver120Filter : number;
		maxOver120FilterEmpty : number;
		minOver120Filter : number;
		minOver120FilterEmpty : number;




    constructor(
        injector: Injector,
        private _agedReceivablesPeriodsServiceProxy: AgedReceivablesPeriodsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getAgedReceivablesPeriods(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._agedReceivablesPeriodsServiceProxy.getAll(
            this.filterText,
            this.maxPeriodFilter,
            this.minPeriodFilter,
            this.maxCurrentFilter == null ? this.maxCurrentFilterEmpty: this.maxCurrentFilter,
            this.minCurrentFilter == null ? this.minCurrentFilterEmpty: this.minCurrentFilter,
            this.maxOver30Filter == null ? this.maxOver30FilterEmpty: this.maxOver30Filter,
            this.minOver30Filter == null ? this.minOver30FilterEmpty: this.minOver30Filter,
            this.maxOver60Filter == null ? this.maxOver60FilterEmpty: this.maxOver60Filter,
            this.minOver60Filter == null ? this.minOver60FilterEmpty: this.minOver60Filter,
            this.maxOver90Filter == null ? this.maxOver90FilterEmpty: this.maxOver90Filter,
            this.minOver90Filter == null ? this.minOver90FilterEmpty: this.minOver90Filter,
            this.maxOver120Filter == null ? this.maxOver120FilterEmpty: this.maxOver120Filter,
            this.minOver120Filter == null ? this.minOver120FilterEmpty: this.minOver120Filter,
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

    createAgedReceivablesPeriod(): void {
        this.createOrEditAgedReceivablesPeriodModal.show();
    }

    deleteAgedReceivablesPeriod(agedReceivablesPeriod: AgedReceivablesPeriodDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._agedReceivablesPeriodsServiceProxy.delete(agedReceivablesPeriod.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._agedReceivablesPeriodsServiceProxy.getAgedReceivablesPeriodsToExcel(
        this.filterText,
            this.maxPeriodFilter,
            this.minPeriodFilter,
            this.maxCurrentFilter == null ? this.maxCurrentFilterEmpty: this.maxCurrentFilter,
            this.minCurrentFilter == null ? this.minCurrentFilterEmpty: this.minCurrentFilter,
            this.maxOver30Filter == null ? this.maxOver30FilterEmpty: this.maxOver30Filter,
            this.minOver30Filter == null ? this.minOver30FilterEmpty: this.minOver30Filter,
            this.maxOver60Filter == null ? this.maxOver60FilterEmpty: this.maxOver60Filter,
            this.minOver60Filter == null ? this.minOver60FilterEmpty: this.minOver60Filter,
            this.maxOver90Filter == null ? this.maxOver90FilterEmpty: this.maxOver90Filter,
            this.minOver90Filter == null ? this.minOver90FilterEmpty: this.minOver90Filter,
            this.maxOver120Filter == null ? this.maxOver120FilterEmpty: this.maxOver120Filter,
            this.minOver120Filter == null ? this.minOver120FilterEmpty: this.minOver120Filter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
