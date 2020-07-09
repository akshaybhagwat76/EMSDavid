import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetItemTypeForViewDto, ItemTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewItemTypeModal',
    templateUrl: './view-itemType-modal.component.html'
})
export class ViewItemTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetItemTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetItemTypeForViewDto();
        this.item.itemType = new ItemTypeDto();
    }

    show(item: GetItemTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
