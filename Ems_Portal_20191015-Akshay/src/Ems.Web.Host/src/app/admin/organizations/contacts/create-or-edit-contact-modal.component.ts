import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { ContactsServiceProxy, CreateOrEditContactDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ContactUserLookupTableModalComponent } from './contact-user-lookup-table-modal.component';
import { ContactVendorLookupTableModalComponent } from './contact-vendor-lookup-table-modal.component';
import { ContactAssetOwnerLookupTableModalComponent } from './contact-assetOwner-lookup-table-modal.component';
import { ContactCustomerLookupTableModalComponent } from './contact-customer-lookup-table-modal.component';


@Component({
    selector: 'createOrEditContactModal',
    templateUrl: './create-or-edit-contact-modal.component.html'
})
export class CreateOrEditContactModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('contactUserLookupTableModal', { static: true }) contactUserLookupTableModal: ContactUserLookupTableModalComponent;
    @ViewChild('contactVendorLookupTableModal', { static: true }) contactVendorLookupTableModal: ContactVendorLookupTableModalComponent;
    @ViewChild('contactAssetOwnerLookupTableModal', { static: true }) contactAssetOwnerLookupTableModal: ContactAssetOwnerLookupTableModalComponent;
    @ViewChild('contactCustomerLookupTableModal', { static: true }) contactCustomerLookupTableModal: ContactCustomerLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    contact: CreateOrEditContactDto = new CreateOrEditContactDto();

    userName = '';
    vendorName = '';
    assetOwnerName = '';
    customerName = '';


    constructor(
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy
    ) {
        super(injector);
    }

    show(contactId?: number): void {

        if (!contactId) {
            this.contact = new CreateOrEditContactDto();
            this.contact.id = contactId;
            this.userName = '';
            this.vendorName = '';
            this.assetOwnerName = '';
            this.customerName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._contactsServiceProxy.getContactForEdit(contactId).subscribe(result => {
                this.contact = result.contact;

                this.userName = result.userName;
                this.vendorName = result.vendorName;
                this.assetOwnerName = result.assetOwnerName;
                this.customerName = result.customerName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._contactsServiceProxy.createOrEdit(this.contact)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

        openSelectUserModal() {
        this.contactUserLookupTableModal.id = this.contact.userId;
        this.contactUserLookupTableModal.displayName = this.userName;
        this.contactUserLookupTableModal.show();
    }
        openSelectVendorModal() {
        this.contactVendorLookupTableModal.id = this.contact.vendorId;
        this.contactVendorLookupTableModal.displayName = this.vendorName;
        this.contactVendorLookupTableModal.show();
    }
        openSelectAssetOwnerModal() {
        this.contactAssetOwnerLookupTableModal.id = this.contact.assetOwnerId;
        this.contactAssetOwnerLookupTableModal.displayName = this.assetOwnerName;
        this.contactAssetOwnerLookupTableModal.show();
    }
        openSelectCustomerModal() {
        this.contactCustomerLookupTableModal.id = this.contact.customerId;
        this.contactCustomerLookupTableModal.displayName = this.customerName;
        this.contactCustomerLookupTableModal.show();
    }


        setUserIdNull() {
        this.contact.userId = null;
        this.userName = '';
    }
        setVendorIdNull() {
        this.contact.vendorId = null;
        this.vendorName = '';
    }
        setAssetOwnerIdNull() {
        this.contact.assetOwnerId = null;
        this.assetOwnerName = '';
    }
        setCustomerIdNull() {
        this.contact.customerId = null;
        this.customerName = '';
    }


        getNewUserId() {
        this.contact.userId = this.contactUserLookupTableModal.id;
        this.userName = this.contactUserLookupTableModal.displayName;
    }
        getNewVendorId() {
        this.contact.vendorId = this.contactVendorLookupTableModal.id;
        this.vendorName = this.contactVendorLookupTableModal.displayName;
    }
        getNewAssetOwnerId() {
        this.contact.assetOwnerId = this.contactAssetOwnerLookupTableModal.id;
        this.assetOwnerName = this.contactAssetOwnerLookupTableModal.displayName;
    }
        getNewCustomerId() {
        this.contact.customerId = this.contactCustomerLookupTableModal.id;
        this.customerName = this.contactCustomerLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
