import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AssetOwnershipsServiceProxy, AssetOwnershipDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAssetOwnershipModalComponent } from './create-or-edit-assetOwnership-modal.component';
import { ViewAssetOwnershipModalComponent } from './view-assetOwnership-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './assetOwnerships.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AssetOwnershipsComponent extends AppComponentBase {

    @ViewChild('createOrEditAssetOwnershipModal', { static: true }) createOrEditAssetOwnershipModal: CreateOrEditAssetOwnershipModalComponent;
    @ViewChild('viewAssetOwnershipModalComponent', { static: true }) viewAssetOwnershipModal: ViewAssetOwnershipModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    maxStartDateFilter : moment.Moment;
		minStartDateFilter : moment.Moment;
    maxFinishDateFilter : moment.Moment;
		minFinishDateFilter : moment.Moment;
    maxPercentageOwnershipFilter : number;
		maxPercentageOwnershipFilterEmpty : number;
		minPercentageOwnershipFilter : number;
		minPercentageOwnershipFilterEmpty : number;
        assetReferenceFilter = '';
        assetOwnerNameFilter = '';


    _entityTypeFullName = 'Ems.Assets.AssetOwnership';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _assetOwnershipsServiceProxy: AssetOwnershipsServiceProxy,
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

    getAssetOwnerships(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._assetOwnershipsServiceProxy.getAll(
            this.filterText,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxFinishDateFilter,
            this.minFinishDateFilter,
            this.maxPercentageOwnershipFilter == null ? this.maxPercentageOwnershipFilterEmpty: this.maxPercentageOwnershipFilter,
            this.minPercentageOwnershipFilter == null ? this.minPercentageOwnershipFilterEmpty: this.minPercentageOwnershipFilter,
            this.assetReferenceFilter,
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

    createAssetOwnership(): void {
        this.createOrEditAssetOwnershipModal.show();
    }

    showHistory(assetOwnership: AssetOwnershipDto): void {
        this.entityTypeHistoryModal.show({
            entityId: assetOwnership.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteAssetOwnership(assetOwnership: AssetOwnershipDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._assetOwnershipsServiceProxy.delete(assetOwnership.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._assetOwnershipsServiceProxy.getAssetOwnershipsToExcel(
        this.filterText,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxFinishDateFilter,
            this.minFinishDateFilter,
            this.maxPercentageOwnershipFilter == null ? this.maxPercentageOwnershipFilterEmpty: this.maxPercentageOwnershipFilter,
            this.minPercentageOwnershipFilter == null ? this.minPercentageOwnershipFilterEmpty: this.minPercentageOwnershipFilter,
            this.assetReferenceFilter,
            this.assetOwnerNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
