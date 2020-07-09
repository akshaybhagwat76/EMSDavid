import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IncidentsServiceProxy, IncidentDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditIncidentModalComponent } from './create-or-edit-incident-modal.component';
import { ViewIncidentModalComponent } from './view-incident-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './incidents.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class IncidentsComponent extends AppComponentBase {

    @ViewChild('createOrEditIncidentModal', { static: true }) createOrEditIncidentModal: CreateOrEditIncidentModalComponent;
    @ViewChild('viewIncidentModalComponent', { static: true }) viewIncidentModal: ViewIncidentModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    descriptionFilter = '';
    maxIncidentDateFilter: moment.Moment;
    minIncidentDateFilter: moment.Moment;
    locationFilter = '';
    remarksFilter = '';
    maxResolvedAtFilter: moment.Moment;
    minResolvedAtFilter: moment.Moment;
    incidentPriorityPriorityFilter = '';
    incidentStatusStatusFilter = '';
    customerNameFilter = '';
    assetReferenceFilter = '';
    supportItemDescriptionFilter = '';
    incidentTypeTypeFilter = '';
    userNameFilter = '';


    _entityTypeFullName = 'Ems.Support.Incident';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _incidentsServiceProxy: IncidentsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getIncidents(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._incidentsServiceProxy.getAll(
            this.filterText,
            this.descriptionFilter,
            this.maxIncidentDateFilter,
            this.minIncidentDateFilter,
            this.locationFilter,
            this.remarksFilter,
            this.maxResolvedAtFilter,
            this.minResolvedAtFilter,
            this.incidentPriorityPriorityFilter,
            this.incidentStatusStatusFilter,
            this.customerNameFilter,
            this.assetReferenceFilter,
            this.supportItemDescriptionFilter,
            this.incidentTypeTypeFilter,
            this.userNameFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    clearSearch() {
        this.filterText = this.descriptionFilter = this.locationFilter = '';
        this.minIncidentDateFilter = this.maxIncidentDateFilter = null;
        this.minResolvedAtFilter = this.maxResolvedAtFilter = null;
        this.remarksFilter = this.incidentPriorityPriorityFilter = '';
        this.incidentStatusStatusFilter = this.customerNameFilter = '';
        this.assetReferenceFilter = this.supportItemDescriptionFilter = ''
        this.incidentTypeTypeFilter = this.userNameFilter = '';

        this.getIncidents();
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createIncident(): void {
        this.createOrEditIncidentModal.show();
    }

    showHistory(incident: IncidentDto): void {
        this.entityTypeHistoryModal.show({
            entityId: incident.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteIncident(incident: IncidentDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._incidentsServiceProxy.delete(incident.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._incidentsServiceProxy.getIncidentsToExcel(
            this.filterText,
            this.descriptionFilter,
            this.maxIncidentDateFilter,
            this.minIncidentDateFilter,
            this.locationFilter,
            this.remarksFilter,
            this.maxResolvedAtFilter,
            this.minResolvedAtFilter,
            this.incidentPriorityPriorityFilter,
            this.incidentStatusStatusFilter,
            this.customerNameFilter,
            this.assetReferenceFilter,
            this.supportItemDescriptionFilter,
            this.incidentTypeTypeFilter,
            this.userNameFilter,
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }

    viewIncident(incidentId): void {
        this._router.navigate(['app/main/support/incidents/viewIncident'], { queryParams: { incidentId: incidentId } });
    }

    createWorkOrder(incidentId): void {
        this._router.navigate(['app/main/support/workOrders'], { queryParams: { incidentId: incidentId } });
    }
}
