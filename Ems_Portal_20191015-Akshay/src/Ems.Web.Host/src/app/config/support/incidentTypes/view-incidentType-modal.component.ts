import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetIncidentTypeForViewDto, IncidentTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewIncidentTypeModal',
    templateUrl: './view-incidentType-modal.component.html'
})
export class ViewIncidentTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetIncidentTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetIncidentTypeForViewDto();
        this.item.incidentType = new IncidentTypeDto();
    }

    show(item: GetIncidentTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
