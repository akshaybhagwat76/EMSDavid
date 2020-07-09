import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { RfqsServiceProxy, CreateOrEditRfqDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { RfqRfqTypeLookupTableModalComponent } from './rfq-rfqType-lookup-table-modal.component';
import { RfqAssetOwnerLookupTableModalComponent } from './rfq-assetOwner-lookup-table-modal.component';
import { RfqCustomerLookupTableModalComponent } from './rfq-customer-lookup-table-modal.component';
import { RfqAssetClassLookupTableModalComponent } from './rfq-assetClass-lookup-table-modal.component';
import { RfqIncidentLookupTableModalComponent } from './rfq-incident-lookup-table-modal.component';
import { RfqVendorLookupTableModalComponent } from './rfq-vendor-lookup-table-modal.component';
import { RfqUserLookupTableModalComponent } from './rfq-user-lookup-table-modal.component';


@Component({
    selector: 'createOrEditRfqModal',
    templateUrl: './create-or-edit-rfq-modal.component.html'
})
export class CreateOrEditRfqModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('rfqRfqTypeLookupTableModal', { static: true }) rfqRfqTypeLookupTableModal: RfqRfqTypeLookupTableModalComponent;
    @ViewChild('rfqAssetOwnerLookupTableModal', { static: true }) rfqAssetOwnerLookupTableModal: RfqAssetOwnerLookupTableModalComponent;
    @ViewChild('rfqCustomerLookupTableModal', { static: true }) rfqCustomerLookupTableModal: RfqCustomerLookupTableModalComponent;
    @ViewChild('rfqAssetClassLookupTableModal', { static: true }) rfqAssetClassLookupTableModal: RfqAssetClassLookupTableModalComponent;
    @ViewChild('rfqIncidentLookupTableModal', { static: true }) rfqIncidentLookupTableModal: RfqIncidentLookupTableModalComponent;
    @ViewChild('rfqVendorLookupTableModal', { static: true }) rfqVendorLookupTableModal: RfqVendorLookupTableModalComponent;
    @ViewChild('rfqUserLookupTableModal', { static: true }) rfqUserLookupTableModal: RfqUserLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    rfq: CreateOrEditRfqDto = new CreateOrEditRfqDto();

    rfqTypeType = '';
    assetOwnerName = '';
    customerName = '';
    assetClassClass = '';
    incidentDescription = '';
    vendorName = '';
    userName = '';


    constructor(
        injector: Injector,
        private _rfqsServiceProxy: RfqsServiceProxy
    ) {
        super(injector);
    }

    show(rfqId?: number): void {

        if (!rfqId) {
            this.rfq = new CreateOrEditRfqDto();
            this.rfq.id = rfqId;
            this.rfq.requestDate = moment().startOf('day');
            this.rfq.requiredBy = moment().startOf('day');
            this.rfqTypeType = '';
            this.assetOwnerName = '';
            this.customerName = '';
            this.assetClassClass = '';
            this.incidentDescription = '';
            this.vendorName = '';
            this.userName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._rfqsServiceProxy.getRfqForEdit(rfqId).subscribe(result => {
                this.rfq = result.rfq;

                this.rfqTypeType = result.rfqTypeType;
                this.assetOwnerName = result.assetOwnerName;
                this.customerName = result.customerName;
                this.assetClassClass = result.assetClassClass;
                this.incidentDescription = result.incidentDescription;
                this.vendorName = result.vendorName;
                this.userName = result.userName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._rfqsServiceProxy.createOrEdit(this.rfq)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

        openSelectRfqTypeModal() {
        this.rfqRfqTypeLookupTableModal.id = this.rfq.rfqTypeId;
        this.rfqRfqTypeLookupTableModal.displayName = this.rfqTypeType;
        this.rfqRfqTypeLookupTableModal.show();
    }
        openSelectAssetOwnerModal() {
        this.rfqAssetOwnerLookupTableModal.id = this.rfq.assetOwnerId;
        this.rfqAssetOwnerLookupTableModal.displayName = this.assetOwnerName;
        this.rfqAssetOwnerLookupTableModal.show();
    }
        openSelectCustomerModal() {
        this.rfqCustomerLookupTableModal.id = this.rfq.customerId;
        this.rfqCustomerLookupTableModal.displayName = this.customerName;
        this.rfqCustomerLookupTableModal.show();
    }
        openSelectAssetClassModal() {
        this.rfqAssetClassLookupTableModal.id = this.rfq.assetClassId;
        this.rfqAssetClassLookupTableModal.displayName = this.assetClassClass;
        this.rfqAssetClassLookupTableModal.show();
    }
        openSelectIncidentModal() {
        this.rfqIncidentLookupTableModal.id = this.rfq.incidentId;
        this.rfqIncidentLookupTableModal.displayName = this.incidentDescription;
        this.rfqIncidentLookupTableModal.show();
    }
        openSelectVendorModal() {
        this.rfqVendorLookupTableModal.id = this.rfq.vendorId;
        this.rfqVendorLookupTableModal.displayName = this.vendorName;
        this.rfqVendorLookupTableModal.show();
    }
        openSelectUserModal() {
        this.rfqUserLookupTableModal.id = this.rfq.userId;
        this.rfqUserLookupTableModal.displayName = this.userName;
        this.rfqUserLookupTableModal.show();
    }


        setRfqTypeIdNull() {
        this.rfq.rfqTypeId = null;
        this.rfqTypeType = '';
    }
        setAssetOwnerIdNull() {
        this.rfq.assetOwnerId = null;
        this.assetOwnerName = '';
    }
        setCustomerIdNull() {
        this.rfq.customerId = null;
        this.customerName = '';
    }
        setAssetClassIdNull() {
        this.rfq.assetClassId = null;
        this.assetClassClass = '';
    }
        setIncidentIdNull() {
        this.rfq.incidentId = null;
        this.incidentDescription = '';
    }
        setVendorIdNull() {
        this.rfq.vendorId = null;
        this.vendorName = '';
    }
        setUserIdNull() {
        this.rfq.userId = null;
        this.userName = '';
    }


        getNewRfqTypeId() {
        this.rfq.rfqTypeId = this.rfqRfqTypeLookupTableModal.id;
        this.rfqTypeType = this.rfqRfqTypeLookupTableModal.displayName;
    }
        getNewAssetOwnerId() {
        this.rfq.assetOwnerId = this.rfqAssetOwnerLookupTableModal.id;
        this.assetOwnerName = this.rfqAssetOwnerLookupTableModal.displayName;
    }
        getNewCustomerId() {
        this.rfq.customerId = this.rfqCustomerLookupTableModal.id;
        this.customerName = this.rfqCustomerLookupTableModal.displayName;
    }
        getNewAssetClassId() {
        this.rfq.assetClassId = this.rfqAssetClassLookupTableModal.id;
        this.assetClassClass = this.rfqAssetClassLookupTableModal.displayName;
    }
        getNewIncidentId() {
        this.rfq.incidentId = this.rfqIncidentLookupTableModal.id;
        this.incidentDescription = this.rfqIncidentLookupTableModal.displayName;
    }
        getNewVendorId() {
        this.rfq.vendorId = this.rfqVendorLookupTableModal.id;
        this.vendorName = this.rfqVendorLookupTableModal.displayName;
    }
        getNewUserId() {
        this.rfq.userId = this.rfqUserLookupTableModal.id;
        this.userName = this.rfqUserLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
