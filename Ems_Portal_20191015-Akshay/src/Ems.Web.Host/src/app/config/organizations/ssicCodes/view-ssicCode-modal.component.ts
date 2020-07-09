import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetSsicCodeForViewDto, SsicCodeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewSsicCodeModal',
    templateUrl: './view-ssicCode-modal.component.html'
})
export class ViewSsicCodeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetSsicCodeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetSsicCodeForViewDto();
        this.item.ssicCode = new SsicCodeDto();
    }

    show(item: GetSsicCodeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
