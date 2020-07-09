import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAddressForViewDto, AddressDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewVendorAddressModal',
    templateUrl: './view-vendor-address-modal.component.html'
})
export class ViewVendorAddressModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAddressForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAddressForViewDto();
        this.item.address = new AddressDto();
    }

    show(item: GetAddressForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
