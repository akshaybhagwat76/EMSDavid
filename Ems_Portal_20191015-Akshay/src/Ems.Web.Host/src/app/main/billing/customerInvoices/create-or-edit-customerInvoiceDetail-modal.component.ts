import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomerInvoiceDetailsServiceProxy, CustomerInvoicesServiceProxy, WorkOrderUpdatesServiceProxy, EstimateDetailsServiceProxy, CreateOrEditCustomerInvoiceDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ItemTypeLookupTableModalComponent } from '@app/config/quotations/itemTypes/itemType-lookup-table-modal.component';
import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';
import { WorkOrderWorkOrderActionLookupTableModalComponent } from '@app/main/support/workOrders/workOrder-workOrderAction-lookup-table-modal.component';
import { CustomerInvoiceDetailLeaseItemLookupTableModalComponent } from './customerInvoiceDetail-leaseItem-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditCustomerInvoiceDetailModal',
    templateUrl: './create-or-edit-customerInvoiceDetail-modal.component.html'
})
export class CreateOrEditCustomerInvoiceDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    @ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;
    @ViewChild('workOrderWorkOrderActionLookupTableModal', { static: true }) workOrderWorkOrderActionLookupTableModal: WorkOrderWorkOrderActionLookupTableModalComponent;
    @ViewChild('customerInvoiceDetailLeaseItemLookupTableModal', { static: true }) customerInvoiceDetailLeaseItemLookupTableModal: CustomerInvoiceDetailLeaseItemLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    customerInvoiceDetail: CreateOrEditCustomerInvoiceDetailDto = new CreateOrEditCustomerInvoiceDetailDto();

    itemTypeType = '';
    uomUnitOfMeasurement = '';
    actionWorkOrderAction = '';
    leaseItemItem = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _customerInvoiceDetailsServiceProxy: CustomerInvoiceDetailsServiceProxy,
        private _customerInvoicesServiceProxy: CustomerInvoicesServiceProxy,
        private _estimateDetailsServiceProxy: EstimateDetailsServiceProxy,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy
    ) {
        super(injector);
    }

    show(customerInvoiceDetailId?: number, customerInvoiceId?: number): void {

        if (!customerInvoiceDetailId) {
            this.customerInvoiceDetail = new CreateOrEditCustomerInvoiceDetailDto();
            this.customerInvoiceDetail.id = customerInvoiceDetailId;
            this.customerInvoiceDetail.customerInvoiceId = customerInvoiceId;
            this.customerInvoiceDetail.isTaxable = true;

            this.leaseItemItem = '';
            this.itemTypeType = '';
            this.uomUnitOfMeasurement = '';
            this.actionWorkOrderAction = '';

            this.active = true;
            this.modal.show();
        } else {
            this._customerInvoiceDetailsServiceProxy.getCustomerInvoiceDetailForEdit(customerInvoiceDetailId).subscribe(result => {
                this.customerInvoiceDetail = result.customerInvoiceDetail;
                //this.customerInvoiceDetail.customerInvoiceId = customerInvoiceId;

                this.itemTypeType = result.itemTypeType;
                this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;
                this.actionWorkOrderAction = result.actionWorkOrderAction;
                this.leaseItemItem = result.leaseItemItem;

                if (this.customerInvoiceDetail.tax > 0)
                    this.customerInvoiceDetail.isTaxable = true;
                else
                    this.customerInvoiceDetail.isTaxable = false;

                this.active = true;
                this.modal.show();
            });
        }
    }

    cloneEstimateDetail(estimateDetailId: number, customerInvoiceId: number): void {

        this._estimateDetailsServiceProxy.getEstimateDetailForEdit(estimateDetailId).subscribe(result => {
            var res = result.estimateDetail;

            this.customerInvoiceDetail = new CreateOrEditCustomerInvoiceDetailDto();
            this.customerInvoiceDetail.customerInvoiceId = customerInvoiceId;
            this.customerInvoiceDetail.id = null;

            this.customerInvoiceDetail.charge = res.charge;
            this.customerInvoiceDetail.description = res.description;
            this.customerInvoiceDetail.discount = res.discount;
            this.customerInvoiceDetail.gross = res.cost;
            this.customerInvoiceDetail.net = res.markUp;
            this.customerInvoiceDetail.quantity = res.quantity;
            this.customerInvoiceDetail.tax = res.tax;
            this.customerInvoiceDetail.unitPrice = res.unitPrice;
            this.customerInvoiceDetail.itemTypeId = res.itemTypeId;
            this.customerInvoiceDetail.uomId = res.uomId;
            this.customerInvoiceDetail.workOrderActionId = res.workOrderActionId;

            this.itemTypeType = result.itemTypeType;
            this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;
            this.actionWorkOrderAction = result.actionWorkOrderAction;
            this.leaseItemItem = '';

            if (this.customerInvoiceDetail.tax > 0)
                this.customerInvoiceDetail.isTaxable = true;
            else
                this.customerInvoiceDetail.isTaxable = false;

            this.active = true;
            this.modal.show();
        });
    }

    cloneWorkOrderUpdate(workOrderUpdateId: number, customerInvoiceId: number): void {

        this._workOrderUpdatesServiceProxy.getWorkOrderUpdateForEdit(workOrderUpdateId).subscribe(result => {

            this.customerInvoiceDetail = new CreateOrEditCustomerInvoiceDetailDto();

            this.customerInvoiceDetail.customerInvoiceId = customerInvoiceId;
            this.customerInvoiceDetail.id = null;
            this.customerInvoiceDetail.quantity = result.workOrderUpdate.number;
            this.customerInvoiceDetail.description = result.workOrderUpdate.comments;
            this.customerInvoiceDetail.isTaxable = true;
            this.customerInvoiceDetail.itemTypeId = result.workOrderUpdate.itemTypeId;
            this.customerInvoiceDetail.uomId = null;
            this.customerInvoiceDetail.workOrderActionId = result.workOrderUpdate.workOrderActionId;

            this.itemTypeType = result.itemTypeType;
            this.uomUnitOfMeasurement = "";
            this.actionWorkOrderAction = result.workOrderActionAction;
            this.leaseItemItem = '';

            this.active = true;
            this.modal.show();
        });
    }


    save(): void {
        if (!this.customerInvoiceDetail.itemTypeId && !this.customerInvoiceDetail.uomId
            && !this.customerInvoiceDetail.workOrderActionId && !this.customerInvoiceDetail.description) {
            this.isFormValid = false;
            this.errorMsg = "Fill atleast one of the fields (#)";
        }
        else if (!this.customerInvoiceDetail.customerInvoiceId || !this.customerInvoiceDetail.leaseItemId 
            || !this.customerInvoiceDetail.quantity || !this.customerInvoiceDetail.unitPrice) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.customerInvoiceDetail.quantity <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Quantity must be greater than zero";
        }
        else if (this.customerInvoiceDetail.unitPrice <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Unit price must be greater than zero";
        }
        else if (this.customerInvoiceDetail.gross <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Gross must be greater than zero";
        }
        else if (this.customerInvoiceDetail.charge <= 0) {
            this.isFormValid = false;
            this.errorMsg = "Charge must be greater than zero";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            if (this.customerInvoiceDetail.isTaxable)
                this.customerInvoiceDetail.tax = environment.taxPercent;
            else
                this.customerInvoiceDetail.tax = 0;

            this.customerInvoiceDetail.net = this.customerInvoiceDetail.net ? this.customerInvoiceDetail.net : 0;
            this.customerInvoiceDetail.discount = this.customerInvoiceDetail.discount ? this.customerInvoiceDetail.discount : 0;

            this._customerInvoiceDetailsServiceProxy.createOrEdit(this.customerInvoiceDetail)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this._customerInvoicesServiceProxy.updateCustomerInvoicePrices(this.customerInvoiceDetail.customerInvoiceId)
                        .subscribe(() => {
                            this.notify.info(this.l('SavedSuccessfully'));
                            this.close();
                            this.modalSave.emit(null);
                        });
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectItemTypeModal() {
        this.itemTypeLookupTableModal.id = this.customerInvoiceDetail.itemTypeId;
        this.itemTypeLookupTableModal.displayName = this.itemTypeType;
        this.itemTypeLookupTableModal.show();
    }
    openSelectUomModal() {
        this.uomLookupTableModal.id = this.customerInvoiceDetail.uomId;
        this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
        this.uomLookupTableModal.show();
    }
    openSelectWoActionModal() {
        this.workOrderWorkOrderActionLookupTableModal.id = this.customerInvoiceDetail.workOrderActionId;
        this.workOrderWorkOrderActionLookupTableModal.displayName = this.actionWorkOrderAction;
        this.workOrderWorkOrderActionLookupTableModal.show();
    }
    openSelectLeaseItemModal() {
        this.customerInvoiceDetailLeaseItemLookupTableModal.id = this.customerInvoiceDetail.leaseItemId;
        this.customerInvoiceDetailLeaseItemLookupTableModal.displayName = this.leaseItemItem;
        this.customerInvoiceDetailLeaseItemLookupTableModal.show();
    }

    setItemTypeIdNull() {
        this.customerInvoiceDetail.itemTypeId = null;
        this.itemTypeType = '';
    }
    setUomIdNull() {
        this.customerInvoiceDetail.uomId = null;
        this.uomUnitOfMeasurement = '';
    }
    setWoActionIdNull() {
        this.customerInvoiceDetail.workOrderActionId = null;
        this.actionWorkOrderAction = '';
    }
    setLeaseItemIdNull() {
        this.customerInvoiceDetail.leaseItemId = null;
        this.leaseItemItem = '';
    }


    getNewItemTypeId() {
        this.customerInvoiceDetail.itemTypeId = this.itemTypeLookupTableModal.id;
        this.itemTypeType = this.itemTypeLookupTableModal.displayName;
    }
    getNewUomId() {
        this.customerInvoiceDetail.uomId = this.uomLookupTableModal.id;
        this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    }
    getNewWoActionId() {
        this.customerInvoiceDetail.workOrderActionId = this.workOrderWorkOrderActionLookupTableModal.id;
        this.actionWorkOrderAction = this.workOrderWorkOrderActionLookupTableModal.displayName;
    }
    getNewLeaseItemId() {
        this.customerInvoiceDetail.leaseItemId = this.customerInvoiceDetailLeaseItemLookupTableModal.id;
        this.leaseItemItem = this.customerInvoiceDetailLeaseItemLookupTableModal.displayName;
    }

    calculateAmount() {
        let markUp = this.customerInvoiceDetail.net;
        let unitPrice = this.customerInvoiceDetail.unitPrice;
        let quantity = this.customerInvoiceDetail.quantity;
        let tax = environment.taxPercent;
        let discount = this.customerInvoiceDetail.discount;

        if (quantity > 0 && unitPrice > 0) {
            let costPrice = unitPrice * quantity;

            if (markUp > 0) {
                costPrice += costPrice * (markUp / 100);
            }

            this.customerInvoiceDetail.gross = costPrice;

            let discountPrice = 0;
            if (discount > 0) {
                discountPrice = costPrice * (discount / 100);
            }

            let taxPrice = 0;
            if (this.customerInvoiceDetail.isTaxable) {
                taxPrice = (costPrice - discountPrice) * (tax / 100);
            }

            this.customerInvoiceDetail.charge = (costPrice - discountPrice) + taxPrice;
        }
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
