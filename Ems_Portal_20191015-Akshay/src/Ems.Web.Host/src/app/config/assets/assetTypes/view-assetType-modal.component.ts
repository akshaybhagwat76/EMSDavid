import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetTypeForViewDto, AssetTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAssetTypeModal',
    templateUrl: './view-assetType-modal.component.html'
})
export class ViewAssetTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAssetTypeForViewDto();
        this.item.assetType = new AssetTypeDto();
    }

    show(item: GetAssetTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
