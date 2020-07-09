import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetOwnershipForViewDto, AssetOwnershipDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAssetOwnershipModal',
    templateUrl: './view-assetOwnership-modal.component.html'
})
export class ViewAssetOwnershipModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetOwnershipForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAssetOwnershipForViewDto();
        this.item.assetOwnership = new AssetOwnershipDto();
    }

    show(item: GetAssetOwnershipForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
