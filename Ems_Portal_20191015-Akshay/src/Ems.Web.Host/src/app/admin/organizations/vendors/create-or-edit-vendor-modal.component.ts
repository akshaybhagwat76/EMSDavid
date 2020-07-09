import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { VendorsServiceProxy, CreateOrEditVendorDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { VendorSsicCodeLookupTableModalComponent } from './vendor-ssicCode-lookup-table-modal.component';
import { VendorCurrencyLookupTableModalComponent } from './vendor-currency-lookup-table-modal.component';


@Component({
    selector: 'createOrEditVendorModal',
    templateUrl: './create-or-edit-vendor-modal.component.html'
})
export class CreateOrEditVendorModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('vendorSsicCodeLookupTableModal', { static: true }) vendorSsicCodeLookupTableModal: VendorSsicCodeLookupTableModalComponent;
    @ViewChild('vendorCurrencyLookupTableModal', { static: true }) vendorCurrencyLookupTableModal: VendorCurrencyLookupTableModalComponent;


    @Output() vendorModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    vendor: CreateOrEditVendorDto = new CreateOrEditVendorDto();

    ssicCodeCode = '';
    currencyCode = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _vendorsServiceProxy: VendorsServiceProxy
    ) {
        super(injector);
    }

    show(vendorId?: number): void {

        if (!vendorId) {
            this.vendor = new CreateOrEditVendorDto();
            this.vendor.id = vendorId;
            this.ssicCodeCode = '';
            this.currencyCode = '';

            this.active = true;
            this.modal.show();
        } else {
            this._vendorsServiceProxy.getVendorForEdit(vendorId).subscribe(result => {
                this.vendor = result.vendor;

                this.ssicCodeCode = result.ssicCodeCode;
                this.currencyCode = result.currencyCode;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.vendor.reference || !this.vendor.name || !this.vendor.identifier) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._vendorsServiceProxy.createOrEdit(this.vendor)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.vendorModalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectSsicCodeModal() {
        this.vendorSsicCodeLookupTableModal.id = this.vendor.ssicCodeId;
        this.vendorSsicCodeLookupTableModal.displayName = this.ssicCodeCode;
        this.vendorSsicCodeLookupTableModal.show();
    }
    openSelectCurrencyModal() {
        this.vendorCurrencyLookupTableModal.id = this.vendor.currencyId;
        this.vendorCurrencyLookupTableModal.displayName = this.currencyCode;
        this.vendorCurrencyLookupTableModal.show();
    }


    setSsicCodeIdNull() {
        this.vendor.ssicCodeId = null;
        this.ssicCodeCode = '';
    }
    setCurrencyIdNull() {
        this.vendor.currencyId = null;
        this.currencyCode = '';
    }


    getNewSsicCodeId() {
        this.vendor.ssicCodeId = this.vendorSsicCodeLookupTableModal.id;
        this.ssicCodeCode = this.vendorSsicCodeLookupTableModal.displayName;
    }
    getNewCurrencyId() {
        this.vendor.currencyId = this.vendorCurrencyLookupTableModal.id;
        this.currencyCode = this.vendorCurrencyLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
