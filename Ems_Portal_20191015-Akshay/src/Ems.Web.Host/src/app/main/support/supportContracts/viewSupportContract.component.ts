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
import { SupportContractsServiceProxy, SupportContractDto, SupportItemsServiceProxy, SupportItemDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditSupportContractModalComponent } from './create-or-edit-supportContract-modal.component';
import { CreateOrEditSupportItemModalComponent } from './create-or-edit-supportItem-modal.component';
import { ViewSupportItemModalComponent } from './view-supportItem-modal.component';
import * as moment from 'moment';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments//view-attachment-modal.component';
import { AttachmentsServiceProxy, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewSupportContract',
    templateUrl: './viewSupportContract.component.html',
    animations: [appModuleAnimation()]
})
export class ViewSupportContractComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditSupportContractModal', { static: true }) createOrEditSupportContractModal: CreateOrEditSupportContractModalComponent;
    @ViewChild('createOrEditSupportItemModal', { static: true }) createOrEditSupportItemModal: CreateOrEditSupportItemModalComponent;
    @ViewChild('viewSupportItemModalComponent', { static: true }) viewSupportItemModal: ViewSupportItemModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    primengTableHelper1: PrimengTableHelper;

    _entityTypeFullName = 'Ems.Support.SupportContract';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _supportContractsServiceProxy: SupportContractsServiceProxy,
        private _supportItemsServiceProxy: SupportItemsServiceProxy,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
    }

    active = false;
    saving = false;

    supportContractId: number;
    supportContract: SupportContractDto;
    assetOwnerName: string;
    vendorName: string;

    ngOnInit(): void {

        this.supportContractId = this._activatedRoute.snapshot.queryParams['supportContractId'];
        this.supportContract = new SupportContractDto();

        this.getSupportContract();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getSupportContract(event?: LazyLoadEvent) {
        this._supportContractsServiceProxy.getSupportContractForView(this.supportContractId)
            .subscribe((scResult) => {

                if (scResult.supportContract == null) {
                    this.close();
                }
                else {
                    this.supportContract = scResult.supportContract;
                    this.assetOwnerName = scResult.assetOwnerName;
                    this.vendorName = scResult.vendorName;

                    this.getSupportItems();
                    this.getAttachments();
                }
            });
    }

    getSupportItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._supportItemsServiceProxy.getSome(
            this.supportContractId,
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSorting(this.dataTable)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    deleteSupportItem(supportItem: SupportItemDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._supportItemsServiceProxy.delete(supportItem.id)
                        .subscribe(() => {
                            this.paginator.changePage(this.paginator.getPage());
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    getAttachments(event?: LazyLoadEvent) {

        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._attachmentsServiceProxy.getSome(
            "SupportContract",
            this.supportContractId,
            this.primengTableHelper1.getSorting(this.dataTable1),
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event)
        ).subscribe(result => {
            this.primengTableHelper1.totalRecordsCount = result.totalCount;
            this.primengTableHelper1.records = result.items;
            this.primengTableHelper1.hideLoadingIndicator();
        });
    }

    createSupportItem(): void {
        this.createOrEditSupportItemModal.show(null, this.supportContractId);
    }

    createAttachment(): void {
        this.createOrEditAttachmentModal.show(null, 'SupportContract', this.supportContract.id);
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
        this.paginator1.changePage(this.paginator1.getPage())
    }

    close(): void {
        this._router.navigate(['app/main/support/supportContracts']);
    }
}
