import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { IncidentTypesServiceProxy, CreateOrEditIncidentTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditIncidentTypeModal',
    templateUrl: './create-or-edit-incidentType-modal.component.html'
})
export class CreateOrEditIncidentTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    incidentType: CreateOrEditIncidentTypeDto = new CreateOrEditIncidentTypeDto();



    constructor(
        injector: Injector,
        private _incidentTypesServiceProxy: IncidentTypesServiceProxy
    ) {
        super(injector);
    }

    show(incidentTypeId?: number): void {

        if (!incidentTypeId) {
            this.incidentType = new CreateOrEditIncidentTypeDto();
            this.incidentType.id = incidentTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._incidentTypesServiceProxy.getIncidentTypeForEdit(incidentTypeId).subscribe(result => {
                this.incidentType = result.incidentType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._incidentTypesServiceProxy.createOrEdit(this.incidentType)
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
