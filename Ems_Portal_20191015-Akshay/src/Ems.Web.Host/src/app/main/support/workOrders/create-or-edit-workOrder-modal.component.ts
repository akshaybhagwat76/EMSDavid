import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrdersServiceProxy, CreateOrEditWorkOrderDto, IncidentsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { WorkOrderWorkOrderPriorityLookupTableModalComponent } from './workOrder-workOrderPriority-lookup-table-modal.component';
import { WorkOrderWorkOrderTypeLookupTableModalComponent } from './workOrder-workOrderType-lookup-table-modal.component';
import { WorkOrderWorkOrderStatusLookupTableModalComponent } from './workOrder-workOrderStatus-lookup-table-modal.component';
import { WorkOrderVendorLookupTableModalComponent } from './workOrder-vendor-lookup-table-modal.component';
import { WorkOrderIncidentLookupTableModalComponent } from './workOrder-incident-lookup-table-modal.component';
import { WorkOrderSupportItemLookupTableModalComponent } from './workOrder-supportItem-lookup-table-modal.component';
import { WorkOrderAssetOwnershipLookupTableModalComponent } from './workOrder-assetOwnership-lookup-table-modal.component';
import { WorkOrderUserLookupTableModalComponent } from './workOrder-user-lookup-table-modal.component';
import { WorkOrderCustomerLookupTableModalComponent } from './workOrder-customer-lookup-table-modal.component';
import { WorkOrderLocationLookupTableModalComponent } from './workOrder-location-lookup-table-modal.component';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditWorkOrderModal',
    templateUrl: './create-or-edit-workOrder-modal.component.html'
})
export class CreateOrEditWorkOrderModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('workOrderWorkOrderPriorityLookupTableModal', { static: true }) workOrderWorkOrderPriorityLookupTableModal: WorkOrderWorkOrderPriorityLookupTableModalComponent;
    @ViewChild('workOrderWorkOrderTypeLookupTableModal', { static: true }) workOrderWorkOrderTypeLookupTableModal: WorkOrderWorkOrderTypeLookupTableModalComponent;
    @ViewChild('workOrderWorkOrderStatusLookupTableModal', { static: true }) workOrderWorkOrderStatusLookupTableModal: WorkOrderWorkOrderStatusLookupTableModalComponent;
    @ViewChild('workOrderVendorLookupTableModal', { static: true }) workOrderVendorLookupTableModal: WorkOrderVendorLookupTableModalComponent;
    @ViewChild('workOrderIncidentLookupTableModal', { static: true }) workOrderIncidentLookupTableModal: WorkOrderIncidentLookupTableModalComponent;
    @ViewChild('workOrderSupportItemLookupTableModal', { static: true }) workOrderSupportItemLookupTableModal: WorkOrderSupportItemLookupTableModalComponent;
    @ViewChild('workOrderAssetOwnershipLookupTableModal', { static: true }) workOrderAssetOwnershipLookupTableModal: WorkOrderAssetOwnershipLookupTableModalComponent;
    @ViewChild('workOrderUserLookupTableModal', { static: true }) workOrderUserLookupTableModal: WorkOrderUserLookupTableModalComponent;
    @ViewChild('workOrderCustomerLookupTableModal', { static: true }) workOrderCustomerLookupTableModal: WorkOrderCustomerLookupTableModalComponent;
    @ViewChild('workOrderLocationLookupTableModal', { static: true }) workOrderLocationLookupTableModal: WorkOrderLocationLookupTableModalComponent;

    @Output() workOrderModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrder: CreateOrEditWorkOrderDto = new CreateOrEditWorkOrderDto();

    startDate: Date;
    endDate: Date;
    workOrderPriorityPriority = '';
    workOrderStatusStatus = '';
    workOrderTypeType = '';
    workOrderLocation = '';
    vendorName = '';
    incidentDescription = '';
    supportItemDescription = '';
    assetOwnershipAssetDisplayName = '';
    userName = '';
    customerName = '';
    isCustomerHidden = true;
    isSupportItemHidden = true;
    isVendorHidden = true;
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _incidentsServiceProxy: IncidentsServiceProxy,
        private _workOrdersServiceProxy: WorkOrdersServiceProxy
    ) {
        super(injector);
    }

    show(workOrderId?: number, incidentId?: number): void {
        this.endDate = null;

        if (!workOrderId) {
            this.workOrder = new CreateOrEditWorkOrderDto();
            this.workOrder.id = workOrderId;
            this.startDate = moment().utcOffset(0, true).toDate();
            this.workOrderPriorityPriority = '';
            this.workOrderStatusStatus = '';
            this.workOrderTypeType = '';
            this.incidentDescription = '';
            this.userName = '';
            this.workOrderLocation = '';

            this.getDefaultWorkOrderStatus();
            this.getDefaultCreator();

            if (incidentId > 0) {
                this.getIncident(incidentId);
            }
            else {
                this.supportItemDescription = '';
                this.assetOwnershipAssetDisplayName = '';
                this.customerName = '';
                this.vendorName = '';

                this.isCustomerHidden = true;
                this.isSupportItemHidden = true;
                this.isVendorHidden = true;
            }

            this.active = true;
            this.modal.show();
        } else {
            this._workOrdersServiceProxy.getWorkOrderForEdit(workOrderId).subscribe(result => {
                this.workOrder = result.workOrder;

                if (this.workOrder.startDate) {
                    this.startDate = this.workOrder.startDate.utcOffset(0, true).toDate();
                }
                if (this.workOrder.endDate) {
                    this.endDate = this.workOrder.endDate.utcOffset(0, true).toDate();
                }

                this.workOrderPriorityPriority = result.workOrderPriorityPriority;
                this.workOrderTypeType = result.workOrderTypeType;
                this.workOrderStatusStatus = result.workOrderStatusStatus;
                this.vendorName = result.vendorName;
                this.incidentDescription = result.incidentDescription;
                this.supportItemDescription = result.supportItemDescription;
                this.assetOwnershipAssetDisplayName = result.assetOwnershipAssetDisplayName;
                this.userName = result.userName;
                this.customerName = result.customerName;
                this.workOrderLocation = this.workOrder.location;

                this.isCustomerHidden = (result.tenantType == "C") ? true : false;
                this.isVendorHidden = (result.tenantType == "V") ? true : false;
                this.isSupportItemHidden = false;

                this.active = true;
                this.modal.show();
            });
        }
    }

    getDefaultCreator(): void {
        this._workOrdersServiceProxy.getDefaultCreator().subscribe(result => {
            this.workOrder.userId = result.id;
            this.userName = result.displayName;
        });
    }

    getDefaultWorkOrderStatus(): void {
        this._workOrdersServiceProxy.getAllWorkOrderStatusForLookupTable(environment.defaultStatus, '', 0, 1).subscribe(result => {
            if (result.items && result.items.length > 0) {
                let workOrderStatus = result.items[0];

                this.workOrder.workOrderStatusId = workOrderStatus.id;
                this.workOrderStatusStatus = workOrderStatus.displayName;
            }
        });
    }

    save(): void {

        if (this.startDate) {
            if (!this.workOrder.startDate) {
                this.workOrder.startDate = moment(this.startDate).startOf('day').utcOffset(0, true);
            }
            else {
                this.workOrder.startDate = moment(this.startDate).utcOffset(0, true);
            }
        }
        else {
            this.workOrder.startDate = null;
        }
        if (this.endDate) {
            if (!this.workOrder.endDate) {
                this.workOrder.endDate = moment(this.endDate).startOf('day').utcOffset(0, true);
            }
            else {
                this.workOrder.endDate = moment(this.endDate).utcOffset(0, true);
            }
        }
        else {
            this.workOrder.endDate = null;
        }

        if (!this.workOrder.assetOwnershipId || !this.workOrder.vendorId
            || !this.workOrder.workOrderTypeId || !this.workOrder.workOrderPriorityId
            || !this.workOrder.workOrderStatusId || !this.workOrder.userId
            || !this.workOrder.subject || !this.workOrder.location || !this.workOrder.startDate) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else if (this.workOrder.endDate && this.workOrder.endDate < this.workOrder.startDate) {
            this.isFormValid = false;
            this.errorMsg = "End Date must be greater or equals to Start Date";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._workOrdersServiceProxy.createOrEdit(this.workOrder)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe((result: number) => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.workOrderModalSave.emit(result);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    getIncident(incidentId?: number): void {
        this._incidentsServiceProxy.getIncidentForEdit(incidentId).subscribe(result => {
            let iResult = result.incident;

            this.workOrder.incidentId = iResult.id;
            this.incidentDescription = iResult.description;

            this.appendFKRelatedData(this.workOrder.incidentId, 0);

            this.workOrder.workOrderPriorityId = iResult.incidentPriorityId;
            this.workOrderPriorityPriority = result.incidentPriorityPriority;

            this.workOrder.location = iResult.location;

            this.active = true;
            this.modal.show();
        });
    }

    openSelectWorkOrderPriorityModal() {
        this.workOrderWorkOrderPriorityLookupTableModal.id = this.workOrder.workOrderPriorityId;
        this.workOrderWorkOrderPriorityLookupTableModal.displayName = this.workOrderPriorityPriority;
        this.workOrderWorkOrderPriorityLookupTableModal.show();
    }
    openSelectWorkOrderTypeModal() {
        this.workOrderWorkOrderTypeLookupTableModal.id = this.workOrder.workOrderTypeId;
        this.workOrderWorkOrderTypeLookupTableModal.displayName = this.workOrderTypeType;
        this.workOrderWorkOrderTypeLookupTableModal.show();
    }
    openSelectWorkOrderStatusModal() {
        this.workOrderWorkOrderStatusLookupTableModal.id = this.workOrder.workOrderStatusId;
        this.workOrderWorkOrderStatusLookupTableModal.displayName = this.workOrderStatusStatus;
        this.workOrderWorkOrderStatusLookupTableModal.show();
    }
    openSelectVendorModal() {
        this.workOrderVendorLookupTableModal.id = this.workOrder.vendorId;
        this.workOrderVendorLookupTableModal.displayName = this.vendorName;
        this.workOrderVendorLookupTableModal.assetOwnerShipId = this.workOrder.assetOwnershipId;
        this.workOrderVendorLookupTableModal.show();
    }
    openSelectIncidentModal() {
        this.workOrderIncidentLookupTableModal.id = this.workOrder.incidentId;
        this.workOrderIncidentLookupTableModal.displayName = this.incidentDescription;
        this.workOrderIncidentLookupTableModal.show();
    }
    openSelectSupportItemModal() {
        this.workOrderSupportItemLookupTableModal.id = this.workOrder.supportItemId;
        this.workOrderSupportItemLookupTableModal.displayName = this.supportItemDescription;
        this.workOrderSupportItemLookupTableModal.assetOwnerShipId = this.workOrder.assetOwnershipId;
        this.workOrderSupportItemLookupTableModal.show();
    }
    openSelectAssetOwnershipModal() {
        this.workOrderAssetOwnershipLookupTableModal.id = this.workOrder.assetOwnershipId;
        this.workOrderAssetOwnershipLookupTableModal.displayName = this.assetOwnershipAssetDisplayName;
        this.workOrderAssetOwnershipLookupTableModal.incidentId = this.workOrder.incidentId ? this.workOrder.incidentId : 0;
        this.workOrderAssetOwnershipLookupTableModal.show();
    }
    openSelectUserModal() {
        this.workOrderUserLookupTableModal.id = this.workOrder.userId;
        this.workOrderUserLookupTableModal.displayName = this.userName;
        this.workOrderUserLookupTableModal.show();
    }
    openSelectCustomerModal() {
        this.workOrderCustomerLookupTableModal.id = this.workOrder.customerId;
        this.workOrderCustomerLookupTableModal.displayName = this.customerName;
        this.workOrderCustomerLookupTableModal.assetOwnerShipId = this.workOrder.assetOwnershipId;
        this.workOrderCustomerLookupTableModal.show();
    }
    openSelectLocationModal() {
        //this.workOrderLocationLookupTableModal.id = this.workOrder.locationId;
        this.workOrderLocationLookupTableModal.displayName = this.workOrderLocation;
        this.workOrderLocationLookupTableModal.show();
    }

    setWorkOrderPriorityIdNull() {
        this.workOrder.workOrderPriorityId = null;
        this.workOrderPriorityPriority = '';
    }
    setWorkOrderTypeIdNull() {
        this.workOrder.workOrderTypeId = null;
        this.workOrderTypeType = '';

        this.setWorkOrderPriorityIdNull();
    }
    setWorkOrderStatusIdNull() {
        this.workOrder.workOrderStatusId = null;
        this.workOrderStatusStatus = '';
    }
    setVendorIdNull() {
        this.workOrder.vendorId = null;
        this.vendorName = '';
    }
    setIncidentIdNull() {
        if (this.workOrder.incidentId)
            this.setSupportItemAndCustomerIdNull(true);

        this.workOrder.incidentId = null;
        this.incidentDescription = '';

        if (!this.workOrder.id) {
            this.workOrder.location = "";
            this.setWorkOrderPriorityIdNull();
        }
    }
    setSupportItemIdNull() {
        this.workOrder.supportItemId = null;
        this.supportItemDescription = '';
    }
    setAssetOwnershipIdNull() {
        this.workOrder.assetOwnershipId = null;
        this.assetOwnershipAssetDisplayName = '';

        this.setSupportItemAndCustomerIdNull(false);
    }
    setUserIdNull() {
        this.workOrder.userId = null;
        this.userName = '';
    }
    setCustomerIdNull() {
        this.workOrder.customerId = null;
        this.customerName = '';
    }
    setSupportItemAndCustomerIdNull(nullAssetOwnerShip?: boolean) {
        this.isCustomerHidden = true;
        this.isSupportItemHidden = true;
        this.isVendorHidden = true;

        this.setSupportItemIdNull();
        this.setCustomerIdNull();
        this.setVendorIdNull();

        if (nullAssetOwnerShip)
            this.setAssetOwnershipIdNull();
    }
    setLocationIdNull() {
        //this.workOrder.locationId = null;
        this.workOrderLocation = this.workOrder.location = '';
    }

    getNewWorkOrderPriorityId() {
        this.workOrder.workOrderPriorityId = this.workOrderWorkOrderPriorityLookupTableModal.id;
        this.workOrderPriorityPriority = this.workOrderWorkOrderPriorityLookupTableModal.displayName;
    }
    getNewWorkOrderTypeId() {
        this.workOrder.workOrderTypeId = this.workOrderWorkOrderTypeLookupTableModal.id;
        this.workOrderTypeType = this.workOrderWorkOrderTypeLookupTableModal.displayName;

        if (this.workOrder.workOrderTypeId > 0) {
            this.getWoPriorityByType(this.workOrder.workOrderTypeId);
        }
        else {
            this.setWorkOrderPriorityIdNull();
        }
    }
    getNewWorkOrderStatusId() {
        this.workOrder.workOrderStatusId = this.workOrderWorkOrderStatusLookupTableModal.id;
        this.workOrderStatusStatus = this.workOrderWorkOrderStatusLookupTableModal.displayName;
    }
    getNewVendorId() {
        this.workOrder.vendorId = this.workOrderVendorLookupTableModal.id;
        this.vendorName = this.workOrderVendorLookupTableModal.displayName;
    }
    getNewIncidentId() {
        this.workOrder.incidentId = this.workOrderIncidentLookupTableModal.id;
        this.incidentDescription = this.workOrderIncidentLookupTableModal.displayName;

        if (this.workOrder.incidentId > 0) {
            if (!this.workOrder.id) {
                this.getIncident(this.workOrder.incidentId);
            }
            else {
                this.appendFKRelatedData(this.workOrder.incidentId, 0);
            }
        }
        else {
            this.setSupportItemAndCustomerIdNull(true);
        }
    }
    getNewSupportItemId() {
        this.workOrder.supportItemId = this.workOrderSupportItemLookupTableModal.id;
        this.supportItemDescription = this.workOrderSupportItemLookupTableModal.displayName;
    }
    getNewAssetOwnershipId() {
        this.workOrder.assetOwnershipId = this.workOrderAssetOwnershipLookupTableModal.id;
        this.assetOwnershipAssetDisplayName = this.workOrderAssetOwnershipLookupTableModal.displayName;

        if (this.workOrder.assetOwnershipId > 0) {
            this.appendFKRelatedData(0, this.workOrder.assetOwnershipId);
        }
        else {
            this.setSupportItemAndCustomerIdNull(false);
        }
    }
    getNewUserId() {
        this.workOrder.userId = this.workOrderUserLookupTableModal.id;
        this.userName = this.workOrderUserLookupTableModal.displayName;
    }
    getNewCustomerId() {
        this.workOrder.customerId = this.workOrderCustomerLookupTableModal.id;
        this.customerName = this.workOrderCustomerLookupTableModal.displayName;
    }
    getNewLocationId() {
        //this.workOrder.locationId = this.workOrderLocationLookupTableModal.id;
        this.workOrderLocation = this.workOrder.location = this.workOrderLocationLookupTableModal.displayName;
    }


    appendFKRelatedData(incidentId: number, assetOwnerShipId: number) {
        this._workOrdersServiceProxy.getAssetFkList(incidentId, assetOwnerShipId)
            .subscribe(result => {
                let customerList = result.customerList;
                let supportItemList = result.supportItemList;
                let vendorList = result.vendorList;
                let assetOwnerShipList = result.assetOwnerList;

                //this.isVendorHidden = (vendorList && vendorList.length > 0) ? false : true;
                this.isVendorHidden = false;
                this.isSupportItemHidden = (supportItemList && supportItemList.length > 0) ? false : true;
                this.isCustomerHidden = (customerList && customerList.length > 0) ? false : true;

                if (customerList && customerList.length == 1) {
                    this.workOrder.customerId = customerList[0].id;
                    this.customerName = customerList[0].displayName;
                }
                else {
                    this.setCustomerIdNull();
                }

                if (supportItemList && supportItemList.length == 1) {
                    this.workOrder.supportItemId = supportItemList[0].id;
                    this.supportItemDescription = supportItemList[0].displayName;
                }
                else {
                    this.setSupportItemIdNull();
                }

                if (vendorList && vendorList.length == 1) {
                    this.workOrder.vendorId = vendorList[0].id;
                    this.vendorName = vendorList[0].displayName;
                }
                else {
                    this.setVendorIdNull();
                }

                if (incidentId && incidentId > 0) {
                    if (assetOwnerShipList && assetOwnerShipList.length == 1) {
                        this.workOrder.assetOwnershipId = assetOwnerShipList[0].id;
                        this.assetOwnershipAssetDisplayName = assetOwnerShipList[0].displayName;
                    }
                    else {
                        this.setAssetOwnershipIdNull();
                    }
                }
            });
    }

    getWoPriorityByType(workOrderTypeId: number) {
        this._workOrdersServiceProxy.getWorkOrderPriorityByType(workOrderTypeId)
            .subscribe(result => {
                this.workOrder.workOrderPriorityId = result.id;
                this.workOrderPriorityPriority = result.displayName;
            });
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
