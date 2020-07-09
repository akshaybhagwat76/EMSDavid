import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetWorkOrderStatusForViewDto, WorkOrderStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewWorkOrderStatusModal',
    templateUrl: './view-workOrderStatus-modal.component.html'
})
export class ViewWorkOrderStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetWorkOrderStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetWorkOrderStatusForViewDto();
        this.item.workOrderStatus = new WorkOrderStatusDto();
    }

    show(item: GetWorkOrderStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
