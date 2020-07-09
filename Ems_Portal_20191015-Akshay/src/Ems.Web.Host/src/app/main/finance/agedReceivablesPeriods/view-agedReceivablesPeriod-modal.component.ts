import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAgedReceivablesPeriodForViewDto, AgedReceivablesPeriodDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAgedReceivablesPeriodModal',
    templateUrl: './view-agedReceivablesPeriod-modal.component.html'
})
export class ViewAgedReceivablesPeriodModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAgedReceivablesPeriodForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAgedReceivablesPeriodForViewDto();
        this.item.agedReceivablesPeriod = new AgedReceivablesPeriodDto();
    }

    show(item: GetAgedReceivablesPeriodForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
