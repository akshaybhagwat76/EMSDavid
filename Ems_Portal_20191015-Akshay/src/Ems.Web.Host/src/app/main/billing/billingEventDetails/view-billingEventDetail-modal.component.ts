import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetBillingEventDetailForViewDto, BillingEventDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewBillingEventDetailModal',
    templateUrl: './view-billingEventDetail-modal.component.html'
})
export class ViewBillingEventDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetBillingEventDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetBillingEventDetailForViewDto();
        this.item.billingEventDetail = new BillingEventDetailDto();
    }

    show(item: GetBillingEventDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
