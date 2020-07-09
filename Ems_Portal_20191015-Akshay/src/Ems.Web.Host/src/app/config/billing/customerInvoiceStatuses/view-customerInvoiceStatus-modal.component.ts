import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetCustomerInvoiceStatusForViewDto, CustomerInvoiceStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCustomerInvoiceStatusModal',
    templateUrl: './view-customerInvoiceStatus-modal.component.html'
})
export class ViewCustomerInvoiceStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCustomerInvoiceStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCustomerInvoiceStatusForViewDto();
        this.item.customerInvoiceStatus = new CustomerInvoiceStatusDto();
    }

    show(item: GetCustomerInvoiceStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
