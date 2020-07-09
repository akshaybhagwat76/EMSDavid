import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VendorChargeDetailsServiceProxy, VendorChargeDetailDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditVendorChargeDetailModalComponent } from './create-or-edit-vendorChargeDetail-modal.component';
import { ViewVendorChargeDetailModalComponent } from './view-vendorChargeDetail-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './vendorChargeDetails.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class VendorChargeDetailsComponent extends AppComponentBase {

    @ViewChild('createOrEditVendorChargeDetailModal', { static: true }) createOrEditVendorChargeDetailModal: CreateOrEditVendorChargeDetailModalComponent;
    @ViewChild('viewVendorChargeDetailModalComponent', { static: true }) viewVendorChargeDetailModal: ViewVendorChargeDetailModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    invoiceDetailFilter = '';
    maxQuantityFilter : number;
		maxQuantityFilterEmpty : number;
		minQuantityFilter : number;
		minQuantityFilterEmpty : number;
    maxUnitPriceFilter : number;
		maxUnitPriceFilterEmpty : number;
		minUnitPriceFilter : number;
		minUnitPriceFilterEmpty : number;
    maxTaxFilter : number;
		maxTaxFilterEmpty : number;
		minTaxFilter : number;
		minTaxFilterEmpty : number;
    maxSubTotalFilter : number;
		maxSubTotalFilterEmpty : number;
		minSubTotalFilter : number;
		minSubTotalFilterEmpty : number;


    _entityTypeFullName = 'Ems.Vendors.VendorChargeDetail';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _vendorChargeDetailsServiceProxy: VendorChargeDetailsServiceProxy,
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

    getVendorChargeDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._vendorChargeDetailsServiceProxy.getAll(
            this.filterText,
            this.invoiceDetailFilter,
            this.maxQuantityFilter == null ? this.maxQuantityFilterEmpty: this.maxQuantityFilter,
            this.minQuantityFilter == null ? this.minQuantityFilterEmpty: this.minQuantityFilter,
            this.maxUnitPriceFilter == null ? this.maxUnitPriceFilterEmpty: this.maxUnitPriceFilter,
            this.minUnitPriceFilter == null ? this.minUnitPriceFilterEmpty: this.minUnitPriceFilter,
            this.maxTaxFilter == null ? this.maxTaxFilterEmpty: this.maxTaxFilter,
            this.minTaxFilter == null ? this.minTaxFilterEmpty: this.minTaxFilter,
            this.maxSubTotalFilter == null ? this.maxSubTotalFilterEmpty: this.maxSubTotalFilter,
            this.minSubTotalFilter == null ? this.minSubTotalFilterEmpty: this.minSubTotalFilter,
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

    createVendorChargeDetail(): void {
        this.createOrEditVendorChargeDetailModal.show();
    }

    showHistory(vendorChargeDetail: VendorChargeDetailDto): void {
        this.entityTypeHistoryModal.show({
            entityId: vendorChargeDetail.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteVendorChargeDetail(vendorChargeDetail: VendorChargeDetailDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._vendorChargeDetailsServiceProxy.delete(vendorChargeDetail.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
}
