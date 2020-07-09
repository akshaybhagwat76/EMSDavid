import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomerTypesServiceProxy, CreateOrEditCustomerTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCustomerTypeModal',
    templateUrl: './create-or-edit-customerType-modal.component.html'
})
export class CreateOrEditCustomerTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    customerType: CreateOrEditCustomerTypeDto = new CreateOrEditCustomerTypeDto();



    constructor(
        injector: Injector,
        private _customerTypesServiceProxy: CustomerTypesServiceProxy
    ) {
        super(injector);
    }

    show(customerTypeId?: number): void {

        if (!customerTypeId) {
            this.customerType = new CreateOrEditCustomerTypeDto();
            this.customerType.id = customerTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._customerTypesServiceProxy.getCustomerTypeForEdit(customerTypeId).subscribe(result => {
                this.customerType = result.customerType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._customerTypesServiceProxy.createOrEdit(this.customerType)
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
