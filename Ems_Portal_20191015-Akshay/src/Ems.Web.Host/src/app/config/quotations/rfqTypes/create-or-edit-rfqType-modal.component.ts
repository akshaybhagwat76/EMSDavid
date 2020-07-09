import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { RfqTypesServiceProxy, CreateOrEditRfqTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditRfqTypeModal',
    templateUrl: './create-or-edit-rfqType-modal.component.html'
})
export class CreateOrEditRfqTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    rfqType: CreateOrEditRfqTypeDto = new CreateOrEditRfqTypeDto();



    constructor(
        injector: Injector,
        private _rfqTypesServiceProxy: RfqTypesServiceProxy
    ) {
        super(injector);
    }

    show(rfqTypeId?: number): void {

        if (!rfqTypeId) {
            this.rfqType = new CreateOrEditRfqTypeDto();
            this.rfqType.id = rfqTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._rfqTypesServiceProxy.getRfqTypeForEdit(rfqTypeId).subscribe(result => {
                this.rfqType = result.rfqType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._rfqTypesServiceProxy.createOrEdit(this.rfqType)
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
