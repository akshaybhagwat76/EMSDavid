import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetLeaseItemForViewDto, LeaseItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewLeaseItemModal',
    templateUrl: './view-leaseItem-modal.component.html'
})
export class ViewLeaseItemModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetLeaseItemForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetLeaseItemForViewDto();
        this.item.leaseItem = new LeaseItemDto();
    }

    show(item: GetLeaseItemForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
