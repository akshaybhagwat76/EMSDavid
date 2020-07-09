import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { XeroInvoicesServiceProxy, CreateOrEditXeroInvoiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { XeroInvoiceCustomerInvoiceLookupTableModalComponent } from './xeroInvoice-customerInvoice-lookup-table-modal.component';

@Component({
    selector: 'createOrEditXeroInvoiceModal',
    templateUrl: './create-or-edit-xeroInvoice-modal.component.html'
})
export class CreateOrEditXeroInvoiceModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('xeroInvoiceCustomerInvoiceLookupTableModal', { static: true }) xeroInvoiceCustomerInvoiceLookupTableModal: XeroInvoiceCustomerInvoiceLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    xeroInvoice: CreateOrEditXeroInvoiceDto = new CreateOrEditXeroInvoiceDto();

    customerInvoiceCustomerReference = '';


    constructor(
        injector: Injector,
        private _xeroInvoicesServiceProxy: XeroInvoicesServiceProxy
    ) {
        super(injector);
    }

    show(xeroInvoiceId?: number): void {

        if (!xeroInvoiceId) {
            this.xeroInvoice = new CreateOrEditXeroInvoiceDto();
            this.xeroInvoice.id = xeroInvoiceId;
            this.customerInvoiceCustomerReference = '';

            this.active = true;
            this.modal.show();
        } else {
            this._xeroInvoicesServiceProxy.getXeroInvoiceForEdit(xeroInvoiceId).subscribe(result => {
                this.xeroInvoice = result.xeroInvoice;

                this.customerInvoiceCustomerReference = result.customerInvoiceCustomerReference;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._xeroInvoicesServiceProxy.createOrEdit(this.xeroInvoice)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectCustomerInvoiceModal() {
        this.xeroInvoiceCustomerInvoiceLookupTableModal.id = this.xeroInvoice.customerInvoiceId;
        this.xeroInvoiceCustomerInvoiceLookupTableModal.displayName = this.customerInvoiceCustomerReference;
        this.xeroInvoiceCustomerInvoiceLookupTableModal.show();
    }


    setCustomerInvoiceIdNull() {
        this.xeroInvoice.customerInvoiceId = null;
        this.customerInvoiceCustomerReference = '';
    }


    getNewCustomerInvoiceId() {
        this.xeroInvoice.customerInvoiceId = this.xeroInvoiceCustomerInvoiceLookupTableModal.id;
        this.customerInvoiceCustomerReference = this.xeroInvoiceCustomerInvoiceLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
