import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { ItemTypesServiceProxy, CreateOrEditItemTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditItemTypeModal',
    templateUrl: './create-or-edit-itemType-modal.component.html'
})
export class CreateOrEditItemTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    itemType: CreateOrEditItemTypeDto = new CreateOrEditItemTypeDto();



    constructor(
        injector: Injector,
        private _itemTypesServiceProxy: ItemTypesServiceProxy
    ) {
        super(injector);
    }

    show(itemTypeId?: number): void {

        if (!itemTypeId) {
            this.itemType = new CreateOrEditItemTypeDto();
            this.itemType.id = itemTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._itemTypesServiceProxy.getItemTypeForEdit(itemTypeId).subscribe(result => {
                this.itemType = result.itemType;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._itemTypesServiceProxy.createOrEdit(this.itemType)
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
