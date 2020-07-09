import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SupportContractsServiceProxy, SupportContractDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditSupportContractModalComponent } from './create-or-edit-supportContract-modal.component';
import { ViewSupportContractModalComponent } from './view-supportContract-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './supportContracts.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class SupportContractsComponent extends AppComponentBase {

    @ViewChild('createOrEditSupportContractModal', { static: true }) createOrEditSupportContractModal: CreateOrEditSupportContractModalComponent;
    @ViewChild('viewSupportContractModalComponent', { static: true }) viewSupportContractModal: ViewSupportContractModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    titleFilter = '';
    referenceFilter = '';
    descriptionFilter = '';
    maxStartDateFilter : moment.Moment;
		minStartDateFilter : moment.Moment;
    maxEndDateFilter : moment.Moment;
		minEndDateFilter : moment.Moment;
    //attachmentsFilter = '';
    isRFQTemplateFilter = -1;
    isAcknowledgedFilter = -1;
    acknowledgedByFilter = '';
    maxAcknowledgedAtFilter : moment.Moment;
		minAcknowledgedAtFilter : moment.Moment;
        vendorNameFilter = '';
        assetOwnerNameFilter = '';


    _entityTypeFullName = 'Ems.Support.SupportContract';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _supportContractsServiceProxy: SupportContractsServiceProxy,
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

    getSupportContracts(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._supportContractsServiceProxy.getAll(
            this.filterText,
            this.titleFilter,
            this.referenceFilter,
            this.descriptionFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            //this.attachmentsFilter,
            this.isRFQTemplateFilter,
            this.isAcknowledgedFilter,
            this.acknowledgedByFilter,
            this.maxAcknowledgedAtFilter,
            this.minAcknowledgedAtFilter,
            this.vendorNameFilter,
            this.assetOwnerNameFilter,
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

    createSupportContract(): void {
        this.createOrEditSupportContractModal.show();
    }

    showHistory(supportContract: SupportContractDto): void {
        this.entityTypeHistoryModal.show({
            entityId: supportContract.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteSupportContract(supportContract: SupportContractDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._supportContractsServiceProxy.delete(supportContract.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._supportContractsServiceProxy.getSupportContractsToExcel(
        this.filterText,
            this.titleFilter,
            this.referenceFilter,
            this.descriptionFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            //this.attachmentsFilter,
            this.isRFQTemplateFilter,
            this.isAcknowledgedFilter,
            this.acknowledgedByFilter,
            this.maxAcknowledgedAtFilter,
            this.minAcknowledgedAtFilter,
            this.vendorNameFilter,
            this.assetOwnerNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }

    viewSupportContract(supportContractId): void {
        this._router.navigate(['app/main/support/supportContracts/viewSupportContract'], { queryParams: { supportContractId: supportContractId } });
    }
}
