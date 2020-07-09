import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { BillingEventDetailsServiceProxy, CreateOrEditBillingEventDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { BillingEventDetailBillingRuleLookupTableModalComponent } from './billingEventDetail-billingRule-lookup-table-modal.component';
import { BillingEventDetailLeaseItemLookupTableModalComponent } from './billingEventDetail-leaseItem-lookup-table-modal.component';
import { BillingEventDetailBillingEventLookupTableModalComponent } from './billingEventDetail-billingEvent-lookup-table-modal.component';


@Component({
    selector: 'createOrEditBillingEventDetailModal',
    templateUrl: './create-or-edit-billingEventDetail-modal.component.html'
})
export class CreateOrEditBillingEventDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('billingEventDetailBillingRuleLookupTableModal', { static: true }) billingEventDetailBillingRuleLookupTableModal: BillingEventDetailBillingRuleLookupTableModalComponent;
    @ViewChild('billingEventDetailLeaseItemLookupTableModal', { static: true }) billingEventDetailLeaseItemLookupTableModal: BillingEventDetailLeaseItemLookupTableModalComponent;
    @ViewChild('billingEventDetailBillingEventLookupTableModal', { static: true }) billingEventDetailBillingEventLookupTableModal: BillingEventDetailBillingEventLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billingEventDetail: CreateOrEditBillingEventDetailDto = new CreateOrEditBillingEventDetailDto();

    billingRuleName = '';
    leaseItemItem = '';
    billingEventPurpose = '';


    constructor(
        injector: Injector,
        private _billingEventDetailsServiceProxy: BillingEventDetailsServiceProxy
    ) {
        super(injector);
    }

    show(billingEventDetailId?: number): void {

        if (!billingEventDetailId) {
            this.billingEventDetail = new CreateOrEditBillingEventDetailDto();
            this.billingEventDetail.id = billingEventDetailId;
            this.billingRuleName = '';
            this.leaseItemItem = '';
            this.billingEventPurpose = '';

            this.active = true;
            this.modal.show();
        } else {
            this._billingEventDetailsServiceProxy.getBillingEventDetailForEdit(billingEventDetailId).subscribe(result => {
                this.billingEventDetail = result.billingEventDetail;

                this.billingRuleName = result.billingRuleName;
                this.leaseItemItem = result.leaseItemItem;
                this.billingEventPurpose = result.billingEventPurpose;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._billingEventDetailsServiceProxy.createOrEdit(this.billingEventDetail)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

        openSelectBillingRuleModal() {
        this.billingEventDetailBillingRuleLookupTableModal.id = this.billingEventDetail.billingRuleId;
        this.billingEventDetailBillingRuleLookupTableModal.displayName = this.billingRuleName;
        this.billingEventDetailBillingRuleLookupTableModal.show();
    }
        openSelectLeaseItemModal() {
        this.billingEventDetailLeaseItemLookupTableModal.id = this.billingEventDetail.leaseItemId;
        this.billingEventDetailLeaseItemLookupTableModal.displayName = this.leaseItemItem;
        this.billingEventDetailLeaseItemLookupTableModal.show();
    }
        openSelectBillingEventModal() {
        this.billingEventDetailBillingEventLookupTableModal.id = this.billingEventDetail.billingEventId;
        this.billingEventDetailBillingEventLookupTableModal.displayName = this.billingEventPurpose;
        this.billingEventDetailBillingEventLookupTableModal.show();
    }


        setBillingRuleIdNull() {
        this.billingEventDetail.billingRuleId = null;
        this.billingRuleName = '';
    }
        setLeaseItemIdNull() {
        this.billingEventDetail.leaseItemId = null;
        this.leaseItemItem = '';
    }
        setBillingEventIdNull() {
        this.billingEventDetail.billingEventId = null;
        this.billingEventPurpose = '';
    }


        getNewBillingRuleId() {
        this.billingEventDetail.billingRuleId = this.billingEventDetailBillingRuleLookupTableModal.id;
        this.billingRuleName = this.billingEventDetailBillingRuleLookupTableModal.displayName;
    }
        getNewLeaseItemId() {
        this.billingEventDetail.leaseItemId = this.billingEventDetailLeaseItemLookupTableModal.id;
        this.leaseItemItem = this.billingEventDetailLeaseItemLookupTableModal.displayName;
    }
        getNewBillingEventId() {
        this.billingEventDetail.billingEventId = this.billingEventDetailBillingEventLookupTableModal.id;
        this.billingEventPurpose = this.billingEventDetailBillingEventLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
