import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { UomsServiceProxy, CreateOrEditUomDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditUomModal',
    templateUrl: './create-or-edit-uom-modal.component.html'
})
export class CreateOrEditUomModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    uom: CreateOrEditUomDto = new CreateOrEditUomDto();



    constructor(
        injector: Injector,
        private _uomsServiceProxy: UomsServiceProxy
    ) {
        super(injector);
    }

    show(uomId?: number): void {

        if (!uomId) {
            this.uom = new CreateOrEditUomDto();
            this.uom.id = uomId;

            this.active = true;
            this.modal.show();
        } else {
            this._uomsServiceProxy.getUomForEdit(uomId).subscribe(result => {
                this.uom = result.uom;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._uomsServiceProxy.createOrEdit(this.uom)
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
