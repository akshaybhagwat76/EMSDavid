import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomerGroupMembershipsServiceProxy, CreateOrEditCustomerGroupMembershipDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { CustomerGroupMembershipCustomerGroupLookupTableModalComponent } from './customerGroupMembership-customerGroup-lookup-table-modal.component';
import { CustomerGroupMembershipCustomerLookupTableModalComponent } from './customerGroupMembership-customer-lookup-table-modal.component';


@Component({
    selector: 'createOrEditCustomerGroupMembershipModal',
    templateUrl: './create-or-edit-customerGroupMembership-modal.component.html'
})
export class CreateOrEditCustomerGroupMembershipModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('customerGroupMembershipCustomerGroupLookupTableModal', { static: true }) customerGroupMembershipCustomerGroupLookupTableModal: CustomerGroupMembershipCustomerGroupLookupTableModalComponent;
    @ViewChild('customerGroupMembershipCustomerLookupTableModal', { static: true }) customerGroupMembershipCustomerLookupTableModal: CustomerGroupMembershipCustomerLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    customerGroupMembership: CreateOrEditCustomerGroupMembershipDto = new CreateOrEditCustomerGroupMembershipDto();

            dateJoined: Date;
    customerGroupName = '';
    customerName = '';


    constructor(
        injector: Injector,
        private _customerGroupMembershipsServiceProxy: CustomerGroupMembershipsServiceProxy
    ) {
        super(injector);
    }

    show(customerGroupMembershipId?: number): void {
this.dateJoined = null;

        if (!customerGroupMembershipId) {
            this.customerGroupMembership = new CreateOrEditCustomerGroupMembershipDto();
            this.customerGroupMembership.id = customerGroupMembershipId;
            this.customerGroupMembership.dateLeft = moment().startOf('day');
            this.customerGroupName = '';
            this.customerName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._customerGroupMembershipsServiceProxy.getCustomerGroupMembershipForEdit(customerGroupMembershipId).subscribe(result => {
                this.customerGroupMembership = result.customerGroupMembership;

                if (this.customerGroupMembership.dateJoined) {
					this.dateJoined = this.customerGroupMembership.dateJoined.toDate();
                }
                this.customerGroupName = result.customerGroupName;
                this.customerName = result.customerName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
        if (this.dateJoined) {
            if (!this.customerGroupMembership.dateJoined) {
                this.customerGroupMembership.dateJoined = moment(this.dateJoined).startOf('day');
            }
            else {
                this.customerGroupMembership.dateJoined = moment(this.dateJoined);
            }
        }
        else {
            this.customerGroupMembership.dateJoined = null;
        }
            this._customerGroupMembershipsServiceProxy.createOrEdit(this.customerGroupMembership)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

        openSelectCustomerGroupModal() {
        this.customerGroupMembershipCustomerGroupLookupTableModal.id = this.customerGroupMembership.customerGroupId;
        this.customerGroupMembershipCustomerGroupLookupTableModal.displayName = this.customerGroupName;
        this.customerGroupMembershipCustomerGroupLookupTableModal.show();
    }
        openSelectCustomerModal() {
        this.customerGroupMembershipCustomerLookupTableModal.id = this.customerGroupMembership.customerId;
        this.customerGroupMembershipCustomerLookupTableModal.displayName = this.customerName;
        this.customerGroupMembershipCustomerLookupTableModal.show();
    }


        setCustomerGroupIdNull() {
        this.customerGroupMembership.customerGroupId = null;
        this.customerGroupName = '';
    }
        setCustomerIdNull() {
        this.customerGroupMembership.customerId = null;
        this.customerName = '';
    }


        getNewCustomerGroupId() {
        this.customerGroupMembership.customerGroupId = this.customerGroupMembershipCustomerGroupLookupTableModal.id;
        this.customerGroupName = this.customerGroupMembershipCustomerGroupLookupTableModal.displayName;
    }
        getNewCustomerId() {
        this.customerGroupMembership.customerId = this.customerGroupMembershipCustomerLookupTableModal.id;
        this.customerName = this.customerGroupMembershipCustomerLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
