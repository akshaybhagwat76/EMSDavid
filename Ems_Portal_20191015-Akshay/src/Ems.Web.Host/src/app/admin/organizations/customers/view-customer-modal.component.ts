import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetCustomerForViewDto, CustomerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCustomerModal',
    templateUrl: './view-customer-modal.component.html'
})
export class ViewCustomerModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCustomerForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCustomerForViewDto();
        this.item.customer = new CustomerDto();
    }

    show(item: GetCustomerForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
