import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import * as _ from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { CreateOrEditLeaseItemModalComponent } from './create-or-edit-leaseItem-modal.component';
import { ViewLeaseItemModalComponent } from './view-leaseItem-modal.component';
import { LeaseAgreementsServiceProxy, LeaseAgreementDto, LeaseItemsServiceProxy, LeaseItemDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditLeaseAgreementModalComponent } from './create-or-edit-leaseAgreement-modal.component';
import * as moment from 'moment';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments//view-attachment-modal.component';
import { AttachmentsServiceProxy, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewLeaseAgreement',
    templateUrl: './viewLeaseAgreement.component.html',
    animations: [appModuleAnimation()]
})
export class ViewLeaseAgreementComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditLeaseAgreementModal', { static: true }) createOrEditLeaseAgreementModal: CreateOrEditLeaseAgreementModalComponent;
    @ViewChild('createOrEditLeaseItemModal', { static: true }) createOrEditLeaseItemModal: CreateOrEditLeaseItemModalComponent;
    @ViewChild('viewLeaseItemModalComponent', { static: true }) viewLeaseItemModal: ViewLeaseItemModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    primengTableHelper1: PrimengTableHelper;

    advancedFiltersAreShown = false;
    filterText = '';
    maxUpdatedAtFilter: moment.Moment;
    minUpdatedAtFilter: moment.Moment;
    updateFilter = '';
    maxUpdatedByUserIdFilter: number;
    maxUpdatedByUserIdFilterEmpty: number;
    minUpdatedByUserIdFilter: number;
    minUpdatedByUserIdFilterEmpty: number;

    _entityTypeFullName = 'Ems.Assets.LeaseAgreement';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _leaseAgreementsServiceProxy: LeaseAgreementsServiceProxy,
        private _leaseItemsServiceProxy: LeaseItemsServiceProxy,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
    }

    active = false;
    saving = false;

    leaseAgreementId : number;
    leaseAgreement: LeaseAgreementDto;
    contactContactName: string;
    assetOwnerName: string;
    customerName: string;

    maxDateAllocatedFilter : moment.Moment;
    minDateAllocatedFilter: moment.Moment;
    maxAllocationPercentageFilter: number;
    maxAllocationPercentageFilterEmpty: number;
    minAllocationPercentageFilter: number;
    minAllocationPercentageFilterEmpty: number;
    termsFilter = '';
    maxUnitRentalRateFilter: number;
    maxUnitRentalRateFilterEmpty: number;
    minUnitRentalRateFilter: number;
    minUnitRentalRateFilterEmpty: number;
    maxUnitDepositRateFilter: number;
    maxUnitDepositRateFilterEmpty: number;
    minUnitDepositRateFilter: number;
    minUnitDepositRateFilterEmpty: number;
    maxStartDateFilter: moment.Moment;
    minStartDateFilter: moment.Moment;
    maxEndDateFilter: moment.Moment;
    minEndDateFilter: moment.Moment;
    attachmentsFilter = '';
    maxRentalUomRefIdFilter: number;
    maxRentalUomRefIdFilterEmpty: number;
    minRentalUomRefIdFilter: number;
    minRentalUomRefIdFilterEmpty: number;
    maxDepositUomRefIdFilter: number;
    maxDepositUomRefIdFilterEmpty: number;
    minDepositUomRefIdFilter: number;
    minDepositUomRefIdFilterEmpty: number;
    itemFilter = '';
    descriptionFilter = '';
    assetClassClassFilter = '';
    assetReferenceFilter = '';
    leaseAgreementTitleFilter = '';

    ngOnInit(): void {

        this.leaseAgreementId = this._activatedRoute.snapshot.queryParams['leaseAgreementId'];
        this.leaseAgreement = new LeaseAgreementDto();
        this.contactContactName = '';
        this.assetOwnerName = '';
        this.customerName = '';

        this.getLeaseAgreements();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }
    
    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }
    
    getLeaseAgreements(event?: LazyLoadEvent){
        this._leaseAgreementsServiceProxy.getLeaseAgreementForView(this.leaseAgreementId)
            .subscribe((leaseAgreementResult) => { 

                if (leaseAgreementResult.leaseAgreement == null)
                    {this.close();
                }
                else{
                    this.leaseAgreement = leaseAgreementResult.leaseAgreement;
                    this.contactContactName = leaseAgreementResult.contactContactName;
                    this.assetOwnerName = leaseAgreementResult.assetOwnerName;
                    this.customerName = leaseAgreementResult.customerName;
                    this.leaseAgreementId = leaseAgreementResult.leaseAgreement.id;

                    this.getLeaseItems();
                    this.getAttachments();
                }
            });
    }

    getLeaseItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._leaseItemsServiceProxy.getAll(
            this.leaseAgreementId,
            this.filterText,
            this.maxDateAllocatedFilter,
            this.minDateAllocatedFilter,
            this.maxAllocationPercentageFilter == null ? this.maxAllocationPercentageFilterEmpty : this.maxAllocationPercentageFilter,
            this.minAllocationPercentageFilter == null ? this.minAllocationPercentageFilterEmpty : this.minAllocationPercentageFilter,
            this.termsFilter,
            this.maxUnitRentalRateFilter == null ? this.maxUnitRentalRateFilterEmpty : this.maxUnitRentalRateFilter,
            this.minUnitRentalRateFilter == null ? this.minUnitRentalRateFilterEmpty : this.minUnitRentalRateFilter,
            this.maxUnitDepositRateFilter == null ? this.maxUnitDepositRateFilterEmpty : this.maxUnitDepositRateFilter,
            this.minUnitDepositRateFilter == null ? this.minUnitDepositRateFilterEmpty : this.minUnitDepositRateFilter,
            this.maxStartDateFilter,
            this.minStartDateFilter,
            this.maxEndDateFilter,
            this.minEndDateFilter,
            //this.attachmentsFilter,
            this.maxRentalUomRefIdFilter == null ? this.maxRentalUomRefIdFilterEmpty : this.maxRentalUomRefIdFilter,
            this.minRentalUomRefIdFilter == null ? this.minRentalUomRefIdFilterEmpty : this.minRentalUomRefIdFilter,
            this.maxDepositUomRefIdFilter == null ? this.maxDepositUomRefIdFilterEmpty : this.maxDepositUomRefIdFilter,
            this.minDepositUomRefIdFilter == null ? this.minDepositUomRefIdFilterEmpty : this.minDepositUomRefIdFilter,
            this.itemFilter,
            this.descriptionFilter,
            this.assetClassClassFilter,
            this.assetReferenceFilter,
            this.leaseAgreementTitleFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    getAttachments(event?: LazyLoadEvent) {

        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._attachmentsServiceProxy.getSome(
            "LeaseAgreement",
            this.leaseAgreementId,
            this.primengTableHelper1.getSorting(this.dataTable1),
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event)
        ).subscribe(result => {
            this.primengTableHelper1.totalRecordsCount = result.totalCount;
            this.primengTableHelper1.records = result.items;
            this.primengTableHelper1.hideLoadingIndicator();
        });
    }

    createAttachment(): void {
        this.createOrEditAttachmentModal.show(null, 'LeaseAgreement', this.leaseAgreement.id);
    }

    viewAttachment(attachment: AttachmentDto): void {
        let url = AppConsts.remoteServiceBaseUrl + '/Attachments/GetAttachmentURI?resourcePath=' + attachment.blobFolder + '/' + attachment.blobId;

        let customHeaders = {
            'Abp.TenantId': abp.multiTenancy.getTenantIdCookie(),
            'Authorization': 'Bearer ' + abp.auth.getToken()
        };

        XmlHttpRequestHelper.ajax('GET', url, customHeaders, null, (response) => {
            if (response.result) {
                window.open(response.result, '_blank');
            }
            else {
                this.message.error(this.l('Failed to Download'));
            }
        });
    }

    deleteAttachment(attachment: AttachmentDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._attachmentsServiceProxy.delete(attachment.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
        this.paginator1.changePage(this.paginator1.getPage())
    }

    createLeaseItem(): void {
        this.createOrEditLeaseItemModal.show(null, this.leaseAgreementId);
    }

    deleteLeaseItem(leaseItem: LeaseItemDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._leaseItemsServiceProxy.delete(leaseItem.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    close(): void {
        this._router.navigate(['app/main/assets/leaseAgreements']);
    }
}
