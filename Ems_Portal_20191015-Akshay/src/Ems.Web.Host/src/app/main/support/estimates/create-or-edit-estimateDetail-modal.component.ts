import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { EstimateDetailsServiceProxy, CreateOrEditEstimateDetailDto, EstimatesServiceProxy, WorkOrderUpdatesServiceProxy, QuotationDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ItemTypeLookupTableModalComponent } from '@app/config/quotations/itemTypes/itemType-lookup-table-modal.component';
import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';
import { WorkOrderWorkOrderActionLookupTableModalComponent } from '@app/main/support/workOrders/workOrder-workOrderAction-lookup-table-modal.component';
import { environment } from 'environments/environment';

@Component({
    selector: 'createOrEditEstimateDetailModal',
    templateUrl: './create-or-edit-estimateDetail-modal.component.html'
})
export class CreateOrEditEstimateDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    @ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;
    @ViewChild('workOrderWorkOrderActionLookupTableModal', { static: true }) workOrderWorkOrderActionLookupTableModal: WorkOrderWorkOrderActionLookupTableModalComponent;

    @Output() estimateDetailModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    estimateDetail: CreateOrEditEstimateDetailDto = new CreateOrEditEstimateDetailDto();
    quotationSubject = '';
    itemTypeType = '';
    estimateTitle = '';
    uomUnitOfMeasurement = '';
    actionWorkOrderAction = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _estimateDetailsServiceProxy: EstimateDetailsServiceProxy,
        private _estimatesServiceProxy: EstimatesServiceProxy,
        private _quotationDetailsServiceProxy: QuotationDetailsServiceProxy,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy
    ) {
        super(injector);
    }

    show(estimateDetailId?: number, estimateId?: number): void {

        if (!estimateDetailId) {
            this.estimateDetail = new CreateOrEditEstimateDetailDto();
            this.estimateDetail.id = estimateDetailId;
            this.estimateDetail.estimateId = estimateId;
            this.estimateDetail.isTaxable = true;

            this.itemTypeType = '';
            this.estimateTitle = '';
            this.uomUnitOfMeasurement = '';
            this.actionWorkOrderAction = '';

            this.active = true;
            this.modal.show();
        } else {
            this._estimateDetailsServiceProxy.getEstimateDetailForEdit(estimateDetailId).subscribe(result => {

                this.estimateDetail = result.estimateDetail;
                this.estimateDetail.estimateId = estimateId;

                this.itemTypeType = result.itemTypeType;
                this.estimateTitle = result.estimateTitle;
                this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;
                this.actionWorkOrderAction = result.actionWorkOrderAction;

                if (this.estimateDetail.tax > 0)
                    this.estimateDetail.isTaxable = true;
                else
                    this.estimateDetail.isTaxable = false;

                this.active = true;
                this.modal.show();
            });
        }
    }

    cloneQuotationDetail(quotationDetailId: number, estimateId: number): void {

        this._quotationDetailsServiceProxy.getQuotationDetailForEdit(quotationDetailId).subscribe(result => {

            this.estimateDetail = Object.assign({}, this.estimateDetail, result.quotationDetail);

            //this.estimateDetail = result.estimateDetail;
            this.estimateDetail.estimateId = estimateId;
            this.estimateDetail.id = null;

            this.itemTypeType = result.itemTypeType;
            this.estimateTitle = result.quotationTitle;
            this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;
            this.actionWorkOrderAction = '';

            if (this.estimateDetail.tax > 0)
                this.estimateDetail.isTaxable = true;
            else
                this.estimateDetail.isTaxable = false;

            this.active = true;
            this.modal.show();
        });
    }

    cloneWorkOrderUpdate(workOrderUpdateId: number, estimateId: number): void {

        this._workOrderUpdatesServiceProxy.getWorkOrderUpdateForEdit(workOrderUpdateId).subscribe(result => {

            this.estimateDetail = new CreateOrEditEstimateDetailDto();

            this.estimateDetail.estimateId = estimateId;
            this.estimateDetail.id = null;
            this.estimateDetail.quantity = result.workOrderUpdate.number;
            this.estimateDetail.itemTypeId = result.workOrderUpdate.itemTypeId;
            this.estimateDetail.description = result.workOrderUpdate.comments;
            this.estimateDetail.isTaxable = true;
            this.estimateDetail.workOrderActionId = result.workOrderUpdate.workOrderActionId;

            this.itemTypeType = result.itemTypeType;
            this.estimateTitle = result.workOrderSubject;
            this.uomUnitOfMeasurement = "";
            this.actionWorkOrderAction = result.workOrderActionAction;

            this.active = true;
            this.modal.show();
        });
    }

    save(): void {
        if (!this.estimateDetail.itemTypeId && !this.estimateDetail.uomId
            && !this.estimateDetail.workOrderActionId && !this.estimateDetail.description) {
            this.isFormValid = false;
            this.errorMsg = "Fill atleast one of the fields (#)";
        }
        else if (!this.estimateDetail.quantity || !this.estimateDetail.unitPrice) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.estimateDetail.quantity <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Quantity must be greater than zero";
        }
        else if (this.estimateDetail.unitPrice <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Unit price must be greater than zero";
        }
        else if (this.estimateDetail.cost <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Cost must be greater than zero";
        }
        else if (this.estimateDetail.charge <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Charge must be greater than zero";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            if (this.estimateDetail.isTaxable)
                this.estimateDetail.tax = environment.taxPercent;
            else
                this.estimateDetail.tax = 0;

            this.estimateDetail.markUp = this.estimateDetail.markUp ? this.estimateDetail.markUp : 0;
            this.estimateDetail.discount = this.estimateDetail.discount ? this.estimateDetail.discount : 0;

            this._estimateDetailsServiceProxy.createOrEdit(this.estimateDetail)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this._estimatesServiceProxy.updateEstimatePrices(this.estimateDetail.estimateId)
                        .subscribe(() => {
                            this.notify.info(this.l('SavedSuccessfully'));
                            this.close();
                            this.estimateDetailModalSave.emit(null);
                        });
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectItemTypeModal() {
        this.itemTypeLookupTableModal.id = this.estimateDetail.itemTypeId;
        this.itemTypeLookupTableModal.displayName = this.itemTypeType;
        this.itemTypeLookupTableModal.show();
    }
    openSelectUomModal() {
        this.uomLookupTableModal.id = this.estimateDetail.uomId;
        this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
        this.uomLookupTableModal.show();
    }
    openSelectWoActionModal() {
        this.workOrderWorkOrderActionLookupTableModal.id = this.estimateDetail.workOrderActionId;
        this.workOrderWorkOrderActionLookupTableModal.displayName = this.actionWorkOrderAction;
        this.workOrderWorkOrderActionLookupTableModal.show();
    }

    setItemTypeIdNull() {
        this.estimateDetail.itemTypeId = null;
        this.itemTypeType = '';
    }

    setUomIdNull() {
        this.estimateDetail.uomId = null;
        this.uomUnitOfMeasurement = '';
    }

    setWoActionIdNull() {
        this.estimateDetail.workOrderActionId = null;
        this.actionWorkOrderAction = '';
    }

    getNewItemTypeId() {
        this.estimateDetail.itemTypeId = this.itemTypeLookupTableModal.id;
        this.itemTypeType = this.itemTypeLookupTableModal.displayName;
    }
    getNewUomId() {
        this.estimateDetail.uomId = this.uomLookupTableModal.id;
        this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    }
    getNewWoActionId() {
        this.estimateDetail.workOrderActionId = this.workOrderWorkOrderActionLookupTableModal.id;
        this.actionWorkOrderAction = this.workOrderWorkOrderActionLookupTableModal.displayName;
    }


    calculateAmount() {
        let markUp = this.estimateDetail.markUp;
        let unitPrice = this.estimateDetail.unitPrice;
        let quantity = this.estimateDetail.quantity;
        let tax = environment.taxPercent;
        let discount = this.estimateDetail.discount;

        if (quantity > 0 && unitPrice > 0) {
            let costPrice = unitPrice * quantity;

            if (markUp > 0) {
                costPrice += costPrice * (markUp / 100);
            }

            this.estimateDetail.cost = costPrice;

            let discountPrice = 0;
            if (discount > 0) {
                discountPrice = costPrice * (discount / 100);
            }

            let taxPrice = 0;
            if (this.estimateDetail.isTaxable) {
                taxPrice = (costPrice - discountPrice) * (tax / 100);
            }

            this.estimateDetail.charge = (costPrice - discountPrice) + taxPrice;
        }
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }

}
