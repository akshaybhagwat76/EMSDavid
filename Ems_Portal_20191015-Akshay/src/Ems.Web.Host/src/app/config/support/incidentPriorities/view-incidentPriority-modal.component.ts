import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetIncidentPriorityForViewDto, IncidentPriorityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewIncidentPriorityModal',
    templateUrl: './view-incidentPriority-modal.component.html'
})
export class ViewIncidentPriorityModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetIncidentPriorityForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetIncidentPriorityForViewDto();
        this.item.incidentPriority = new IncidentPriorityDto();
    }

    show(item: GetIncidentPriorityForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
