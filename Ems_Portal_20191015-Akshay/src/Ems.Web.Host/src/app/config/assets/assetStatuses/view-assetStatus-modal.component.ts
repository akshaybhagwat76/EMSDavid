import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetStatusForViewDto, AssetStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAssetStatusModal',
    templateUrl: './view-assetStatus-modal.component.html'
})
export class ViewAssetStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAssetStatusForViewDto();
        this.item.assetStatus = new AssetStatusDto();
    }

    show(item: GetAssetStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
