import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { IncidentsServiceProxy, CreateOrEditIncidentDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { IncidentIncidentPriorityLookupTableModalComponent } from './incident-incidentPriority-lookup-table-modal.component';
import { IncidentIncidentStatusLookupTableModalComponent } from './incident-incidentStatus-lookup-table-modal.component';
import { IncidentCustomerLookupTableModalComponent } from './incident-customer-lookup-table-modal.component';
import { IncidentAssetLookupTableModalComponent } from './incident-asset-lookup-table-modal.component';
import { IncidentSupportItemLookupTableModalComponent } from './incident-supportItem-lookup-table-modal.component';
import { IncidentIncidentTypeLookupTableModalComponent } from './incident-incidentType-lookup-table-modal.component';
import { IncidentUserLookupTableModalComponent } from './incident-user-lookup-table-modal.component';
import { IncidentLocationLookupTableModalComponent } from './incident-location-lookup-table-modal.component';


@Component({
    selector: 'createOrEditIncidentModal',
    templateUrl: './create-or-edit-incident-modal.component.html'
})
export class CreateOrEditIncidentModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('incidentIncidentPriorityLookupTableModal', { static: true }) incidentIncidentPriorityLookupTableModal: IncidentIncidentPriorityLookupTableModalComponent;
    @ViewChild('incidentIncidentStatusLookupTableModal', { static: true }) incidentIncidentStatusLookupTableModal: IncidentIncidentStatusLookupTableModalComponent;
    @ViewChild('incidentCustomerLookupTableModal', { static: true }) incidentCustomerLookupTableModal: IncidentCustomerLookupTableModalComponent;
    @ViewChild('incidentAssetLookupTableModal', { static: true }) incidentAssetLookupTableModal: IncidentAssetLookupTableModalComponent;
    @ViewChild('incidentSupportItemLookupTableModal', { static: true }) incidentSupportItemLookupTableModal: IncidentSupportItemLookupTableModalComponent;
    @ViewChild('incidentIncidentTypeLookupTableModal', { static: true }) incidentIncidentTypeLookupTableModal: IncidentIncidentTypeLookupTableModalComponent;
    @ViewChild('incidentUserLookupTableModal', { static: true }) incidentUserLookupTableModal: IncidentUserLookupTableModalComponent;
    @ViewChild('incidentLocationLookupTableModal', { static: true }) incidentLocationLookupTableModal: IncidentLocationLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    incident: CreateOrEditIncidentDto = new CreateOrEditIncidentDto();

    resolvedAt: Date;
    incidentPriorityPriority = '';
    incidentStatusStatus = '';
    incidentLocation = '';
    customerName = '';
    assetReference = '';
    supportItemDescription = '';
    incidentTypeType = '';
    userName = '';
    isFormValid = false;
    errorMsg = "";
    defaultIncidentStatusFilterText = 'Raised';
    isCustomerHidden = true;
    isSupportItemHidden = true;

    constructor(
        injector: Injector,
        private _incidentsServiceProxy: IncidentsServiceProxy
    ) {
        super(injector);
    }

    show(incidentId?: number): void {
        this.resolvedAt = null;

        if (!incidentId) {
            this.incident = new CreateOrEditIncidentDto();
            this.incident.id = incidentId;
            this.incident.incidentDate = moment().startOf('day');
            this.incidentPriorityPriority = '';
            this.incidentStatusStatus = '';
            this.incidentLocation = '';
            this.customerName = '';
            this.assetReference = '';
            this.supportItemDescription = '';
            this.incidentTypeType = '';
            this.userName = '';

            this.isCustomerHidden = true;
            this.isSupportItemHidden = true;

            this.getDefaultIncidentStatus();
            this.getDefaultCreator();

            this.active = true;
            this.modal.show();
        } else {
            this._incidentsServiceProxy.getIncidentForEdit(incidentId).subscribe(result => {
                this.incident = result.incident;

                if (this.incident.resolvedAt) {
                    this.resolvedAt = this.incident.resolvedAt.toDate();
                }
                this.incidentPriorityPriority = result.incidentPriorityPriority;
                this.incidentStatusStatus = result.incidentStatusStatus;
                this.customerName = result.customerName;
                this.assetReference = result.assetReference;
                this.supportItemDescription = result.supportItemDescription;
                this.incidentTypeType = result.incidentTypeType;
                this.userName = result.userName;
                this.incidentLocation = this.incident.location;

                this.isCustomerHidden = (result.tenantType == "C") ? true : false;
                this.isSupportItemHidden = false;

                this.active = true;
                this.modal.show();
            });
        }
    }

    getDefaultCreator(): void {
        this._incidentsServiceProxy.getDefaultCreator().subscribe(result => {
            this.incident.userId = result.id;
            this.userName = result.displayName;
        });
    }

    getDefaultIncidentStatus(): void {
        this._incidentsServiceProxy.getAllIncidentStatusForLookupTable(this.defaultIncidentStatusFilterText, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let incidentStatus = result.items[0];

                this.incident.incidentStatusId = incidentStatus.id;
                this.incidentStatusStatus = incidentStatus.displayName;
            }
        });
    }

    save(): void {

        if (this.resolvedAt) {
            this.incident.resolvedAt = moment(this.resolvedAt);
        }
        else {
            this.incident.resolvedAt = null;
        }

        if (!this.incident.assetId || (!this.isCustomerHidden && !this.incident.customerId) || (!this.isSupportItemHidden && !this.incident.supportItemId) || !this.incident.incidentTypeId || !this.incident.description || !this.incident.incidentDate
            || !this.incident.location) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._incidentsServiceProxy.createOrEdit(this.incident)
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

    openSelectIncidentPriorityModal() {
        this.incidentIncidentPriorityLookupTableModal.id = this.incident.incidentPriorityId;
        this.incidentIncidentPriorityLookupTableModal.displayName = this.incidentPriorityPriority;
        this.incidentIncidentPriorityLookupTableModal.show();
    }
    openSelectIncidentStatusModal() {
        this.incidentIncidentStatusLookupTableModal.id = this.incident.incidentStatusId;
        this.incidentIncidentStatusLookupTableModal.displayName = this.incidentStatusStatus;
        this.incidentIncidentStatusLookupTableModal.show();
    }
    openSelectCustomerModal() {
        this.incidentCustomerLookupTableModal.id = this.incident.customerId;
        this.incidentCustomerLookupTableModal.displayName = this.customerName;
        this.incidentCustomerLookupTableModal.assetId = this.incident.assetId;
        this.incidentCustomerLookupTableModal.show();
    }
    openSelectAssetModal() {
        this.incidentAssetLookupTableModal.id = this.incident.assetId;
        this.incidentAssetLookupTableModal.displayName = this.assetReference;
        this.incidentAssetLookupTableModal.show();
    }
    openSelectSupportItemModal() {
        this.incidentSupportItemLookupTableModal.id = this.incident.supportItemId;
        this.incidentSupportItemLookupTableModal.displayName = this.supportItemDescription;
        this.incidentSupportItemLookupTableModal.assetId = this.incident.assetId;
        this.incidentSupportItemLookupTableModal.show();
    }
    openSelectIncidentTypeModal() {
        this.incidentIncidentTypeLookupTableModal.id = this.incident.incidentTypeId;
        this.incidentIncidentTypeLookupTableModal.displayName = this.incidentTypeType;
        this.incidentIncidentTypeLookupTableModal.show();
    }
    openSelectUserModal() {
        this.incidentUserLookupTableModal.id = this.incident.userId;
        this.incidentUserLookupTableModal.displayName = this.userName;
        this.incidentUserLookupTableModal.show();
    }
    openSelectLocationModal() {
        //this.incidentLocationLookupTableModal.id = this.incident.locationId;
        this.incidentLocationLookupTableModal.displayName = this.incidentLocation;
        this.incidentLocationLookupTableModal.show();
    }


    setIncidentPriorityIdNull() {
        this.incident.incidentPriorityId = null;
        this.incidentPriorityPriority = '';
    }
    setIncidentStatusIdNull() {
        this.incident.incidentStatusId = null;
        this.incidentStatusStatus = '';
    }
    setCustomerIdNull() {
        this.incident.customerId = null;
        this.customerName = '';
    }
    setAssetIdNull() {
        this.incident.assetId = null;
        this.assetReference = '';

        this.setSupportItemAndCustomerIdNull();
    }
    setSupportItemIdNull() {
        this.incident.supportItemId = null;
        this.supportItemDescription = '';
    }
    setIncidentTypeIdNull() {
        this.incident.incidentTypeId = null;
        this.incidentTypeType = '';
    }
    setUserIdNull() {
        this.incident.userId = null;
        this.userName = '';
    }
    setLocationIdNull() {
        //this.incident.locationId = null;
        this.incidentLocation = this.incident.location = '';
    }

    setSupportItemAndCustomerIdNull() {
        this.isCustomerHidden = true;
        this.isSupportItemHidden = true;

        this.setSupportItemIdNull();
        this.setCustomerIdNull();
    }


    getNewIncidentPriorityId() {
        this.incident.incidentPriorityId = this.incidentIncidentPriorityLookupTableModal.id;
        this.incidentPriorityPriority = this.incidentIncidentPriorityLookupTableModal.displayName;
    }
    getNewIncidentStatusId() {
        this.incident.incidentStatusId = this.incidentIncidentStatusLookupTableModal.id;
        this.incidentStatusStatus = this.incidentIncidentStatusLookupTableModal.displayName;
    }
    getNewCustomerId() {
        this.incident.customerId = this.incidentCustomerLookupTableModal.id;
        this.customerName = this.incidentCustomerLookupTableModal.displayName;
    }
    getNewAssetId() {
        this.incident.assetId = this.incidentAssetLookupTableModal.id;
        this.assetReference = this.incidentAssetLookupTableModal.displayName;

        if (this.incident.assetId > 0) {
            this._incidentsServiceProxy.getSupportItemAndCustomerList(this.incident.assetId)
                .subscribe(result => {
                    let customerList = result.customerList;
                    let supportItemList = result.supportItemList;

                    this.isCustomerHidden = (customerList && customerList.length > 0) ? false : true;
                    this.isSupportItemHidden = (supportItemList && supportItemList.length > 0) ? false : true;

                    if (customerList && customerList.length == 1) {
                        this.incident.customerId = customerList[0].id;
                        this.customerName = customerList[0].displayName;
                    }
                    else {
                        this.setCustomerIdNull();
                    }

                    if (supportItemList && supportItemList.length == 1) {
                        this.incident.supportItemId = supportItemList[0].id;
                        this.supportItemDescription = supportItemList[0].displayName;
                    }
                    else {
                        this.setSupportItemIdNull();
                    }
                });
        }
        else {
            this.setSupportItemAndCustomerIdNull();
        }
    }
    getNewSupportItemId() {
        this.incident.supportItemId = this.incidentSupportItemLookupTableModal.id;
        this.supportItemDescription = this.incidentSupportItemLookupTableModal.displayName;
    }
    getNewIncidentTypeId() {
        this.incident.incidentTypeId = this.incidentIncidentTypeLookupTableModal.id;
        this.incidentTypeType = this.incidentIncidentTypeLookupTableModal.displayName;
    }
    getNewUserId() {
        this.incident.userId = this.incidentUserLookupTableModal.id;
        this.userName = this.incidentUserLookupTableModal.displayName;
    }
    getNewLocationId() {
        //this.incident.locationId = this.incidentLocationLookupTableModal.id;
        this.incidentLocation = this.incident.location = this.incidentLocationLookupTableModal.displayName;
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
