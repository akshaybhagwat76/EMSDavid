import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { SsicCodesServiceProxy, CreateOrEditSsicCodeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditSsicCodeModal',
    templateUrl: './create-or-edit-ssicCode-modal.component.html'
})
export class CreateOrEditSsicCodeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    ssicCode: CreateOrEditSsicCodeDto = new CreateOrEditSsicCodeDto();



    constructor(
        injector: Injector,
        private _ssicCodesServiceProxy: SsicCodesServiceProxy
    ) {
        super(injector);
    }

    show(ssicCodeId?: number): void {

        if (!ssicCodeId) {
            this.ssicCode = new CreateOrEditSsicCodeDto();
            this.ssicCode.id = ssicCodeId;

            this.active = true;
            this.modal.show();
        } else {
            this._ssicCodesServiceProxy.getSsicCodeForEdit(ssicCodeId).subscribe(result => {
                this.ssicCode = result.ssicCode;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._ssicCodesServiceProxy.createOrEdit(this.ssicCode)
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
