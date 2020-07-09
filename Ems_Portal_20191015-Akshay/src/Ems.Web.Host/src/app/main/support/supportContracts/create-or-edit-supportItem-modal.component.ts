import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { SupportItemsServiceProxy, CreateOrEditSupportItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { SupportItemAssetLookupTableModalComponent } from './supportItem-asset-lookup-table-modal.component';
import { SupportItemAssetClassLookupTableModalComponent } from './supportItem-assetClass-lookup-table-modal.component';
import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';
//import { SupportItemSupportContractLookupTableModalComponent } from './supportItem-supportContract-lookup-table-modal.component';
import { SupportItemConsumableTypeLookupTableModalComponent } from './supportItem-consumableType-lookup-table-modal.component';
import { SupportItemSupportTypeLookupTableModalComponent } from './supportItem-supportType-lookup-table-modal.component';


@Component({
    selector: 'createOrEditSupportItemModal',
    templateUrl: './create-or-edit-supportItem-modal.component.html'
})
export class CreateOrEditSupportItemModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('supportItemAssetLookupTableModal', { static: true }) supportItemAssetLookupTableModal: SupportItemAssetLookupTableModalComponent;
    @ViewChild('supportItemAssetClassLookupTableModal', { static: true }) supportItemAssetClassLookupTableModal: SupportItemAssetClassLookupTableModalComponent;
    @ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;
    //@ViewChild('supportItemSupportContractLookupTableModal', { static: true }) supportItemSupportContractLookupTableModal: SupportItemSupportContractLookupTableModalComponent;
    @ViewChild('supportItemConsumableTypeLookupTableModal', { static: true }) supportItemConsumableTypeLookupTableModal: SupportItemConsumableTypeLookupTableModalComponent;
    @ViewChild('supportItemSupportTypeLookupTableModal', { static: true }) supportItemSupportTypeLookupTableModal: SupportItemSupportTypeLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    supportItem: CreateOrEditSupportItemDto = new CreateOrEditSupportItemDto();

    assetReference = '';
    assetClassClass = '';
    uomUnitOfMeasurement = '';
    supportContractTitle = '';
    consumableTypeType = '';
    supportTypeType = '';

    constructor(
        injector: Injector,
        private _supportItemsServiceProxy: SupportItemsServiceProxy
    ) {
        super(injector);
    }

    show(supportItemId?: number, supportContractId?: number): void {

        if (!supportItemId) {
            this.supportItem = new CreateOrEditSupportItemDto();
            this.supportItem.id = supportItemId;
            this.supportItem.supportContractId = supportContractId;
            this.assetReference = '';
            this.assetClassClass = '';
            this.uomUnitOfMeasurement = '';
            this.supportContractTitle = '';
            this.consumableTypeType = '';
            this.supportTypeType = '';

            this.active = true;
            this.modal.show();
        } else {
            this._supportItemsServiceProxy.getSupportItemForEdit(supportItemId).subscribe(result => {
                this.supportItem = result.supportItem;
                this.supportItem.supportContractId = supportContractId;

                this.assetReference = result.assetReference;
                this.assetClassClass = result.assetClassClass;
                this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;
                this.supportContractTitle = result.supportContractTitle;
                this.consumableTypeType = result.consumableTypeType;
                this.supportTypeType = result.supportTypeType;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;


        this._supportItemsServiceProxy.createOrEdit(this.supportItem)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    openSelectAssetModal() {
        this.supportItemAssetLookupTableModal.id = this.supportItem.assetId;
        this.supportItemAssetLookupTableModal.displayName = this.assetReference;
        this.supportItemAssetLookupTableModal.show();
    }
    openSelectAssetClassModal() {
        this.supportItemAssetClassLookupTableModal.id = this.supportItem.assetClassId;
        this.supportItemAssetClassLookupTableModal.displayName = this.assetClassClass;
        this.supportItemAssetClassLookupTableModal.show();
    }
    openSelectUomModal() {
        this.uomLookupTableModal.id = this.supportItem.uomId;
        this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
        this.uomLookupTableModal.show();
    }
    //openSelectSupportContractModal() {
    //    this.supportItemSupportContractLookupTableModal.id = this.supportItem.supportContractId;
    //    this.supportItemSupportContractLookupTableModal.displayName = this.supportContractTitle;
    //    this.supportItemSupportContractLookupTableModal.show();
    //}
    openSelectConsumableTypeModal() {
        this.supportItemConsumableTypeLookupTableModal.id = this.supportItem.consumableTypeId;
        this.supportItemConsumableTypeLookupTableModal.displayName = this.consumableTypeType;
        this.supportItemConsumableTypeLookupTableModal.show();
    }
    openSelectSupportTypeModal() {
        this.supportItemSupportTypeLookupTableModal.id = this.supportItem.supportTypeId;
        this.supportItemSupportTypeLookupTableModal.displayName = this.supportTypeType;
        this.supportItemSupportTypeLookupTableModal.show();
    }


    setAssetIdNull() {
        this.supportItem.assetId = null;
        this.assetReference = '';
    }
    setAssetClassIdNull() {
        this.supportItem.assetClassId = null;
        this.assetClassClass = '';
    }
    setUomIdNull() {
        this.supportItem.uomId = null;
        this.uomUnitOfMeasurement = '';
    }
    //setSupportContractIdNull() {
    //    this.supportItem.supportContractId = null;
    //    this.supportContractTitle = '';
    //}
    setConsumableTypeIdNull() {
        this.supportItem.consumableTypeId = null;
        this.consumableTypeType = '';
    }
    setSupportTypeIdNull() {
        this.supportItem.supportTypeId = null;
        this.supportTypeType = '';
    }


    getNewAssetId() {
        this.supportItem.assetId = this.supportItemAssetLookupTableModal.id;
        this.assetReference = this.supportItemAssetLookupTableModal.displayName;
    }
    getNewAssetClassId() {
        this.supportItem.assetClassId = this.supportItemAssetClassLookupTableModal.id;
        this.assetClassClass = this.supportItemAssetClassLookupTableModal.displayName;
    }
    getNewUomId() {
        this.supportItem.uomId = this.uomLookupTableModal.id;
        this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    }
    //getNewSupportContractId() {
    //    this.supportItem.supportContractId = this.supportItemSupportContractLookupTableModal.id;
    //    this.supportContractTitle = this.supportItemSupportContractLookupTableModal.displayName;
    //}
    getNewConsumableTypeId() {
        this.supportItem.consumableTypeId = this.supportItemConsumableTypeLookupTableModal.id;
        this.consumableTypeType = this.supportItemConsumableTypeLookupTableModal.displayName;
    }
    getNewSupportTypeId() {
        this.supportItem.supportTypeId = this.supportItemSupportTypeLookupTableModal.id;
        this.supportTypeType = this.supportItemSupportTypeLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
