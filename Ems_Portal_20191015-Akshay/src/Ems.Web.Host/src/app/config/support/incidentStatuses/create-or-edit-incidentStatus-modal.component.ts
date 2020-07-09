import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { IncidentStatusesServiceProxy, CreateOrEditIncidentStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditIncidentStatusModal',
    templateUrl: './create-or-edit-incidentStatus-modal.component.html'
})
export class CreateOrEditIncidentStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    incidentStatus: CreateOrEditIncidentStatusDto = new CreateOrEditIncidentStatusDto();



    constructor(
        injector: Injector,
        private _incidentStatusesServiceProxy: IncidentStatusesServiceProxy
    ) {
        super(injector);
    }

    show(incidentStatusId?: number): void {

        if (!incidentStatusId) {
            this.incidentStatus = new CreateOrEditIncidentStatusDto();
            this.incidentStatus.id = incidentStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._incidentStatusesServiceProxy.getIncidentStatusForEdit(incidentStatusId).subscribe(result => {
                this.incidentStatus = result.incidentStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._incidentStatusesServiceProxy.createOrEdit(this.incidentStatus)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
