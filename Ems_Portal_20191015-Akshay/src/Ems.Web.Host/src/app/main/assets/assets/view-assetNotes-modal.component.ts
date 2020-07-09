import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetNotesForViewDto, AssetNotesDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DomSanitizer } from '@angular/platform-browser';
 
@Component({
    selector: 'viewAssetNotesModal',
    templateUrl: './view-assetNotes-modal.component.html'
})
export class ViewAssetNotesModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetNotesForViewDto;


    constructor(
        injector: Injector,
        private sanitization: DomSanitizer
    ) {
        super(injector);
        this.item = new GetAssetNotesForViewDto();
        this.item.assetNotes = new AssetNotesDto();
    }

    show(item: GetAssetNotesForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    getSanitizedHtml() {
        if (this.item && this.item.assetNotes)
            return this.sanitization.bypassSecurityTrustHtml(this.item.assetNotes.notes);

        return '';
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
