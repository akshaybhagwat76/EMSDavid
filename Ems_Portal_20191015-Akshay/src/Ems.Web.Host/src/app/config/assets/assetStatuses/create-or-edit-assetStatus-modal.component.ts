import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetStatusesServiceProxy, CreateOrEditAssetStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditAssetStatusModal',
    templateUrl: './create-or-edit-assetStatus-modal.component.html'
})
export class CreateOrEditAssetStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetStatus: CreateOrEditAssetStatusDto = new CreateOrEditAssetStatusDto();



    constructor(
        injector: Injector,
        private _assetStatusesServiceProxy: AssetStatusesServiceProxy
    ) {
        super(injector);
    }

    show(assetStatusId?: number): void {

        if (!assetStatusId) {
            this.assetStatus = new CreateOrEditAssetStatusDto();
            this.assetStatus.id = assetStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._assetStatusesServiceProxy.getAssetStatusForEdit(assetStatusId).subscribe(result => {
                this.assetStatus = result.assetStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._assetStatusesServiceProxy.createOrEdit(this.assetStatus)
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
