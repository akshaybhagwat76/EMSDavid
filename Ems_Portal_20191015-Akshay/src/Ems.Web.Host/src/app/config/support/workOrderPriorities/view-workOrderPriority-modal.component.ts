import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetWorkOrderPriorityForViewDto, WorkOrderPriorityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewWorkOrderPriorityModal',
    templateUrl: './view-workOrderPriority-modal.component.html'
})
export class ViewWorkOrderPriorityModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetWorkOrderPriorityForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetWorkOrderPriorityForViewDto();
        this.item.workOrderPriority = new WorkOrderPriorityDto();
    }

    show(item: GetWorkOrderPriorityForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
