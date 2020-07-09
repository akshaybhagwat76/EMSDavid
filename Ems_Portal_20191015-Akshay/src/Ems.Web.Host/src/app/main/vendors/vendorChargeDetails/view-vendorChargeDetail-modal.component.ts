import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetVendorChargeDetailForViewDto, VendorChargeDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewVendorChargeDetailModal',
    templateUrl: './view-vendorChargeDetail-modal.component.html'
})
export class ViewVendorChargeDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetVendorChargeDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetVendorChargeDetailForViewDto();
        this.item.vendorChargeDetail = new VendorChargeDetailDto();
    }

    show(item: GetVendorChargeDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
