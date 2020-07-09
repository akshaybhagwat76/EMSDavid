import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkOrdersServiceProxy, WorkOrderDto, CreateOrEditWorkOrderDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditWorkOrderModalComponent } from './create-or-edit-workOrder-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './workOrders.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class WorkOrdersComponent extends AppComponentBase {

    @ViewChild('createOrEditWorkOrderModal', { static: true }) createOrEditWorkOrderModal: CreateOrEditWorkOrderModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    incidentId: number;
    createOrEditWorkOrderDto= new CreateOrEditWorkOrderDto();

    advancedFiltersAreShown = false;
    filterText = '';
    loc8GUIDFilter = '';
    subjectFilter = '';
    descriptionFilter = '';
    locationFilter = '';
    maxStartDateFilter : moment.Moment;
    minStartDateFilter : moment.Moment;
    maxEndDateFilter : moment.Moment;
    minEndDateFilter : moment.Moment;
    remarksFilter = '';
    workOrderPriorityPriorityFilter = '';
    workOrderTypeTypeFilter = '';
    workOrderStatusStatusFilter = '';
    vendorNameFilter = '';
    incidentDescriptionFilter = '';
    supportItemDescriptionFilter = '';
    assetDisplayNameFilter = '';
    userNameFilter = '';
    customerNameFilter = '';
    isCompletedFilter = false;
    isPreventativeFilter = false;

    _entityTypeFullName = 'Ems.Support.WorkOrder';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
        this.incidentId = this._activatedRoute.snapshot.queryParams['incidentId'];
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getWorkOrders(event?: LazyLoadEvent, createWO?: boolean) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._workOrdersServiceProxy.getAll(
            this.filterText,
            this.loc8GUIDFilter,
            this.subjectFilter,
            this.descriptionFilter,
            this.locationFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.remarksFilter,
            this.workOrderPriorityPriorityFilter,
            this.workOrderTypeTypeFilter,
            this.vendorNameFilter,
            this.incidentDescriptionFilter,
            this.supportItemDescriptionFilter,
            this.userNameFilter,
            this.customerNameFilter,
            this.assetDisplayNameFilter,
            this.workOrderStatusStatusFilter,
            this.isCompletedFilter,
            this.isPreventativeFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();

            if (createWO && this.incidentId > 0) {
                this.createOrEditWorkOrderModal.show(null, this.incidentId);
            }
        });
    }

    clearSearch() {
        this.filterText = this.subjectFilter = this.locationFilter = '';
        this.minStartDateFilter = this.maxStartDateFilter = null;
        this.minEndDateFilter = this.maxEndDateFilter = null;
        this.remarksFilter = this.workOrderPriorityPriorityFilter = '';
        this.workOrderStatusStatusFilter = this.workOrderTypeTypeFilter = '';
        this.vendorNameFilter = this.incidentDescriptionFilter = '';
        this.supportItemDescriptionFilter = this.assetDisplayNameFilter = ''
        this.userNameFilter = this.customerNameFilter = '';
        this.isCompletedFilter = this.isPreventativeFilter = false;

        this.getWorkOrders();
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createWorkOrder(): void {
        this.createOrEditWorkOrderModal.show();
    }

    showHistory(workOrder: WorkOrderDto): void {
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

                    setTimeout(() => {   
                        this.viewWorkOrder(workOrder.id);
                    }, 1500);
                }
                else {
                    this.reloadPage();
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
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._workOrdersServiceProxy.getWorkOrdersToExcel(
        this.filterText,
            this.loc8GUIDFilter,
            this.subjectFilter,
            this.descriptionFilter,
            this.locationFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            this.remarksFilter,
            this.workOrderPriorityPriorityFilter,
            this.workOrderTypeTypeFilter,
            this.workOrderStatusStatusFilter,
            this.vendorNameFilter,
            this.incidentDescriptionFilter,
            this.supportItemDescriptionFilter,
            this.assetDisplayNameFilter,
            this.userNameFilter,
            this.customerNameFilter,
            this.isCompletedFilter,
            this.isPreventativeFilter
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }

    navigateWorkOrderView(workOrderId: number): void {
        if (workOrderId > 0) {
            this.viewWorkOrder(workOrderId);
        }
        else {
            this.getWorkOrders();
        }
    }

    viewWorkOrder(workOrderId): void {
        this._router.navigate(['app/main/support/workOrders/viewWorkOrder'], { queryParams: { workOrderId: workOrderId } });
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
}
