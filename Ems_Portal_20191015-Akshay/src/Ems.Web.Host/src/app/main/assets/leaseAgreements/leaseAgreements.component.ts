import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LeaseAgreementsServiceProxy, LeaseAgreementDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditLeaseAgreementModalComponent } from './create-or-edit-leaseAgreement-modal.component';
import { ViewLeaseAgreementModalComponent } from './view-leaseAgreement-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './leaseAgreements.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class LeaseAgreementsComponent extends AppComponentBase {

    @ViewChild('createOrEditLeaseAgreementModal', { static: true }) createOrEditLeaseAgreementModal: CreateOrEditLeaseAgreementModalComponent;
    @ViewChild('viewLeaseAgreementModalComponent', { static: true }) viewLeaseAgreementModal: ViewLeaseAgreementModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    descriptionFilter = '';
    titleFilter = '';
    termsFilter = '';
    maxStartDateFilter: moment.Moment;
    minStartDateFilter: moment.Moment;
    maxEndDateFilter: moment.Moment;
    minEndDateFilter: moment.Moment;
    contactContactNameFilter = '';
    assetOwnerNameFilter = '';
    customerNameFilter = '';


    _entityTypeFullName = 'Ems.Assets.LeaseAgreement';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _leaseAgreementsServiceProxy: LeaseAgreementsServiceProxy,
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

    getLeaseAgreements(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._leaseAgreementsServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.descriptionFilter,
            this.titleFilter,
            this.termsFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.contactContactNameFilter,
            this.assetOwnerNameFilter,
            this.customerNameFilter,
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

    createLeaseAgreement(): void {
        this.createOrEditLeaseAgreementModal.show();
    }

    showHistory(leaseAgreement: LeaseAgreementDto): void {
        this.entityTypeHistoryModal.show({
            entityId: leaseAgreement.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteLeaseAgreement(leaseAgreement: LeaseAgreementDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._leaseAgreementsServiceProxy.delete(leaseAgreement.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._leaseAgreementsServiceProxy.getLeaseAgreementsToExcel(
            this.filterText,
            this.referenceFilter,
            this.descriptionFilter,
            this.titleFilter,
            this.termsFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.contactContactNameFilter,
            this.assetOwnerNameFilter,
            this.customerNameFilter,
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }

    viewLeaseAgreement(leaseAgreementId): void {
        this._router.navigate(['app/main/assets/leaseAgreements/viewLeaseAgreement'], { queryParams: { leaseAgreementId: leaseAgreementId } });
    }
}
