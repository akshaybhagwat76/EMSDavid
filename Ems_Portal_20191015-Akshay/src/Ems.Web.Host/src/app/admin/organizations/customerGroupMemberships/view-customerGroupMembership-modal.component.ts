import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetCustomerGroupMembershipForViewDto, CustomerGroupMembershipDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCustomerGroupMembershipModal',
    templateUrl: './view-customerGroupMembership-modal.component.html'
})
export class ViewCustomerGroupMembershipModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCustomerGroupMembershipForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCustomerGroupMembershipForViewDto();
        this.item.customerGroupMembership = new CustomerGroupMembershipDto();
    }

    show(item: GetCustomerGroupMembershipForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
