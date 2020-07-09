import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AddressesServiceProxy, CreateOrEditAddressDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCustomerAddressModal',
    templateUrl: './create-or-edit-customer-address-modal.component.html'
})
export class CreateOrEditCustomerAddressModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() addressModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    address: CreateOrEditAddressDto = new CreateOrEditAddressDto();

    customerName = '';
    assetOwnerName = '';
    vendorName = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _addressesServiceProxy: AddressesServiceProxy
    ) {
        super(injector);
    }

    show(addressId?: number, customerId?: number): void {

        if (!addressId) {
            this.address = new CreateOrEditAddressDto();
            this.address.id = addressId;
            this.address.customerId = customerId;
            this.customerName = '';
            this.assetOwnerName = '';
            this.vendorName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._addressesServiceProxy.getAddressForEdit(addressId).subscribe(result => {
                this.address = result.address;
                this.address.customerId = customerId;
                this.customerName = result.customerName;
                this.assetOwnerName = result.assetOwnerName;
                this.vendorName = result.vendorName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.address.addressEntryName || !this.address.addressLine1 || !this.address.postalCode
            || !this.address.city || !this.address.state || !this.address.country) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._addressesServiceProxy.createOrEdit(this.address)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.addressModalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    setCustomerIdNull() {
        this.address.customerId = null;
        this.customerName = '';
    }
    setAssetOwnerIdNull() {
        this.address.assetOwnerId = null;
        this.assetOwnerName = '';
    }
    setVendorIdNull() {
        this.address.vendorId = null;
        this.vendorName = '';
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
