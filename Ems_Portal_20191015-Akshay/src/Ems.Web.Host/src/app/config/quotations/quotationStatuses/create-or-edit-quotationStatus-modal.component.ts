import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { QuotationStatusesServiceProxy, CreateOrEditQuotationStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditQuotationStatusModal',
    templateUrl: './create-or-edit-quotationStatus-modal.component.html'
})
export class CreateOrEditQuotationStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    quotationStatus: CreateOrEditQuotationStatusDto = new CreateOrEditQuotationStatusDto();



    constructor(
        injector: Injector,
        private _quotationStatusesServiceProxy: QuotationStatusesServiceProxy
    ) {
        super(injector);
    }

    show(quotationStatusId?: number): void {

        if (!quotationStatusId) {
            this.quotationStatus = new CreateOrEditQuotationStatusDto();
            this.quotationStatus.id = quotationStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._quotationStatusesServiceProxy.getQuotationStatusForEdit(quotationStatusId).subscribe(result => {
                this.quotationStatus = result.quotationStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._quotationStatusesServiceProxy.createOrEdit(this.quotationStatus)
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
