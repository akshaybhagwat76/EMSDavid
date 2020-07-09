import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderActionsServiceProxy, CreateOrEditWorkOrderActionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditWorkOrderActionModal',
    templateUrl: './create-or-edit-workOrderAction-modal.component.html'
})
export class CreateOrEditWorkOrderActionModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrderAction: CreateOrEditWorkOrderActionDto = new CreateOrEditWorkOrderActionDto();



    constructor(
        injector: Injector,
        private _workOrderActionsServiceProxy: WorkOrderActionsServiceProxy
    ) {
        super(injector);
    }

    show(workOrderActionId?: number): void {

        if (!workOrderActionId) {
            this.workOrderAction = new CreateOrEditWorkOrderActionDto();
            this.workOrderAction.id = workOrderActionId;

            this.active = true;
            this.modal.show();
        } else {
            this._workOrderActionsServiceProxy.getWorkOrderActionForEdit(workOrderActionId).subscribe(result => {
                this.workOrderAction = result.workOrderAction;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;

        this._workOrderActionsServiceProxy.createOrEdit(this.workOrderAction)
            .pipe(finalize(() => { this.saving = false; }))
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
