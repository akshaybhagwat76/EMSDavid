import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { BillingRulesServiceProxy, CreateOrEditBillingRuleDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { BillingRuleBillingRuleTypeLookupTableModalComponent } from './billingRule-billingRuleType-lookup-table-modal.component';
import { BillingRuleUsageMetricLookupTableModalComponent } from './billingRule-usageMetric-lookup-table-modal.component';
import { BillingRuleLeaseAgreementLookupTableModalComponent } from './billingRule-leaseAgreement-lookup-table-modal.component';
import { BillingRuleVendorLookupTableModalComponent } from './billingRule-vendor-lookup-table-modal.component';
import { BillingRuleLeaseItemLookupTableModalComponent } from './billingRule-leaseItem-lookup-table-modal.component';
import { BillingRuleCurrencyLookupTableModalComponent } from './billingRule-currency-lookup-table-modal.component';


@Component({
    selector: 'createOrEditBillingRuleModal',
    templateUrl: './create-or-edit-billingRule-modal.component.html'
})
export class CreateOrEditBillingRuleModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('billingRuleBillingRuleTypeLookupTableModal', { static: true }) billingRuleBillingRuleTypeLookupTableModal: BillingRuleBillingRuleTypeLookupTableModalComponent;
    @ViewChild('billingRuleUsageMetricLookupTableModal', { static: true }) billingRuleUsageMetricLookupTableModal: BillingRuleUsageMetricLookupTableModalComponent;
    @ViewChild('billingRuleLeaseAgreementLookupTableModal', { static: true }) billingRuleLeaseAgreementLookupTableModal: BillingRuleLeaseAgreementLookupTableModalComponent;
    @ViewChild('billingRuleVendorLookupTableModal', { static: true }) billingRuleVendorLookupTableModal: BillingRuleVendorLookupTableModalComponent;
    @ViewChild('billingRuleLeaseItemLookupTableModal', { static: true }) billingRuleLeaseItemLookupTableModal: BillingRuleLeaseItemLookupTableModalComponent;
    @ViewChild('billingRuleCurrencyLookupTableModal', { static: true }) billingRuleCurrencyLookupTableModal: BillingRuleCurrencyLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    billingRule: CreateOrEditBillingRuleDto = new CreateOrEditBillingRuleDto();

    billingRuleTypeType = '';
    usageMetricMetric = '';
    leaseAgreementTitle = '';
    vendorName = '';
    leaseItemItem = '';
    currencyCode = '';
    isFormValid = false;
    errorMsg = "";

    constructor(
        injector: Injector,
        private _billingRulesServiceProxy: BillingRulesServiceProxy
    ) {
        super(injector);
    }

    show(billingRuleId?: number): void {

        if (!billingRuleId) {
            this.billingRule = new CreateOrEditBillingRuleDto();
            this.billingRule.id = billingRuleId;
            this.billingRuleTypeType = '';
            this.usageMetricMetric = '';
            this.leaseAgreementTitle = '';
            this.vendorName = '';
            this.leaseItemItem = '';
            this.currencyCode = '';

            this.active = true;
            this.modal.show();
        } else {
            this._billingRulesServiceProxy.getBillingRuleForEdit(billingRuleId).subscribe(result => {
                this.billingRule = result.billingRule;

                this.billingRuleTypeType = result.billingRuleTypeType;
                this.usageMetricMetric = result.usageMetricMetric;
                this.leaseAgreementTitle = result.leaseAgreementTitle;
                this.vendorName = result.vendorName;
                this.leaseItemItem = result.leaseItemItem;
                this.currencyCode = result.currencyCode;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.billingRule.billingRuleTypeId || !this.billingRule.usageMetricId || !this.billingRule.name) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._billingRulesServiceProxy.createOrEdit(this.billingRule)
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

    openSelectBillingRuleTypeModal() {
        this.billingRuleBillingRuleTypeLookupTableModal.id = this.billingRule.billingRuleTypeId;
        this.billingRuleBillingRuleTypeLookupTableModal.displayName = this.billingRuleTypeType;
        this.billingRuleBillingRuleTypeLookupTableModal.show();
    }
    openSelectUsageMetricModal() {
        this.billingRuleUsageMetricLookupTableModal.id = this.billingRule.usageMetricId;
        this.billingRuleUsageMetricLookupTableModal.displayName = this.usageMetricMetric;
        this.billingRuleUsageMetricLookupTableModal.show();
    }
    openSelectLeaseAgreementModal() {
        this.billingRuleLeaseAgreementLookupTableModal.id = this.billingRule.leaseAgreementId;
        this.billingRuleLeaseAgreementLookupTableModal.displayName = this.leaseAgreementTitle;
        this.billingRuleLeaseAgreementLookupTableModal.show();
    }
    openSelectVendorModal() {
        this.billingRuleVendorLookupTableModal.id = this.billingRule.vendorId;
        this.billingRuleVendorLookupTableModal.displayName = this.vendorName;
        this.billingRuleVendorLookupTableModal.show();
    }
    openSelectLeaseItemModal() {
        this.billingRuleLeaseItemLookupTableModal.id = this.billingRule.leaseItemId;
        this.billingRuleLeaseItemLookupTableModal.displayName = this.leaseItemItem;
        this.billingRuleLeaseItemLookupTableModal.show();
    }
    openSelectCurrencyModal() {
        this.billingRuleCurrencyLookupTableModal.id = this.billingRule.currencyId;
        this.billingRuleCurrencyLookupTableModal.displayName = this.currencyCode;
        this.billingRuleCurrencyLookupTableModal.show();
    }


    setBillingRuleTypeIdNull() {
        this.billingRule.billingRuleTypeId = null;
        this.billingRuleTypeType = '';
    }
    setUsageMetricIdNull() {
        this.billingRule.usageMetricId = null;
        this.usageMetricMetric = '';
    }
    setLeaseAgreementIdNull() {
        this.billingRule.leaseAgreementId = null;
        this.leaseAgreementTitle = '';
    }
    setVendorIdNull() {
        this.billingRule.vendorId = null;
        this.vendorName = '';
    }
    setLeaseItemIdNull() {
        this.billingRule.leaseItemId = null;
        this.leaseItemItem = '';
    }
    setCurrencyIdNull() {
        this.billingRule.currencyId = null;
        this.currencyCode = '';
    }


    getNewBillingRuleTypeId() {
        this.billingRule.billingRuleTypeId = this.billingRuleBillingRuleTypeLookupTableModal.id;
        this.billingRuleTypeType = this.billingRuleBillingRuleTypeLookupTableModal.displayName;
    }
    getNewUsageMetricId() {
        this.billingRule.usageMetricId = this.billingRuleUsageMetricLookupTableModal.id;
        this.usageMetricMetric = this.billingRuleUsageMetricLookupTableModal.displayName;
    }
    getNewLeaseAgreementId() {
        this.billingRule.leaseAgreementId = this.billingRuleLeaseAgreementLookupTableModal.id;
        this.leaseAgreementTitle = this.billingRuleLeaseAgreementLookupTableModal.displayName;
    }
    getNewVendorId() {
        this.billingRule.vendorId = this.billingRuleVendorLookupTableModal.id;
        this.vendorName = this.billingRuleVendorLookupTableModal.displayName;
    }
    getNewLeaseItemId() {
        this.billingRule.leaseItemId = this.billingRuleLeaseItemLookupTableModal.id;
        this.leaseItemItem = this.billingRuleLeaseItemLookupTableModal.displayName;
    }
    getNewCurrencyId() {
        this.billingRule.currencyId = this.billingRuleCurrencyLookupTableModal.id;
        this.currencyCode = this.billingRuleCurrencyLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
