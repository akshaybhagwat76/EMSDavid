import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetNotesServiceProxy, CreateOrEditAssetNotesDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditAssetNotesModal',
    templateUrl: './create-or-edit-assetNotes-modal.component.html'
})

export class CreateOrEditAssetNotesModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    isFormValid = false;
    errorMsg = "";

    assetNotes: CreateOrEditAssetNotesDto = new CreateOrEditAssetNotesDto();

    constructor(
        injector: Injector,
        private _assetNotesServiceProxy: AssetNotesServiceProxy
    ) {
        super(injector);
    }

    show(assetNoteId?: number, assetId?: number): void {
        if (!assetNoteId) {
            this.assetNotes = new CreateOrEditAssetNotesDto();
            this.assetNotes.assetId = assetId;

            this.active = true;
            this.modal.show();
        } else {
            this._assetNotesServiceProxy.getAssetNotesForEdit(assetNoteId).subscribe(result => {
                this.assetNotes = result.assetNotes;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.assetNotes.notes || !this.assetNotes.title) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._assetNotesServiceProxy.createOrEdit(this.assetNotes)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.modalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}