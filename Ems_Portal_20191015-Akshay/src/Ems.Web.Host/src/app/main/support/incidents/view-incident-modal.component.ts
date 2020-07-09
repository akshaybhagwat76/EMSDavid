import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetIncidentForViewDto, IncidentDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewIncidentModal',
    templateUrl: './view-incident-modal.component.html'
})
export class ViewIncidentModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetIncidentForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetIncidentForViewDto();
        this.item.incident = new IncidentDto();
    }

    show(item: GetIncidentForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
