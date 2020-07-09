import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BillingEventDetailsServiceProxy, BillingEventDetailDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditBillingEventDetailModalComponent } from './create-or-edit-billingEventDetail-modal.component';
import { ViewBillingEventDetailModalComponent } from './view-billingEventDetail-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './billingEventDetails.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BillingEventDetailsComponent extends AppComponentBase {

    @ViewChild('createOrEditBillingEventDetailModal', { static: true }) createOrEditBillingEventDetailModal: CreateOrEditBillingEventDetailModalComponent;
    @ViewChild('viewBillingEventDetailModalComponent', { static: true }) viewBillingEventDetailModal: ViewBillingEventDetailModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    ruleExecutedSuccessfullyFilter = -1;
    exceptionFilter = '';
        billingRuleNameFilter = '';
        leaseItemItemFilter = '';
        billingEventPurposeFilter = '';


    _entityTypeFullName = 'Ems.Billing.BillingEventDetail';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _billingEventDetailsServiceProxy: BillingEventDetailsServiceProxy,
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

    getBillingEventDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._billingEventDetailsServiceProxy.getAll(
            this.filterText,
            this.ruleExecutedSuccessfullyFilter,
            this.exceptionFilter,
            this.billingRuleNameFilter,
            this.leaseItemItemFilter,
            this.billingEventPurposeFilter,
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

    createBillingEventDetail(): void {
        this.createOrEditBillingEventDetailModal.show();
    }

    showHistory(billingEventDetail: BillingEventDetailDto): void {
        this.entityTypeHistoryModal.show({
            entityId: billingEventDetail.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteBillingEventDetail(billingEventDetail: BillingEventDetailDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._billingEventDetailsServiceProxy.delete(billingEventDetail.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._billingEventDetailsServiceProxy.getBillingEventDetailsToExcel(
        this.filterText,
            this.ruleExecutedSuccessfullyFilter,
            this.exceptionFilter,
            this.billingRuleNameFilter,
            this.leaseItemItemFilter,
            this.billingEventPurposeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
