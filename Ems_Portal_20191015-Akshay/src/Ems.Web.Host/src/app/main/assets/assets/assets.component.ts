import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AssetsServiceProxy, AssetDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAssetModalComponent } from './create-or-edit-asset-modal.component';
import { ViewAssetModalComponent } from './view-asset-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './assets.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AssetsComponent extends AppComponentBase {

    @ViewChild('createOrEditAssetModal', { static: true }) createOrEditAssetModal: CreateOrEditAssetModalComponent;
    @ViewChild('viewAssetModalComponent', { static: true }) viewAssetModal: ViewAssetModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    vehicleRegistrationNoFilter = '';
    isExternalAssetFilter = -1;
    locationFilter = '';
    serialNumberFilter = '';
    engineNoFilter = '';
    chassisNoFilter = '';
    descriptionFilter = '';
    purchaseOrderNoFilter = '';
    maxPurchaseDateFilter : moment.Moment;
		minPurchaseDateFilter : moment.Moment;
    maxPurchaseCostFilter : number;
		maxPurchaseCostFilterEmpty : number;
		minPurchaseCostFilter : number;
		minPurchaseCostFilterEmpty : number;
    assetLoc8GUIDFilter = '';
    //attachmentsFilter = '';
        assetClassClassFilter = '';
        assetStatusStatusFilter = '';


    _entityTypeFullName = 'Ems.Assets.Asset';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _assetsServiceProxy: AssetsServiceProxy,
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

    getAssets(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._assetsServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.vehicleRegistrationNoFilter,
            this.isExternalAssetFilter,
            this.locationFilter,
            this.serialNumberFilter,
            this.engineNoFilter,
            this.chassisNoFilter,
            this.descriptionFilter,
            this.purchaseOrderNoFilter,
            this.maxPurchaseDateFilter,
            this.minPurchaseDateFilter,
            this.maxPurchaseCostFilter == null ? this.maxPurchaseCostFilterEmpty: this.maxPurchaseCostFilter,
            this.minPurchaseCostFilter == null ? this.minPurchaseCostFilterEmpty: this.minPurchaseCostFilter,
            this.assetLoc8GUIDFilter,
            //this.attachmentsFilter,
            this.assetClassClassFilter,
            this.assetStatusStatusFilter,
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

    createAsset(): void {
        this.createOrEditAssetModal.show();
    }

    showHistory(asset: AssetDto): void {
        this.entityTypeHistoryModal.show({
            entityId: asset.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteAsset(asset: AssetDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._assetsServiceProxy.delete(asset.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._assetsServiceProxy.getAssetsToExcel(
        this.filterText,
            this.referenceFilter,
            this.vehicleRegistrationNoFilter,
            this.isExternalAssetFilter,
            this.locationFilter,
            this.serialNumberFilter,
            this.engineNoFilter,
            this.chassisNoFilter,
            this.descriptionFilter,
            this.purchaseOrderNoFilter,
            this.maxPurchaseDateFilter,
            this.minPurchaseDateFilter,
            this.maxPurchaseCostFilter == null ? this.maxPurchaseCostFilterEmpty: this.maxPurchaseCostFilter,
            this.minPurchaseCostFilter == null ? this.minPurchaseCostFilterEmpty: this.minPurchaseCostFilter,
            this.assetLoc8GUIDFilter,
            //this.attachmentsFilter,
            this.assetClassClassFilter,
            this.assetStatusStatusFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }

    viewAsset(assetId): void {
        this._router.navigate(['app/main/assets/assets/viewAsset'], { queryParams: { assetId: assetId } });
    }
}
