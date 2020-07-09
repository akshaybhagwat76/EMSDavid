import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetSupportContractForViewDto, SupportContractDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewSupportContractModal',
    templateUrl: './view-supportContract-modal.component.html'
})
export class ViewSupportContractModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetSupportContractForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetSupportContractForViewDto();
        this.item.supportContract = new SupportContractDto();
    }

    show(item: GetSupportContractForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
