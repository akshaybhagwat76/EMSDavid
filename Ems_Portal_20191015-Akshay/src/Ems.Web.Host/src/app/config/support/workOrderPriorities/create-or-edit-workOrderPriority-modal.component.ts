import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderPrioritiesServiceProxy, CreateOrEditWorkOrderPriorityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditWorkOrderPriorityModal',
    templateUrl: './create-or-edit-workOrderPriority-modal.component.html'
})
export class CreateOrEditWorkOrderPriorityModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrderPriority: CreateOrEditWorkOrderPriorityDto = new CreateOrEditWorkOrderPriorityDto();



    constructor(
        injector: Injector,
        private _workOrderPrioritiesServiceProxy: WorkOrderPrioritiesServiceProxy
    ) {
        super(injector);
    }

    show(workOrderPriorityId?: number): void {

        if (!workOrderPriorityId) {
            this.workOrderPriority = new CreateOrEditWorkOrderPriorityDto();
            this.workOrderPriority.id = workOrderPriorityId;

            this.active = true;
            this.modal.show();
        } else {
            this._workOrderPrioritiesServiceProxy.getWorkOrderPriorityForEdit(workOrderPriorityId).subscribe(result => {
                this.workOrderPriority = result.workOrderPriority;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._workOrderPrioritiesServiceProxy.createOrEdit(this.workOrderPriority)
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
