import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetPartStatusesServiceProxy, CreateOrEditAssetPartStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditAssetPartStatusModal',
    templateUrl: './create-or-edit-assetPartStatus-modal.component.html'
})
export class CreateOrEditAssetPartStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    assetPartStatus: CreateOrEditAssetPartStatusDto = new CreateOrEditAssetPartStatusDto();



    constructor(
        injector: Injector,
        private _assetPartStatusesServiceProxy: AssetPartStatusesServiceProxy
    ) {
        super(injector);
    }

    show(assetPartStatusId?: number): void {

        if (!assetPartStatusId) {
            this.assetPartStatus = new CreateOrEditAssetPartStatusDto();
            this.assetPartStatus.id = assetPartStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._assetPartStatusesServiceProxy.getAssetPartStatusForEdit(assetPartStatusId).subscribe(result => {
                this.assetPartStatus = result.assetPartStatus;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._assetPartStatusesServiceProxy.createOrEdit(this.assetPartStatus)
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
