import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { EstimatesServiceProxy, QuotationsServiceProxy, WorkOrdersServiceProxy, CreateOrEditEstimateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { EstimateWorkOrderLookupTableModalComponent } from './estimate-workOrder-lookup-table-modal.component';
import { EstimateCustomerLookupTableModalComponent } from './estimate-customer-lookup-table-modal.component';
import { EstimateQuotationLookupTableModalComponent } from './estimate-quotation-lookup-table-modal.component';
import { EstimateEstimateStatusLookupTableModalComponent } from './estimate-estimateStatus-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditEstimateModal',
    templateUrl: './create-or-edit-estimate-modal.component.html'
})
export class CreateOrEditEstimateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('estimateWorkOrderLookupTableModal', { static: true }) estimateWorkOrderLookupTableModal: EstimateWorkOrderLookupTableModalComponent;
    @ViewChild('estimateCustomerLookupTableModal', { static: true }) estimateCustomerLookupTableModal: EstimateCustomerLookupTableModalComponent;
    @ViewChild('estimateQuotationLookupTableModal', { static: true }) estimateQuotationLookupTableModal: EstimateQuotationLookupTableModalComponent;
    @ViewChild('estimateEstimateStatusLookupTableModal', { static: true }) estimateEstimateStatusLookupTableModal: EstimateEstimateStatusLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    estimate: CreateOrEditEstimateDto = new CreateOrEditEstimateDto();

    startDate: Date;
    endDate: Date;
    acknowledgedAt: Date;
    workOrderSubject = '';
    customerName = '';
    quotationTitle = '';
    estimateStatusStatus = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _estimatesServiceProxy: EstimatesServiceProxy,
        private _quotationsServiceProxy: QuotationsServiceProxy,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy
    ) {
        super(injector);
    }

    show(estimateId?: number, quotationId?: number, workOrderId?: number): void {
        this.startDate = null;
        this.endDate = null;
        this.acknowledgedAt = null;

        if (!estimateId) {
            this.estimate = new CreateOrEditEstimateDto();
            this.estimate.id = estimateId;
            this.startDate = moment().toDate();
            this.quotationTitle = '';
            this.workOrderSubject = '';
            this.customerName = '';
            this.estimateStatusStatus = '';

            if (quotationId > 0) {
                this.getQuotation(quotationId);
            }

            if (workOrderId > 0) {
                this.getWorkOrder(workOrderId);
            }

            this.getDefaultEstimateStatus();

            this.active = true;
            this.modal.show();
        } else {
            this._estimatesServiceProxy.getEstimateForEdit(estimateId).subscribe(result => {
                this.estimate = result.estimate;

                if (this.estimate.startDate) {
                    this.startDate = this.estimate.startDate.toDate();
                }
                if (this.estimate.endDate) {
                    this.endDate = this.estimate.endDate.toDate();
                }
                if (this.estimate.acknowledgedAt) {
                    this.acknowledgedAt = this.estimate.acknowledgedAt.toDate();
                }
                this.quotationTitle = result.quotationTitle;
                this.workOrderSubject = result.workOrderSubject;
                this.customerName = result.customerName;
                this.estimateStatusStatus = result.estimateStatusStatus;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (this.startDate) {
            this.estimate.startDate = moment(this.startDate);
        }
        else {
            this.estimate.startDate = null;
        }

        if (this.endDate) {
            this.estimate.endDate = moment(this.endDate);
        }
        else {
            this.estimate.endDate = null;
        }

        if (this.acknowledgedAt) {
            this.estimate.acknowledgedAt = moment(this.acknowledgedAt);
        }
        else {
            this.estimate.acknowledgedAt = null;
        }


        if (!this.estimate.customerId || !this.estimate.estimateStatusId || !this.estimate.reference
            || !this.estimate.title || !this.estimate.startDate) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.estimate.endDate && this.estimate.endDate < this.estimate.startDate) {
            this.isFormValid = false;
            this.errorMsg = "End Date must be greater or equals to Start Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._estimatesServiceProxy.createOrEdit(this.estimate)
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

    getQuotation(quotationId?: number): void {
        this._quotationsServiceProxy.getQuotationForEdit(quotationId).subscribe(result => {
            let qResult = result.quotation;

            this.estimate.id = null;
            this.estimate.quotationId = qResult.id;
            this.estimate.description = qResult.description;
            this.estimate.endDate = qResult.endDate;
            this.estimate.startDate = qResult.startDate;
            this.estimate.remark = qResult.remark;
            this.estimate.reference = qResult.reference;
            this.quotationTitle = qResult.title;

            this.appendQuotationFKData();
        });
    }

    getWorkOrder(workOrderId: number): void {
        this._workOrdersServiceProxy.getWorkOrderForEdit(workOrderId).subscribe(result => {
            let wResult = result.workOrder;

            this.estimate.id = null;
            this.estimate.description = wResult.description;
            this.estimate.endDate = wResult.endDate;
            this.estimate.title = wResult.subject;
            this.estimate.remark = wResult.remarks;
            this.estimate.startDate = wResult.startDate;
            this.estimate.workOrderId = wResult.id;
            this.workOrderSubject = wResult.subject;

            this.appendWorkOrderFKData();
        });
    }

    getDefaultEstimateStatus(): void {
        this._estimatesServiceProxy.getAllEstimateStatusForLookupTable(environment.defaultStatus, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let estimateStatus = result.items[0];

                this.estimate.estimateStatusId = estimateStatus.id;
                this.estimateStatusStatus = estimateStatus.displayName;
            }
        });
    }

    openSelectWorkOrderModal() {
        this.estimateWorkOrderLookupTableModal.id = this.estimate.workOrderId;
        this.estimateWorkOrderLookupTableModal.displayName = this.workOrderSubject;
        this.estimateWorkOrderLookupTableModal.show();
    }
    openSelectCustomerModal() {
        this.estimateCustomerLookupTableModal.id = this.estimate.customerId;
        this.estimateCustomerLookupTableModal.workOrderId = this.estimate.workOrderId ? this.estimate.workOrderId : 0;
        this.estimateCustomerLookupTableModal.quotationId = this.estimate.quotationId ? this.estimate.quotationId : 0;
        this.estimateCustomerLookupTableModal.displayName = this.customerName;
        this.estimateCustomerLookupTableModal.show();
    }
    openSelectQuotationModal() {
        this.estimateQuotationLookupTableModal.id = this.estimate.quotationId;
        this.estimateQuotationLookupTableModal.workOrderId = this.estimate.workOrderId ? this.estimate.workOrderId : 0;
        this.estimateQuotationLookupTableModal.displayName = this.quotationTitle;
        this.estimateQuotationLookupTableModal.show();
    }
    openSelectEstimateStatusModal() {
        this.estimateEstimateStatusLookupTableModal.id = this.estimate.estimateStatusId;
        this.estimateEstimateStatusLookupTableModal.displayName = this.estimateStatusStatus;
        this.estimateEstimateStatusLookupTableModal.show();
    }

    setWorkOrderIdNull() {
        this.estimate.workOrderId = null;
        this.workOrderSubject = '';
    }
    setCustomerIdNull() {
        this.estimate.customerId = null;
        this.customerName = '';
    }
    setQuotationIdNull() {
        this.estimate.quotationId = null;
        this.quotationTitle = '';
    }
    setEstimateStatusIdNull() {
        this.estimate.estimateStatusId = null;
        this.estimateStatusStatus = '';
    }


    getNewWorkOrderId() {
        this.estimate.workOrderId = this.estimateWorkOrderLookupTableModal.id;
        this.workOrderSubject = this.estimateWorkOrderLookupTableModal.displayName;

        this.appendWorkOrderFKData();
    }
    getNewCustomerId() {
        this.estimate.customerId = this.estimateCustomerLookupTableModal.id;
        this.customerName = this.estimateCustomerLookupTableModal.displayName;
    }
    getNewQuotationId() {
        this.estimate.quotationId = this.estimateQuotationLookupTableModal.id;
        this.quotationTitle = this.estimateQuotationLookupTableModal.displayName;

        this.appendQuotationFKData();
    }
    getNewEstimateStatusId() {
        this.estimate.estimateStatusId = this.estimateEstimateStatusLookupTableModal.id;
        this.estimateStatusStatus = this.estimateEstimateStatusLookupTableModal.displayName;
    }


    appendWorkOrderFKData() {
        this._estimatesServiceProxy.getWorkOrderFkData(
            this.estimate.workOrderId ? this.estimate.workOrderId : 0
        ).subscribe(result => {
            let customerList = result.customerList;
            let quotationList = result.quotationList;

            if (quotationList && quotationList.length == 1) {
                this.estimate.quotationId = quotationList[0].id;
                this.quotationTitle = quotationList[0].displayName;
            }
            else {
                this.setQuotationIdNull();
            }

            if (customerList && customerList.length == 1) {
                this.estimate.customerId = customerList[0].id;
                this.customerName = customerList[0].displayName;
            }
            else {
                this.setCustomerIdNull();
            }

        });
    }

    appendQuotationFKData() {
        this._estimatesServiceProxy.getQuotationFkData(
            this.estimate.quotationId ? this.estimate.quotationId : 0
        ).subscribe(result => {
            let customerList = result.customerList;

            if (customerList && customerList.length == 1) {
                this.estimate.customerId = customerList[0].id;
                this.customerName = customerList[0].displayName;
            }
            else {
                this.setCustomerIdNull();
            }
        });
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
