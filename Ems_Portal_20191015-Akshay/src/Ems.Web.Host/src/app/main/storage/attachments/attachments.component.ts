import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AttachmentsServiceProxy, AttachmentDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAttachmentModalComponent } from './create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from './view-attachment-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    templateUrl: './attachments.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AttachmentsComponent extends AppComponentBase {

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    filenameFilter = '';
    descriptionFilter = '';
    maxUploadedAtFilter : moment.Moment;
	minUploadedAtFilter : moment.Moment;
    maxUploadedByFilter : number;
	maxUploadedByFilterEmpty : number;
	minUploadedByFilter : number;
	minUploadedByFilterEmpty : number;
    blobFolderFilter = '';
    blobIdFilter = '';
    assetReferenceFilter = '';
    incidentDescriptionFilter = '';
    leaseAgreementReferenceFilter = '';
    quotationTitleFilter = '';
    supportContractTitleFilter = '';
    workOrderSubjectFilter = '';
    customerInvoiceDescriptionFilter = '';
    
    constructor(
        injector: Injector,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getAttachments(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._attachmentsServiceProxy.getAll(
            this.filterText,
            this.filenameFilter,
            this.descriptionFilter,
            this.maxUploadedAtFilter,
            this.minUploadedAtFilter,
            this.maxUploadedByFilter == null ? this.maxUploadedByFilterEmpty: this.maxUploadedByFilter,
            this.minUploadedByFilter == null ? this.minUploadedByFilterEmpty: this.minUploadedByFilter,
            this.blobFolderFilter,
            this.blobIdFilter,
            this.assetReferenceFilter,
            this.incidentDescriptionFilter,
            this.leaseAgreementReferenceFilter,
            this.quotationTitleFilter,
            this.supportContractTitleFilter,
            this.workOrderSubjectFilter,
            this.customerInvoiceDescriptionFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createAttachment(): void {
        this.createOrEditAttachmentModal.show();
    }

    viewAttachment(attachment: AttachmentDto): void {
        let url = AppConsts.remoteServiceBaseUrl + '/Attachments/GetAttachmentURI?resourcePath=' + attachment.blobFolder + '/' + attachment.blobId;

        let customHeaders = {
            'Abp.TenantId': abp.multiTenancy.getTenantIdCookie(),
            'Authorization': 'Bearer ' + abp.auth.getToken()
        };

        XmlHttpRequestHelper.ajax('GET', url, customHeaders, null, (response) => {
            if (response.result) {
                window.open(response.result, '_blank');
            }
            else {
                this.message.error(this.l('Failed to Download'));
            }
        });
    }

    deleteAttachment(attachment: AttachmentDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._attachmentsServiceProxy.delete(attachment.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._attachmentsServiceProxy.getAttachmentsToExcel(
        this.filterText,
            this.filenameFilter,
            this.descriptionFilter,
            this.maxUploadedAtFilter,
            this.minUploadedAtFilter,
            this.maxUploadedByFilter == null ? this.maxUploadedByFilterEmpty: this.maxUploadedByFilter,
            this.minUploadedByFilter == null ? this.minUploadedByFilterEmpty: this.minUploadedByFilter,
            this.blobFolderFilter,
            this.blobIdFilter,
            this.assetReferenceFilter,
            this.incidentDescriptionFilter,
            this.leaseAgreementReferenceFilter,
            this.quotationTitleFilter,
            this.supportContractTitleFilter,
            this.workOrderSubjectFilter,
            this.customerInvoiceDescriptionFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
