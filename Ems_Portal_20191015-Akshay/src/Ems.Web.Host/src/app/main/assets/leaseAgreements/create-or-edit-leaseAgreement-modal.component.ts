import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { LeaseAgreementsServiceProxy, CreateOrEditLeaseAgreementDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { LeaseAgreementContactLookupTableModalComponent } from './leaseAgreement-contact-lookup-table-modal.component';
import { LeaseAgreementAssetOwnerLookupTableModalComponent } from './leaseAgreement-assetOwner-lookup-table-modal.component';
import { LeaseAgreementCustomerLookupTableModalComponent } from './leaseAgreement-customer-lookup-table-modal.component';


@Component({
    selector: 'createOrEditLeaseAgreementModal',
    templateUrl: './create-or-edit-leaseAgreement-modal.component.html'
})
export class CreateOrEditLeaseAgreementModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('leaseAgreementContactLookupTableModal', { static: true }) leaseAgreementContactLookupTableModal: LeaseAgreementContactLookupTableModalComponent;
    @ViewChild('leaseAgreementAssetOwnerLookupTableModal', { static: true }) leaseAgreementAssetOwnerLookupTableModal: LeaseAgreementAssetOwnerLookupTableModalComponent;
    @ViewChild('leaseAgreementCustomerLookupTableModal', { static: true }) leaseAgreementCustomerLookupTableModal: LeaseAgreementCustomerLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    leaseAgreement: CreateOrEditLeaseAgreementDto = new CreateOrEditLeaseAgreementDto();

    startDate: Date;
    endDate: Date;
    contactContactName = '';
    assetOwnerName = '';
    customerName = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _leaseAgreementsServiceProxy: LeaseAgreementsServiceProxy
    ) {
        super(injector);
    }

    show(leaseAgreementId?: number): void {
        this.startDate = null;
        this.endDate = null;

        if (!leaseAgreementId) {
            this.leaseAgreement = new CreateOrEditLeaseAgreementDto();
            this.leaseAgreement.id = leaseAgreementId;
            this.contactContactName = '';
            this.assetOwnerName = '';
            this.customerName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._leaseAgreementsServiceProxy.getLeaseAgreementForEdit(leaseAgreementId).subscribe(result => {
                this.leaseAgreement = result.leaseAgreement;

                if (this.leaseAgreement.startDate) {
                    this.startDate = this.leaseAgreement.startDate.toDate();
                }
                if (this.leaseAgreement.endDate) {
                    this.endDate = this.leaseAgreement.endDate.toDate();
                }
                this.contactContactName = result.contactContactName;
                this.assetOwnerName = result.assetOwnerName;
                this.customerName = result.customerName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (this.startDate) {
            this.leaseAgreement.startDate = moment(this.startDate);
        }
        else {
            this.leaseAgreement.startDate = null;
        }

        if (this.endDate) {
            this.leaseAgreement.endDate = moment(this.endDate);
        }
        else {
            this.leaseAgreement.endDate = null;
        }

        if (!this.leaseAgreement.title || !this.leaseAgreement.description ||
            !this.leaseAgreement.startDate || !this.leaseAgreement.endDate ||
            !this.leaseAgreement.assetOwnerId || !this.leaseAgreement.customerId) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.leaseAgreement.endDate < this.leaseAgreement.startDate) {
            this.isFormValid = false;
            this.errorMsg = "End Date must be greater or equals to Start Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._leaseAgreementsServiceProxy.createOrEdit(this.leaseAgreement)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.modalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectContactModal() {
        this.leaseAgreementContactLookupTableModal.id = this.leaseAgreement.contactId;
        this.leaseAgreementContactLookupTableModal.displayName = this.contactContactName;
        this.leaseAgreementContactLookupTableModal.show();
    }
    openSelectAssetOwnerModal() {
        this.leaseAgreementAssetOwnerLookupTableModal.id = this.leaseAgreement.assetOwnerId;
        this.leaseAgreementAssetOwnerLookupTableModal.displayName = this.assetOwnerName;
        this.leaseAgreementAssetOwnerLookupTableModal.contactId = this.leaseAgreement.contactId ? this.leaseAgreement.contactId : 0;
        this.leaseAgreementAssetOwnerLookupTableModal.show();
    }
    openSelectCustomerModal() {
        this.leaseAgreementCustomerLookupTableModal.id = this.leaseAgreement.customerId;
        this.leaseAgreementCustomerLookupTableModal.displayName = this.customerName;
        this.leaseAgreementCustomerLookupTableModal.contactId = this.leaseAgreement.contactId ? this.leaseAgreement.contactId : 0;
        this.leaseAgreementCustomerLookupTableModal.show();
    }

    setContactIdNull() {
        this.leaseAgreement.contactId = null;
        this.contactContactName = '';

        this.setCustomerIdNull();
        this.setAssetOwnerIdNull();
    }
    setAssetOwnerIdNull() {
        this.leaseAgreement.assetOwnerId = null;
        this.assetOwnerName = '';
    }
    setCustomerIdNull() {
        this.leaseAgreement.customerId = null;
        this.customerName = '';
    }

    getNewContactId() {
        this.leaseAgreement.contactId = this.leaseAgreementContactLookupTableModal.id;
        this.contactContactName = this.leaseAgreementContactLookupTableModal.displayName;

        if (this.leaseAgreement.contactId > 0) {
            this.appendContactFKRelatedData(this.leaseAgreement.contactId);
        }
        else {
            this.setCustomerIdNull();
            this.setAssetOwnerIdNull();
        }
    }
    getNewAssetOwnerId() {
        this.leaseAgreement.assetOwnerId = this.leaseAgreementAssetOwnerLookupTableModal.id;
        this.assetOwnerName = this.leaseAgreementAssetOwnerLookupTableModal.displayName;
    }
    getNewCustomerId() {
        this.leaseAgreement.customerId = this.leaseAgreementCustomerLookupTableModal.id;
        this.customerName = this.leaseAgreementCustomerLookupTableModal.displayName;
    }

    appendContactFKRelatedData(contactId: number) {
        this._leaseAgreementsServiceProxy.getCustomerAndAssetOwnerInfo(contactId)
            .subscribe(result => {

                if (result) {
                    let customerInfo = result.customerInfo;
                    let assetOwnerInfo = result.assetOwnerInfo;

                    if (customerInfo) {
                        this.leaseAgreement.customerId = customerInfo.id;
                        this.customerName = customerInfo.displayName;
                    }
                    else {
                        this.setCustomerIdNull();
                    }

                    if (assetOwnerInfo) {
                        this.leaseAgreement.assetOwnerId = assetOwnerInfo.id;
                        this.assetOwnerName = assetOwnerInfo.displayName;
                    }
                    else {
                        this.setAssetOwnerIdNull();
                    }
                }
            });
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
