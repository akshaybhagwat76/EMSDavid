import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAttachmentForViewDto, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAttachmentModal',
    templateUrl: './view-attachment-modal.component.html'
})
export class ViewAttachmentModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAttachmentForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAttachmentForViewDto();
        this.item.attachment = new AttachmentDto();
    }

    show(item: GetAttachmentForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
