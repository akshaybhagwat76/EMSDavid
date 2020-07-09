import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetCustomerInvoiceForViewDto, CustomerInvoiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCustomerInvoiceModal',
    templateUrl: './view-customerInvoice-modal.component.html'
})
export class ViewCustomerInvoiceModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCustomerInvoiceForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCustomerInvoiceForViewDto();
        this.item.customerInvoice = new CustomerInvoiceDto();
    }

    show(item: GetCustomerInvoiceForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
