import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { EstimateStatusesServiceProxy, CreateOrEditEstimateStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditEstimateStatusModal',
    templateUrl: './create-or-edit-estimateStatus-modal.component.html'
})
export class CreateOrEditEstimateStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    estimateStatus: CreateOrEditEstimateStatusDto = new CreateOrEditEstimateStatusDto();



    constructor(
        injector: Injector,
        private _estimateStatusesServiceProxy: EstimateStatusesServiceProxy
    ) {
        super(injector);
    }

    show(estimateStatusId?: number): void {

        if (!estimateStatusId) {
            this.estimateStatus = new CreateOrEditEstimateStatusDto();
            this.estimateStatus.id = estimateStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._estimateStatusesServiceProxy.getEstimateStatusForEdit(estimateStatusId).subscribe(result => {
                this.estimateStatus = result.estimateStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._estimateStatusesServiceProxy.createOrEdit(this.estimateStatus)
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
