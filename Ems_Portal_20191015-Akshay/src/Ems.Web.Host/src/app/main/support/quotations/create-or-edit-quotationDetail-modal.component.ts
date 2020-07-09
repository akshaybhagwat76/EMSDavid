import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { QuotationDetailsServiceProxy, CreateOrEditQuotationDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { environment } from 'environments/environment';
import * as moment from 'moment';
import { ItemTypeLookupTableModalComponent } from '@app/config/quotations/itemTypes/itemType-lookup-table-modal.component';
import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';
//import { QuotationDetailAssetLookupTableModalComponent } from './quotationDetail-asset-lookup-table-modal.component';
//import { QuotationDetailAssetClassLookupTableModalComponent } from './quotationDetail-assetClass-lookup-table-modal.component';
//import { QuotationDetailSupportTypeLookupTableModalComponent } from './quotationDetail-supportType-lookup-table-modal.component';
//import { QuotationDetailSupportItemLookupTableModalComponent } from './quotationDetail-supportItem-lookup-table-modal.component';
//import { QuotationDetailWorkOrderLookupTableModalComponent } from './quotationDetail-workOrder-lookup-table-modal.component';


@Component({
    selector: 'createOrEditQuotationDetailModal',
    templateUrl: './create-or-edit-quotationDetail-modal.component.html'
})
export class CreateOrEditQuotationDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    //@ViewChild('quotationDetailAssetLookupTableModal', { static: true }) quotationDetailAssetLookupTableModal: QuotationDetailAssetLookupTableModalComponent;
    //@ViewChild('quotationDetailAssetClassLookupTableModal', { static: true }) quotationDetailAssetClassLookupTableModal: QuotationDetailAssetClassLookupTableModalComponent;
    //@ViewChild('quotationDetailSupportTypeLookupTableModal', { static: true }) quotationDetailSupportTypeLookupTableModal: QuotationDetailSupportTypeLookupTableModalComponent;
    //@ViewChild('quotationDetailSupportItemLookupTableModal', { static: true }) quotationDetailSupportItemLookupTableModal: QuotationDetailSupportItemLookupTableModalComponent;
    //@ViewChild('quotationDetailWorkOrderLookupTableModal', { static: true }) quotationDetailWorkOrderLookupTableModal: QuotationDetailWorkOrderLookupTableModalComponent;

    @Output() quotationDetailModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    quotationDetail: CreateOrEditQuotationDetailDto = new CreateOrEditQuotationDetailDto();
    quotationId: number;
    quotationSubject = '';
    itemTypeType = '';
    quotationTitle = '';
    uomUnitOfMeasurement = '';
    isFormValid = false;
    errorMsg = "";

    //assetReference = '';
    //assetClassClass = '';
    //supportTypeType = '';
    //supportItemDescription = '';
    //workOrderSubject = '';
    //ao_address = '';

    constructor(
        injector: Injector,
        private _quotationDetailsServiceProxy: QuotationDetailsServiceProxy
    ) {
        super(injector);
    }

    show(quotationDetailId?: number, quotationId?: number): void {

        if (!quotationDetailId) {
            this.quotationDetail = new CreateOrEditQuotationDetailDto();
            this.quotationDetail.id = quotationDetailId;
            this.quotationDetail.quotationId = quotationId;
            this.quotationDetail.isTaxable = true;

            this.itemTypeType = '';
            this.quotationTitle = '';
            this.uomUnitOfMeasurement = '';

            //this.assetReference = '';
            //this.assetClassClass = '';
            //this.supportTypeType = '';
            //this.supportItemDescription = '';
            //this.workOrderSubject = '';

            this.active = true;
            this.modal.show();
        } else {
            this._quotationDetailsServiceProxy.getQuotationDetailForEdit(quotationDetailId).subscribe(result => {

                this.quotationDetail = result.quotationDetail;
                this.quotationDetail.quotationId = quotationId;

                this.itemTypeType = result.itemTypeType;
                this.quotationTitle = result.quotationTitle;
                this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;

                if (this.quotationDetail.tax > 0)
                    this.quotationDetail.isTaxable = true;
                else
                    this.quotationDetail.isTaxable = false;

                //this.assetReference = result.assetReference;
                //this.assetClassClass = result.assetClassClass;
                //this.supportTypeType = result.supportTypeType;
                //this.supportItemDescription = result.supportItemDescription;
                //this.workOrderSubject = result.workOrderSubject;

                //let addressInfo = result.aO_Address;
                //if (addressInfo != null) {
                //    this.ao_address = addressInfo.addressEntryName + ', ' + addressInfo.addressLine1 + ', ' + addressInfo.addressLine2 + ', ' + addressInfo.city + ', ' + addressInfo.state + ', ' + addressInfo.country + ', ' + addressInfo.postalCode;
                //}

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (//!this.quotationDetail.assetId || !this.quotationDetail.supportItemId ||
            !this.quotationDetail.description || !this.quotationDetail.quantity || !this.quotationDetail.unitPrice
            || !this.quotationDetail.cost || !this.quotationDetail.charge) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.quotationDetail.quantity <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Quantity must be greater than zero";
        }
        else if (this.quotationDetail.unitPrice <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Unit price must be greater than zero";
        }
        else if (this.quotationDetail.cost <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Cost must be greater than zero";
        }
        //else if (this.quotationDetail.tax <= 0) {
        //    this.isFormValid = false;
        //    this.errorMsg = "Tax must be greater than zero";
        //}
        else if (this.quotationDetail.charge <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Charge must be greater than zero";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            if (this.quotationDetail.isTaxable)
                this.quotationDetail.tax = environment.taxPercent;
            else
                this.quotationDetail.tax = 0;

            this.quotationDetail.markUp = this.quotationDetail.markUp ? this.quotationDetail.markUp : 0;
            this.quotationDetail.discount = this.quotationDetail.discount ? this.quotationDetail.discount : 0;

            this._quotationDetailsServiceProxy.createOrEdit(this.quotationDetail)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this._quotationDetailsServiceProxy.updateQuotationPrices(this.quotationDetail.quotationId)
                        .subscribe(() => {
                            this.notify.info(this.l('SavedSuccessfully'));
                            this.close();
                            this.quotationDetailModalSave.emit(null);
                        });
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectItemTypeModal() {
        this.itemTypeLookupTableModal.id = this.quotationDetail.itemTypeId;
        this.itemTypeLookupTableModal.displayName = this.itemTypeType;
        this.itemTypeLookupTableModal.show();
    }
    openSelectUomModal() {
        this.uomLookupTableModal.id = this.quotationDetail.uomId;
        this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
        this.uomLookupTableModal.show();
    }


    setItemTypeIdNull() {
        this.quotationDetail.itemTypeId = null;
        this.itemTypeType = '';
    }
    setUomIdNull() {
        this.quotationDetail.uomId = null;
        this.uomUnitOfMeasurement = '';
    }


    getNewItemTypeId() {
        this.quotationDetail.itemTypeId = this.itemTypeLookupTableModal.id;
        this.itemTypeType = this.itemTypeLookupTableModal.displayName;
    }
    getNewUomId() {
        this.quotationDetail.uomId = this.uomLookupTableModal.id;
        this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    }


    calculateAmount() {
        let markUp = this.quotationDetail.markUp;
        let unitPrice = this.quotationDetail.unitPrice;
        let quantity = this.quotationDetail.quantity;
        let tax = environment.taxPercent;
        let discount = this.quotationDetail.discount;

        if (quantity > 0 && unitPrice > 0) {
            let costPrice = unitPrice * quantity;

            if (markUp > 0) {
                costPrice += costPrice * (markUp / 100);
            }

            this.quotationDetail.cost = costPrice;

            let discountPrice = 0;
            if (discount > 0) {
                discountPrice = costPrice * (discount / 100);
            }

            let taxPrice = 0;
            if (this.quotationDetail.isTaxable) {
                taxPrice = (costPrice - discountPrice) * (tax / 100);
            }

            this.quotationDetail.charge = (costPrice - discountPrice) + taxPrice;
        }
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }



    //openSelectAssetModal() {
    //    this.quotationDetailAssetLookupTableModal.id = this.quotationDetail.assetId;
    //    this.quotationDetailAssetLookupTableModal.displayName = this.assetReference;
    //    this.quotationDetailAssetLookupTableModal.workOrderId = this.quotationDetail.workOrderId ? this.quotationDetail.workOrderId : 0;
    //    this.quotationDetailAssetLookupTableModal.show();
    //}
    //openSelectAssetClassModal() {
    //    this.quotationDetailAssetClassLookupTableModal.id = this.quotationDetail.assetClassId;
    //    this.quotationDetailAssetClassLookupTableModal.displayName = this.assetClassClass;
    //    this.quotationDetailAssetClassLookupTableModal.assetId = this.quotationDetail.assetId;
    //    this.quotationDetailAssetClassLookupTableModal.show();
    //}
    //openSelectSupportTypeModal() {
    //    this.quotationDetailSupportTypeLookupTableModal.id = this.quotationDetail.supportTypeId;
    //    this.quotationDetailSupportTypeLookupTableModal.displayName = this.supportTypeType;
    //    this.quotationDetailSupportTypeLookupTableModal.assetId = this.quotationDetail.assetId;
    //    this.quotationDetailSupportTypeLookupTableModal.show();
    //}
    //openSelectSupportItemModal() {
    //    this.quotationDetailSupportItemLookupTableModal.id = this.quotationDetail.supportItemId;
    //    this.quotationDetailSupportItemLookupTableModal.displayName = this.supportItemDescription;
    //    this.quotationDetailSupportItemLookupTableModal.assetId = this.quotationDetail.assetId;
    //    this.quotationDetailSupportItemLookupTableModal.show();
    //}
    //openSelectWorkOrderModal() {
    //    this.quotationDetailWorkOrderLookupTableModal.id = this.quotationDetail.workOrderId;
    //    this.quotationDetailWorkOrderLookupTableModal.displayName = this.workOrderSubject;
    //    this.quotationDetailWorkOrderLookupTableModal.show();
    //}


    //setAssetIdNull() {
    //    this.quotationDetail.assetId = null;
    //    this.assetReference = '';

    //    this.setAssetAndSupportItemNull(false);
    //}
    //setAssetClassIdNull() {
    //    this.quotationDetail.assetClassId = null;
    //    this.assetClassClass = '';
    //}
    //setSupportTypeIdNull() {
    //    this.quotationDetail.supportTypeId = null;
    //    this.supportTypeType = '';
    //}
    //setSupportItemIdNull() {
    //    this.quotationDetail.supportItemId = null;
    //    this.supportItemDescription = '';
    //}
    //setWorkOrderIdNull() {
    //    this.quotationDetail.workOrderId = null;
    //    this.workOrderSubject = '';

    //    this.setAssetAndSupportItemNull(true);
    //}
    //setAssetAndSupportItemNull(isWorkOrder: boolean) {
    //    if (isWorkOrder)
    //        this.setAssetIdNull();

    //    this.setSupportItemIdNull();
    //    this.setAssetClassIdNull();
    //    this.setSupportTypeIdNull();
    //}


    //getNewAssetId() {
    //    this.quotationDetail.assetId = this.quotationDetailAssetLookupTableModal.id;
    //    this.assetReference = this.quotationDetailAssetLookupTableModal.displayName;

    //    if (this.quotationDetail.assetId > 0) {
    //        this.appendWorkOrderRelatedFKData(0, this.quotationDetail.assetId);
    //    }
    //    else {
    //        this.setAssetAndSupportItemNull(false);
    //    }
    //}
    //getNewAssetClassId() {
    //    this.quotationDetail.assetClassId = this.quotationDetailAssetClassLookupTableModal.id;
    //    this.assetClassClass = this.quotationDetailAssetClassLookupTableModal.displayName;
    //}
    //getNewSupportTypeId() {
    //    this.quotationDetail.supportTypeId = this.quotationDetailSupportTypeLookupTableModal.id;
    //    this.supportTypeType = this.quotationDetailSupportTypeLookupTableModal.displayName;
    //}
    //getNewSupportItemId() {
    //    this.quotationDetail.supportItemId = this.quotationDetailSupportItemLookupTableModal.id;
    //    this.supportItemDescription = this.quotationDetailSupportItemLookupTableModal.displayName;
    //}
    //getNewWorkOrderId() {
    //    this.quotationDetail.workOrderId = this.quotationDetailWorkOrderLookupTableModal.id;
    //    this.workOrderSubject = this.quotationDetailWorkOrderLookupTableModal.displayName;

    //    if (this.quotationDetail.workOrderId > 0) {
    //        this.appendWorkOrderRelatedFKData(this.quotationDetail.workOrderId, 0);
    //    }
    //    else {
    //        this.setAssetAndSupportItemNull(true);
    //    }
    //}

    //appendWorkOrderRelatedFKData(workOrderId: number, assetId: number) {
    //    this._quotationDetailsServiceProxy.getQuotationAssetAndSupportItemList(workOrderId, assetId)
    //        .subscribe(result => {
    //            let assetList = result.assetList;
    //            let assetClassList = result.assetClassList;
    //            let supportItemList = result.supportItemList;
    //            let supportTypeList = result.supportTypeList;
    //            let addressInfo = result.aO_Address;

    //            if (assetList && assetList.length == 1) {
    //                this.quotationDetail.assetId = assetList[0].id;
    //                this.assetReference = assetList[0].displayName;
    //            }

    //            if (supportItemList && supportItemList.length == 1) {
    //                this.quotationDetail.supportItemId = supportItemList[0].id;
    //                this.supportItemDescription = supportItemList[0].displayName;
    //            }

    //            if (assetClassList && assetClassList.length == 1) {
    //                this.quotationDetail.assetClassId = assetClassList[0].id;
    //                this.assetClassClass = assetClassList[0].displayName;
    //            }

    //            if (supportTypeList && supportTypeList.length == 1) {
    //                this.quotationDetail.supportTypeId = supportTypeList[0].id;
    //                this.supportTypeType = supportTypeList[0].displayName;
    //            }

    //            if (addressInfo != null) {
    //                this.ao_address = addressInfo.addressEntryName + (addressInfo.addressLine1 ? ', ' + addressInfo.addressLine1 : '') + (addressInfo.addressLine2 ? ', ' + addressInfo.addressLine2 : '') + (addressInfo.city ? ', ' + addressInfo.city : '') + (addressInfo.state ? ', ' + addressInfo.state : '') + (addressInfo.country ? ', ' + addressInfo.country : '') + (addressInfo.postalCode ? ', ' + addressInfo.postalCode : ''); 
    //            }
    //        });
    //}
}
