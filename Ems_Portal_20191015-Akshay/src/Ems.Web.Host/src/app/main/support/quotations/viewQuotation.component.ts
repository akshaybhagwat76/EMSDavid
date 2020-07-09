import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import * as _ from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { ViewQuotationDetailModalComponent } from './view-quotationDetail-modal.component';
import { CreateOrEditQuotationDetailModalComponent } from './create-or-edit-quotationDetail-modal.component';
import { QuotationsServiceProxy, QuotationDetailsServiceProxy, TokenAuthServiceProxy, QuotationDto, QuotationDetailDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditQuotationModalComponent } from './create-or-edit-quotation-modal.component';
import * as moment from 'moment';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments//view-attachment-modal.component';
import { AttachmentsServiceProxy, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewQuotation',
    templateUrl: './viewQuotation.component.html',
    animations: [appModuleAnimation()]
})
export class ViewQuotationComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditQuotationModal', { static: true }) createOrEditQuotationModal: CreateOrEditQuotationModalComponent;
    @ViewChild('createOrEditQuotationDetailModal', { static: true }) createOrEditQuotationDetailModal: CreateOrEditQuotationDetailModalComponent;
    @ViewChild('viewQuotationDetailModalComponent', { static: true }) viewQuotationDetailModal: ViewQuotationDetailModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    primengTableHelper1: PrimengTableHelper;

    advancedFiltersAreShown = false;
    filterText = '';
    descriptionFilter = '';
    loc8GUIDFilter = '';
    assetReferenceFilter = '';
    assetClassClassFilter = '';
    itemTypeTypeFilter = '';
    supportTypeTypeFilter = '';
    quotationTitleFilter = '';
    uomUnitOfMeasurementFilter = '';
    supportItemDescriptionFilter = '';
    workOrderSubjectFilter = '';

    _entityTypeFullName = 'Ems.Support.QuotationDetail';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _quotationsServiceProxy: QuotationsServiceProxy,
        private _quotationDetailsServiceProxy: QuotationDetailsServiceProxy,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
        //private _notifyService: NotifyService,
        //private _tokenAuth: TokenAuthServiceProxy,
        //private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
    }

    active = false;
    saving = false;

    //item: GetQuotationForViewDto;

    quotationId: number;
    quotation: QuotationDto;

    quotationStatus: string;
    supportContractTitle: string;
    assetReference: string;
    assetClassClass: string;
    supportTypeType: string;
    supportItemDescription: string;
    workOrderSubject: string;

    ngOnInit(): void {

        this.quotationId = this._activatedRoute.snapshot.queryParams['quotationId'];
        this.quotation = new QuotationDto();

        this.quotationStatus = '';
        this.supportContractTitle = '';
        this.assetReference = '';
        this.assetClassClass = '';
        this.supportTypeType = '';
        this.supportItemDescription = '';
        this.workOrderSubject = '';

        this.getQuotation();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getQuotation(event?: LazyLoadEvent, skipAttachment?: boolean) {
        this._quotationsServiceProxy.getQuotationForView(this.quotationId)
            .subscribe((quotationResult) => {

                if (quotationResult.quotation == null) {
                    this.close();
                }
                else {
                    this.quotation = quotationResult.quotation;
                    this.quotationStatus = quotationResult.quotationStatusStatus;
                    this.supportContractTitle = quotationResult.supportContractTitle;
                    this.assetReference = quotationResult.assetReference;
                    this.assetClassClass = quotationResult.assetClassClass;
                    this.supportTypeType = quotationResult.supportTypeType;
                    this.supportItemDescription = quotationResult.supportItemDescription;
                    this.workOrderSubject = quotationResult.workOrderSubject;
                    this.quotationId = quotationResult.quotation.id;

                    this.getQuotationDetails();

                    if (!skipAttachment)
                        this.getAttachments();
                }
            });
    }

    getQuotationDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._quotationDetailsServiceProxy.getAll(
            this.quotationId,
            this.filterText,
            this.descriptionFilter,
            this.loc8GUIDFilter,
            this.assetReferenceFilter,
            this.assetClassClassFilter,
            this.itemTypeTypeFilter,
            this.supportTypeTypeFilter,
            this.quotationTitleFilter,
            this.uomUnitOfMeasurementFilter,
            this.supportItemDescriptionFilter,
            this.workOrderSubjectFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    createQuotationDetail(): void {
        this.createOrEditQuotationDetailModal.show(null, this.quotationId);
    }

    deleteQuotationDetail(quotationDetail: QuotationDetailDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._quotationDetailsServiceProxy.delete(quotationDetail.id)
                        .subscribe(() => {
                            this._quotationDetailsServiceProxy.updateQuotationPrices(quotationDetail.quotationId)
                                .subscribe(() => {
                                    this.reloadPage();
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                });
                        });
                }
            }
        );
    }

    getAttachments(event?: LazyLoadEvent) {

        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._attachmentsServiceProxy.getSome(
            "Quotation",
            this.quotationId,
            this.primengTableHelper1.getSorting(this.dataTable1),
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event)
        ).subscribe(result => {
            this.primengTableHelper1.totalRecordsCount = result.totalCount;
            this.primengTableHelper1.records = result.items;
            this.primengTableHelper1.hideLoadingIndicator();
        });
    }

    createAttachment(): void {
        this.createOrEditAttachmentModal.show(null, 'Quotation', this.quotation.id);
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

    reloadPage(): void {
        //this.paginator.changePage(this.paginator.getPage());
        //this.paginator1.changePage(this.paginator1.getPage())

        this.getQuotation();
    }

    showHistory(quotationDetail: QuotationDetailDto): void {
        this.entityTypeHistoryModal.show({
            entityId: quotationDetail.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }


    showQuotationHistory(quotation: QuotationDto): void {
        this.entityTypeHistoryModal.show({
            entityId: quotation.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteQuotation(quotation: QuotationDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._quotationsServiceProxy.delete(quotation.id)
                        .subscribe(() => {
                            this.close();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    createEstimate(quotationId): void {
        this._router.navigate(['app/main/support/estimates'], { queryParams: { quotationId: quotationId } });
    }

    generatePDF(quotationId): void {
        this._router.navigate(['app/main/support/quotations/quotationPDF'], { queryParams: { quotationId: quotationId } });
    }


    close(): void {
        this._router.navigate(['app/main/support/quotations']);
    }
}
