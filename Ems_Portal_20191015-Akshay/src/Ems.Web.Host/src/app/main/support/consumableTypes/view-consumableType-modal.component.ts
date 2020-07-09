import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetConsumableTypeForViewDto, ConsumableTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewConsumableTypeModal',
    templateUrl: './view-consumableType-modal.component.html'
})
export class ViewConsumableTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetConsumableTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetConsumableTypeForViewDto();
        this.item.consumableType = new ConsumableTypeDto();
    }

    show(item: GetConsumableTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
