import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetTypesServiceProxy, CreateOrEditAssetTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditAssetTypeModal',
    templateUrl: './create-or-edit-assetType-modal.component.html'
})
export class CreateOrEditAssetTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetType: CreateOrEditAssetTypeDto = new CreateOrEditAssetTypeDto();



    constructor(
        injector: Injector,
        private _assetTypesServiceProxy: AssetTypesServiceProxy
    ) {
        super(injector);
    }

    show(assetTypeId?: number): void {

        if (!assetTypeId) {
            this.assetType = new CreateOrEditAssetTypeDto();
            this.assetType.id = assetTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._assetTypesServiceProxy.getAssetTypeForEdit(assetTypeId).subscribe(result => {
                this.assetType = result.assetType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._assetTypesServiceProxy.createOrEdit(this.assetType)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
