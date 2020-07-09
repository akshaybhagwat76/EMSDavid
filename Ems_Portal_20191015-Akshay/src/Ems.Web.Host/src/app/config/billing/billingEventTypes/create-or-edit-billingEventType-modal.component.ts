import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { BillingEventTypesServiceProxy, CreateOrEditBillingEventTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditBillingEventTypeModal',
    templateUrl: './create-or-edit-billingEventType-modal.component.html'
})
export class CreateOrEditBillingEventTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billingEventType: CreateOrEditBillingEventTypeDto = new CreateOrEditBillingEventTypeDto();



    constructor(
        injector: Injector,
        private _billingEventTypesServiceProxy: BillingEventTypesServiceProxy
    ) {
        super(injector);
    }

    show(billingEventTypeId?: number): void {

        if (!billingEventTypeId) {
            this.billingEventType = new CreateOrEditBillingEventTypeDto();
            this.billingEventType.id = billingEventTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._billingEventTypesServiceProxy.getBillingEventTypeForEdit(billingEventTypeId).subscribe(result => {
                this.billingEventType = result.billingEventType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._billingEventTypesServiceProxy.createOrEdit(this.billingEventType)
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
