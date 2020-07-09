import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetForViewDto, AssetDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAssetModal',
    templateUrl: './view-asset-modal.component.html'
})
export class ViewAssetModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAssetForViewDto();
        this.item.asset = new AssetDto();
    }

    show(item: GetAssetForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
