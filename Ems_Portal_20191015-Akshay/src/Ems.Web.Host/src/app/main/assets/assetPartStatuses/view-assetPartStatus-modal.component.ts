import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetPartStatusForViewDto, AssetPartStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAssetPartStatusModal',
    templateUrl: './view-assetPartStatus-modal.component.html'
})
export class ViewAssetPartStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetPartStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAssetPartStatusForViewDto();
        this.item.assetPartStatus = new AssetPartStatusDto();
    }

    show(item: GetAssetPartStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
