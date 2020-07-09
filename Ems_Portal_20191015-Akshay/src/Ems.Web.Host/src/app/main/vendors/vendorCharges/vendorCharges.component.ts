import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VendorChargesServiceProxy, VendorChargeDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditVendorChargeModalComponent } from './create-or-edit-vendorCharge-modal.component';
import { ViewVendorChargeModalComponent } from './view-vendorCharge-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './vendorCharges.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class VendorChargesComponent extends AppComponentBase {

    @ViewChild('createOrEditVendorChargeModal', { static: true }) createOrEditVendorChargeModal: CreateOrEditVendorChargeModalComponent;
    @ViewChild('viewVendorChargeModalComponent', { static: true }) viewVendorChargeModal: ViewVendorChargeModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    descriptionFilter = '';
    maxDateIssuedFilter : moment.Moment;
	minDateIssuedFilter : moment.Moment;
    maxDateDueFilter : moment.Moment;
	minDateDueFilter : moment.Moment;
    maxTotalTaxFilter : number;
	maxTotalTaxFilterEmpty : number;
	minTotalTaxFilter : number;
	minTotalTaxFilterEmpty : number;
    maxTotalPriceFilter : number;
	maxTotalPriceFilterEmpty : number;
	minTotalPriceFilter : number;
	minTotalPriceFilterEmpty : number;
    vendorNameFilter = '';
    supportContractTitleFilter = '';
    workOrderSubjectFilter = '';
    vendorChargeStatusStatusFilter = '';

    _entityTypeFullName = 'Ems.Vendors.VendorCharge';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _vendorChargesServiceProxy: VendorChargesServiceProxy,
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

    getVendorCharges(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._vendorChargesServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.descriptionFilter,
            this.maxDateIssuedFilter,
            this.minDateIssuedFilter,
            this.maxDateDueFilter,
            this.minDateDueFilter,
            this.maxTotalTaxFilter == null ? this.maxTotalTaxFilterEmpty: this.maxTotalTaxFilter,
            this.minTotalTaxFilter == null ? this.minTotalTaxFilterEmpty: this.minTotalTaxFilter,
            this.maxTotalPriceFilter == null ? this.maxTotalPriceFilterEmpty: this.maxTotalPriceFilter,
            this.minTotalPriceFilter == null ? this.minTotalPriceFilterEmpty: this.minTotalPriceFilter,
            this.vendorNameFilter,
            this.supportContractTitleFilter,
            this.workOrderSubjectFilter,
            this.vendorChargeStatusStatusFilter,
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

    createVendorCharge(): void {
        this.createOrEditVendorChargeModal.show();
    }

    showHistory(vendorCharge: VendorChargeDto): void {
        this.entityTypeHistoryModal.show({
            entityId: vendorCharge.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteVendorCharge(vendorCharge: VendorChargeDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._vendorChargesServiceProxy.delete(vendorCharge.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._vendorChargesServiceProxy.getVendorChargesToExcel(
        this.filterText,
            this.referenceFilter,
            this.descriptionFilter,
            this.maxDateIssuedFilter,
            this.minDateIssuedFilter,
            this.maxDateDueFilter,
            this.minDateDueFilter,
            this.maxTotalTaxFilter == null ? this.maxTotalTaxFilterEmpty: this.maxTotalTaxFilter,
            this.minTotalTaxFilter == null ? this.minTotalTaxFilterEmpty: this.minTotalTaxFilter,
            this.maxTotalPriceFilter == null ? this.maxTotalPriceFilterEmpty: this.maxTotalPriceFilter,
            this.minTotalPriceFilter == null ? this.minTotalPriceFilterEmpty: this.minTotalPriceFilter,
            this.vendorNameFilter,
            this.supportContractTitleFilter,
            this.workOrderSubjectFilter,
            this.vendorChargeStatusStatusFilter
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
