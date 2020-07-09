import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomerInvoicesServiceProxy, CreateOrEditCustomerInvoiceDto, WorkOrdersServiceProxy, EstimatesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { CustomerInvoiceCustomerLookupTableModalComponent } from './customerInvoice-customer-lookup-table-modal.component';
import { CustomerInvoiceCurrencyLookupTableModalComponent } from './customerInvoice-currency-lookup-table-modal.component';
import { CustomerInvoiceBillingRuleLookupTableModalComponent } from './customerInvoice-billingRule-lookup-table-modal.component';
import { CustomerInvoiceBillingEventLookupTableModalComponent } from './customerInvoice-billingEvent-lookup-table-modal.component';
import { CustomerInvoiceInvoiceStatusLookupTableModalComponent } from './customerInvoice-invoiceStatus-lookup-table-modal.component';
import { CustomerInvoiceWorkOrderLookupTableModalComponent } from './customerInvoice-workOrder-lookup-table-modal.component';
import { CustomerInvoiceEstimateLookupTableModalComponent } from './customerInvoice-estimate-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditCustomerInvoiceModal',
    templateUrl: './create-or-edit-customerInvoice-modal.component.html'
})
export class CreateOrEditCustomerInvoiceModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('customerInvoiceCustomerLookupTableModal', { static: true }) customerInvoiceCustomerLookupTableModal: CustomerInvoiceCustomerLookupTableModalComponent;
    @ViewChild('customerInvoiceCurrencyLookupTableModal', { static: true }) customerInvoiceCurrencyLookupTableModal: CustomerInvoiceCurrencyLookupTableModalComponent;
    @ViewChild('customerInvoiceBillingRuleLookupTableModal', { static: true }) customerInvoiceBillingRuleLookupTableModal: CustomerInvoiceBillingRuleLookupTableModalComponent;
    @ViewChild('customerInvoiceBillingEventLookupTableModal', { static: true }) customerInvoiceBillingEventLookupTableModal: CustomerInvoiceBillingEventLookupTableModalComponent;
    @ViewChild('customerInvoiceInvoiceStatusLookupTableModal', { static: true }) customerInvoiceInvoiceStatusLookupTableModal: CustomerInvoiceInvoiceStatusLookupTableModalComponent;
    @ViewChild('customerInvoiceWorkOrderLookupTableModal', { static: true }) customerInvoiceWorkOrderLookupTableModal: CustomerInvoiceWorkOrderLookupTableModalComponent;
    @ViewChild('customerInvoiceEstimateLookupTableModal', { static: true }) customerInvoiceEstimateLookupTableModal: CustomerInvoiceEstimateLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    customerInvoice: CreateOrEditCustomerInvoiceDto = new CreateOrEditCustomerInvoiceDto();

    dateIssued: Date;
    dateDue: Date;
    customerName = '';
    //leaseItemItem = '';
    workorderSubject = '';
    estimateTitle = '';
    currencyCode = '';
    billingRuleName = '';
    billingEventPurpose = '';
    invoiceStatusStatus = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _customerInvoicesServiceProxy: CustomerInvoicesServiceProxy,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy,
        private _estimatesServiceProxy: EstimatesServiceProxy
    ) {
        super(injector);
    }

    show(customerInvoiceId?: number, estimateId?: number, workOrderId?: number): void {
        this.dateIssued = null;
        this.dateDue = null;

        if (!customerInvoiceId) {
            this.customerInvoice = new CreateOrEditCustomerInvoiceDto();
            this.customerInvoice.id = customerInvoiceId;
            this.customerName = '';
            //this.leaseItemItem = '';
            this.workorderSubject = '';
            this.estimateTitle = '';
            this.currencyCode = '';
            this.billingRuleName = '';
            this.billingEventPurpose = '';
            this.invoiceStatusStatus = '';

            if (estimateId > 0) {
                this.getEstimate(estimateId);
            }

            if (workOrderId > 0) {
                this.getWorkOrder(workOrderId);
            }

            this.getDefaultInvoiceStatus();

            this.active = true;
            this.modal.show();
        } else {
            this._customerInvoicesServiceProxy.getCustomerInvoiceForEdit(customerInvoiceId).subscribe(result => {
                this.customerInvoice = result.customerInvoice;

                if (this.customerInvoice.dateIssued) {
                    this.dateIssued = this.customerInvoice.dateIssued.toDate();
                }
                if (this.customerInvoice.dateDue) {
                    this.dateDue = this.customerInvoice.dateDue.toDate();
                }
                this.customerName = result.customerName;
                //this.leaseItemItem = result.leaseItemItem;
                this.workorderSubject = result.workOrderSubject;
                this.estimateTitle = result.estimateTitle;
                this.currencyCode = result.currencyCode;
                this.billingRuleName = result.billingRuleName;
                this.billingEventPurpose = result.billingEventPurpose;
                this.invoiceStatusStatus = result.invoiceStatusStatus;

                this.active = true;
                this.modal.show();
            });
        }
    }

    getDefaultInvoiceStatus(): void {
        this._customerInvoicesServiceProxy.getAllInvoiceStatusForLookupTable(environment.defaultStatus, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let status = result.items[0];

                this.customerInvoice.invoiceStatusId = status.id;
                this.invoiceStatusStatus = status.displayName;
            }
        });
    }

    save(): void {

        if (this.dateIssued) {
            this.customerInvoice.dateIssued = moment(this.dateIssued);
        }
        else {
            this.customerInvoice.dateIssued = null;
        }

        if (this.dateDue) {
            this.customerInvoice.dateDue = moment(this.dateDue);
        }
        else {
            this.customerInvoice.dateDue = null;
        }

        if (!this.customerInvoice.customerId || !this.customerInvoice.currencyId || !this.customerInvoice.billingRuleId
            || !this.customerInvoice.billingEventId || !this.customerInvoice.description || !this.customerInvoice.dateIssued
            || !this.customerInvoice.dateDue) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.customerInvoice.dateDue < this.customerInvoice.dateIssued) {
            this.isFormValid = false;
            this.errorMsg = "Due Date must be greater or equals to Issued Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._customerInvoicesServiceProxy.createOrEdit(this.customerInvoice)
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

    getEstimate(estimateId?: number): void {
        this._estimatesServiceProxy.getEstimateForEdit(estimateId).subscribe(result => {
            let eResult = result.estimate;

            this.customerInvoice.id = null;
            this.customerInvoice.estimateId = eResult.id;
            this.customerInvoice.description = eResult.description;
            this.customerInvoice.dateDue = eResult.endDate;
            this.customerInvoice.dateIssued = eResult.startDate;
            this.customerInvoice.remarks = eResult.remark;
            this.customerInvoice.customerReference = eResult.reference;
            this.estimateTitle = eResult.title;

            this.appendEstimateFKData();
        });
    }

    getWorkOrder(workOrderId: number): void {
        this._workOrdersServiceProxy.getWorkOrderForEdit(workOrderId).subscribe(result => {
            let wResult = result.workOrder;

            this.customerInvoice.id = null;
            this.customerInvoice.description = wResult.description;
            this.customerInvoice.dateDue = wResult.endDate;
            this.customerInvoice.remarks = wResult.remarks;
            this.customerInvoice.dateIssued = wResult.startDate;
            this.customerInvoice.workOrderId = wResult.id;
            this.workorderSubject = wResult.subject;

            this.appendWorkOrderFKData();
        });
    }

    openSelectCustomerModal() {
        this.customerInvoiceCustomerLookupTableModal.id = this.customerInvoice.customerId;
        this.customerInvoiceCustomerLookupTableModal.workOrderId = this.customerInvoice.workOrderId ? this.customerInvoice.workOrderId : 0;
        this.customerInvoiceCustomerLookupTableModal.estimateId = this.customerInvoice.estimateId ? this.customerInvoice.estimateId : 0;
        this.customerInvoiceCustomerLookupTableModal.displayName = this.customerName;
        this.customerInvoiceCustomerLookupTableModal.show();
    }
    //openSelectLeaseItemModal() {
    //    this.customerInvoiceLeaseItemLookupTableModal.id = this.customerInvoice.leaseItemId;
    //    this.customerInvoiceLeaseItemLookupTableModal.workOrderId = this.customerInvoice.workOrderId ? this.customerInvoice.workOrderId : 0;
    //    this.customerInvoiceLeaseItemLookupTableModal.estimateId = this.customerInvoice.estimateId ? this.customerInvoice.estimateId : 0;
    //    this.customerInvoiceLeaseItemLookupTableModal.displayName = this.leaseItemItem;
    //    this.customerInvoiceLeaseItemLookupTableModal.show();
    //}
    openSelectWorkOrderModal() {
        this.customerInvoiceWorkOrderLookupTableModal.id = this.customerInvoice.workOrderId;
        this.customerInvoiceWorkOrderLookupTableModal.displayName = this.workorderSubject;
        this.customerInvoiceWorkOrderLookupTableModal.show();
    }
    openSelectEstimateModal() {
        this.customerInvoiceEstimateLookupTableModal.id = this.customerInvoice.estimateId;
        this.customerInvoiceEstimateLookupTableModal.workOrderId = this.customerInvoice.workOrderId ? this.customerInvoice.workOrderId : 0;
        this.customerInvoiceEstimateLookupTableModal.displayName = this.estimateTitle;
        this.customerInvoiceEstimateLookupTableModal.show();
    }
    openSelectCurrencyModal() {
        this.customerInvoiceCurrencyLookupTableModal.id = this.customerInvoice.currencyId;
        this.customerInvoiceCurrencyLookupTableModal.displayName = this.currencyCode;
        this.customerInvoiceCurrencyLookupTableModal.show();
    }
    openSelectBillingRuleModal() {
        this.customerInvoiceBillingRuleLookupTableModal.id = this.customerInvoice.billingRuleId;
        this.customerInvoiceBillingRuleLookupTableModal.displayName = this.billingRuleName;
        this.customerInvoiceBillingRuleLookupTableModal.show();
    }
    openSelectBillingEventModal() {
        this.customerInvoiceBillingEventLookupTableModal.id = this.customerInvoice.billingEventId;
        this.customerInvoiceBillingEventLookupTableModal.displayName = this.billingEventPurpose;
        this.customerInvoiceBillingEventLookupTableModal.show();
    }
    openSelectInvoiceStatusModal() {
        this.customerInvoiceInvoiceStatusLookupTableModal.id = this.customerInvoice.invoiceStatusId;
        this.customerInvoiceInvoiceStatusLookupTableModal.displayName = this.invoiceStatusStatus;
        this.customerInvoiceInvoiceStatusLookupTableModal.show();
    }


    setCustomerIdNull() {
        this.customerInvoice.customerId = null;
        this.customerName = '';
    }
    //setLeaseItemIdNull() {
    //    this.customerInvoice.leaseItemId = null;
    //    this.leaseItemItem = '';
    //}
    setWorkOrderIdNull() {
        this.customerInvoice.workOrderId = null;
        this.workorderSubject = '';

        this.setEstimateIdNull();
        this.setCustomerIdNull();
    }
    setEstimateIdNull() {
        this.customerInvoice.estimateId = null;
        this.estimateTitle = '';

        this.setCustomerIdNull();
    }
    setCurrencyIdNull() {
        this.customerInvoice.currencyId = null;
        this.currencyCode = '';
    }
    setBillingRuleIdNull() {
        this.customerInvoice.billingRuleId = null;
        this.billingRuleName = '';
    }
    setBillingEventIdNull() {
        this.customerInvoice.billingEventId = null;
        this.billingEventPurpose = '';
    }
    setInvoiceStatusIdNull() {
        this.customerInvoice.invoiceStatusId = null;
        this.invoiceStatusStatus = '';
    }


    getNewCustomerId() {
        this.customerInvoice.customerId = this.customerInvoiceCustomerLookupTableModal.id;
        this.customerName = this.customerInvoiceCustomerLookupTableModal.displayName;
    }
    //getNewLeaseItemId() {
    //    this.customerInvoice.leaseItemId = this.customerInvoiceLeaseItemLookupTableModal.id;
    //    this.leaseItemItem = this.customerInvoiceLeaseItemLookupTableModal.displayName;
    //}
    getNewWorkOrderId() {
        this.customerInvoice.workOrderId = this.customerInvoiceWorkOrderLookupTableModal.id;
        this.workorderSubject = this.customerInvoiceWorkOrderLookupTableModal.displayName;

        this.appendWorkOrderFKData();
    }
    getNewEstimateId() {
        this.customerInvoice.estimateId = this.customerInvoiceEstimateLookupTableModal.id;
        this.estimateTitle = this.customerInvoiceEstimateLookupTableModal.displayName;

        this.appendEstimateFKData();
    }
    getNewCurrencyId() {
        this.customerInvoice.currencyId = this.customerInvoiceCurrencyLookupTableModal.id;
        this.currencyCode = this.customerInvoiceCurrencyLookupTableModal.displayName;
    }
    getNewBillingRuleId() {
        this.customerInvoice.billingRuleId = this.customerInvoiceBillingRuleLookupTableModal.id;
        this.billingRuleName = this.customerInvoiceBillingRuleLookupTableModal.displayName;
    }
    getNewBillingEventId() {
        this.customerInvoice.billingEventId = this.customerInvoiceBillingEventLookupTableModal.id;
        this.billingEventPurpose = this.customerInvoiceBillingEventLookupTableModal.displayName;
    }
    getNewInvoiceStatusId() {
        this.customerInvoice.invoiceStatusId = this.customerInvoiceInvoiceStatusLookupTableModal.id;
        this.invoiceStatusStatus = this.customerInvoiceInvoiceStatusLookupTableModal.displayName;
    }


    appendWorkOrderFKData() {
        this._customerInvoicesServiceProxy.getWorkOrderFkData(
            this.customerInvoice.workOrderId ? this.customerInvoice.workOrderId : 0
        ).subscribe(result => {
            let customerList = result.customerList;
            //let leaseItemList = result.leaseItemList;
            let estimateList = result.estimateList;

            if (estimateList && estimateList.length == 1) {
                this.customerInvoice.estimateId = estimateList[0].id;
                this.estimateTitle = estimateList[0].displayName;
            }
            else {
                this.setEstimateIdNull();
            }

            if (customerList && customerList.length == 1) {
                this.customerInvoice.customerId = customerList[0].id;
                this.customerName = customerList[0].displayName;
            }
            else {
                this.setCustomerIdNull();
            }

            //if (leaseItemList && leaseItemList.length == 1) {
            //    this.customerInvoice.leaseItemId = leaseItemList[0].id;
            //    this.leaseItemItem = leaseItemList[0].displayName;
            //}
            //else {
            //    this.setLeaseItemIdNull();
            //}

        });
    }

    appendEstimateFKData() {
        this._customerInvoicesServiceProxy.getEstimateFkData(
            this.customerInvoice.estimateId ? this.customerInvoice.estimateId : 0
        ).subscribe(result => {
            let customerList = result.customerList;
            //let leaseItemList = result.leaseItemList;

            if (customerList && customerList.length == 1) {
                this.customerInvoice.customerId = customerList[0].id;
                this.customerName = customerList[0].displayName;
            }
            else {
                this.setCustomerIdNull();
            }

            //if (leaseItemList && leaseItemList.length == 1) {
            //    this.customerInvoice.leaseItemId = leaseItemList[0].id;
            //    this.leaseItemItem = leaseItemList[0].displayName;
            //}
            //else {
            //    this.setLeaseItemIdNull();
            //}
        });
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
