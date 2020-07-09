import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetVendorChargeForViewDto, VendorChargeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewVendorChargeModal',
    templateUrl: './view-vendorCharge-modal.component.html'
})
export class ViewVendorChargeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetVendorChargeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetVendorChargeForViewDto();
        this.item.vendorCharge = new VendorChargeDto();
    }

    show(item: GetVendorChargeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
