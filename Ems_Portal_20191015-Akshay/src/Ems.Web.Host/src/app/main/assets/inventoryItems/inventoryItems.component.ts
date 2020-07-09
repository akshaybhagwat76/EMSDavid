import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InventoryItemsServiceProxy, InventoryItemDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditInventoryItemModalComponent } from './create-or-edit-inventoryItem-modal.component';
import { ViewInventoryItemModalComponent } from './view-inventoryItem-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './inventoryItems.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class InventoryItemsComponent extends AppComponentBase {

    @ViewChild('createOrEditInventoryItemModal', { static: true }) createOrEditInventoryItemModal: CreateOrEditInventoryItemModalComponent;
    @ViewChild('viewInventoryItemModalComponent', { static: true }) viewInventoryItemModal: ViewInventoryItemModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';
    referenceFilter = '';
    maxQtyInWarehouseFilter : number;
		maxQtyInWarehouseFilterEmpty : number;
		minQtyInWarehouseFilter : number;
		minQtyInWarehouseFilterEmpty : number;
    maxRestockLimitFilter : number;
		maxRestockLimitFilterEmpty : number;
		minRestockLimitFilter : number;
		minRestockLimitFilterEmpty : number;
    maxQtyOnOrderFilter : number;
		maxQtyOnOrderFilterEmpty : number;
		minQtyOnOrderFilter : number;
		minQtyOnOrderFilterEmpty : number;
        itemTypeTypeFilter = '';
        assetReferenceFilter = '';




    constructor(
        injector: Injector,
        private _inventoryItemsServiceProxy: InventoryItemsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getInventoryItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._inventoryItemsServiceProxy.getAll(
            this.filterText,
            this.nameFilter,
            this.referenceFilter,
            this.maxQtyInWarehouseFilter == null ? this.maxQtyInWarehouseFilterEmpty: this.maxQtyInWarehouseFilter,
            this.minQtyInWarehouseFilter == null ? this.minQtyInWarehouseFilterEmpty: this.minQtyInWarehouseFilter,
            this.maxRestockLimitFilter == null ? this.maxRestockLimitFilterEmpty: this.maxRestockLimitFilter,
            this.minRestockLimitFilter == null ? this.minRestockLimitFilterEmpty: this.minRestockLimitFilter,
            this.maxQtyOnOrderFilter == null ? this.maxQtyOnOrderFilterEmpty: this.maxQtyOnOrderFilter,
            this.minQtyOnOrderFilter == null ? this.minQtyOnOrderFilterEmpty: this.minQtyOnOrderFilter,
            this.itemTypeTypeFilter,
            this.assetReferenceFilter,
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

    createInventoryItem(): void {
        this.createOrEditInventoryItemModal.show();
    }

    deleteInventoryItem(inventoryItem: InventoryItemDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._inventoryItemsServiceProxy.delete(inventoryItem.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._inventoryItemsServiceProxy.getInventoryItemsToExcel(
        this.filterText,
            this.nameFilter,
            this.referenceFilter,
            this.maxQtyInWarehouseFilter == null ? this.maxQtyInWarehouseFilterEmpty: this.maxQtyInWarehouseFilter,
            this.minQtyInWarehouseFilter == null ? this.minQtyInWarehouseFilterEmpty: this.minQtyInWarehouseFilter,
            this.maxRestockLimitFilter == null ? this.maxRestockLimitFilterEmpty: this.maxRestockLimitFilter,
            this.minRestockLimitFilter == null ? this.minRestockLimitFilterEmpty: this.minRestockLimitFilter,
            this.maxQtyOnOrderFilter == null ? this.maxQtyOnOrderFilterEmpty: this.maxQtyOnOrderFilter,
            this.minQtyOnOrderFilter == null ? this.minQtyOnOrderFilterEmpty: this.minQtyOnOrderFilter,
            this.itemTypeTypeFilter,
            this.assetReferenceFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
