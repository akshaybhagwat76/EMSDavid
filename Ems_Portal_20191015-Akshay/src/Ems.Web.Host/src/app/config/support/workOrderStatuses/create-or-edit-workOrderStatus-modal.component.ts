import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderStatusesServiceProxy, CreateOrEditWorkOrderStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditWorkOrderStatusModal',
    templateUrl: './create-or-edit-workOrderStatus-modal.component.html'
})
export class CreateOrEditWorkOrderStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrderStatus: CreateOrEditWorkOrderStatusDto = new CreateOrEditWorkOrderStatusDto();



    constructor(
        injector: Injector,
        private _workOrderStatusesServiceProxy: WorkOrderStatusesServiceProxy
    ) {
        super(injector);
    }

    show(workOrderStatusId?: number): void {

        if (!workOrderStatusId) {
            this.workOrderStatus = new CreateOrEditWorkOrderStatusDto();
            this.workOrderStatus.id = workOrderStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._workOrderStatusesServiceProxy.getWorkOrderStatusForEdit(workOrderStatusId).subscribe(result => {
                this.workOrderStatus = result.workOrderStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._workOrderStatusesServiceProxy.createOrEdit(this.workOrderStatus)
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
