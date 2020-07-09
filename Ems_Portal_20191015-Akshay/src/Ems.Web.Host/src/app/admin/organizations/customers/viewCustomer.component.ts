import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
//import { NotifyService } from 'abp-ng2-module/dist/src/notify/notify.service';
//import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { ViewCustomerAddressModalComponent } from './view-customer-address-modal.component';
import { CreateOrEditCustomerAddressModalComponent } from './create-or-edit-customer-address-modal.component';
import { CustomersServiceProxy, AddressesServiceProxy, TokenAuthServiceProxy, CustomerDto, AddressDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerModalComponent } from './create-or-edit-customer-modal.component';

@Component({
    selector: 'viewCustomer',
    templateUrl: './viewCustomer.component.html',
    animations: [appModuleAnimation()]
})
export class ViewCustomerComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditCustomerModal', { static: true }) createOrEditCustomerModal: CreateOrEditCustomerModalComponent;
    @ViewChild('createOrEditCustomerAddressModal', { static: true }) createOrEditCustomerAddressModal: CreateOrEditCustomerAddressModalComponent;
    @ViewChild('viewCustomerAddressModalComponent', { static: true }) viewCustomerAddressModal: ViewCustomerAddressModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    addressEntryNameFilter = '';
    isHeadOfficeFilter = -1;
    addressLine1Filter = '';
    addressLine2Filter = '';
    postalCodeFilter = '';
    cityFilter = '';
    stateFilter = '';
    countryFilter = '';
    addressLoc8GUIDFilter = '';
    isDefaultForBillingFilter = -1;
    isDefaultForShippingFilter = -1;
    customerNameFilter = '';
    assetOwnerNameFilter = '';
    vendorNameFilter = '';

    _entityTypeFullName = 'Ems.Organizations.Address';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _customersServiceProxy: CustomersServiceProxy,
        private _addressesServiceProxy: AddressesServiceProxy,
        //private _notifyService: NotifyService,
        //private _tokenAuth: TokenAuthServiceProxy,
        //private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    active = false;
    saving = false;

    //item: GetCustomerForViewDto;

    customerId : number;
    customer: CustomerDto;
    customerType: string;
    currencyCode: string;

    ngOnInit(): void {

        this.customerId = this._activatedRoute.snapshot.queryParams['customerId'];
        this.customer = new CustomerDto();
        this.currencyCode = '';
        this.customerType='';
        this.getCustomer();

        //this.ssic = ''; <----------- TODO - need to fix the ENTITY
        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }
    
    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }
    
    getCustomer(){
        this._customersServiceProxy.getCustomerForView(this.customerId)
            .subscribe((customerResult) => { 

                if(customerResult.customer == null)
                    {this.close();
                }
                else{
                    this.customer = customerResult.customer;
                    this.customerType = customerResult.customerTypeType;
                    this.currencyCode = customerResult.currencyCode;
                    this.customerId = customerResult.customer.id;
                }
            });
    }

    getAddresses(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._addressesServiceProxy.getAll(
            this.filterText,
            this.addressEntryNameFilter,
            this.isHeadOfficeFilter,
            this.addressLine1Filter,
            this.addressLine2Filter,
            this.postalCodeFilter,
            this.cityFilter,
            this.stateFilter,
            this.countryFilter,
            this.addressLoc8GUIDFilter,
            this.isDefaultForBillingFilter,
            this.isDefaultForShippingFilter,
            this.customerNameFilter,
            this.assetOwnerNameFilter,
            this.vendorNameFilter,
            this.customerId,
            0,
            0,
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

    createAddress(): void {
        this.createOrEditCustomerAddressModal.show(null, this.customerId);
    }

    showHistory(address: AddressDto): void {
        this.entityTypeHistoryModal.show({
            entityId: address.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteAddress(address: AddressDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._addressesServiceProxy.delete(address.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    close(): void {
        this._router.navigate(['app/admin/organizations/customers']);
    }
}
