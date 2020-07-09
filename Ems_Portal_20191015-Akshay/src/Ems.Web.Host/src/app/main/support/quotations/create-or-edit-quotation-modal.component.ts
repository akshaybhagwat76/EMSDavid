import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { QuotationsServiceProxy, WorkOrdersServiceProxy, CreateOrEditQuotationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { QuotationSupportContractLookupTableModalComponent } from './quotation-supportContract-lookup-table-modal.component';
import { QuotationQuotationStatusLookupTableModalComponent } from './quotation-quotationStatus-lookup-table-modal.component';
import { QuotationWorkOrderLookupTableModalComponent } from './quotation-workOrder-lookup-table-modal.component';
import { QuotationAssetLookupTableModalComponent } from './quotation-asset-lookup-table-modal.component';
import { QuotationAssetClassLookupTableModalComponent } from './quotation-assetClass-lookup-table-modal.component';
import { QuotationSupportTypeLookupTableModalComponent } from './quotation-supportType-lookup-table-modal.component';
import { QuotationSupportItemLookupTableModalComponent } from './quotation-supportItem-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditQuotationModal',
    templateUrl: './create-or-edit-quotation-modal.component.html'
})
export class CreateOrEditQuotationModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('quotationSupportContractLookupTableModal', { static: true }) quotationSupportContractLookupTableModal: QuotationSupportContractLookupTableModalComponent;
    @ViewChild('quotationQuotationStatusLookupTableModal', { static: true }) quotationQuotationStatusLookupTableModal: QuotationQuotationStatusLookupTableModalComponent;
    @ViewChild('quotationWorkOrderLookupTableModal', { static: true }) quotationWorkOrderLookupTableModal: QuotationWorkOrderLookupTableModalComponent;
    @ViewChild('quotationAssetLookupTableModal', { static: true }) quotationAssetLookupTableModal: QuotationAssetLookupTableModalComponent;
    @ViewChild('quotationAssetClassLookupTableModal', { static: true }) quotationAssetClassLookupTableModal: QuotationAssetClassLookupTableModalComponent;
    @ViewChild('quotationSupportTypeLookupTableModal', { static: true }) quotationSupportTypeLookupTableModal: QuotationSupportTypeLookupTableModalComponent;
    @ViewChild('quotationSupportItemLookupTableModal', { static: true }) quotationSupportItemLookupTableModal: QuotationSupportItemLookupTableModalComponent;

    @Output() quotationModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    quotation: CreateOrEditQuotationDto = new CreateOrEditQuotationDto();

    startDate: Date;
    endDate: Date;
    acknowledgedAt: Date;
    supportContractTitle = '';
    quotationStatusStatus = '';
    assetReference = '';
    assetClassClass = '';
    supportTypeType = '';
    supportItemDescription = '';
    ao_address = '';
    isFormValid = false;
    errorMsg = "";
    workOrderSubject = '';

    constructor(
        injector: Injector,
        private _quotationsServiceProxy: QuotationsServiceProxy,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy
    ) {
        super(injector);
    }

    show(quotationId?: number, workOrderId?: number): void {
        this.endDate = null;
        this.acknowledgedAt = null;

        if (!quotationId) {
            this.quotation = new CreateOrEditQuotationDto();
            this.quotation.id = quotationId;
            this.startDate = moment().utcOffset(0, true).toDate();
            this.supportContractTitle = '';
            this.quotationStatusStatus = '';
            this.workOrderSubject = '';
            this.ao_address = '';

            if (workOrderId && workOrderId > 0) {
                this.getWorkOrder(workOrderId);
            }
            else {
                this.assetReference = '';
                this.assetClassClass = '';
                this.supportItemDescription = '';
                this.supportTypeType = '';
            }

            this.getDefaultStatus();

            this.active = true;
            this.modal.show();
        } else {
            this._quotationsServiceProxy.getQuotationForEdit(quotationId).subscribe(result => {
                this.quotation = result.quotation;

                if (this.quotation.startDate) {
                    this.startDate = this.quotation.startDate.utcOffset(0, true).toDate();
                }
                if (this.quotation.endDate) {
                    this.endDate = this.quotation.endDate.utcOffset(0, true).toDate();
                }
                if (this.quotation.acknowledgedAt) {
                    this.acknowledgedAt = this.quotation.acknowledgedAt.utcOffset(0, true).toDate();
                }

                this.supportContractTitle = result.supportContractTitle;
                this.quotationStatusStatus = result.quotationStatusStatus;
                this.workOrderSubject = result.workOrderSubject;
                this.assetReference = result.assetReference;
                this.assetClassClass = result.assetClassClass;
                this.supportTypeType = result.supportTypeType;
                this.supportItemDescription = result.supportItemDescription;

                let addressInfo = result.aO_Address;
                if (addressInfo != null) {
                    this.ao_address = addressInfo.addressEntryName + ', ' + addressInfo.addressLine1 + ', ' + addressInfo.addressLine2 + ', ' + addressInfo.city + ', ' + addressInfo.state + ', ' + addressInfo.country + ', ' + addressInfo.postalCode;
                }

                this.active = true;
                this.modal.show();
            });
        }
    }

    getDefaultStatus(): void {
        this._quotationsServiceProxy.getAllQuotationStatusForLookupTable(environment.defaultStatus, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let status = result.items[0];

                this.quotation.quotationStatusId = status.id;
                this.quotationStatusStatus = status.displayName;
            }
        });
    }

    getWorkOrder(workOrderId?: number): void {
        this._workOrdersServiceProxy.getWorkOrderForEdit(workOrderId).subscribe(result => {
            let wResult = result.workOrder;

            this.quotation.workOrderId = wResult.id;
            this.quotation.title = wResult.subject;
            this.quotation.description = wResult.description;
            this.workOrderSubject = wResult.subject;

            this.appendWorkOrderRelatedFKData(this.quotation.workOrderId, 0);

            this.active = true;
            this.modal.show();
        });
    }

    save(): void {

        if (this.startDate) {
            if (!this.quotation.startDate) {
                this.quotation.startDate = moment(this.startDate).startOf('day').utcOffset(0, true);
            }
            else {
                this.quotation.startDate = moment(this.startDate).utcOffset(0, true);
            }
        }
        else {
            this.quotation.startDate = null;
        }

        if (this.endDate) {
            if (!this.quotation.endDate) {
                this.quotation.endDate = moment(this.endDate).startOf('day').utcOffset(0, true);
            }
            else {
                this.quotation.endDate = moment(this.endDate).utcOffset(0, true);
            }
        }
        else {
            this.quotation.endDate = null;
        }

        if (this.acknowledgedAt) {
            if (!this.quotation.acknowledgedAt) {
                this.quotation.acknowledgedAt = moment(this.acknowledgedAt).startOf('day').utcOffset(0, true);
            }
            else {
                this.quotation.acknowledgedAt = moment(this.acknowledgedAt).utcOffset(0, true);
            }
        }
        else {
            this.quotation.acknowledgedAt = null;
        }

        if (!this.quotation.supportContractId || !this.quotation.reference
            || !this.quotation.title || !this.quotation.startDate) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.quotation.endDate && this.quotation.endDate < this.quotation.startDate) {
            this.isFormValid = false;
            this.errorMsg = "End Date must be greater or equals to Start Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._quotationsServiceProxy.createOrEdit(this.quotation)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.quotationModalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectSupportContractModal() {
        this.quotationSupportContractLookupTableModal.id = this.quotation.supportContractId;
        this.quotationSupportContractLookupTableModal.displayName = this.supportContractTitle;
        this.quotationSupportContractLookupTableModal.assetId = this.quotation.assetId;
        this.quotationSupportContractLookupTableModal.show();
    }
    openSelectQuotationStatusModal() {
        this.quotationQuotationStatusLookupTableModal.id = this.quotation.quotationStatusId;
        this.quotationQuotationStatusLookupTableModal.displayName = this.quotationStatusStatus;
        this.quotationQuotationStatusLookupTableModal.show();
    }
    openSelectWorkOrderModal() {
        this.quotationWorkOrderLookupTableModal.id = this.quotation.workOrderId;
        this.quotationWorkOrderLookupTableModal.displayName = this.workOrderSubject;
        this.quotationWorkOrderLookupTableModal.show();
    }
    openSelectAssetModal() {
        this.quotationAssetLookupTableModal.id = this.quotation.assetId;
        this.quotationAssetLookupTableModal.displayName = this.assetReference;
        this.quotationAssetLookupTableModal.workOrderId = this.quotation.workOrderId ? this.quotation.workOrderId : 0;
        this.quotationAssetLookupTableModal.show();
    }
    openSelectAssetClassModal() {
        this.quotationAssetClassLookupTableModal.id = this.quotation.assetClassId;
        this.quotationAssetClassLookupTableModal.displayName = this.assetClassClass;
        this.quotationAssetClassLookupTableModal.assetId = this.quotation.assetId;
        this.quotationAssetClassLookupTableModal.show();
    }
    openSelectSupportTypeModal() {
        this.quotationSupportTypeLookupTableModal.id = this.quotation.supportTypeId;
        this.quotationSupportTypeLookupTableModal.displayName = this.supportTypeType;
        this.quotationSupportTypeLookupTableModal.assetId = this.quotation.assetId;
        this.quotationSupportTypeLookupTableModal.show();
    }
    openSelectSupportItemModal() {
        this.quotationSupportItemLookupTableModal.id = this.quotation.supportItemId;
        this.quotationSupportItemLookupTableModal.displayName = this.supportItemDescription;
        this.quotationSupportItemLookupTableModal.supportContractId = this.quotation.supportContractId;
        this.quotationSupportItemLookupTableModal.show();
    }


    setSupportContractIdNull() {
        this.quotation.supportContractId = null;
        this.supportContractTitle = '';

        this.setSupportItemIdNull();
    }
    setQuotationStatusIdNull() {
        this.quotation.quotationStatusId = null;
        this.quotationStatusStatus = '';
    }
    setAssetIdNull() {
        this.quotation.assetId = null;
        this.assetReference = '';

        this.setAssetAndSupportItemNull(false);
    }
    setAssetClassIdNull() {
        this.quotation.assetClassId = null;
        this.assetClassClass = '';
    }
    setSupportTypeIdNull() {
        this.quotation.supportTypeId = null;
        this.supportTypeType = '';
    }
    setSupportItemIdNull() {
        this.quotation.supportItemId = null;
        this.supportItemDescription = '';
    }
    setWorkOrderIdNull() {
        this.quotation.workOrderId = null;
        this.workOrderSubject = '';

        this.setAssetAndSupportItemNull(true);
    }
    setAssetAndSupportItemNull(isWorkOrder: boolean) {
        if (isWorkOrder)
            this.setAssetIdNull();

        this.setSupportContractIdNull();
        this.setSupportItemIdNull();
        this.setAssetClassIdNull();
        this.setSupportTypeIdNull();
    }




    getNewSupportContractId() {
        this.quotation.supportContractId = this.quotationSupportContractLookupTableModal.id;
        this.supportContractTitle = this.quotationSupportContractLookupTableModal.displayName;
    }
    getNewQuotationStatusId() {
        this.quotation.quotationStatusId = this.quotationQuotationStatusLookupTableModal.id;
        this.quotationStatusStatus = this.quotationQuotationStatusLookupTableModal.displayName;
    }
    getNewAssetId() {
        this.quotation.assetId = this.quotationAssetLookupTableModal.id;
        this.assetReference = this.quotationAssetLookupTableModal.displayName;

        if (this.quotation.assetId > 0) {
            this.appendWorkOrderRelatedFKData(0, this.quotation.assetId);
        }
        else {
            this.setAssetAndSupportItemNull(false);
        }
    }
    getNewAssetClassId() {
        this.quotation.assetClassId = this.quotationAssetClassLookupTableModal.id;
        this.assetClassClass = this.quotationAssetClassLookupTableModal.displayName;
    }
    getNewSupportTypeId() {
        this.quotation.supportTypeId = this.quotationSupportTypeLookupTableModal.id;
        this.supportTypeType = this.quotationSupportTypeLookupTableModal.displayName;
    }
    getNewSupportItemId() {
        this.quotation.supportItemId = this.quotationSupportItemLookupTableModal.id;
        this.supportItemDescription = this.quotationSupportItemLookupTableModal.displayName;
    }
    getNewWorkOrderId() {
        this.quotation.workOrderId = this.quotationWorkOrderLookupTableModal.id;
        this.workOrderSubject = this.quotationWorkOrderLookupTableModal.displayName;

        if (this.quotation.workOrderId > 0) {
            this.appendWorkOrderRelatedFKData(this.quotation.workOrderId, 0);
        }
        else {
            this.setAssetAndSupportItemNull(true);
        }
    }

    appendWorkOrderRelatedFKData(workOrderId: number, assetId: number) {
        this._quotationsServiceProxy.getQuotationAssetAndSupportItemList(workOrderId, assetId)
            .subscribe(result => {
                let assetList = result.assetList;
                let assetClassList = result.assetClassList;
                let supportItemList = result.supportItemList;
                let supportContractList = result.supportContractList;
                let supportTypeList = result.supportTypeList;
                let addressInfo = result.aO_Address;

                if (assetList && assetList.length == 1) {
                    this.quotation.assetId = assetList[0].id;
                    this.assetReference = assetList[0].displayName;
                }

                if (supportContractList && supportContractList.length == 1) {
                    this.quotation.supportContractId = supportContractList[0].id;
                    this.supportContractTitle = supportContractList[0].displayName;
                }

                if (supportItemList && supportItemList.length == 1) {
                    this.quotation.supportItemId = supportItemList[0].id;
                    this.supportItemDescription = supportItemList[0].displayName;
                }

                if (assetClassList && assetClassList.length == 1) {
                    this.quotation.assetClassId = assetClassList[0].id;
                    this.assetClassClass = assetClassList[0].displayName;
                }

                if (supportTypeList && supportTypeList.length == 1) {
                    this.quotation.supportTypeId = supportTypeList[0].id;
                    this.supportTypeType = supportTypeList[0].displayName;
                }

                if (addressInfo != null) {
                    this.ao_address = addressInfo.addressEntryName + (addressInfo.addressLine1 ? ', ' + addressInfo.addressLine1 : '') + (addressInfo.addressLine2 ? ', ' + addressInfo.addressLine2 : '') + (addressInfo.city ? ', ' + addressInfo.city : '') + (addressInfo.state ? ', ' + addressInfo.state : '') + (addressInfo.country ? ', ' + addressInfo.country : '') + (addressInfo.postalCode ? ', ' + addressInfo.postalCode : '');
                }
                else
                    this.ao_address = '';
            });
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
