import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetOwnersServiceProxy, CreateOrEditAssetOwnerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AssetOwnerSsicCodeLookupTableModalComponent } from './assetOwner-ssicCode-lookup-table-modal.component';
import { AssetOwnerCurrencyLookupTableModalComponent } from './assetOwner-currency-lookup-table-modal.component';


@Component({
    selector: 'createOrEditAssetOwnerModal',
    templateUrl: './create-or-edit-assetOwner-modal.component.html'
})
export class CreateOrEditAssetOwnerModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('assetOwnerSsicCodeLookupTableModal', { static: true }) assetOwnerSsicCodeLookupTableModal: AssetOwnerSsicCodeLookupTableModalComponent;
    @ViewChild('assetOwnerCurrencyLookupTableModal', { static: true }) assetOwnerCurrencyLookupTableModal: AssetOwnerCurrencyLookupTableModalComponent;

    @Output() assetOwnerModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetOwner: CreateOrEditAssetOwnerDto = new CreateOrEditAssetOwnerDto();

    ssicCodeCode = '';
    currencyCode = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _assetOwnersServiceProxy: AssetOwnersServiceProxy
    ) {
        super(injector);
    }

    show(assetOwnerId?: number): void {

        if (!assetOwnerId) {
            this.assetOwner = new CreateOrEditAssetOwnerDto();
            this.assetOwner.id = assetOwnerId;
            this.ssicCodeCode = '';
            this.currencyCode = '';

            this.active = true;
            this.modal.show();
        } else {
            this._assetOwnersServiceProxy.getAssetOwnerForEdit(assetOwnerId).subscribe(result => {
                this.assetOwner = result.assetOwner;

                this.ssicCodeCode = result.ssicCodeCode;
                this.currencyCode = result.currencyCode;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.assetOwner.reference || !this.assetOwner.name || !this.assetOwner.identifier) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._assetOwnersServiceProxy.createOrEdit(this.assetOwner)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.assetOwnerModalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectSsicCodeModal() {
        this.assetOwnerSsicCodeLookupTableModal.id = this.assetOwner.ssicCodeId;
        this.assetOwnerSsicCodeLookupTableModal.displayName = this.ssicCodeCode;
        this.assetOwnerSsicCodeLookupTableModal.show();
    }
    openSelectCurrencyModal() {
        this.assetOwnerCurrencyLookupTableModal.id = this.assetOwner.currencyId;
        this.assetOwnerCurrencyLookupTableModal.displayName = this.currencyCode;
        this.assetOwnerCurrencyLookupTableModal.show();
    }


    setSsicCodeIdNull() {
        this.assetOwner.ssicCodeId = null;
        this.ssicCodeCode = '';
    }
    setCurrencyIdNull() {
        this.assetOwner.currencyId = null;
        this.currencyCode = '';
    }


    getNewSsicCodeId() {
        this.assetOwner.ssicCodeId = this.assetOwnerSsicCodeLookupTableModal.id;
        this.ssicCodeCode = this.assetOwnerSsicCodeLookupTableModal.displayName;
    }
    getNewCurrencyId() {
        this.assetOwner.currencyId = this.assetOwnerCurrencyLookupTableModal.id;
        this.currencyCode = this.assetOwnerCurrencyLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
