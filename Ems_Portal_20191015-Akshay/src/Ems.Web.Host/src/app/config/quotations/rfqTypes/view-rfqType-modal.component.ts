import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetRfqTypeForViewDto, RfqTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewRfqTypeModal',
    templateUrl: './view-rfqType-modal.component.html'
})
export class ViewRfqTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetRfqTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetRfqTypeForViewDto();
        this.item.rfqType = new RfqTypeDto();
    }

    show(item: GetRfqTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
