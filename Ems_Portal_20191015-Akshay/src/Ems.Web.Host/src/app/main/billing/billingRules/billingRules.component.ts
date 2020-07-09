import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BillingRulesServiceProxy, BillingRuleDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditBillingRuleModalComponent } from './create-or-edit-billingRule-modal.component';
import { ViewBillingRuleModalComponent } from './view-billingRule-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './billingRules.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BillingRulesComponent extends AppComponentBase {

    @ViewChild('createOrEditBillingRuleModal', { static: true }) createOrEditBillingRuleModal: CreateOrEditBillingRuleModalComponent;
    @ViewChild('viewBillingRuleModalComponent', { static: true }) viewBillingRuleModal: ViewBillingRuleModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';
    isParentFilter = -1;
    maxParentBillingRuleRefIdFilter : number;
		maxParentBillingRuleRefIdFilterEmpty : number;
		minParentBillingRuleRefIdFilter : number;
		minParentBillingRuleRefIdFilterEmpty : number;
    maxChargePerUnitFilter : number;
		maxChargePerUnitFilterEmpty : number;
		minChargePerUnitFilter : number;
		minChargePerUnitFilterEmpty : number;
        billingRuleTypeTypeFilter = '';
        usageMetricMetricFilter = '';
        leaseAgreementTitleFilter = '';
        vendorNameFilter = '';
        leaseItemItemFilter = '';
        currencyCodeFilter = '';


    _entityTypeFullName = 'Ems.Billing.BillingRule';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _billingRulesServiceProxy: BillingRulesServiceProxy,
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

    getBillingRules(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._billingRulesServiceProxy.getAll(
            this.filterText,
            this.nameFilter,
            this.isParentFilter,
            this.maxParentBillingRuleRefIdFilter == null ? this.maxParentBillingRuleRefIdFilterEmpty: this.maxParentBillingRuleRefIdFilter,
            this.minParentBillingRuleRefIdFilter == null ? this.minParentBillingRuleRefIdFilterEmpty: this.minParentBillingRuleRefIdFilter,
            this.maxChargePerUnitFilter == null ? this.maxChargePerUnitFilterEmpty: this.maxChargePerUnitFilter,
            this.minChargePerUnitFilter == null ? this.minChargePerUnitFilterEmpty: this.minChargePerUnitFilter,
            this.billingRuleTypeTypeFilter,
            this.usageMetricMetricFilter,
            this.leaseAgreementTitleFilter,
            this.vendorNameFilter,
            this.leaseItemItemFilter,
            this.currencyCodeFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createBillingRule(): void {
        this.createOrEditBillingRuleModal.show();
    }

    showHistory(billingRule: BillingRuleDto): void {
        this.entityTypeHistoryModal.show({
            entityId: billingRule.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteBillingRule(billingRule: BillingRuleDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._billingRulesServiceProxy.delete(billingRule.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._billingRulesServiceProxy.getBillingRulesToExcel(
        this.filterText,
            this.nameFilter,
            this.isParentFilter,
            this.maxParentBillingRuleRefIdFilter == null ? this.maxParentBillingRuleRefIdFilterEmpty: this.maxParentBillingRuleRefIdFilter,
            this.minParentBillingRuleRefIdFilter == null ? this.minParentBillingRuleRefIdFilterEmpty: this.minParentBillingRuleRefIdFilter,
            this.maxChargePerUnitFilter == null ? this.maxChargePerUnitFilterEmpty: this.maxChargePerUnitFilter,
            this.minChargePerUnitFilter == null ? this.minChargePerUnitFilterEmpty: this.minChargePerUnitFilter,
            this.billingRuleTypeTypeFilter,
            this.usageMetricMetricFilter,
            this.leaseAgreementTitleFilter,
            this.vendorNameFilter,
            this.leaseItemItemFilter,
            this.currencyCodeFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
