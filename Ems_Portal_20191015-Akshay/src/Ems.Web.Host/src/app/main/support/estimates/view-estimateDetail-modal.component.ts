import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetEstimateDetailForViewDto, EstimateDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewEstimateDetailModal',
    templateUrl: './view-estimateDetail-modal.component.html'
})
export class ViewEstimateDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetEstimateDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetEstimateDetailForViewDto();
        this.item.estimateDetail = new EstimateDetailDto();
    }

    show(item: GetEstimateDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
