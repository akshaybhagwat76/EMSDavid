import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderUpdatesServiceProxy, CreateOrEditWorkOrderUpdateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ItemTypeLookupTableModalComponent } from '@app/config/quotations/itemTypes/itemType-lookup-table-modal.component';
import { WorkOrderWorkOrderActionLookupTableModalComponent } from './workOrder-workOrderAction-lookup-table-modal.component';
//import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';


@Component({
    selector: 'createOrEditWorkOrderUpdateModal',
    templateUrl: './create-or-edit-workOrderUpdate-modal.component.html'
})
export class CreateOrEditWorkOrderUpdateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    @ViewChild('workOrderWorkOrderActionLookupTableModal', { static: true }) workOrderWorkOrderActionLookupTableModal: WorkOrderWorkOrderActionLookupTableModalComponent;
    //@ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;

    @Output() workOrderUpdateModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrderUpdate: CreateOrEditWorkOrderUpdateDto = new CreateOrEditWorkOrderUpdateDto();
    workOrderId: number;
    workOrderSubject = '';
    workOrderActionAction = '';
    itemTypeType = '';


    constructor(
        injector: Injector,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy
    ) {
        super(injector);
    }

    show(workOrderUpdateId?: number, workOrderId?: number): void {

        if (!workOrderUpdateId) {
            this.workOrderUpdate = new CreateOrEditWorkOrderUpdateDto();
            this.workOrderUpdate.id = workOrderUpdateId;
            this.workOrderUpdate.workOrderId = workOrderId;
            //this.workOrderUpdate.updatedAt = moment().startOf('day');
            this.workOrderSubject = '';
            this.workOrderActionAction = '';
            this.itemTypeType = '';

            this.active = true;
            this.modal.show();
        } else {
            this._workOrderUpdatesServiceProxy.getWorkOrderUpdateForEdit(workOrderUpdateId).subscribe(result => {

                this.workOrderUpdate = result.workOrderUpdate;
                this.workOrderUpdate.workOrderId = workOrderId;

                this.workOrderSubject = result.workOrderSubject;
                this.workOrderActionAction = result.workOrderActionAction;
                this.itemTypeType = result.itemTypeType;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;
        this._workOrderUpdatesServiceProxy.createOrEdit(this.workOrderUpdate)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.workOrderUpdateModalSave.emit(null);
            });
    }

    setWorkOrderIdNull() {
        this.workOrderUpdate.workOrderId = null;
        this.workOrderSubject = '';
    }

    openSelectItemTypeModal() {
        this.itemTypeLookupTableModal.id = this.workOrderUpdate.itemTypeId;
        this.itemTypeLookupTableModal.displayName = this.itemTypeType;
        this.itemTypeLookupTableModal.show();
    }
    openSelectActionModal() {
        this.workOrderWorkOrderActionLookupTableModal.id = this.workOrderUpdate.workOrderActionId;
        this.workOrderWorkOrderActionLookupTableModal.displayName = this.workOrderActionAction;
        this.workOrderWorkOrderActionLookupTableModal.show();
    }

    setItemTypeIdNull() {
        this.workOrderUpdate.itemTypeId = null;
        this.itemTypeType = '';
    }
    setActionIdNull() {
        this.workOrderUpdate.workOrderActionId = null;
        this.workOrderActionAction = '';
    }


    getNewItemTypeId() {
        this.workOrderUpdate.itemTypeId = this.itemTypeLookupTableModal.id;
        this.itemTypeType = this.itemTypeLookupTableModal.displayName;
    }
    getNewActionId() {
        this.workOrderUpdate.workOrderActionId = this.workOrderWorkOrderActionLookupTableModal.id;
        this.workOrderActionAction = this.workOrderWorkOrderActionLookupTableModal.displayName;
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
