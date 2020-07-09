import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetEstimateForViewDto, EstimateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewEstimateModal',
    templateUrl: './view-estimate-modal.component.html'
})
export class ViewEstimateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetEstimateForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetEstimateForViewDto();
        this.item.estimate = new EstimateDto();
    }

    show(item: GetEstimateForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
