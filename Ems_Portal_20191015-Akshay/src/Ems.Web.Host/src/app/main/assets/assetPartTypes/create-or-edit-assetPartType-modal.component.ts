import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetPartTypesServiceProxy, CreateOrEditAssetPartTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditAssetPartTypeModal',
    templateUrl: './create-or-edit-assetPartType-modal.component.html'
})
export class CreateOrEditAssetPartTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetPartType: CreateOrEditAssetPartTypeDto = new CreateOrEditAssetPartTypeDto();



    constructor(
        injector: Injector,
        private _assetPartTypesServiceProxy: AssetPartTypesServiceProxy
    ) {
        super(injector);
    }

    show(assetPartTypeId?: number): void {

        if (!assetPartTypeId) {
            this.assetPartType = new CreateOrEditAssetPartTypeDto();
            this.assetPartType.id = assetPartTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._assetPartTypesServiceProxy.getAssetPartTypeForEdit(assetPartTypeId).subscribe(result => {
                this.assetPartType = result.assetPartType;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._assetPartTypesServiceProxy.createOrEdit(this.assetPartType)
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
