import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { VendorChargesServiceProxy, CreateOrEditVendorChargeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { VendorChargeVendorLookupTableModalComponent } from './vendorCharge-vendor-lookup-table-modal.component';
import { VendorChargeSupportContractLookupTableModalComponent } from './vendorCharge-supportContract-lookup-table-modal.component';
import { VendorChargeWorkOrderLookupTableModalComponent } from './vendorCharge-workOrder-lookup-table-modal.component';
import { VendorChargeVendorChargeStatusLookupTableModalComponent } from './vendorCharge-vendorChargeStatus-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditVendorChargeModal',
    templateUrl: './create-or-edit-vendorCharge-modal.component.html'
})
export class CreateOrEditVendorChargeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('vendorChargeVendorLookupTableModal', { static: true }) vendorChargeVendorLookupTableModal: VendorChargeVendorLookupTableModalComponent;
    @ViewChild('vendorChargeSupportContractLookupTableModal', { static: true }) vendorChargeSupportContractLookupTableModal: VendorChargeSupportContractLookupTableModalComponent;
    @ViewChild('vendorChargeWorkOrderLookupTableModal', { static: true }) vendorChargeWorkOrderLookupTableModal: VendorChargeWorkOrderLookupTableModalComponent;
    @ViewChild('vendorChargeVendorChargeStatusLookupTableModal', { static: true }) vendorChargeVendorChargeStatusLookupTableModal: VendorChargeVendorChargeStatusLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    vendorCharge: CreateOrEditVendorChargeDto = new CreateOrEditVendorChargeDto();

    dateIssued: Date;
    dateDue: Date;
    vendorName = '';
    supportContractTitle = '';
    workOrderSubject = '';
    isFormValid = false;
    errorMsg = "";
    vendorChargeStatusStatus = '';

    constructor(
        injector: Injector,
        private _vendorChargesServiceProxy: VendorChargesServiceProxy
    ) {
        super(injector);
    }

    show(vendorChargeId?: number): void {
        this.dateIssued = null;
        this.dateDue = null;

        if (!vendorChargeId) {
            this.vendorCharge = new CreateOrEditVendorChargeDto();
            this.vendorCharge.id = vendorChargeId;
            this.vendorName = '';
            this.supportContractTitle = '';
            this.workOrderSubject = '';
            this.vendorChargeStatusStatus = '';

            this.getDefaultVendorChargeStatus();

            this.active = true;
            this.modal.show();
        } else {
            this._vendorChargesServiceProxy.getVendorChargeForEdit(vendorChargeId).subscribe(result => {
                this.vendorCharge = result.vendorCharge;

                if (this.vendorCharge.dateIssued) {
                    this.dateIssued = this.vendorCharge.dateIssued.toDate();
                }
                if (this.vendorCharge.dateDue) {
                    this.dateDue = this.vendorCharge.dateDue.toDate();
                }
                this.vendorName = result.vendorName;
                this.supportContractTitle = result.supportContractTitle;
                this.workOrderSubject = result.workOrderSubject;
                this.vendorChargeStatusStatus = result.vendorChargeStatusStatus;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;

        if (this.dateIssued) {
            this.vendorCharge.dateIssued = moment(this.dateIssued);
        }
        else {
            this.vendorCharge.dateIssued = null;
        }

        if (this.dateDue) {
            this.vendorCharge.dateDue = moment(this.dateDue);
        }
        else {
            this.vendorCharge.dateDue = null;
        }

        if (!this.vendorCharge.vendorId || !this.vendorCharge.reference
            || !this.vendorCharge.description || !this.vendorCharge.dateIssued) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.vendorCharge.dateDue && this.vendorCharge.dateDue < this.vendorCharge.dateIssued) {
            this.isFormValid = false;
            this.errorMsg = "Due Date must be greater or equals to Issued Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._vendorChargesServiceProxy.createOrEdit(this.vendorCharge)
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

    getDefaultVendorChargeStatus(): void {
        this._vendorChargesServiceProxy.getAllVendorChargeStatusForLookupTable(environment.defaultStatus, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let vendorChargeStatus = result.items[0];

                this.vendorCharge.vendorChargeStatusId = vendorChargeStatus.id;
                this.vendorChargeStatusStatus = vendorChargeStatus.displayName;
            }
        });
    }

    openSelectVendorModal() {
        this.vendorChargeVendorLookupTableModal.id = this.vendorCharge.vendorId;
        this.vendorChargeVendorLookupTableModal.displayName = this.vendorName;
        this.vendorChargeVendorLookupTableModal.show();
    }
    openSelectSupportContractModal() {
        this.vendorChargeSupportContractLookupTableModal.id = this.vendorCharge.supportContractId;
        this.vendorChargeSupportContractLookupTableModal.displayName = this.supportContractTitle;
        this.vendorChargeSupportContractLookupTableModal.show();
    }
    openSelectWorkOrderModal() {
        this.vendorChargeWorkOrderLookupTableModal.id = this.vendorCharge.workOrderId;
        this.vendorChargeWorkOrderLookupTableModal.displayName = this.workOrderSubject;
        this.vendorChargeWorkOrderLookupTableModal.show();
    }
    openSelectVendorChargeStatusModal() {
        this.vendorChargeVendorChargeStatusLookupTableModal.id = this.vendorCharge.vendorChargeStatusId;
        this.vendorChargeVendorChargeStatusLookupTableModal.displayName = this.vendorChargeStatusStatus;
        this.vendorChargeVendorChargeStatusLookupTableModal.show();
    }


    setVendorIdNull() {
        this.vendorCharge.vendorId = null;
        this.vendorName = '';
    }
    setSupportContractIdNull() {
        this.vendorCharge.supportContractId = null;
        this.supportContractTitle = '';
    }
    setWorkOrderIdNull() {
        this.vendorCharge.workOrderId = null;
        this.workOrderSubject = '';
    }
    setVendorChargeStatusIdNull() {
        this.vendorCharge.vendorChargeStatusId = null;
        this.vendorChargeStatusStatus = '';
    }


    getNewVendorId() {
        this.vendorCharge.vendorId = this.vendorChargeVendorLookupTableModal.id;
        this.vendorName = this.vendorChargeVendorLookupTableModal.displayName;
    }
    getNewSupportContractId() {
        this.vendorCharge.supportContractId = this.vendorChargeSupportContractLookupTableModal.id;
        this.supportContractTitle = this.vendorChargeSupportContractLookupTableModal.displayName;
    }
    getNewWorkOrderId() {
        this.vendorCharge.workOrderId = this.vendorChargeWorkOrderLookupTableModal.id;
        this.workOrderSubject = this.vendorChargeWorkOrderLookupTableModal.displayName;
    }
    getNewVendorChargeStatusId() {
        this.vendorCharge.vendorChargeStatusId = this.vendorChargeVendorChargeStatusLookupTableModal.id;
        this.vendorChargeStatusStatus = this.vendorChargeVendorChargeStatusLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
