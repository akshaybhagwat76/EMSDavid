import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BillingEventsServiceProxy, BillingEventDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditBillingEventModalComponent } from './create-or-edit-billingEvent-modal.component';
import { ViewBillingEventModalComponent } from './view-billingEvent-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './billingEvents.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BillingEventsComponent extends AppComponentBase {

    @ViewChild('createOrEditBillingEventModal', { static: true }) createOrEditBillingEventModal: CreateOrEditBillingEventModalComponent;
    @ViewChild('viewBillingEventModalComponent', { static: true }) viewBillingEventModal: ViewBillingEventModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    maxBillingEventDateFilter : moment.Moment;
		minBillingEventDateFilter : moment.Moment;
    triggeredByFilter = '';
    purposeFilter = '';
    wasInvoiceGeneratedFilter = -1;
        leaseAgreementTitleFilter = '';
        vendorChargeReferenceFilter = '';
        billingEventTypeTypeFilter = '';


    _entityTypeFullName = 'Ems.Billing.BillingEvent';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _billingEventsServiceProxy: BillingEventsServiceProxy,
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

    getBillingEvents(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._billingEventsServiceProxy.getAll(
            this.filterText,
            this.maxBillingEventDateFilter,
            this.minBillingEventDateFilter,
            this.triggeredByFilter,
            this.purposeFilter,
            this.wasInvoiceGeneratedFilter,
            this.leaseAgreementTitleFilter,
            this.vendorChargeReferenceFilter,
            this.billingEventTypeTypeFilter,
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

    createBillingEvent(): void {
        this.createOrEditBillingEventModal.show();
    }

    showHistory(billingEvent: BillingEventDto): void {
        this.entityTypeHistoryModal.show({
            entityId: billingEvent.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteBillingEvent(billingEvent: BillingEventDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._billingEventsServiceProxy.delete(billingEvent.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._billingEventsServiceProxy.getBillingEventsToExcel(
        this.filterText,
            this.maxBillingEventDateFilter,
            this.minBillingEventDateFilter,
            this.triggeredByFilter,
            this.purposeFilter,
            this.wasInvoiceGeneratedFilter,
            this.leaseAgreementTitleFilter,
            this.vendorChargeReferenceFilter,
            this.billingEventTypeTypeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
