import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AttachmentsServiceProxy, CreateOrEditAttachmentDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AttachmentAssetLookupTableModalComponent } from './attachment-asset-lookup-table-modal.component';
import { AttachmentIncidentLookupTableModalComponent } from './attachment-incident-lookup-table-modal.component';
import { AttachmentLeaseAgreementLookupTableModalComponent } from './attachment-leaseAgreement-lookup-table-modal.component';
import { AttachmentQuotationLookupTableModalComponent } from './attachment-quotation-lookup-table-modal.component';
import { AttachmentSupportContractLookupTableModalComponent } from './attachment-supportContract-lookup-table-modal.component';
import { AttachmentWorkOrderLookupTableModalComponent } from './attachment-workOrder-lookup-table-modal.component';
import { AttachmentCustomerInvoiceLookupTableModalComponent } from './attachment-customerInvoice-lookup-table-modal.component';

import { FileUploader, FileUploaderOptions, FileItem } from 'ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { TokenService } from '@abp/auth/token.service';
import { IAjaxResponse } from '@abp/abpHttpInterceptor';

@Component({
    selector: 'createOrEditAttachmentModal',
    templateUrl: './create-or-edit-attachment-modal.component.html'
})
export class CreateOrEditAttachmentModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('attachmentAssetLookupTableModal', { static: true }) attachmentAssetLookupTableModal: AttachmentAssetLookupTableModalComponent;
    @ViewChild('attachmentIncidentLookupTableModal', { static: true }) attachmentIncidentLookupTableModal: AttachmentIncidentLookupTableModalComponent;
    @ViewChild('attachmentLeaseAgreementLookupTableModal', { static: true }) attachmentLeaseAgreementLookupTableModal: AttachmentLeaseAgreementLookupTableModalComponent;
    @ViewChild('attachmentQuotationLookupTableModal', { static: true }) attachmentQuotationLookupTableModal: AttachmentQuotationLookupTableModalComponent;
    @ViewChild('attachmentSupportContractLookupTableModal', { static: true }) attachmentSupportContractLookupTableModal: AttachmentSupportContractLookupTableModalComponent;
    @ViewChild('attachmentWorkOrderLookupTableModal', { static: true }) attachmentWorkOrderLookupTableModal: AttachmentWorkOrderLookupTableModalComponent;
    @ViewChild('attachmentCustomerInvoiceLookupTableModal', { static: true }) attachmentCustomerInvoiceLookupTableModal: AttachmentCustomerInvoiceLookupTableModalComponent;

    @Output() attachmentModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    attachment: CreateOrEditAttachmentDto = new CreateOrEditAttachmentDto();

    assetReference = '';
    incidentDescription = '';
    leaseAgreementReference = '';
    quotationTitle = '';
    supportContractTitle = '';
    workOrderSubject = '';
    customerInvoiceDescription = '';
    blobSubDirectoryName = '';
    hasReferenceId = false;

    attachmentUploader: FileUploader;
    remoteServiceBaseUrl = AppConsts.remoteServiceBaseUrl;
    private _uploaderOptions: FileUploaderOptions = {};

    constructor(
        injector: Injector,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
        private _tokenService: TokenService
    ) {
        super(injector);
    }

    initializeModal(): void {
        this.initFileUploader();
    }

    initFileUploader(): void {
        this.attachmentUploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Attachments/UploadAttachment' });
        this._uploaderOptions.autoUpload = false;
        this._uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        this._uploaderOptions.removeAfterUpload = true;
        this.attachmentUploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        this.assignBlobSubDirectory();

        this.attachmentUploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
            form.append('subDirectory', this.blobSubDirectoryName);
        };

        this.attachmentUploader.onSuccessItem = (item, response, status) => {
            const resp = <IAjaxResponse>JSON.parse(response);

            if (resp.success) {
                this.attachment.filename = resp.result.filename;
                this.attachment.blobFolder = resp.result.bolbFolder;
                this.attachment.blobId = resp.result.bolbId;

                this.createOrEditAttachment();
            } else {
                this.message.error(resp.error.message);
            }
        };

        this.attachmentUploader.setOptions(this._uploaderOptions);
    }

    assignBlobSubDirectory(): void {
        if (this.attachment.assetId > 0)
            this.blobSubDirectoryName = "Asset";
        else if (this.attachment.customerInvoiceId > 0)
            this.blobSubDirectoryName = "CustomerInvoice";
        else if (this.attachment.incidentId > 0)
            this.blobSubDirectoryName = "Incident";
        else if (this.attachment.leaseAgreementId > 0)
            this.blobSubDirectoryName = "LeaseAgreement";
        else if (this.attachment.quotationId > 0)
            this.blobSubDirectoryName = "Quotation";
        else if (this.attachment.supportContractId > 0)
            this.blobSubDirectoryName = "SupportContract";
        else if (this.attachment.workOrderId > 0)
            this.blobSubDirectoryName = "WorkOrder";
        else
            this.blobSubDirectoryName = "";
    }

    show(attachmentId?: number, relatedEntity?: string, referenceId?: number): void {
        this.hasReferenceId = (referenceId && referenceId > 0) ? true : false;

        if (!attachmentId) {

            this.attachment = new CreateOrEditAttachmentDto();
            this.attachment.id = attachmentId;
            this.attachment.uploadedAt = moment().startOf('day');

            if (relatedEntity == "Asset") { this.attachment.assetId = referenceId };
            if (relatedEntity == "CustomerInvoice") { this.attachment.customerInvoiceId = referenceId };
            if (relatedEntity == "Incident") { this.attachment.incidentId = referenceId };
            if (relatedEntity == "LeaseAgreement") { this.attachment.leaseAgreementId = referenceId };
            if (relatedEntity == "Quotation") { this.attachment.quotationId = referenceId };
            if (relatedEntity == "SupportContract") { this.attachment.supportContractId = referenceId };
            if (relatedEntity == "WorkOrder") { this.attachment.workOrderId = referenceId };

            this.assetReference = '';
            this.incidentDescription = '';
            this.leaseAgreementReference = '';
            this.quotationTitle = '';
            this.supportContractTitle = '';
            this.workOrderSubject = '';
            this.customerInvoiceDescription = '';

            this.active = true;
            this.modal.show();
        } else {
            this._attachmentsServiceProxy.getAttachmentForEdit(attachmentId).subscribe(result => {
                this.attachment = result.attachment;

                this.assetReference = result.assetReference;
                this.incidentDescription = result.incidentDescription;
                this.leaseAgreementReference = result.leaseAgreementReference;
                this.quotationTitle = result.quotationTitle;
                this.supportContractTitle = result.supportContractTitle;
                this.workOrderSubject = result.workOrderSubject;
                this.customerInvoiceDescription = result.customerInvoiceDescription;

                this.active = true;
                this.modal.show();
            });
        }

        this.initializeModal();
    }

    save(): void {
        this.saving = true;

        if (this.attachment.id > 0)
            this.createOrEditAttachment();
        else
            this.attachmentUploader.uploadAll();
    }

    createOrEditAttachment(): void {
        this._attachmentsServiceProxy.createOrEdit(this.attachment)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('savedsuccessfully'));
                this.close();
                this.attachmentModalSave.emit(null);
            });
    }

    openSelectAssetModal() {
        this.attachmentAssetLookupTableModal.id = this.attachment.assetId;
        this.attachmentAssetLookupTableModal.displayName = this.assetReference;
        this.attachmentAssetLookupTableModal.show();
    }
    openSelectIncidentModal() {
        this.attachmentIncidentLookupTableModal.id = this.attachment.incidentId;
        this.attachmentIncidentLookupTableModal.displayName = this.incidentDescription;
        this.attachmentIncidentLookupTableModal.show();
    }
    openSelectLeaseAgreementModal() {
        this.attachmentLeaseAgreementLookupTableModal.id = this.attachment.leaseAgreementId;
        this.attachmentLeaseAgreementLookupTableModal.displayName = this.leaseAgreementReference;
        this.attachmentLeaseAgreementLookupTableModal.show();
    }
    openSelectQuotationModal() {
        this.attachmentQuotationLookupTableModal.id = this.attachment.quotationId;
        this.attachmentQuotationLookupTableModal.displayName = this.quotationTitle;
        this.attachmentQuotationLookupTableModal.show();
    }
    openSelectSupportContractModal() {
        this.attachmentSupportContractLookupTableModal.id = this.attachment.supportContractId;
        this.attachmentSupportContractLookupTableModal.displayName = this.supportContractTitle;
        this.attachmentSupportContractLookupTableModal.show();
    }
    openSelectWorkOrderModal() {
        this.attachmentWorkOrderLookupTableModal.id = this.attachment.workOrderId;
        this.attachmentWorkOrderLookupTableModal.displayName = this.workOrderSubject;
        this.attachmentWorkOrderLookupTableModal.show();
    }
    openSelectCustomerInvoiceModal() {
        this.attachmentCustomerInvoiceLookupTableModal.id = this.attachment.customerInvoiceId;
        this.attachmentCustomerInvoiceLookupTableModal.displayName = this.customerInvoiceDescription;
        this.attachmentCustomerInvoiceLookupTableModal.show();
    }


    setAssetIdNull() {
        this.attachment.assetId = null;
        this.assetReference = '';
        this.assignBlobSubDirectory();
    }
    setIncidentIdNull() {
        this.attachment.incidentId = null;
        this.incidentDescription = '';
        this.assignBlobSubDirectory();
    }
    setLeaseAgreementIdNull() {
        this.attachment.leaseAgreementId = null;
        this.leaseAgreementReference = '';
        this.assignBlobSubDirectory();
    }
    setQuotationIdNull() {
        this.attachment.quotationId = null;
        this.quotationTitle = '';
        this.assignBlobSubDirectory();
    }
    setSupportContractIdNull() {
        this.attachment.supportContractId = null;
        this.supportContractTitle = '';
        this.assignBlobSubDirectory();
    }
    setWorkOrderIdNull() {
        this.attachment.workOrderId = null;
        this.workOrderSubject = '';
        this.assignBlobSubDirectory();
    }
    setCustomerInvoiceIdNull() {
        this.attachment.customerInvoiceId = null;
        this.customerInvoiceDescription = '';
        this.assignBlobSubDirectory();
    }


    getNewAssetId() {
        this.attachment.assetId = this.attachmentAssetLookupTableModal.id;
        this.assetReference = this.attachmentAssetLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }
    getNewIncidentId() {
        this.attachment.incidentId = this.attachmentIncidentLookupTableModal.id;
        this.incidentDescription = this.attachmentIncidentLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }
    getNewLeaseAgreementId() {
        this.attachment.leaseAgreementId = this.attachmentLeaseAgreementLookupTableModal.id;
        this.leaseAgreementReference = this.attachmentLeaseAgreementLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }
    getNewQuotationId() {
        this.attachment.quotationId = this.attachmentQuotationLookupTableModal.id;
        this.quotationTitle = this.attachmentQuotationLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }
    getNewSupportContractId() {
        this.attachment.supportContractId = this.attachmentSupportContractLookupTableModal.id;
        this.supportContractTitle = this.attachmentSupportContractLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }
    getNewWorkOrderId() {
        this.attachment.workOrderId = this.attachmentWorkOrderLookupTableModal.id;
        this.workOrderSubject = this.attachmentWorkOrderLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }
    getNewCustomerInvoiceId() {
        this.attachment.customerInvoiceId = this.attachmentCustomerInvoiceLookupTableModal.id;
        this.customerInvoiceDescription = this.attachmentCustomerInvoiceLookupTableModal.displayName;
        this.assignBlobSubDirectory();
    }

    clearFileQueue(): void {
        this.attachmentUploader.clearQueue();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
