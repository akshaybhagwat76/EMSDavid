import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetSupportItemForViewDto, SupportItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewSupportItemModal',
    templateUrl: './view-supportItem-modal.component.html'
})
export class ViewSupportItemModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetSupportItemForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetSupportItemForViewDto();
        this.item.supportItem = new SupportItemDto();
    }

    show(item: GetSupportItemForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
