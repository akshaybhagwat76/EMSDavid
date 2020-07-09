import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetXeroInvoiceForViewDto, XeroInvoiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewXeroInvoiceModal',
    templateUrl: './view-xeroInvoice-modal.component.html'
})
export class ViewXeroInvoiceModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetXeroInvoiceForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetXeroInvoiceForViewDto();
        this.item.xeroInvoice = new XeroInvoiceDto();
    }

    show(item: GetXeroInvoiceForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
