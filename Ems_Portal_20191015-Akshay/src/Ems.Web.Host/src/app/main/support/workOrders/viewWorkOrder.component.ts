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
import { ViewWorkOrderUpdateModalComponent } from './view-workOrderUpdate-modal.component';
import { CreateOrEditWorkOrderUpdateModalComponent } from './create-or-edit-workOrderUpdate-modal.component';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments//view-attachment-modal.component';
import { WorkOrdersServiceProxy, WorkOrderUpdatesServiceProxy, WorkOrderDto, CreateOrEditWorkOrderDto, WorkOrderUpdateDto, UsageMetricsServiceProxy, UsageMetricDto } from '@shared/service-proxies/service-proxies';
import { AttachmentsServiceProxy, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditWorkOrderModalComponent } from './create-or-edit-workOrder-modal.component';
import { ViewUsageMetricModalComponent } from '../../telematics/usageMetrics/view-usageMetric-modal.component';
import { ViewUsageMetricRecordModalComponent } from '../../telematics/usageMetricRecords/view-usageMetricRecord-modal.component';
import { CreateOrEditUsageMetricModalComponent } from '../../telematics/usageMetrics/create-or-edit-usageMetric-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import * as moment from 'moment';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewWorkOrder',
    templateUrl: './viewWorkOrder.component.html',
    animations: [appModuleAnimation()]
})
export class ViewWorkOrderComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditWorkOrderModal', { static: true }) createOrEditWorkOrderModal: CreateOrEditWorkOrderModalComponent;
    @ViewChild('createOrEditWorkOrderUpdateModal', { static: true }) createOrEditWorkOrderUpdateModal: CreateOrEditWorkOrderUpdateModalComponent;
    @ViewChild('viewWorkOrderUpdateModalComponent', { static: true }) viewWorkOrderUpdateModal: ViewWorkOrderUpdateModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('viewUsageMetricModal', { static: true }) viewUsageMetricModal: ViewUsageMetricModalComponent;
    @ViewChild('createOrEditUsageMetricModal', { static: true }) createOrEditUsageMetricModal: CreateOrEditUsageMetricModalComponent;
    @ViewChild('viewUsageMetricRecordModal', { static: true }) viewUsageMetricRecordModal: ViewUsageMetricRecordModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    @ViewChild('dataTable2', { static: true }) dataTable2: Table; // For the 3rd table
    @ViewChild('paginator2', { static: true }) paginator2: Paginator; // For the 3rd table

    primengTableHelper1: PrimengTableHelper;
    primengTableHelper2: PrimengTableHelper;

    advancedFiltersAreShown = false;
    filterText = '';
    maxUpdatedAtFilter: moment.Moment;
    minUpdatedAtFilter: moment.Moment;
    itemTypeTypeFilter = '';
    maxUpdatedByUserIdFilter: number;
    maxUpdatedByUserIdFilterEmpty: number;
    minUpdatedByUserIdFilter: number;
    minUpdatedByUserIdFilterEmpty: number;
    workOrderSubjectFilter = '';

    /*
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
    */

    _entityTypeFullName = 'Ems.Support.WorkOrderUpdate';
    entityHistoryEnabled = false;
    createOrEditWorkOrderDto = new CreateOrEditWorkOrderDto();

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
        private _usageMetricsServiceProxy: UsageMetricsServiceProxy,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
        this.primengTableHelper2 = new PrimengTableHelper(); // For the 3rd table
    }

    active = false;
    saving = false;

    workOrderId: number;
    workOrder: WorkOrderDto;
    assetId: number;
    workOrderStatus: string;
    workOrderType: string;
    assetDisplayName: string;
    customerName: string;
    incidentDescription: string;
    supportItemDescription: string;
    userName: string;
    vendorName: string;
    workOrderPriority: string;

    ngOnInit(): void {

        this.workOrderId = this._activatedRoute.snapshot.queryParams['workOrderId'];
        this.workOrder = new WorkOrderDto();
        this.assetDisplayName = '';
        this.customerName = '';
        this.incidentDescription = '';
        this.supportItemDescription = '';
        this.userName = '';
        this.vendorName = '';
        this.workOrderPriority = '';
        this.workOrderStatus = '';
        this.workOrderType = '';

        this.getWorkOrder();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getAttachments(event?: LazyLoadEvent) {

        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._attachmentsServiceProxy.getSome(
            "WorkOrder",
            this.workOrderId,
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
        this.createOrEditAttachmentModal.show(null, 'WorkOrder', this.workOrder.id);
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

    getWorkOrder(event?: LazyLoadEvent) {
        this._workOrdersServiceProxy.getWorkOrderForView(this.workOrderId)
            .subscribe((workOrderResult) => {

                if (workOrderResult.workOrder == null) {
                    this.close();
                }
                else {
                    this.workOrder = workOrderResult.workOrder;
                    this.assetId = workOrderResult.assetId;
                    this.workOrderType = workOrderResult.workOrderTypeType;
                    this.workOrderStatus = workOrderResult.workOrderStatusStatus;
                    this.assetDisplayName = workOrderResult.assetOwnershipAssetDisplayName;
                    this.customerName = workOrderResult.customerName;
                    this.incidentDescription = workOrderResult.incidentDescription;
                    this.supportItemDescription = workOrderResult.supportItemDescription;
                    this.userName = workOrderResult.userName;
                    this.vendorName = workOrderResult.vendorName;
                    this.workOrderPriority = workOrderResult.workOrderPriorityPriority;
                    this.workOrderId = workOrderResult.workOrder.id;

                    this.getWorkOrderUpdates();
                    this.getAttachments();
                    this.getUsageMetrics();
                }
            });
    }

    getWorkOrderUpdates(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._workOrderUpdatesServiceProxy.getAll(
            this.workOrderId,
            this.filterText,
            this.workOrderSubjectFilter,
            this.itemTypeTypeFilter,
            //this.maxUpdatedAtFilter,
            //this.minUpdatedAtFilter,
            //this.maxUpdatedByUserIdFilter == null ? this.maxUpdatedByUserIdFilterEmpty : this.maxUpdatedByUserIdFilter,
            //this.minUpdatedByUserIdFilter == null ? this.minUpdatedByUserIdFilterEmpty : this.minUpdatedByUserIdFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    getUsageMetrics(event?: LazyLoadEvent) {
        if (this.primengTableHelper2.shouldResetPaging(event)) {
            this.paginator2.changePage(0);
            return;
        }

        this.primengTableHelper2.showLoadingIndicator();

        this._usageMetricsServiceProxy.getSome("WorkOrder", this.workOrderId,
            this.primengTableHelper2.getSorting(this.dataTable2),
            this.primengTableHelper2.getSkipCount(this.paginator2, event),
            this.primengTableHelper2.getMaxResultCount(this.paginator2, event)
        ).subscribe(result => {
            this.primengTableHelper2.totalRecordsCount = result.totalCount;
            this.primengTableHelper2.records = result.items;
            this.primengTableHelper2.hideLoadingIndicator();
        });
    }

    createUsageMetric(): void {
        this.createOrEditUsageMetricModal.show(false, this.assetId, null);
    }

    deleteUsageMetric(usageMetric: UsageMetricDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._usageMetricsServiceProxy.delete(usageMetric.id)
                        .subscribe(() => {
                            this.paginator2.changePage(this.paginator2.getPage())
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
        this.paginator1.changePage(this.paginator1.getPage());
    }

    createWorkOrderUpdate(): void {
        this.createOrEditWorkOrderUpdateModal.show(null, this.workOrderId);
    }

    showHistory(workOrderUpdate: WorkOrderUpdateDto): void {
        this.entityTypeHistoryModal.show({
            entityId: workOrderUpdate.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteWorkOrderUpdate(workOrderUpdate: WorkOrderUpdateDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._workOrderUpdatesServiceProxy.delete(workOrderUpdate.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }



    showWoHistory(workOrder: WorkOrderDto): void {
        this.entityTypeHistoryModal.show({
            entityId: workOrder.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    setWorkOrderStatusComplete(workOrder: WorkOrderDto): void {
        this.createOrEditWorkOrderDto = Object.assign({}, this.createOrEditWorkOrderDto, workOrder);

        this._workOrdersServiceProxy.setWorkOrderStatusComplete(this.createOrEditWorkOrderDto)
            .subscribe(result => {

                if (result && result.errorInfo) {
                    this.message.info(result.errorInfo.details, result.errorInfo.message);
                }
                else {
                    this.getWorkOrder();
                    this.notify.success(this.l('SavedSuccessfully'));
                }
            }
            );
    }

    deleteWorkOrder(workOrder: WorkOrderDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._workOrdersServiceProxy.delete(workOrder.id)
                        .subscribe(() => {
                            this.notify.success(this.l('SuccessfullyDeleted'));
                            this.close();
                        });
                }
            }
        );
    }

    createQuotation(workOrderId): void {
        this._router.navigate(['app/main/support/quotations'], { queryParams: { workOrderId: workOrderId } });
    }

    createEstimate(workOrderId): void {
        this._router.navigate(['app/main/support/estimates'], { queryParams: { workOrderId: workOrderId } });
    }

    createCustomerInvoice(workOrderId): void {
        this._router.navigate(['app/main/billing/customerInvoices'], { queryParams: { workOrderId: workOrderId } });
    }


    close(): void {
        this._router.navigate(['app/main/support/workOrders']);
    }
}
