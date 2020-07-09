import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetCustomerInvoiceDetailForViewDto, CustomerInvoiceDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCustomerInvoiceDetailModal',
    templateUrl: './view-customerInvoiceDetail-modal.component.html'
})
export class ViewCustomerInvoiceDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCustomerInvoiceDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCustomerInvoiceDetailForViewDto();
        this.item.customerInvoiceDetail = new CustomerInvoiceDetailDto();
    }

    show(item: GetCustomerInvoiceDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
