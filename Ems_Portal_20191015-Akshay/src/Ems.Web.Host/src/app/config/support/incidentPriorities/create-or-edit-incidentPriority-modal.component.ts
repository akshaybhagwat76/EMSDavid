import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { IncidentPrioritiesServiceProxy, CreateOrEditIncidentPriorityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditIncidentPriorityModal',
    templateUrl: './create-or-edit-incidentPriority-modal.component.html'
})
export class CreateOrEditIncidentPriorityModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    incidentPriority: CreateOrEditIncidentPriorityDto = new CreateOrEditIncidentPriorityDto();



    constructor(
        injector: Injector,
        private _incidentPrioritiesServiceProxy: IncidentPrioritiesServiceProxy
    ) {
        super(injector);
    }

    show(incidentPriorityId?: number): void {

        if (!incidentPriorityId) {
            this.incidentPriority = new CreateOrEditIncidentPriorityDto();
            this.incidentPriority.id = incidentPriorityId;

            this.active = true;
            this.modal.show();
        } else {
            this._incidentPrioritiesServiceProxy.getIncidentPriorityForEdit(incidentPriorityId).subscribe(result => {
                this.incidentPriority = result.incidentPriority;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._incidentPrioritiesServiceProxy.createOrEdit(this.incidentPriority)
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
