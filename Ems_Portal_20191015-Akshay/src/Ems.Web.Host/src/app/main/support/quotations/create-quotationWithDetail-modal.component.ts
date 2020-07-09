import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { QuotationsServiceProxy, WorkOrdersServiceProxy, CreateQuotationWithDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { environment } from 'environments/environment';
import * as moment from 'moment';
import { QuotationSupportContractLookupTableModalComponent } from './quotation-supportContract-lookup-table-modal.component';
import { QuotationQuotationStatusLookupTableModalComponent } from './quotation-quotationStatus-lookup-table-modal.component';
import { QuotationAssetLookupTableModalComponent } from './quotation-asset-lookup-table-modal.component';
import { QuotationAssetClassLookupTableModalComponent } from './quotation-assetClass-lookup-table-modal.component';
import { QuotationSupportTypeLookupTableModalComponent } from './quotation-supportType-lookup-table-modal.component';
import { ItemTypeLookupTableModalComponent } from '@app/config/quotations/itemTypes/itemType-lookup-table-modal.component';
import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';
import { QuotationWorkOrderLookupTableModalComponent } from './quotation-workOrder-lookup-table-modal.component';
import { QuotationSupportItemLookupTableModalComponent } from './quotation-supportItem-lookup-table-modal.component';


@Component({
    selector: 'createQuotationWithDetailModal',
    templateUrl: './create-quotationWithDetail-modal.component.html'
})
export class CreateQuotationWithDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('quotationSupportContractLookupTableModal', { static: true }) quotationSupportContractLookupTableModal: QuotationSupportContractLookupTableModalComponent;
    @ViewChild('quotationQuotationStatusLookupTableModal', { static: true }) quotationQuotationStatusLookupTableModal: QuotationQuotationStatusLookupTableModalComponent;
    @ViewChild('quotationAssetLookupTableModal', { static: true }) quotationAssetLookupTableModal: QuotationAssetLookupTableModalComponent;
    @ViewChild('quotationAssetClassLookupTableModal', { static: true }) quotationAssetClassLookupTableModal: QuotationAssetClassLookupTableModalComponent;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    @ViewChild('quotationSupportTypeLookupTableModal', { static: true }) quotationSupportTypeLookupTableModal: QuotationSupportTypeLookupTableModalComponent;
    @ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;
    @ViewChild('quotationSupportItemLookupTableModal', { static: true }) quotationSupportItemLookupTableModal: QuotationSupportItemLookupTableModalComponent;
    @ViewChild('quotationWorkOrderLookupTableModal', { static: true }) quotationWorkOrderLookupTableModal: QuotationWorkOrderLookupTableModalComponent;

    @Output() quotationWithDetailModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    quotationWithDetail: CreateQuotationWithDetailDto;// = new CreateOrEditQuotationDto();

    startDate: Date;
    endDate: Date;
    acknowledgedAt: Date;
    supportContractTitle = '';
    quotationStatusStatus = '';

    //quotationDetail: CreateQuotationWithDetailDto;// = new CreateQuotationWithDetailDto();
    quotationId: number;
    quotationSubject = '';
    assetReference = '';
    assetClassClass = '';
    itemTypeType = '';
    supportTypeType = '';
    quotationTitle = '';
    uomUnitOfMeasurement = '';
    supportItemDescription = '';
    workOrderSubject = '';
    ao_address = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _quotationsServiceProxy: QuotationsServiceProxy,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy
    ) {
        super(injector);
    }

    show(workOrderId?: number): void {
        this.startDate = moment().toDate();
        this.endDate = null;
        this.acknowledgedAt = null;

        this.quotationWithDetail = new CreateQuotationWithDetailDto();
        this.quotationWithDetail.isTaxable = true;
        this.supportContractTitle = '';
        this.quotationStatusStatus = '';

        this.itemTypeType = '';
        this.quotationTitle = '';
        this.uomUnitOfMeasurement = '';
        this.workOrderSubject = '';

        if (workOrderId > 0) {
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
    }

    getDefaultStatus(): void {
        this._quotationsServiceProxy.getAllQuotationStatusForLookupTable(environment.defaultStatus, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let status = result.items[0];

                this.quotationWithDetail.quotationStatusId = status.id;
                this.quotationStatusStatus = status.displayName;
            }
        });
    }

    save(): void {

        if (this.startDate) {
            this.quotationWithDetail.startDate = moment(this.startDate);
        }
        else {
            this.quotationWithDetail.startDate = null;
        }

        if (this.endDate) {
            this.quotationWithDetail.endDate = moment(this.endDate);
        }
        else {
            this.quotationWithDetail.endDate = null;
        }

        if (this.acknowledgedAt) {
            this.quotationWithDetail.acknowledgedAt = moment(this.acknowledgedAt);
        }
        else {
            this.quotationWithDetail.acknowledgedAt = null;
        }

        if (!this.quotationWithDetail.supportContractId || !this.quotationWithDetail.reference
            || !this.quotationWithDetail.title 
            || !this.quotationWithDetail.startDate || !this.quotationWithDetail.assetId
            || !this.quotationWithDetail.supportItemId || !this.quotationWithDetail.detailDescription || !this.quotationWithDetail.quantity
            || !this.quotationWithDetail.unitPrice || !this.quotationWithDetail.cost || !this.quotationWithDetail.charge) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.quotationWithDetail.endDate && this.quotationWithDetail.endDate < this.quotationWithDetail.startDate) {
            this.isFormValid = false;
            this.errorMsg = "End Date must be greater or equals to Start Date";
        }
        else if (this.quotationWithDetail.quantity <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Quantity must be greater than zero";
        }
        else if (this.quotationWithDetail.unitPrice <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Unit price must be greater than zero";
        }
        else if (this.quotationWithDetail.cost <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Cost must be greater than zero";
        }
        //else if (this.quotationWithDetail.tax <= 0) {
        //    this.isFormValid = false;
        //    this.errorMsg = "Tax must be greater than zero";
        //}
        else if (this.quotationWithDetail.charge <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Charge must be greater than zero";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            if (this.quotationWithDetail.isTaxable)
                this.quotationWithDetail.tax = environment.taxPercent;
            else
                this.quotationWithDetail.tax = 0;

            this.quotationWithDetail.markUp = this.quotationWithDetail.markUp ? this.quotationWithDetail.markUp : 0;
            this.quotationWithDetail.discount = this.quotationWithDetail.discount ? this.quotationWithDetail.discount : 0;

            this._quotationsServiceProxy.createWithDetail(this.quotationWithDetail)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.quotationWithDetailModalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    getWorkOrder(workOrderId?: number): void {
        this._workOrdersServiceProxy.getWorkOrderForEdit(workOrderId).subscribe(result => {
            let wResult = result.workOrder;

            this.quotationWithDetail.workOrderId = wResult.id;
            this.quotationWithDetail.title = wResult.subject;
            this.quotationWithDetail.quotationDescription = wResult.description;
            this.workOrderSubject = wResult.subject;

            this.appendWorkOrderRelatedFKData(this.quotationWithDetail.workOrderId, 0);

            this.active = true;
            this.modal.show();
        });
    }

    openSelectSupportContractModal() {
        this.quotationSupportContractLookupTableModal.id = this.quotationWithDetail.supportContractId;
        this.quotationSupportContractLookupTableModal.displayName = this.supportContractTitle;
        this.quotationSupportContractLookupTableModal.assetId = this.quotationWithDetail.assetId;
        this.quotationSupportContractLookupTableModal.show();
    }
    openSelectQuotationStatusModal() {
        this.quotationQuotationStatusLookupTableModal.id = this.quotationWithDetail.quotationStatusId;
        this.quotationQuotationStatusLookupTableModal.displayName = this.quotationStatusStatus;
        this.quotationQuotationStatusLookupTableModal.show();
    }

    setSupportContractIdNull() {
        this.quotationWithDetail.supportContractId = null;
        this.supportContractTitle = '';

        this.setSupportItemIdNull();
    }
    setQuotationStatusIdNull() {
        this.quotationWithDetail.quotationStatusId = null;
        this.quotationStatusStatus = '';
    }

    getNewSupportContractId() {
        this.quotationWithDetail.supportContractId = this.quotationSupportContractLookupTableModal.id;
        this.supportContractTitle = this.quotationSupportContractLookupTableModal.displayName;
    }
    getNewQuotationStatusId() {
        this.quotationWithDetail.quotationStatusId = this.quotationQuotationStatusLookupTableModal.id;
        this.quotationStatusStatus = this.quotationQuotationStatusLookupTableModal.displayName;
    }

    openSelectAssetModal() {
        this.quotationAssetLookupTableModal.id = this.quotationWithDetail.assetId;
        this.quotationAssetLookupTableModal.displayName = this.assetReference;
        this.quotationAssetLookupTableModal.workOrderId = this.quotationWithDetail.workOrderId ? this.quotationWithDetail.workOrderId : 0;
        this.quotationAssetLookupTableModal.show();
    }
    openSelectAssetClassModal() {
        this.quotationAssetClassLookupTableModal.id = this.quotationWithDetail.assetClassId;
        this.quotationAssetClassLookupTableModal.displayName = this.assetClassClass;
        this.quotationAssetClassLookupTableModal.assetId = this.quotationWithDetail.assetId;
        this.quotationAssetClassLookupTableModal.show();
    }
    openSelectItemTypeModal() {
        this.itemTypeLookupTableModal.id = this.quotationWithDetail.itemTypeId;
        this.itemTypeLookupTableModal.displayName = this.itemTypeType;
        this.itemTypeLookupTableModal.show();
    }
    openSelectSupportTypeModal() {
        this.quotationSupportTypeLookupTableModal.id = this.quotationWithDetail.supportTypeId;
        this.quotationSupportTypeLookupTableModal.displayName = this.supportTypeType;
        this.quotationSupportTypeLookupTableModal.assetId = this.quotationWithDetail.assetId;
        this.quotationSupportTypeLookupTableModal.show();
    }
    openSelectUomModal() {
        this.uomLookupTableModal.id = this.quotationWithDetail.uomId;
        this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
        this.uomLookupTableModal.show();
    }
    openSelectSupportItemModal() {
        this.quotationSupportItemLookupTableModal.id = this.quotationWithDetail.supportItemId;
        this.quotationSupportItemLookupTableModal.displayName = this.supportItemDescription;
        this.quotationSupportItemLookupTableModal.supportContractId = this.quotationWithDetail.supportContractId;
        this.quotationSupportItemLookupTableModal.show();
    }
    openSelectWorkOrderModal() {
        this.quotationWorkOrderLookupTableModal.id = this.quotationWithDetail.workOrderId;
        this.quotationWorkOrderLookupTableModal.displayName = this.workOrderSubject;
        this.quotationWorkOrderLookupTableModal.show();
    }


    setAssetIdNull() {
        this.quotationWithDetail.assetId = null;
        this.assetReference = '';

        this.setAssetAndSupportItemNull(false);
    }
    setAssetClassIdNull() {
        this.quotationWithDetail.assetClassId = null;
        this.assetClassClass = '';
    }
    setItemTypeIdNull() {
        this.quotationWithDetail.itemTypeId = null;
        this.itemTypeType = '';
    }
    setSupportTypeIdNull() {
        this.quotationWithDetail.supportTypeId = null;
        this.supportTypeType = '';
    }

    setUomIdNull() {
        this.quotationWithDetail.uomId = null;
        this.uomUnitOfMeasurement = '';
    }
    setSupportItemIdNull() {
        this.quotationWithDetail.supportItemId = null;
        this.supportItemDescription = '';
    }
    setWorkOrderIdNull() {
        this.quotationWithDetail.workOrderId = null;
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

    getNewAssetId() {
        this.quotationWithDetail.assetId = this.quotationAssetLookupTableModal.id;
        this.assetReference = this.quotationAssetLookupTableModal.displayName;

        if (this.quotationWithDetail.assetId > 0) {
            this.appendWorkOrderRelatedFKData(0, this.quotationWithDetail.assetId);
        }
        else {
            this.setAssetAndSupportItemNull(false);
        }
    }
    getNewAssetClassId() {
        this.quotationWithDetail.assetClassId = this.quotationAssetClassLookupTableModal.id;
        this.assetClassClass = this.quotationAssetClassLookupTableModal.displayName;
    }
    getNewItemTypeId() {
        this.quotationWithDetail.itemTypeId = this.itemTypeLookupTableModal.id;
        this.itemTypeType = this.itemTypeLookupTableModal.displayName;
    }
    getNewSupportTypeId() {
        this.quotationWithDetail.supportTypeId = this.quotationSupportTypeLookupTableModal.id;
        this.supportTypeType = this.quotationSupportTypeLookupTableModal.displayName;
    }
    getNewUomId() {
        this.quotationWithDetail.uomId = this.uomLookupTableModal.id;
        this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    }
    getNewSupportItemId() {
        this.quotationWithDetail.supportItemId = this.quotationSupportItemLookupTableModal.id;
        this.supportItemDescription = this.quotationSupportItemLookupTableModal.displayName;
    }
    getNewWorkOrderId() {
        this.quotationWithDetail.workOrderId = this.quotationWorkOrderLookupTableModal.id;
        this.workOrderSubject = this.quotationWorkOrderLookupTableModal.displayName;

        if (this.quotationWithDetail.workOrderId > 0) {
            this.appendWorkOrderRelatedFKData(this.quotationWithDetail.workOrderId, 0);
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
                let supportContractList = result.supportContractList;
                let supportItemList = result.supportItemList;
                let supportTypeList = result.supportTypeList;
                let addressInfo = result.aO_Address;

                if (assetList && assetList.length == 1) {
                    this.quotationWithDetail.assetId = assetList[0].id;
                    this.assetReference = assetList[0].displayName;
                }

                if (supportContractList && supportContractList.length == 1) {
                    this.quotationWithDetail.supportContractId = supportContractList[0].id;
                    this.supportContractTitle = supportContractList[0].displayName;
                }

                if (supportItemList && supportItemList.length == 1) {
                    this.quotationWithDetail.supportItemId = supportItemList[0].id;
                    this.supportItemDescription = supportItemList[0].displayName;
                }

                if (assetClassList && assetClassList.length == 1) {
                    this.quotationWithDetail.assetClassId = assetClassList[0].id;
                    this.assetClassClass = assetClassList[0].displayName;
                }

                if (supportTypeList && supportTypeList.length == 1) {
                    this.quotationWithDetail.supportTypeId = supportTypeList[0].id;
                    this.supportTypeType = supportTypeList[0].displayName;
                }

                if (addressInfo != null) {
                    this.ao_address = addressInfo.addressEntryName + (addressInfo.addressLine1 ? ', ' + addressInfo.addressLine1 : '') + (addressInfo.addressLine2 ? ', ' + addressInfo.addressLine2 : '') + (addressInfo.city ? ', ' + addressInfo.city : '') + (addressInfo.state ? ', ' + addressInfo.state : '') + (addressInfo.country ? ', ' + addressInfo.country : '') + (addressInfo.postalCode ? ', ' + addressInfo.postalCode : '');
                }
                else
                    this.ao_address = '';
            });
    }

    calculateAmount() {
        let markUp = this.quotationWithDetail.markUp;
        let unitPrice = this.quotationWithDetail.unitPrice;
        let quantity = this.quotationWithDetail.quantity;
        let tax = environment.taxPercent;
        let discount = this.quotationWithDetail.discount;

        if (quantity > 0 && unitPrice > 0) {
            let costPrice = unitPrice * quantity;

            if (markUp > 0) {
                costPrice += costPrice * (markUp / 100);
            }

            this.quotationWithDetail.cost = costPrice;

            let discountPrice = 0;
            if (discount > 0) {
                discountPrice = costPrice * (discount / 100);
            }

            let taxPrice = 0;
            if (this.quotationWithDetail.isTaxable) {
                taxPrice = (costPrice - discountPrice) * (tax / 100);
            }

            this.quotationWithDetail.charge = (costPrice - discountPrice) + taxPrice;
        }
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
