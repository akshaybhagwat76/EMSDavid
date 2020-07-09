import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { BillingRuleTypesServiceProxy, CreateOrEditBillingRuleTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditBillingRuleTypeModal',
    templateUrl: './create-or-edit-billingRuleType-modal.component.html'
})
export class CreateOrEditBillingRuleTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billingRuleType: CreateOrEditBillingRuleTypeDto = new CreateOrEditBillingRuleTypeDto();



    constructor(
        injector: Injector,
        private _billingRuleTypesServiceProxy: BillingRuleTypesServiceProxy
    ) {
        super(injector);
    }

    show(billingRuleTypeId?: number): void {

        if (!billingRuleTypeId) {
            this.billingRuleType = new CreateOrEditBillingRuleTypeDto();
            this.billingRuleType.id = billingRuleTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._billingRuleTypesServiceProxy.getBillingRuleTypeForEdit(billingRuleTypeId).subscribe(result => {
                this.billingRuleType = result.billingRuleType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._billingRuleTypesServiceProxy.createOrEdit(this.billingRuleType)
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
