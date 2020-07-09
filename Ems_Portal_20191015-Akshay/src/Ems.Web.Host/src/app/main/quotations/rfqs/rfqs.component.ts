import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RfqsServiceProxy, RfqDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditRfqModalComponent } from './create-or-edit-rfq-modal.component';
import { ViewRfqModalComponent } from './view-rfq-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './rfqs.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class RfqsComponent extends AppComponentBase {

    @ViewChild('createOrEditRfqModal', { static: true }) createOrEditRfqModal: CreateOrEditRfqModalComponent;
    @ViewChild('viewRfqModalComponent', { static: true }) viewRfqModal: ViewRfqModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    titleFilter = '';
    maxRequestDateFilter : moment.Moment;
		minRequestDateFilter : moment.Moment;
    maxRequiredByFilter : moment.Moment;
		minRequiredByFilter : moment.Moment;
    descriptionFilter = '';
    requirementsFilter = '';
        rfqTypeTypeFilter = '';
        assetOwnerNameFilter = '';
        customerNameFilter = '';
        assetClassClassFilter = '';
        incidentDescriptionFilter = '';
        vendorNameFilter = '';
        userNameFilter = '';


    _entityTypeFullName = 'Ems.Quotations.Rfq';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _rfqsServiceProxy: RfqsServiceProxy,
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

    getRfqs(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._rfqsServiceProxy.getAll(
            this.filterText,
            this.titleFilter,
            this.maxRequestDateFilter,
            this.minRequestDateFilter,
            this.maxRequiredByFilter,
            this.minRequiredByFilter,
            this.descriptionFilter,
            this.requirementsFilter,
            this.rfqTypeTypeFilter,
            this.assetOwnerNameFilter,
            this.customerNameFilter,
            this.assetClassClassFilter,
            this.incidentDescriptionFilter,
            this.vendorNameFilter,
            this.userNameFilter,
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

    createRfq(): void {
        this.createOrEditRfqModal.show();
    }

    showHistory(rfq: RfqDto): void {
        this.entityTypeHistoryModal.show({
            entityId: rfq.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteRfq(rfq: RfqDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._rfqsServiceProxy.delete(rfq.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._rfqsServiceProxy.getRfqsToExcel(
        this.filterText,
            this.titleFilter,
            this.maxRequestDateFilter,
            this.minRequestDateFilter,
            this.maxRequiredByFilter,
            this.minRequiredByFilter,
            this.descriptionFilter,
            this.requirementsFilter,
            this.rfqTypeTypeFilter,
            this.assetOwnerNameFilter,
            this.customerNameFilter,
            this.assetClassClassFilter,
            this.incidentDescriptionFilter,
            this.vendorNameFilter,
            this.userNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
