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
import { IncidentsServiceProxy, IncidentDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditIncidentModalComponent } from './create-or-edit-incident-modal.component';
import { CreateOrEditIncidentUpdateModalComponent } from './create-or-edit-incidentUpdate-modal.component';
import { ViewIncidentUpdateModalComponent } from './view-incidentUpdate-modal.component';
import * as moment from 'moment';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments//view-attachment-modal.component';
import { AttachmentsServiceProxy, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewIncident',
    templateUrl: './viewIncident.component.html',
    animations: [appModuleAnimation()]
})
export class ViewIncidentComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditIncidentModal', { static: true }) createOrEditIncidentModal: CreateOrEditIncidentModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditIncidentUpdateModal', { static: true }) createOrEditIncidentUpdateModal: CreateOrEditIncidentUpdateModalComponent;
    @ViewChild('viewIncidentUpdateModalComponent', { static: true }) viewIncidentUpdateModal: ViewIncidentUpdateModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    primengTableHelper1: PrimengTableHelper;

    _entityTypeFullName = 'Ems.Support.Incident';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _incidentsServiceProxy: IncidentsServiceProxy,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
    }

    active = false;
    saving = false;

    incidentId: number;
    incident: IncidentDto;
    assetReference: string;
    customerName: string;
    incidentPriorityPriority: string;
    incidentStatusStatus: string;
    incidentTypeType: string;
    supportItemDescription: string;
    userName: string;

    ngOnInit(): void {

        this.incidentId = this._activatedRoute.snapshot.queryParams['incidentId'];
        this.incident = new IncidentDto();

        //this.getIncident();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getIncident(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._incidentsServiceProxy.getIncidentForView(
            this.incidentId,
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSorting(this.dataTable)
        ).subscribe((incidentResult) => {

                if (incidentResult.incident == null) {
                    this.primengTableHelper.hideLoadingIndicator();
                    this.close();
                }
                else {
                    this.incident = incidentResult.incident;
                    this.assetReference = incidentResult.assetReference;
                    this.customerName = incidentResult.customerName;
                    this.incidentPriorityPriority = incidentResult.incidentPriorityPriority;
                    this.incidentStatusStatus = incidentResult.incidentStatusStatus;
                    this.incidentTypeType = incidentResult.incidentTypeType;
                    this.supportItemDescription = incidentResult.supportItemDescription;
                    this.userName = incidentResult.userName;

                    this.primengTableHelper.totalRecordsCount = incidentResult.incidentUpdates.totalCount;
                    this.primengTableHelper.records = incidentResult.incidentUpdates.items;
                    this.primengTableHelper.hideLoadingIndicator();

                    this.getAttachments();
                }
            });
    }

    createIncidentUpdate(): void {
        this.createOrEditIncidentUpdateModal.show(null, this.incidentId);
    }

    getAttachments(event?: LazyLoadEvent) {

        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._attachmentsServiceProxy.getSome(
            "Incident",
            this.incidentId,
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
        this.createOrEditAttachmentModal.show(null, 'Incident', this.incident.id);
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
                            this.close();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    createWorkOrder(incidentId): void {
        this._router.navigate(['app/main/support/workOrders'], { queryParams: { incidentId: incidentId } });
    }


    reloadPage(): void {
        this.paginator1.changePage(this.paginator1.getPage())
    }

    close(): void {
        this._router.navigate(['app/main/support/incidents']);
    }
}
