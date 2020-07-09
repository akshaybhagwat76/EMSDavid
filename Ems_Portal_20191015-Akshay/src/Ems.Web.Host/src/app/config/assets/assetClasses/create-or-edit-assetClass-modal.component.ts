import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetClassesServiceProxy, CreateOrEditAssetClassDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AssetClassAssetTypeLookupTableModalComponent } from './assetClass-assetType-lookup-table-modal.component';


@Component({
    selector: 'createOrEditAssetClassModal',
    templateUrl: './create-or-edit-assetClass-modal.component.html'
})
export class CreateOrEditAssetClassModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('assetClassAssetTypeLookupTableModal', { static: true }) assetClassAssetTypeLookupTableModal: AssetClassAssetTypeLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetClass: CreateOrEditAssetClassDto = new CreateOrEditAssetClassDto();

    assetTypeType = '';


    constructor(
        injector: Injector,
        private _assetClassesServiceProxy: AssetClassesServiceProxy
    ) {
        super(injector);
    }

    show(assetClassId?: number): void {

        if (!assetClassId) {
            this.assetClass = new CreateOrEditAssetClassDto();
            this.assetClass.id = assetClassId;
            this.assetTypeType = '';

            this.active = true;
            this.modal.show();
        } else {
            this._assetClassesServiceProxy.getAssetClassForEdit(assetClassId).subscribe(result => {
                this.assetClass = result.assetClass;

                this.assetTypeType = result.assetTypeType;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._assetClassesServiceProxy.createOrEdit(this.assetClass)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

        openSelectAssetTypeModal() {
        this.assetClassAssetTypeLookupTableModal.id = this.assetClass.assetTypeId;
        this.assetClassAssetTypeLookupTableModal.displayName = this.assetTypeType;
        this.assetClassAssetTypeLookupTableModal.show();
    }


        setAssetTypeIdNull() {
        this.assetClass.assetTypeId = null;
        this.assetTypeType = '';
    }


        getNewAssetTypeId() {
        this.assetClass.assetTypeId = this.assetClassAssetTypeLookupTableModal.id;
        this.assetTypeType = this.assetClassAssetTypeLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
