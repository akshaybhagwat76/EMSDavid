import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetsServiceProxy, CreateOrEditAssetDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AssetAssetClassLookupTableModalComponent } from './asset-assetClass-lookup-table-modal.component';
import { AssetAssetStatusLookupTableModalComponent } from './asset-assetStatus-lookup-table-modal.component';
import { AssetLocationLookupTableModalComponent } from './asset-location-lookup-table-modal.component';


@Component({
    selector: 'createOrEditAssetModal',
    templateUrl: './create-or-edit-asset-modal.component.html'
})
export class CreateOrEditAssetModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('assetAssetClassLookupTableModal', { static: true }) assetAssetClassLookupTableModal: AssetAssetClassLookupTableModalComponent;
    @ViewChild('assetAssetStatusLookupTableModal', { static: true }) assetAssetStatusLookupTableModal: AssetAssetStatusLookupTableModalComponent;
    @ViewChild('assetLocationLookupTableModal', { static: true }) assetLocationLookupTableModal: AssetLocationLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    asset: CreateOrEditAssetDto = new CreateOrEditAssetDto();

    purchaseDate: Date;
    assetClassClass = '';
    assetStatusStatus = '';
    assetLocation = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _assetsServiceProxy: AssetsServiceProxy
    ) {
        super(injector);
    }

    show(assetId?: number): void {
        this.purchaseDate = null;

        if (!assetId) {
            this.asset = new CreateOrEditAssetDto();
            this.asset.id = assetId;
            this.assetClassClass = '';
            this.assetStatusStatus = '';
            this.assetLocation = '';

            this.active = true;
            this.modal.show();
        } else {
            this._assetsServiceProxy.getAssetForEdit(assetId).subscribe(result => {
                this.asset = result.asset;

                if (this.asset.purchaseDate) {
                    this.purchaseDate = this.asset.purchaseDate.toDate();
                }
                this.assetClassClass = result.assetClassClass;
                this.assetStatusStatus = result.assetStatusStatus;
                this.assetLocation = this.asset.location;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {

        if (this.purchaseDate) {
            this.asset.purchaseDate = moment(this.purchaseDate);
        }
        else {
            this.asset.purchaseDate = null;
        }

        if (!this.asset.assetStatusId || !this.asset.reference || !this.asset.description) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._assetsServiceProxy.createOrEdit(this.asset)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.modalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectAssetClassModal() {
        this.assetAssetClassLookupTableModal.id = this.asset.assetClassId;
        this.assetAssetClassLookupTableModal.displayName = this.assetClassClass;
        this.assetAssetClassLookupTableModal.show();
    }
    openSelectAssetStatusModal() {
        this.assetAssetStatusLookupTableModal.id = this.asset.assetStatusId;
        this.assetAssetStatusLookupTableModal.displayName = this.assetStatusStatus;
        this.assetAssetStatusLookupTableModal.show();
    }
    openSelectLocationModal() {
        //this.assetLocationLookupTableModal.id = this.asset.locationId;
        this.assetLocationLookupTableModal.displayName = this.assetLocation;
        this.assetLocationLookupTableModal.show();
    }


    setAssetClassIdNull() {
        this.asset.assetClassId = null;
        this.assetClassClass = '';
    }
    setAssetStatusIdNull() {
        this.asset.assetStatusId = null;
        this.assetStatusStatus = '';
    }
    setLocationIdNull() {
        //this.asset.locationId = null;
        this.assetLocation = this.asset.location = '';
    }


    getNewAssetClassId() {
        this.asset.assetClassId = this.assetAssetClassLookupTableModal.id;
        this.assetClassClass = this.assetAssetClassLookupTableModal.displayName;
    }
    getNewAssetStatusId() {
        this.asset.assetStatusId = this.assetAssetStatusLookupTableModal.id;
        this.assetStatusStatus = this.assetAssetStatusLookupTableModal.displayName;
    }
    getNewLocationId() {
        //this.asset.locationId = this.assetLocationLookupTableModal.id;
        this.assetLocation = this.asset.location = this.assetLocationLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
