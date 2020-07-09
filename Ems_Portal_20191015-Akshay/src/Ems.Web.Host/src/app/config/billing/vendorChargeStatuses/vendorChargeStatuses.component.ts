import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VendorChargeStatusesServiceProxy, VendorChargeStatusDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditVendorChargeStatusModalComponent } from './create-or-edit-vendorChargeStatus-modal.component';
import { ViewVendorChargeStatusModalComponent } from './view-vendorChargeStatus-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './vendorChargeStatuses.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class VendorChargeStatusesComponent extends AppComponentBase {

    @ViewChild('createOrEditVendorChargeStatusModal', { static: true }) createOrEditVendorChargeStatusModal: CreateOrEditVendorChargeStatusModalComponent;
    @ViewChild('viewVendorChargeStatusModalComponent', { static: true }) viewVendorChargeStatusModal: ViewVendorChargeStatusModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    statusFilter = '';
    descriptionFilter = '';




    constructor(
        injector: Injector,
        private _vendorChargeStatusesServiceProxy: VendorChargeStatusesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getVendorChargeStatuses(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._vendorChargeStatusesServiceProxy.getAll(
            this.filterText,
            this.statusFilter,
            this.descriptionFilter,
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

    createVendorChargeStatus(): void {
        this.createOrEditVendorChargeStatusModal.show();
    }

    deleteVendorChargeStatus(vendorChargeStatus: VendorChargeStatusDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._vendorChargeStatusesServiceProxy.delete(vendorChargeStatus.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._vendorChargeStatusesServiceProxy.getVendorChargeStatusesToExcel(
        this.filterText,
            this.statusFilter,
            this.descriptionFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
