import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetIncidentUpdateForViewDto, IncidentUpdateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewIncidentUpdateModal',
    templateUrl: './view-incidentUpdate-modal.component.html'
})
export class ViewIncidentUpdateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetIncidentUpdateForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetIncidentUpdateForViewDto();
        this.item.incidentUpdate = new IncidentUpdateDto();
    }

    show(item: GetIncidentUpdateForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
