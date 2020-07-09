import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetOwnershipsServiceProxy, CreateOrEditAssetOwnershipDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AssetOwnershipAssetLookupTableModalComponent } from './assetOwnership-asset-lookup-table-modal.component';
import { AssetOwnershipAssetOwnerLookupTableModalComponent } from './assetOwnership-assetOwner-lookup-table-modal.component';


@Component({
    selector: 'createOrEditAssetOwnershipModal',
    templateUrl: './create-or-edit-assetOwnership-modal.component.html'
})
export class CreateOrEditAssetOwnershipModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('assetOwnershipAssetLookupTableModal', { static: true }) assetOwnershipAssetLookupTableModal: AssetOwnershipAssetLookupTableModalComponent;
    @ViewChild('assetOwnershipAssetOwnerLookupTableModal', { static: true }) assetOwnershipAssetOwnerLookupTableModal: AssetOwnershipAssetOwnerLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetOwnership: CreateOrEditAssetOwnershipDto = new CreateOrEditAssetOwnershipDto();

            startDate: Date;
            finishDate: Date;
    assetReference = '';
    assetOwnerName = '';


    constructor(
        injector: Injector,
        private _assetOwnershipsServiceProxy: AssetOwnershipsServiceProxy
    ) {
        super(injector);
    }

    show(assetOwnershipId?: number): void {
this.startDate = null;
this.finishDate = null;

        if (!assetOwnershipId) {
            this.assetOwnership = new CreateOrEditAssetOwnershipDto();
            this.assetOwnership.id = assetOwnershipId;
            this.assetReference = '';
            this.assetOwnerName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._assetOwnershipsServiceProxy.getAssetOwnershipForEdit(assetOwnershipId).subscribe(result => {
                this.assetOwnership = result.assetOwnership;

                if (this.assetOwnership.startDate) {
					this.startDate = this.assetOwnership.startDate.toDate();
                }
                if (this.assetOwnership.finishDate) {
					this.finishDate = this.assetOwnership.finishDate.toDate();
                }
                this.assetReference = result.assetReference;
                this.assetOwnerName = result.assetOwnerName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
        if (this.startDate) {
            if (!this.assetOwnership.startDate) {
                this.assetOwnership.startDate = moment(this.startDate).startOf('day');
            }
            else {
                this.assetOwnership.startDate = moment(this.startDate);
            }
        }
        else {
            this.assetOwnership.startDate = null;
        }
        if (this.finishDate) {
            if (!this.assetOwnership.finishDate) {
                this.assetOwnership.finishDate = moment(this.finishDate).startOf('day');
            }
            else {
                this.assetOwnership.finishDate = moment(this.finishDate);
            }
        }
        else {
            this.assetOwnership.finishDate = null;
        }
            this._assetOwnershipsServiceProxy.createOrEdit(this.assetOwnership)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

        openSelectAssetModal() {
        this.assetOwnershipAssetLookupTableModal.id = this.assetOwnership.assetId;
        this.assetOwnershipAssetLookupTableModal.displayName = this.assetReference;
        this.assetOwnershipAssetLookupTableModal.show();
    }
        openSelectAssetOwnerModal() {
        this.assetOwnershipAssetOwnerLookupTableModal.id = this.assetOwnership.assetOwnerId;
        this.assetOwnershipAssetOwnerLookupTableModal.displayName = this.assetOwnerName;
        this.assetOwnershipAssetOwnerLookupTableModal.show();
    }


        setAssetIdNull() {
        this.assetOwnership.assetId = null;
        this.assetReference = '';
    }
        setAssetOwnerIdNull() {
        this.assetOwnership.assetOwnerId = null;
        this.assetOwnerName = '';
    }


        getNewAssetId() {
        this.assetOwnership.assetId = this.assetOwnershipAssetLookupTableModal.id;
        this.assetReference = this.assetOwnershipAssetLookupTableModal.displayName;
    }
        getNewAssetOwnerId() {
        this.assetOwnership.assetOwnerId = this.assetOwnershipAssetOwnerLookupTableModal.id;
        this.assetOwnerName = this.assetOwnershipAssetOwnerLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
