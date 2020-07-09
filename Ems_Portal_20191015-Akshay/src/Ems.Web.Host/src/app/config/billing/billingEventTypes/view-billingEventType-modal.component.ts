import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetBillingEventTypeForViewDto, BillingEventTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewBillingEventTypeModal',
    templateUrl: './view-billingEventType-modal.component.html'
})
export class ViewBillingEventTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetBillingEventTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetBillingEventTypeForViewDto();
        this.item.billingEventType = new BillingEventTypeDto();
    }

    show(item: GetBillingEventTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
