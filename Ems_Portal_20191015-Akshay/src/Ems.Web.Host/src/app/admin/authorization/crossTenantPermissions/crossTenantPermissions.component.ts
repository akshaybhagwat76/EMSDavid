import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CrossTenantPermissionsServiceProxy, CrossTenantPermissionDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCrossTenantPermissionModalComponent } from './create-or-edit-crossTenantPermission-modal.component';
import { ViewCrossTenantPermissionModalComponent } from './view-crossTenantPermission-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './crossTenantPermissions.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CrossTenantPermissionsComponent extends AppComponentBase {

    @ViewChild('createOrEditCrossTenantPermissionModal', { static: true }) createOrEditCrossTenantPermissionModal: CreateOrEditCrossTenantPermissionModalComponent;
    @ViewChild('viewCrossTenantPermissionModalComponent', { static: true }) viewCrossTenantPermissionModal: ViewCrossTenantPermissionModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    maxTenantRefIdFilter : number;
		maxTenantRefIdFilterEmpty : number;
		minTenantRefIdFilter : number;
		minTenantRefIdFilterEmpty : number;
    entityTypeFilter = '';
    tenantsFilter = '';
    tenantTypeFilter = '';




    constructor(
        injector: Injector,
        private _crossTenantPermissionsServiceProxy: CrossTenantPermissionsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getCrossTenantPermissions(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._crossTenantPermissionsServiceProxy.getAll(
            this.filterText,
            this.maxTenantRefIdFilter == null ? this.maxTenantRefIdFilterEmpty: this.maxTenantRefIdFilter,
            this.minTenantRefIdFilter == null ? this.minTenantRefIdFilterEmpty: this.minTenantRefIdFilter,
            this.entityTypeFilter,
            this.tenantsFilter,
            this.tenantTypeFilter,
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

    createCrossTenantPermission(): void {
        this.createOrEditCrossTenantPermissionModal.show();
    }

    deleteCrossTenantPermission(crossTenantPermission: CrossTenantPermissionDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._crossTenantPermissionsServiceProxy.delete(crossTenantPermission.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._crossTenantPermissionsServiceProxy.getCrossTenantPermissionsToExcel(
        this.filterText,
            this.maxTenantRefIdFilter == null ? this.maxTenantRefIdFilterEmpty: this.maxTenantRefIdFilter,
            this.minTenantRefIdFilter == null ? this.minTenantRefIdFilterEmpty: this.minTenantRefIdFilter,
            this.entityTypeFilter,
            this.tenantsFilter,
            this.tenantTypeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
