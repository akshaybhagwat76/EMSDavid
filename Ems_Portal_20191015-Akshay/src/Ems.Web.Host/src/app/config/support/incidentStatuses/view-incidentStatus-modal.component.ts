import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetIncidentStatusForViewDto, IncidentStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewIncidentStatusModal',
    templateUrl: './view-incidentStatus-modal.component.html'
})
export class ViewIncidentStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetIncidentStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetIncidentStatusForViewDto();
        this.item.incidentStatus = new IncidentStatusDto();
    }

    show(item: GetIncidentStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
