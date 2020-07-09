import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetEstimateStatusForViewDto, EstimateStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewEstimateStatusModal',
    templateUrl: './view-estimateStatus-modal.component.html'
})
export class ViewEstimateStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetEstimateStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetEstimateStatusForViewDto();
        this.item.estimateStatus = new EstimateStatusDto();
    }

    show(item: GetEstimateStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
