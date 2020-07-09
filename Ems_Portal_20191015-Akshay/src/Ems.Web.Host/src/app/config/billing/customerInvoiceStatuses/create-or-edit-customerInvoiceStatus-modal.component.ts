import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomerInvoiceStatusesServiceProxy, CreateOrEditCustomerInvoiceStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCustomerInvoiceStatusModal',
    templateUrl: './create-or-edit-customerInvoiceStatus-modal.component.html'
})
export class CreateOrEditCustomerInvoiceStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    customerInvoiceStatus: CreateOrEditCustomerInvoiceStatusDto = new CreateOrEditCustomerInvoiceStatusDto();



    constructor(
        injector: Injector,
        private _customerInvoiceStatusesServiceProxy: CustomerInvoiceStatusesServiceProxy
    ) {
        super(injector);
    }

    show(customerInvoiceStatusId?: number): void {

        if (!customerInvoiceStatusId) {
            this.customerInvoiceStatus = new CreateOrEditCustomerInvoiceStatusDto();
            this.customerInvoiceStatus.id = customerInvoiceStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._customerInvoiceStatusesServiceProxy.getCustomerInvoiceStatusForEdit(customerInvoiceStatusId).subscribe(result => {
                this.customerInvoiceStatus = result.customerInvoiceStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._customerInvoiceStatusesServiceProxy.createOrEdit(this.customerInvoiceStatus)
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
