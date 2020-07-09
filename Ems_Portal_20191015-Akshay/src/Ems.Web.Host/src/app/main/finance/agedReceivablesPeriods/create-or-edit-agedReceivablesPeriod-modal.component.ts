import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AgedReceivablesPeriodsServiceProxy, CreateOrEditAgedReceivablesPeriodDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditAgedReceivablesPeriodModal',
    templateUrl: './create-or-edit-agedReceivablesPeriod-modal.component.html'
})
export class CreateOrEditAgedReceivablesPeriodModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    agedReceivablesPeriod: CreateOrEditAgedReceivablesPeriodDto = new CreateOrEditAgedReceivablesPeriodDto();



    constructor(
        injector: Injector,
        private _agedReceivablesPeriodsServiceProxy: AgedReceivablesPeriodsServiceProxy
    ) {
        super(injector);
    }

    show(agedReceivablesPeriodId?: number): void {

        if (!agedReceivablesPeriodId) {
            this.agedReceivablesPeriod = new CreateOrEditAgedReceivablesPeriodDto();
            this.agedReceivablesPeriod.id = agedReceivablesPeriodId;
            this.agedReceivablesPeriod.period = moment().startOf('day');

            this.active = true;
            this.modal.show();
        } else {
            this._agedReceivablesPeriodsServiceProxy.getAgedReceivablesPeriodForEdit(agedReceivablesPeriodId).subscribe(result => {
                this.agedReceivablesPeriod = result.agedReceivablesPeriod;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._agedReceivablesPeriodsServiceProxy.createOrEdit(this.agedReceivablesPeriod)
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
