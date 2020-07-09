import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { VendorChargeStatusesServiceProxy, CreateOrEditVendorChargeStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditVendorChargeStatusModal',
    templateUrl: './create-or-edit-vendorChargeStatus-modal.component.html'
})
export class CreateOrEditVendorChargeStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    vendorChargeStatus: CreateOrEditVendorChargeStatusDto = new CreateOrEditVendorChargeStatusDto();



    constructor(
        injector: Injector,
        private _vendorChargeStatusesServiceProxy: VendorChargeStatusesServiceProxy
    ) {
        super(injector);
    }

    show(vendorChargeStatusId?: number): void {

        if (!vendorChargeStatusId) {
            this.vendorChargeStatus = new CreateOrEditVendorChargeStatusDto();
            this.vendorChargeStatus.id = vendorChargeStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._vendorChargeStatusesServiceProxy.getVendorChargeStatusForEdit(vendorChargeStatusId).subscribe(result => {
                this.vendorChargeStatus = result.vendorChargeStatus;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._vendorChargeStatusesServiceProxy.createOrEdit(this.vendorChargeStatus)
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
