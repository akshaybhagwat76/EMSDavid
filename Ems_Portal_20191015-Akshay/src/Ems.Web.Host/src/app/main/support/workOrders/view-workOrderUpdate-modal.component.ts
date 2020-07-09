import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetWorkOrderUpdateForViewDto, WorkOrderUpdateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewWorkOrderUpdateModal',
    templateUrl: './view-workOrderUpdate-modal.component.html'
})
export class ViewWorkOrderUpdateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetWorkOrderUpdateForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetWorkOrderUpdateForViewDto();
        this.item.workOrderUpdate = new WorkOrderUpdateDto();
    }

    show(item: GetWorkOrderUpdateForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
