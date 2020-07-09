import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetRfqForViewDto, RfqDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewRfqModal',
    templateUrl: './view-rfq-modal.component.html'
})
export class ViewRfqModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetRfqForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetRfqForViewDto();
        this.item.rfq = new RfqDto();
    }

    show(item: GetRfqForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
