import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { LeaseItemsServiceProxy, CreateOrEditLeaseItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { LeaseItemAssetClassLookupTableModalComponent } from './leaseItem-assetClass-lookup-table-modal.component';
import { LeaseItemAssetLookupTableModalComponent } from './leaseItem-asset-lookup-table-modal.component';
import { LeaseItemDepositUomLookupTableModalComponent } from './leaseItem-deposit-uom-lookup-table-modal.component';
import { LeaseItemRentalUomLookupTableModalComponent } from './leaseItem-rental-uom-lookup-table-modal.component';
//import { LeaseItemLeaseAgreementLookupTableModalComponent } from './leaseItem-leaseAgreement-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditLeaseItemModal',
    templateUrl: './create-or-edit-leaseItem-modal.component.html'
})
export class CreateOrEditLeaseItemModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('leaseItemAssetClassLookupTableModal', { static: true }) leaseItemAssetClassLookupTableModal: LeaseItemAssetClassLookupTableModalComponent;
    @ViewChild('leaseItemAssetLookupTableModal', { static: true }) leaseItemAssetLookupTableModal: LeaseItemAssetLookupTableModalComponent;
    @ViewChild('leaseItemDepositUomLookupTableModal', { static: true }) leaseItemDepositUomLookupTableModal: LeaseItemDepositUomLookupTableModalComponent;
    @ViewChild('leaseItemRentalUomLookupTableModal', { static: true }) leaseItemRentalUomLookupTableModal: LeaseItemRentalUomLookupTableModalComponent;
    //@ViewChild('leaseItemLeaseAgreementLookupTableModal', { static: true }) leaseItemLeaseAgreementLookupTableModal: LeaseItemLeaseAgreementLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    leaseItem: CreateOrEditLeaseItemDto = new CreateOrEditLeaseItemDto();
    leaseAgreementId: number;
    dateAllocated: Date;
    startDate: Date;
    endDate: Date;
    assetClassClass = '';
    assetReference = '';
    depositUOM = '';
    rentalUOM = '';
    leaseAgreementTitle = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _leaseItemsServiceProxy: LeaseItemsServiceProxy
    ) {
        super(injector);
    }

    show(leaseItemId?: number, leaseAgreementId?: number): void {
        this.startDate = null;
        this.endDate = null;

        if (!leaseItemId) {
            this.leaseItem = new CreateOrEditLeaseItemDto();
            this.leaseItem.id = leaseItemId;
            this.leaseItem.leaseAgreementId = leaseAgreementId;
            this.assetClassClass = '';
            this.assetReference = '';
            this.depositUOM = '';
            this.rentalUOM = '';
            this.leaseAgreementTitle = '';
            this.dateAllocated = moment().toDate();
            this.leaseItem.allocationPercentage = 100;

            this.getDefaultRentalPeriod();

            this.active = true;
            this.modal.show();
        } else {
            this._leaseItemsServiceProxy.getLeaseItemForEdit(leaseItemId).subscribe(result => {
                this.leaseItem = result.leaseItem;
                this.leaseItem.leaseAgreementId = leaseAgreementId;

                if (this.leaseItem.dateAllocated) {
                    this.dateAllocated = this.leaseItem.dateAllocated.toDate();
                }
                if (this.leaseItem.startDate) {
                    this.startDate = this.leaseItem.startDate.toDate();
                }
                if (this.leaseItem.endDate) {
                    this.endDate = this.leaseItem.endDate.toDate();
                }
                this.assetClassClass = result.assetClassClass;
                this.assetReference = result.assetReference;
                this.depositUOM = result.depositUom;
                this.rentalUOM = result.rentalUom;
                this.leaseAgreementTitle = result.leaseAgreementTitle;

                this.active = true;
                this.modal.show();
            });
        }
    }

    getDefaultRentalPeriod(): void {
        this._leaseItemsServiceProxy.getAllUomForLookupTable(environment.defaultUom, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let rentalPeriod = result.items[0];

                this.leaseItem.rentalUomRefId = rentalPeriod.id;
                this.rentalUOM = rentalPeriod.unitOfMeasurement;
            }
        });
    }

    save(): void {

        if (this.dateAllocated) {
            this.leaseItem.dateAllocated = moment(this.dateAllocated);
        }
        else {
            this.leaseItem.dateAllocated = null;
        }

        if (this.startDate) {
            this.leaseItem.startDate = moment(this.startDate);
        }
        else {
            this.leaseItem.startDate = null;
        }

        if (this.endDate) {
            this.leaseItem.endDate = moment(this.endDate);
        }
        else {
            this.leaseItem.endDate = null;
        }

        if (!this.leaseItem.description)
            this.leaseItem.description = "";

        if (!this.leaseItem.item)
            this.leaseItem.item = "";

        if (!this.leaseItem.dateAllocated || !this.leaseItem.unitRentalRate || !this.leaseItem.unitDepositRate
            || !this.leaseItem.startDate || !this.leaseItem.endDate) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.leaseItem.unitRentalRate <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Unit rental rate must be greater than zero";
        }
        else if (this.leaseItem.unitDepositRate <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Unit deposit rate must be greater than zero";
        }
        else if (this.leaseItem.endDate < this.leaseItem.startDate) {
            this.isFormValid = false;
            this.errorMsg = "End Date must be greater or equals to Start Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._leaseItemsServiceProxy.createOrEdit(this.leaseItem)
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

    openSelectAssetClassModal() {
        this.leaseItemAssetClassLookupTableModal.id = this.leaseItem.assetClassId;
        this.leaseItemAssetClassLookupTableModal.displayName = this.assetClassClass;
        this.leaseItemAssetClassLookupTableModal.show();
    }
    openSelectAssetModal() {
        this.leaseItemAssetLookupTableModal.id = this.leaseItem.assetId;
        this.leaseItemAssetLookupTableModal.displayName = this.assetReference;
        this.leaseItemAssetLookupTableModal.show();
    }
    openSelectDepositUomModal() {
        this.leaseItemDepositUomLookupTableModal.id = this.leaseItem.depositUomRefId;
        this.leaseItemDepositUomLookupTableModal.displayName = this.depositUOM;
        this.leaseItemDepositUomLookupTableModal.show();
    }
    openSelectRentalUomModal() {
        this.leaseItemRentalUomLookupTableModal.id = this.leaseItem.rentalUomRefId;
        this.leaseItemRentalUomLookupTableModal.displayName = this.rentalUOM;
        this.leaseItemRentalUomLookupTableModal.show();
    }
    //openSelectLeaseAgreementModal() {
    //    this.leaseItemLeaseAgreementLookupTableModal.id = this.leaseItem.leaseAgreementId;
    //    this.leaseItemLeaseAgreementLookupTableModal.displayName = this.leaseAgreementTitle;
    //    this.leaseItemLeaseAgreementLookupTableModal.show();
    //}


    setAssetClassIdNull() {
        this.leaseItem.assetClassId = null;
        this.assetClassClass = '';
    }
    setAssetIdNull() {
        this.leaseItem.assetId = null;
        this.assetReference = '';
    }
    setDepositUomIdNull() {
        this.leaseItem.depositUomRefId = null;
        this.depositUOM = '';
    }
    setRentalUomIdNull() {
        this.leaseItem.rentalUomRefId = null;
        this.rentalUOM = '';
    }
    //setLeaseAgreementIdNull() {
    //    this.leaseItem.leaseAgreementId = null;
    //    this.leaseAgreementTitle = '';
    //}


    getNewAssetClassId() {
        this.leaseItem.assetClassId = this.leaseItemAssetClassLookupTableModal.id;
        this.assetClassClass = this.leaseItemAssetClassLookupTableModal.displayName;
    }
    getNewAssetId() {
        this.leaseItem.assetId = this.leaseItemAssetLookupTableModal.id;
        this.assetReference = this.leaseItemAssetLookupTableModal.displayName;
    }
    getNewDepositUomId() {
        this.leaseItem.depositUomRefId = this.leaseItemDepositUomLookupTableModal.id;
        this.depositUOM = this.leaseItemDepositUomLookupTableModal.displayName;
    }
    getNewRentalUomId() {
        this.leaseItem.rentalUomRefId = this.leaseItemRentalUomLookupTableModal.id;
        this.rentalUOM = this.leaseItemRentalUomLookupTableModal.displayName;
    }
    //getNewLeaseAgreementId() {
    //    this.leaseItem.leaseAgreementId = this.leaseItemLeaseAgreementLookupTableModal.id;
    //    this.leaseAgreementTitle = this.leaseItemLeaseAgreementLookupTableModal.displayName;
    //}

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
