import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetSupportTypeForViewDto, SupportTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewSupportTypeModal',
    templateUrl: './view-supportType-modal.component.html'
})
export class ViewSupportTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetSupportTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetSupportTypeForViewDto();
        this.item.supportType = new SupportTypeDto();
    }

    show(item: GetSupportTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
