import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { SupportTypesServiceProxy, CreateOrEditSupportTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditSupportTypeModal',
    templateUrl: './create-or-edit-supportType-modal.component.html'
})
export class CreateOrEditSupportTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    supportType: CreateOrEditSupportTypeDto = new CreateOrEditSupportTypeDto();



    constructor(
        injector: Injector,
        private _supportTypesServiceProxy: SupportTypesServiceProxy
    ) {
        super(injector);
    }

    show(supportTypeId?: number): void {

        if (!supportTypeId) {
            this.supportType = new CreateOrEditSupportTypeDto();
            this.supportType.id = supportTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._supportTypesServiceProxy.getSupportTypeForEdit(supportTypeId).subscribe(result => {
                this.supportType = result.supportType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._supportTypesServiceProxy.createOrEdit(this.supportType)
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
