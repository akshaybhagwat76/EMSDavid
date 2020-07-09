import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { ConsumableTypesServiceProxy, CreateOrEditConsumableTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditConsumableTypeModal',
    templateUrl: './create-or-edit-consumableType-modal.component.html'
})
export class CreateOrEditConsumableTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    consumableType: CreateOrEditConsumableTypeDto = new CreateOrEditConsumableTypeDto();



    constructor(
        injector: Injector,
        private _consumableTypesServiceProxy: ConsumableTypesServiceProxy
    ) {
        super(injector);
    }

    show(consumableTypeId?: number): void {

        if (!consumableTypeId) {
            this.consumableType = new CreateOrEditConsumableTypeDto();
            this.consumableType.id = consumableTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._consumableTypesServiceProxy.getConsumableTypeForEdit(consumableTypeId).subscribe(result => {
                this.consumableType = result.consumableType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._consumableTypesServiceProxy.createOrEdit(this.consumableType)
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
