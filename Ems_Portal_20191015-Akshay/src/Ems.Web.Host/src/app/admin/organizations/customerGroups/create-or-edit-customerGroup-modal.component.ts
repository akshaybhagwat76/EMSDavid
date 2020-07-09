import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomerGroupsServiceProxy, CreateOrEditCustomerGroupDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCustomerGroupModal',
    templateUrl: './create-or-edit-customerGroup-modal.component.html'
})
export class CreateOrEditCustomerGroupModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    customerGroup: CreateOrEditCustomerGroupDto = new CreateOrEditCustomerGroupDto();



    constructor(
        injector: Injector,
        private _customerGroupsServiceProxy: CustomerGroupsServiceProxy
    ) {
        super(injector);
    }

    show(customerGroupId?: number): void {

        if (!customerGroupId) {
            this.customerGroup = new CreateOrEditCustomerGroupDto();
            this.customerGroup.id = customerGroupId;

            this.active = true;
            this.modal.show();
        } else {
            this._customerGroupsServiceProxy.getCustomerGroupForEdit(customerGroupId).subscribe(result => {
                this.customerGroup = result.customerGroup;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._customerGroupsServiceProxy.createOrEdit(this.customerGroup)
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
