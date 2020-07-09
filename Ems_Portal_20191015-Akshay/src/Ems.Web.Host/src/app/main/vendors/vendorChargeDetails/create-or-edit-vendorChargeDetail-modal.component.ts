import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { VendorChargeDetailsServiceProxy, CreateOrEditVendorChargeDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditVendorChargeDetailModal',
    templateUrl: './create-or-edit-vendorChargeDetail-modal.component.html'
})
export class CreateOrEditVendorChargeDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    vendorChargeDetail: CreateOrEditVendorChargeDetailDto = new CreateOrEditVendorChargeDetailDto();



    constructor(
        injector: Injector,
        private _vendorChargeDetailsServiceProxy: VendorChargeDetailsServiceProxy
    ) {
        super(injector);
    }

    show(vendorChargeDetailId?: number): void {

        if (!vendorChargeDetailId) {
            this.vendorChargeDetail = new CreateOrEditVendorChargeDetailDto();
            this.vendorChargeDetail.id = vendorChargeDetailId;

            this.active = true;
            this.modal.show();
        } else {
            this._vendorChargeDetailsServiceProxy.getVendorChargeDetailForEdit(vendorChargeDetailId).subscribe(result => {
                this.vendorChargeDetail = result.vendorChargeDetail;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._vendorChargeDetailsServiceProxy.createOrEdit(this.vendorChargeDetail)
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
