import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderTypesServiceProxy, CreateOrEditWorkOrderTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditWorkOrderTypeModal',
    templateUrl: './create-or-edit-workOrderType-modal.component.html'
})
export class CreateOrEditWorkOrderTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrderType: CreateOrEditWorkOrderTypeDto = new CreateOrEditWorkOrderTypeDto();



    constructor(
        injector: Injector,
        private _workOrderTypesServiceProxy: WorkOrderTypesServiceProxy
    ) {
        super(injector);
    }

    show(workOrderTypeId?: number): void {

        if (!workOrderTypeId) {
            this.workOrderType = new CreateOrEditWorkOrderTypeDto();
            this.workOrderType.id = workOrderTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._workOrderTypesServiceProxy.getWorkOrderTypeForEdit(workOrderTypeId).subscribe(result => {
                this.workOrderType = result.workOrderType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._workOrderTypesServiceProxy.createOrEdit(this.workOrderType)
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
