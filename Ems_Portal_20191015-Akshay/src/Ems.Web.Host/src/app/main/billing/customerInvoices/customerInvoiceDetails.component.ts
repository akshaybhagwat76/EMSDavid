import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CustomerInvoiceDetailsServiceProxy, CustomerInvoiceDetailDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerInvoiceDetailModalComponent } from './create-or-edit-customerInvoiceDetail-modal.component';
import { ViewCustomerInvoiceDetailModalComponent } from './view-customerInvoiceDetail-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './customerInvoiceDetails.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CustomerInvoiceDetailsComponent extends AppComponentBase {

    @ViewChild('createOrEditCustomerInvoiceDetailModal', { static: true }) createOrEditCustomerInvoiceDetailModal: CreateOrEditCustomerInvoiceDetailModalComponent;
    @ViewChild('viewCustomerInvoiceDetailModalComponent', { static: true }) viewCustomerInvoiceDetailModal: ViewCustomerInvoiceDetailModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    descriptionFilter = '';
    maxQuantityFilter : number;
		maxQuantityFilterEmpty : number;
		minQuantityFilter : number;
		minQuantityFilterEmpty : number;
    maxUnitPriceFilter : number;
		maxUnitPriceFilterEmpty : number;
		minUnitPriceFilter : number;
		minUnitPriceFilterEmpty : number;
    maxGrossFilter : number;
		maxGrossFilterEmpty : number;
		minGrossFilter : number;
		minGrossFilterEmpty : number;
    maxTaxFilter : number;
		maxTaxFilterEmpty : number;
		minTaxFilter : number;
		minTaxFilterEmpty : number;
    maxNetFilter : number;
		maxNetFilterEmpty : number;
		minNetFilter : number;
		minNetFilterEmpty : number;
    maxDiscountFilter : number;
		maxDiscountFilterEmpty : number;
		minDiscountFilter : number;
		minDiscountFilterEmpty : number;
    maxChargeFilter : number;
		maxChargeFilterEmpty : number;
		minChargeFilter : number;
		minChargeFilterEmpty : number;
    maxBillingRuleRefIdFilter : number;
		maxBillingRuleRefIdFilterEmpty : number;
		minBillingRuleRefIdFilter : number;
		minBillingRuleRefIdFilterEmpty : number;
        leaseItemItemFilter = '';
        customerInvoiceDescriptionFilter = '';


    _entityTypeFullName = 'Ems.Billing.CustomerInvoiceDetail';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _customerInvoiceDetailsServiceProxy: CustomerInvoiceDetailsServiceProxy,
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

    getCustomerInvoiceDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._customerInvoiceDetailsServiceProxy.getAll(
            this.filterText,
            this.descriptionFilter,
            this.maxQuantityFilter == null ? this.maxQuantityFilterEmpty: this.maxQuantityFilter,
            this.minQuantityFilter == null ? this.minQuantityFilterEmpty: this.minQuantityFilter,
            this.maxUnitPriceFilter == null ? this.maxUnitPriceFilterEmpty: this.maxUnitPriceFilter,
            this.minUnitPriceFilter == null ? this.minUnitPriceFilterEmpty: this.minUnitPriceFilter,
            this.maxGrossFilter == null ? this.maxGrossFilterEmpty: this.maxGrossFilter,
            this.minGrossFilter == null ? this.minGrossFilterEmpty: this.minGrossFilter,
            this.maxTaxFilter == null ? this.maxTaxFilterEmpty: this.maxTaxFilter,
            this.minTaxFilter == null ? this.minTaxFilterEmpty: this.minTaxFilter,
            this.maxNetFilter == null ? this.maxNetFilterEmpty: this.maxNetFilter,
            this.minNetFilter == null ? this.minNetFilterEmpty: this.minNetFilter,
            this.maxDiscountFilter == null ? this.maxDiscountFilterEmpty: this.maxDiscountFilter,
            this.minDiscountFilter == null ? this.minDiscountFilterEmpty: this.minDiscountFilter,
            this.maxChargeFilter == null ? this.maxChargeFilterEmpty: this.maxChargeFilter,
            this.minChargeFilter == null ? this.minChargeFilterEmpty: this.minChargeFilter,
            this.maxBillingRuleRefIdFilter == null ? this.maxBillingRuleRefIdFilterEmpty: this.maxBillingRuleRefIdFilter,
            this.minBillingRuleRefIdFilter == null ? this.minBillingRuleRefIdFilterEmpty: this.minBillingRuleRefIdFilter,
            this.leaseItemItemFilter,
            this.customerInvoiceDescriptionFilter,
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

    createCustomerInvoiceDetail(): void {
        this.createOrEditCustomerInvoiceDetailModal.show();
    }

    showHistory(customerInvoiceDetail: CustomerInvoiceDetailDto): void {
        this.entityTypeHistoryModal.show({
            entityId: customerInvoiceDetail.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteCustomerInvoiceDetail(customerInvoiceDetail: CustomerInvoiceDetailDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._customerInvoiceDetailsServiceProxy.delete(customerInvoiceDetail.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
}
