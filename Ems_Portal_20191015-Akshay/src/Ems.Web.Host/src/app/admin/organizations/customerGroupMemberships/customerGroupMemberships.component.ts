import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CustomerGroupMembershipsServiceProxy, CustomerGroupMembershipDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerGroupMembershipModalComponent } from './create-or-edit-customerGroupMembership-modal.component';
import { ViewCustomerGroupMembershipModalComponent } from './view-customerGroupMembership-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './customerGroupMemberships.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CustomerGroupMembershipsComponent extends AppComponentBase {

    @ViewChild('createOrEditCustomerGroupMembershipModal', { static: true }) createOrEditCustomerGroupMembershipModal: CreateOrEditCustomerGroupMembershipModalComponent;
    @ViewChild('viewCustomerGroupMembershipModalComponent', { static: true }) viewCustomerGroupMembershipModal: ViewCustomerGroupMembershipModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    maxDateJoinedFilter : moment.Moment;
		minDateJoinedFilter : moment.Moment;
    maxDateLeftFilter : moment.Moment;
		minDateLeftFilter : moment.Moment;
        customerGroupNameFilter = '';
        customerNameFilter = '';


    _entityTypeFullName = 'Ems.Customers.CustomerGroupMembership';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _customerGroupMembershipsServiceProxy: CustomerGroupMembershipsServiceProxy,
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

    getCustomerGroupMemberships(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._customerGroupMembershipsServiceProxy.getAll(
            this.filterText,
            this.maxDateJoinedFilter,
            this.minDateJoinedFilter,
            this.maxDateLeftFilter,
            this.minDateLeftFilter,
            this.customerGroupNameFilter,
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

    createCustomerGroupMembership(): void {
        this.createOrEditCustomerGroupMembershipModal.show();
    }

    showHistory(customerGroupMembership: CustomerGroupMembershipDto): void {
        this.entityTypeHistoryModal.show({
            entityId: customerGroupMembership.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteCustomerGroupMembership(customerGroupMembership: CustomerGroupMembershipDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._customerGroupMembershipsServiceProxy.delete(customerGroupMembership.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._customerGroupMembershipsServiceProxy.getCustomerGroupMembershipsToExcel(
        this.filterText,
            this.maxDateJoinedFilter,
            this.minDateJoinedFilter,
            this.maxDateLeftFilter,
            this.minDateLeftFilter,
            this.customerGroupNameFilter,
            this.customerNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
