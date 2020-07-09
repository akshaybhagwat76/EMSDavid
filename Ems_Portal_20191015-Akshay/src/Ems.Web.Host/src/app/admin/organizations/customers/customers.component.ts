import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomersServiceProxy, CustomerDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerModalComponent } from './create-or-edit-customer-modal.component';
//import { ViewCustomerModalComponent } from './view-customer-modal.component';
import { ViewCustomerComponent } from './viewCustomer.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './customers.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CustomersComponent extends AppComponentBase {

    @ViewChild('createOrEditCustomerModal', { static: true }) createOrEditCustomerModal: CreateOrEditCustomerModalComponent;
    //@ViewChild('viewCustomer', { static: true }) viewCustomer: ViewCustomerComponent;
    //@ViewChild('viewCustomerModalComponent', { static: true }) viewCustomerModal: ViewCustomerModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    referenceFilter = '';
    nameFilter = '';
    identifierFilter = '';
    customerLoc8UUIDFilter = '';
    customerTypeTypeFilter = '';
    currencyCodeFilter = '';

    _entityTypeFullName = 'Ems.Customers.Customer';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _customersServiceProxy: CustomersServiceProxy,
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

    getCustomers(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._customersServiceProxy.getAll(
            this.filterText,
            this.referenceFilter,
            this.nameFilter,
            this.identifierFilter,
            this.customerLoc8UUIDFilter,
            this.customerTypeTypeFilter,
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

    createCustomer(): void {
        this.createOrEditCustomerModal.show();
    }

    showHistory(customer: CustomerDto): void {
        this.entityTypeHistoryModal.show({
            entityId: customer.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteCustomer(customer: CustomerDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._customersServiceProxy.delete(customer.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._customersServiceProxy.getCustomersToExcel(
        this.filterText,
            this.referenceFilter,
            this.nameFilter,
            this.identifierFilter,
            this.customerLoc8UUIDFilter,
            this.customerTypeTypeFilter,
            this.currencyCodeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }

    viewCustomer(customerId): void {
        this._router.navigate(['app/admin/organizations/customers/viewCustomer'], { queryParams: { customerId: customerId } });
    }
}
