import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorsServiceProxy, VendorDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditVendorModalComponent } from './create-or-edit-vendor-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './vendors.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class VendorsComponent extends AppComponentBase {

    @ViewChild('createOrEditVendorModal', { static: true }) createOrEditVendorModal: CreateOrEditVendorModalComponent;
    //@ViewChild('viewVendorModalComponent', { static: true }) viewVendorModal: ViewVendorModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    nameFilter = '';
    identifierFilter = '';
    vendorLoc8GUIDFilter = '';
        ssicCodeCodeFilter = '';
        currencyCodeFilter = '';


    _entityTypeFullName = 'Ems.Vendors.Vendor';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _vendorsServiceProxy: VendorsServiceProxy,
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

    getVendors(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._vendorsServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.nameFilter,
            this.identifierFilter,
            this.vendorLoc8GUIDFilter,
            this.ssicCodeCodeFilter,
            this.currencyCodeFilter,
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

    createVendor(): void {
        this.createOrEditVendorModal.show();
    }

    showHistory(vendor: VendorDto): void {
        this.entityTypeHistoryModal.show({
            entityId: vendor.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteVendor(vendor: VendorDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._vendorsServiceProxy.delete(vendor.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._vendorsServiceProxy.getVendorsToExcel(
        this.filterText,
            this.referenceFilter,
            this.nameFilter,
            this.identifierFilter,
            this.vendorLoc8GUIDFilter,
            this.ssicCodeCodeFilter,
            this.currencyCodeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    viewVendor(vendorId): void {
        this._router.navigate(['app/admin/organizations/vendors/viewVendor'], { queryParams: { vendorId: vendorId } });
    }
}
