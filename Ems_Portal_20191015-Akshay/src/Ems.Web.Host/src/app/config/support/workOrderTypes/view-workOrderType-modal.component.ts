import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetWorkOrderTypeForViewDto, WorkOrderTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewWorkOrderTypeModal',
    templateUrl: './view-workOrderType-modal.component.html'
})
export class ViewWorkOrderTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetWorkOrderTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetWorkOrderTypeForViewDto();
        this.item.workOrderType = new WorkOrderTypeDto();
    }

    show(item: GetWorkOrderTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
