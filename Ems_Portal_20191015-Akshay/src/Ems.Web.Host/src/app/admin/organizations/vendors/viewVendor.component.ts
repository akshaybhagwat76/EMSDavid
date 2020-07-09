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
import { ViewVendorAddressModalComponent } from './view-vendor-address-modal.component';
import { CreateOrEditVendorAddressModalComponent } from './create-or-edit-vendor-address-modal.component';
import { VendorsServiceProxy, AddressesServiceProxy, TokenAuthServiceProxy, VendorDto, AddressDto, GetVendorForViewDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditVendorModalComponent } from './create-or-edit-vendor-modal.component';

@Component({
    selector: 'viewVendor',
    templateUrl: './viewVendor.component.html',
    animations: [appModuleAnimation()]
})
export class ViewVendorComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditVendorModal', { static: true }) createOrEditVendorModal: CreateOrEditVendorModalComponent;
    @ViewChild('createOrEditVendorAddressModal', { static: true }) createOrEditVendorAddressModal: CreateOrEditVendorAddressModalComponent;
    @ViewChild('viewVendorAddressModalComponent', { static: true }) viewVendorAddressModal: ViewVendorAddressModalComponent;
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
        private _vendorsServiceProxy: VendorsServiceProxy,
        private _addressesServiceProxy: AddressesServiceProxy,
        //private _notifyService: NotifyService,
        //private _tokenAuth: TokenAuthServiceProxy,
        //private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    active = false;
    saving = false;

    //item: GetVendorForViewDto;

    vendorId : number;
    vendor: VendorDto;
    currencyCode: string;
    ssic: string;

    ngOnInit(): void {
        this.vendorId = this._activatedRoute.snapshot.queryParams['vendorId'];

        this.vendor = new VendorDto();
        this.currencyCode = '';
        this.ssic = '';
        this.getVendor();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getVendor(event?: LazyLoadEvent) {
         this._vendorsServiceProxy.getVendorForView(this.vendorId)
            .subscribe((vendorResult) => {
                if(vendorResult.vendor == null)
                    {this.close();
                }
                else{
                    this.vendor = vendorResult.vendor;
                    this.currencyCode = vendorResult.currencyCode;
                    this.ssic = vendorResult.ssicCodeCode;
                    this.vendorId = vendorResult.vendor.id;
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
            0,
            0,
            this.vendorId,
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
        this.createOrEditVendorAddressModal.show(null, this.vendorId);
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
        this._router.navigate(['app/admin/organizations/vendors']);
    }
}
