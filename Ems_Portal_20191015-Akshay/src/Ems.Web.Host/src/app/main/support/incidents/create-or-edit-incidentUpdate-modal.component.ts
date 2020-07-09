import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { IncidentUpdatesServiceProxy, CreateOrEditIncidentUpdateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { IncidentUpdateUserLookupTableModalComponent } from './incidentUpdate-user-lookup-table-modal.component';

@Component({
    selector: 'createOrEditIncidentUpdateModal',
    templateUrl: './create-or-edit-incidentUpdate-modal.component.html'
})
export class CreateOrEditIncidentUpdateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('incidentUpdateUserLookupTableModal', { static: true }) incidentUpdateUserLookupTableModal: IncidentUpdateUserLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    incidentUpdate: CreateOrEditIncidentUpdateDto = new CreateOrEditIncidentUpdateDto();

    userName = '';

    constructor(
        injector: Injector,
        private _incidentUpdatesServiceProxy: IncidentUpdatesServiceProxy
    ) {
        super(injector);
    }

    show(incidentUpdateId?: number, incidentId?: number): void {
        if (!incidentUpdateId) {
            this.incidentUpdate = new CreateOrEditIncidentUpdateDto();
            this.incidentUpdate.id = incidentUpdateId;
            this.incidentUpdate.incidentId = incidentId;
            this.incidentUpdate.updated = moment().startOf('day');
            this.userName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._incidentUpdatesServiceProxy.getIncidentUpdateForEdit(incidentUpdateId).subscribe(result => {
                this.incidentUpdate = result.incidentUpdate;
                this.incidentUpdate.incidentId = incidentId;

                this.userName = result.userName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;

        this._incidentUpdatesServiceProxy.createOrEdit(this.incidentUpdate)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    openSelectUserModal() {
        this.incidentUpdateUserLookupTableModal.id = this.incidentUpdate.userId;
        this.incidentUpdateUserLookupTableModal.displayName = this.userName;
        this.incidentUpdateUserLookupTableModal.show();
    }

    setUserIdNull() {
        this.incidentUpdate.userId = null;
        this.userName = '';
    }

    getNewUserId() {
        this.incidentUpdate.userId = this.incidentUpdateUserLookupTableModal.id;
        this.userName = this.incidentUpdateUserLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
