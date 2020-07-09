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
import { AssetsServiceProxy, AssetDto, AssetPartsServiceProxy, AssetPartDto, AttachmentsServiceProxy, AttachmentDto, AssetNotesServiceProxy, AssetNotesDto, UsageMetricsServiceProxy, UsageMetricDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAssetModalComponent } from './create-or-edit-asset-modal.component';
import { CreateOrEditAssetNotesModalComponent } from './create-or-edit-assetNotes-modal.component';
import * as moment from 'moment';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments/view-attachment-modal.component';
import { ViewLeaseItemModalComponent } from '../leaseAgreements/view-leaseItem-modal.component';
import { ViewSupportItemModalComponent } from '../../support/supportContracts/view-supportItem-modal.component';
import { ViewAssetNotesModalComponent } from './view-assetNotes-modal.component';
import { ViewUsageMetricModalComponent } from '../../telematics/usageMetrics/view-usageMetric-modal.component';
import { ViewUsageMetricRecordModalComponent } from '../../telematics/usageMetricRecords/view-usageMetricRecord-modal.component';
import { CreateOrEditUsageMetricModalComponent } from '../../telematics/usageMetrics/create-or-edit-usageMetric-modal.component';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

import { NotifyService } from '@abp/notify/notify.service';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAssetPartModalComponent } from '../assetParts/create-or-edit-assetPart-modal.component';
import { ViewAssetPartModalComponent } from '../assetParts/view-assetPart-modal.component';
import { AssetPartTreeComponent } from '../assetParts/assetPart-tree.component';
import { IBasicAssetPartInfo } from '../assetParts/basic-asset-part-info';

@Component({
    selector: 'viewAsset',
    templateUrl: './viewAsset.component.html',
    animations: [appModuleAnimation()]
})
export class ViewAssetComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditAssetModal', { static: true }) createOrEditAssetModal: CreateOrEditAssetModalComponent;
    @ViewChild('createOrEditAssetNotesModal', { static: true }) createOrEditAssetNotesModal: CreateOrEditAssetNotesModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('viewAssetNotesModalComponent', { static: true }) viewAssetNotesModal: ViewAssetNotesModalComponent;
    @ViewChild('viewLeaseItemModalComponent', { static: true }) viewLeaseItemModal: ViewLeaseItemModalComponent;
    @ViewChild('viewSupportItemModalComponent', { static: true }) viewSupportItemModal: ViewSupportItemModalComponent;

    @ViewChild('viewUsageMetricModal', { static: true }) viewUsageMetricModal: ViewUsageMetricModalComponent;
    @ViewChild('createOrEditUsageMetricModal', { static: true }) createOrEditUsageMetricModal: CreateOrEditUsageMetricModalComponent;
    @ViewChild('viewUsageMetricRecordModal', { static: true }) viewUsageMetricRecordModal: ViewUsageMetricRecordModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    @ViewChild('dataTable2', { static: true }) dataTable2: Table; // For the 3rd table
    @ViewChild('paginator2', { static: true }) paginator2: Paginator; // For the 3rd table

    @ViewChild('dataTable3', { static: true }) dataTable3: Table; // For the 4th table
    @ViewChild('paginator3', { static: true }) paginator3: Paginator; // For the 4th table

    @ViewChild('dataTable4', { static: true }) dataTable4: Table; // For the 5th table
    @ViewChild('paginator4', { static: true }) paginator4: Paginator; // For the 5th table

    @ViewChild('dataTable5', { static: true }) dataTable5: Table; // For the 6th table
    @ViewChild('paginator5', { static: true }) paginator5: Paginator; // For the 6th table

    //@ViewChild('dataTable5', { static: true }) dataTable6: Table; // For AssetParts / Components
    //@ViewChild('paginator5', { static: true }) paginator6: Paginator; 

    @ViewChild('createOrEditAssetPartModal', { static: true }) createOrEditAssetPartModal: CreateOrEditAssetPartModalComponent;
    @ViewChild('viewAssetPartModalComponent', { static: true }) viewAssetPartModal: ViewAssetPartModalComponent;
    @ViewChild('apTree', {static: true}) apTree: AssetPartTreeComponent;
    
    primengTableHelper1: PrimengTableHelper;
    primengTableHelper2: PrimengTableHelper;
    primengTableHelper3: PrimengTableHelper;
    primengTableHelper4: PrimengTableHelper;
    primengTableHelper5: PrimengTableHelper; 
    //primengTableHelper6: PrimengTableHelper;


    assetPart: IBasicAssetPartInfo = {
        assetId: 0,
        id: 0, 
        name: "",
        description: "",
        serialNumber: "",
        installDate: moment(""),
        installed: "",
        assetPartType: "",
        assetPartStatus: "",
        assetReference: "",
        itemType: "",
        code: ""
    };


    _entityTypeFullName = 'Ems.Assets.Asset';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _assetsServiceProxy: AssetsServiceProxy,
        private _attachmentsServiceProxy: AttachmentsServiceProxy,
        private _assetNotesServiceProxy: AssetNotesServiceProxy,
        private _usageMetricsServiceProxy: UsageMetricsServiceProxy,
        private _assetPartsServiceProxy: AssetPartsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
        this.primengTableHelper2 = new PrimengTableHelper(); // For the 3rd table
        this.primengTableHelper3 = new PrimengTableHelper(); // For the 4th table
        this.primengTableHelper4 = new PrimengTableHelper(); // For the 5th table
        this.primengTableHelper5 = new PrimengTableHelper(); // For the 6th table
        //this.primengTableHelper6 = new PrimengTableHelper(); // For AssetParts
    }

    active = false;
    saving = false;

    assetId: number;
    asset: AssetDto;
    assetClassClass: string;
    assetStatusStatus: string;


    ngOnInit(): void {

        this.assetId = this._activatedRoute.snapshot.queryParams['assetId'];
        this.asset = new AssetDto();

        this.getAsset();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getAsset(event?: LazyLoadEvent) {
        this._assetsServiceProxy.getAssetForView(this.assetId)
            .subscribe((assetResult) => {

                if (assetResult.asset == null) {
                    this.close();
                }
                else {
                    this.asset = assetResult.asset;
                    this.assetClassClass = assetResult.assetClassClass;
                    this.assetStatusStatus = assetResult.assetStatusStatus;

                    this.getAttachments();
                }
            });
    }

    apSelected(event: any): void {
        this.assetPart = event;
    }

    apUpdated(event: any): void {

        console.log("########## Part Updated - apUpdated()");
        this.assetPart = event;
    }

    createAssetPart(parentId?: number): void {
        this.createOrEditAssetPartModal.show({
            parentId: parentId
        });
    }

    deleteAssetPart(assetPart: AssetPartDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._assetPartsServiceProxy.delete(assetPart.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    getAssetNotes(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
         
        this.primengTableHelper.showLoadingIndicator();

        this._assetNotesServiceProxy.getAll(
            this.assetId,
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSorting(this.dataTable)
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
            "Asset",
            this.assetId,
            this.primengTableHelper1.getSorting(this.dataTable1),
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event)
        ).subscribe(result => {
            this.primengTableHelper1.totalRecordsCount = result.totalCount;
            this.primengTableHelper1.records = result.items;
            this.primengTableHelper1.hideLoadingIndicator();
        });
    }

    getLeaseItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper2.shouldResetPaging(event)) {
            this.paginator2.changePage(0);
            return;
        }

        this.primengTableHelper2.showLoadingIndicator();

        this._assetsServiceProxy.getAllLeaseItems(
            this.assetId,
            this.primengTableHelper2.getSkipCount(this.paginator2, event),
            this.primengTableHelper2.getMaxResultCount(this.paginator2, event),
            this.primengTableHelper2.getSorting(this.dataTable2)
        ).subscribe(result => {
            this.primengTableHelper2.totalRecordsCount = result.totalCount;
            this.primengTableHelper2.records = result.items;
            this.primengTableHelper2.hideLoadingIndicator();
        });
    }

    getSupportItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper3.shouldResetPaging(event)) {
            this.paginator3.changePage(0);
            return;
        }

        this.primengTableHelper3.showLoadingIndicator();

        this._assetsServiceProxy.getAllSupportItems(this.assetId,
            this.primengTableHelper3.getSkipCount(this.paginator3, event),
            this.primengTableHelper3.getMaxResultCount(this.paginator3, event),
            this.primengTableHelper3.getSorting(this.dataTable3)
        ).subscribe(result => {
            this.primengTableHelper3.totalRecordsCount = result.totalCount;
            this.primengTableHelper3.records = result.items;
            this.primengTableHelper3.hideLoadingIndicator();
        });
    }

    getUsageMetrics(event?: LazyLoadEvent) {
        if (this.primengTableHelper4.shouldResetPaging(event)) {
            this.paginator4.changePage(0);
            return;
        }

        this.primengTableHelper4.showLoadingIndicator();

        this._usageMetricsServiceProxy.getSome("Asset", this.assetId,
            this.primengTableHelper4.getSorting(this.dataTable4),
            this.primengTableHelper4.getSkipCount(this.paginator4, event),
            this.primengTableHelper4.getMaxResultCount(this.paginator4, event)
        ).subscribe(result => {
            this.primengTableHelper4.totalRecordsCount = result.totalCount;
            this.primengTableHelper4.records = result.items;
            this.primengTableHelper4.hideLoadingIndicator();
        });
    }

    getWorkOrders(event?: LazyLoadEvent) {
        if (this.primengTableHelper5.shouldResetPaging(event)) {
            this.paginator5.changePage(0);
            return;
        }

        this.primengTableHelper5.showLoadingIndicator();

        this._assetsServiceProxy.getAllWorkOrders(this.assetId,
            this.primengTableHelper5.getSkipCount(this.paginator5, event),
            this.primengTableHelper5.getMaxResultCount(this.paginator5, event),
            this.primengTableHelper5.getSorting(this.dataTable5)
        ).subscribe(result => {
            this.primengTableHelper5.totalRecordsCount = result.totalCount;
            this.primengTableHelper5.records = result.items;
            this.primengTableHelper5.hideLoadingIndicator();
        });
    }

    createAttachment(): void {
        this.createOrEditAttachmentModal.show(null, 'Asset', this.asset.id);
    }

    createUsageMetric(): void {
        this.createOrEditUsageMetricModal.show(false, this.assetId, null);
    }

    deleteUsageMetric(usageMetric: UsageMetricDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._usageMetricsServiceProxy.delete(usageMetric.id)
                        .subscribe(() => {
                            this.paginator4.changePage(this.paginator4.getPage())
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    createAssetNotes(): void {
        this.createOrEditAssetNotesModal.show(null, this.asset.id);
    }

    deleteAssetNotes(assetNote: AssetNotesDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._assetNotesServiceProxy.delete(assetNote.id)
                        .subscribe(() => {
                            this.paginator.changePage(this.paginator.getPage())
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
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

    viewWorkOrder(workOrderId): void {
        this._router.navigate(['app/main/support/workOrders/viewWorkOrder'], { queryParams: { workOrderId: workOrderId } });
    }

    reloadPage(): void {
        this.paginator1.changePage(this.paginator1.getPage())
    }

    close(): void {
        this._router.navigate(['app/main/assets/assets']);
    }
}
