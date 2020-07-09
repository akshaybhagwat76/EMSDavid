import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetWorkOrderActionForViewDto, WorkOrderActionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewWorkOrderActionModal',
    templateUrl: './view-workOrderAction-modal.component.html'
})
export class ViewWorkOrderActionModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetWorkOrderActionForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetWorkOrderActionForViewDto();
        this.item.workOrderAction = new WorkOrderActionDto();
    }

    show(item: GetWorkOrderActionForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
