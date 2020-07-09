import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { CustomerInvoicesServiceProxy, CustomerInvoiceDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerInvoiceModalComponent } from './create-or-edit-customerInvoice-modal.component';
import { ViewCustomerInvoiceModalComponent } from './view-customerInvoice-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';
import { AppConsts } from 'shared/AppConsts';
import { Location } from '@angular/common';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './customerInvoices.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CustomerInvoicesComponent extends AppComponentBase {

    @ViewChild('createOrEditCustomerInvoiceModal', { static: true }) createOrEditCustomerInvoiceModal: CreateOrEditCustomerInvoiceModalComponent;
    @ViewChild('viewCustomerInvoiceModalComponent', { static: true }) viewCustomerInvoiceModal: ViewCustomerInvoiceModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    estimateId: number;
    workOrderId: number;
    saving = false;

    advancedFiltersAreShown = false;
    filterText = '';
    customerReferenceFilter = '';
    descriptionFilter = '';
    maxDateIssuedFilter: moment.Moment;
    minDateIssuedFilter: moment.Moment;
    maxDateDueFilter: moment.Moment;
    minDateDueFilter: moment.Moment;
    maxTotalTaxFilter: number;
    maxTotalTaxFilterEmpty: number;
    minTotalTaxFilter: number;
    minTotalTaxFilterEmpty: number;
    maxTotalPriceFilter: number;
    maxTotalPriceFilterEmpty: number;
    minTotalPriceFilter: number;
    minTotalPriceFilterEmpty: number;
    maxTotalNetFilter: number;
    maxTotalNetFilterEmpty: number;
    minTotalNetFilter: number;
    minTotalNetFilterEmpty: number;
    maxTotalDiscountFilter: number;
    maxTotalDiscountFilterEmpty: number;
    minTotalDiscountFilter: number;
    minTotalDiscountFilterEmpty: number;
    maxTotalChargeFilter: number;
    maxTotalChargeFilterEmpty: number;
    minTotalChargeFilter: number;
    minTotalChargeFilterEmpty: number;
    invoiceRecipientFilter = '';
    remarksFilter = '';
    customerNameFilter = '';
    leaseItemItemFilter = '';
    workOrderSubjectFilter = '';
    estimateTitleFilter = '';
    currencyCodeFilter = '';
    billingRuleNameFilter = '';
    billingEventPurposeFilter = '';
    invoiceStatusStatusFilter = '';


    _entityTypeFullName = 'Ems.Billing.CustomerInvoice';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private location: Location,
        private _customerInvoicesServiceProxy: CustomerInvoicesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._activatedRoute.queryParams.subscribe((params: Params) => {
            if (Object.keys(params).length > 3) {
                this.primengTableHelper.showLoadingIndicator();
                this._customerInvoicesServiceProxy.getCallback(params.code, params.state).subscribe(response => {
                    this.primengTableHelper.hideLoadingIndicator();
                    if (response) {
                        this.notify.info(this.l('InvoiceCreatedXero'));
                        this.location.replaceState(this.location.path().split('?')[0], '');
                    }
                });
            }
        });

        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
        this.estimateId = this._activatedRoute.snapshot.queryParams['estimateId'];
        this.workOrderId = this._activatedRoute.snapshot.queryParams['workOrderId'];
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getCustomerInvoices(event?: LazyLoadEvent, createCustomerInvoice?: boolean) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._customerInvoicesServiceProxy.getAll(
            this.filterText,
            this.customerReferenceFilter,
            this.descriptionFilter,
            this.maxDateIssuedFilter,
            this.minDateIssuedFilter,
            this.maxDateDueFilter,
            this.minDateDueFilter,
            this.maxTotalTaxFilter == null ? this.maxTotalTaxFilterEmpty : this.maxTotalTaxFilter,
            this.minTotalTaxFilter == null ? this.minTotalTaxFilterEmpty : this.minTotalTaxFilter,
            this.maxTotalPriceFilter == null ? this.maxTotalPriceFilterEmpty : this.maxTotalPriceFilter,
            this.minTotalPriceFilter == null ? this.minTotalPriceFilterEmpty : this.minTotalPriceFilter,
            this.maxTotalNetFilter == null ? this.maxTotalNetFilterEmpty : this.maxTotalNetFilter,
            this.minTotalNetFilter == null ? this.minTotalNetFilterEmpty : this.minTotalNetFilter,
            this.maxTotalDiscountFilter == null ? this.maxTotalDiscountFilterEmpty : this.maxTotalDiscountFilter,
            this.minTotalDiscountFilter == null ? this.minTotalDiscountFilterEmpty : this.minTotalDiscountFilter,
            this.maxTotalChargeFilter == null ? this.maxTotalChargeFilterEmpty : this.maxTotalChargeFilter,
            this.minTotalChargeFilter == null ? this.minTotalChargeFilterEmpty : this.minTotalChargeFilter,
            this.invoiceRecipientFilter,
            this.remarksFilter,
            this.customerNameFilter,
            this.workOrderSubjectFilter,
            this.estimateTitleFilter,
            this.currencyCodeFilter,
            this.billingRuleNameFilter,
            this.billingEventPurposeFilter,
            this.invoiceStatusStatusFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();

            if (createCustomerInvoice && this.estimateId > 0) {
                this.createOrEditCustomerInvoiceModal.show(0, this.estimateId);
            }

            if (createCustomerInvoice && this.workOrderId > 0) {
                this.createOrEditCustomerInvoiceModal.show(0, 0, this.workOrderId);
            }
        });
    }

    clearSearch() {
        this.filterText = this.customerReferenceFilter = '';
        this.descriptionFilter = this.invoiceStatusStatusFilter = '';
        this.maxDateIssuedFilter = this.minDateIssuedFilter = null;
        this.maxDateDueFilter = this.minDateDueFilter = null;
        this.maxTotalTaxFilter = this.minTotalTaxFilter = null;
        this.maxTotalPriceFilter = this.minTotalPriceFilter = null;
        this.maxTotalNetFilter = this.minTotalNetFilter = null;
        this.maxTotalDiscountFilter = this.minTotalDiscountFilter = null;
        this.maxTotalChargeFilter = this.minTotalChargeFilter = null;
        this.invoiceRecipientFilter = this.remarksFilter = '';
        this.customerNameFilter = this.workOrderSubjectFilter = '';
        this.estimateTitleFilter = this.currencyCodeFilter = '';
        this.billingRuleNameFilter = this.billingEventPurposeFilter = '';

        this.getCustomerInvoices();
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createCustomerInvoice(): void {
        this.createOrEditCustomerInvoiceModal.show();
    }

    showHistory(customerInvoice: CustomerInvoiceDto): void {
        this.entityTypeHistoryModal.show({
            entityId: customerInvoice.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteCustomerInvoice(customerInvoice: CustomerInvoiceDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._customerInvoicesServiceProxy.delete(customerInvoice.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._customerInvoicesServiceProxy.getCustomerInvoicesToExcel(
            this.filterText,
            this.customerReferenceFilter,
            this.descriptionFilter,
            this.maxDateIssuedFilter,
            this.minDateIssuedFilter,
            this.maxDateDueFilter,
            this.minDateDueFilter,
            this.maxTotalTaxFilter == null ? this.maxTotalTaxFilterEmpty : this.maxTotalTaxFilter,
            this.minTotalTaxFilter == null ? this.minTotalTaxFilterEmpty : this.minTotalTaxFilter,
            this.maxTotalPriceFilter == null ? this.maxTotalPriceFilterEmpty : this.maxTotalPriceFilter,
            this.minTotalPriceFilter == null ? this.minTotalPriceFilterEmpty : this.minTotalPriceFilter,
            this.maxTotalNetFilter == null ? this.maxTotalNetFilterEmpty : this.maxTotalNetFilter,
            this.minTotalNetFilter == null ? this.minTotalNetFilterEmpty : this.minTotalNetFilter,
            this.maxTotalDiscountFilter == null ? this.maxTotalDiscountFilterEmpty : this.maxTotalDiscountFilter,
            this.minTotalDiscountFilter == null ? this.minTotalDiscountFilterEmpty : this.minTotalDiscountFilter,
            this.maxTotalChargeFilter == null ? this.maxTotalChargeFilterEmpty : this.maxTotalChargeFilter,
            this.minTotalChargeFilter == null ? this.minTotalChargeFilterEmpty : this.minTotalChargeFilter,
            this.invoiceRecipientFilter,
            this.remarksFilter,
            this.customerNameFilter,
            this.workOrderSubjectFilter,
            this.estimateTitleFilter,
            this.currencyCodeFilter,
            this.billingRuleNameFilter,
            this.billingEventPurposeFilter,
            this.invoiceStatusStatusFilter,
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }

    viewCustomerInvoice(customerInvoiceId): void {
        this._router.navigate(['app/main/billing/customerInvoices/viewCustomerInvoice'], { queryParams: { customerInvoiceId: customerInvoiceId } });
    }

    base64ToArrayBuffer(base64: any): ArrayBuffer {
        var binary_string = window.atob(base64);
        var len = binary_string.length;
        var bytes = new Uint8Array(len);
        for (var i = 0; i < len; i++) {
            bytes[i] = binary_string.charCodeAt(i);
        }
        return bytes.buffer;
    }

    generatePDF(invoiceId): void {
        this._router.navigate(['app/main/billing/customerInvoices/invoicePDF'], { queryParams: { invoiceId: invoiceId } });
    }

    xeroConnection(invoiceData, flag): void {
        if (invoiceData.invoiceStatusStatus == AppConsts.Submitted && flag == AppConsts.Submit) {
            this.notify.info(this.l('InvoiceAlreadySubmitted'));
        }
        else if (invoiceData.invoiceStatusStatus == AppConsts.Paid && flag == AppConsts.Submit) {
            this.notify.info(this.l('InvoiceAlreadyPaid'));
        }
        else {
            this._router.navigate(['app/main/billing/customerInvoices/invoicePDF'], { queryParams: { invoiceId: invoiceData.customerInvoice.id, flag: AppConsts.Submit } });
        }
    }

    viewPDF(invoiceData) {
        this._router.navigate(['app/main/billing/customerInvoices/invoicePDF'], { queryParams: { invoiceId: invoiceData.customerInvoice.id, flag: AppConsts.PDF } });
    }

    refreshInvoices() {
        this.saving = true;

        this._customerInvoicesServiceProxy.refereshXeroInvoices()
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(
                (data: any) => {
                    if (data == AppConsts.Refreshed) {
                        this.notify.info(this.l('RefreshedSuccessfully'));
                        this.reloadPage();
                    }
                    else {
                        window.open(data);
                    }
                },
                error => {
                    this.notify.info(this.l('Somethingwentwrongmsg'));
                });
    }
}
