import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { BillingEventsServiceProxy, CreateOrEditBillingEventDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { BillingEventLeaseAgreementLookupTableModalComponent } from './billingEvent-leaseAgreement-lookup-table-modal.component';
import { BillingEventVendorChargeLookupTableModalComponent } from './billingEvent-vendorCharge-lookup-table-modal.component';
import { BillingEventBillingEventTypeLookupTableModalComponent } from './billingEvent-billingEventType-lookup-table-modal.component';

@Component({
    selector: 'createOrEditBillingEventModal',
    templateUrl: './create-or-edit-billingEvent-modal.component.html'
})
export class CreateOrEditBillingEventModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('billingEventLeaseAgreementLookupTableModal', { static: true }) billingEventLeaseAgreementLookupTableModal: BillingEventLeaseAgreementLookupTableModalComponent;
    @ViewChild('billingEventVendorChargeLookupTableModal', { static: true }) billingEventVendorChargeLookupTableModal: BillingEventVendorChargeLookupTableModalComponent;
    @ViewChild('billingEventBillingEventTypeLookupTableModal', { static: true }) billingEventBillingEventTypeLookupTableModal: BillingEventBillingEventTypeLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billingEvent: CreateOrEditBillingEventDto = new CreateOrEditBillingEventDto();

    leaseAgreementTitle = '';
    vendorChargeReference = '';
    billingEventTypeType = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _billingEventsServiceProxy: BillingEventsServiceProxy
    ) {
        super(injector);
    }

    show(billingEventId?: number): void {

        if (!billingEventId) {
            this.billingEvent = new CreateOrEditBillingEventDto();
            this.billingEvent.id = billingEventId;
            this.billingEvent.billingEventDate = moment().startOf('day');
            this.leaseAgreementTitle = '';
            this.vendorChargeReference = '';
            this.billingEventTypeType = '';

            this.active = true;
            this.modal.show();
        } else {
            this._billingEventsServiceProxy.getBillingEventForEdit(billingEventId).subscribe(result => {
                this.billingEvent = result.billingEvent;

                this.leaseAgreementTitle = result.leaseAgreementTitle;
                this.vendorChargeReference = result.vendorChargeReference;
                this.billingEventTypeType = result.billingEventTypeType;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.billingEvent.leaseAgreementId || !this.billingEvent.billingEventTypeId || !this.billingEvent.billingEventDate) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._billingEventsServiceProxy.createOrEdit(this.billingEvent)
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

    openSelectLeaseAgreementModal() {
        this.billingEventLeaseAgreementLookupTableModal.id = this.billingEvent.leaseAgreementId;
        this.billingEventLeaseAgreementLookupTableModal.displayName = this.leaseAgreementTitle;
        this.billingEventLeaseAgreementLookupTableModal.show();
    }
    openSelectVendorChargeModal() {
        this.billingEventVendorChargeLookupTableModal.id = this.billingEvent.vendorChargeId;
        this.billingEventVendorChargeLookupTableModal.displayName = this.vendorChargeReference;
        this.billingEventVendorChargeLookupTableModal.show();
    }
    openSelectBillingEventTypeModal() {
        this.billingEventBillingEventTypeLookupTableModal.id = this.billingEvent.billingEventTypeId;
        this.billingEventBillingEventTypeLookupTableModal.displayName = this.billingEventTypeType;
        this.billingEventBillingEventTypeLookupTableModal.show();
    }


    setLeaseAgreementIdNull() {
        this.billingEvent.leaseAgreementId = null;
        this.leaseAgreementTitle = '';
    }
    setVendorChargeIdNull() {
        this.billingEvent.vendorChargeId = null;
        this.vendorChargeReference = '';
    }
    setBillingEventTypeIdNull() {
        this.billingEvent.billingEventTypeId = null;
        this.billingEventTypeType = '';
    }


    getNewLeaseAgreementId() {
        this.billingEvent.leaseAgreementId = this.billingEventLeaseAgreementLookupTableModal.id;
        this.leaseAgreementTitle = this.billingEventLeaseAgreementLookupTableModal.displayName;
    }
    getNewVendorChargeId() {
        this.billingEvent.vendorChargeId = this.billingEventVendorChargeLookupTableModal.id;
        this.vendorChargeReference = this.billingEventVendorChargeLookupTableModal.displayName;
    }
    getNewBillingEventTypeId() {
        this.billingEvent.billingEventTypeId = this.billingEventBillingEventTypeLookupTableModal.id;
        this.billingEventTypeType = this.billingEventBillingEventTypeLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
