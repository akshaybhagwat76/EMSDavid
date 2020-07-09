import { Component, ViewChild, Injector, Output, EventEmitter, Input} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetPartsServiceProxy, CreateOrEditAssetPartDto, AssetPartExtendedDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AssetPartAssetPartTypeLookupTableModalComponent } from './assetPart-assetPartType-lookup-table-modal.component';
import { AssetPartAssetPartLookupTableModalComponent } from './assetPart-assetPart-lookup-table-modal.component';
import { AssetPartAssetPartStatusLookupTableModalComponent } from './assetPart-assetPartStatus-lookup-table-modal.component';
import { AssetPartUsageMetricLookupTableModalComponent } from './assetPart-usageMetric-lookup-table-modal.component';
import { AssetPartAttachmentLookupTableModalComponent } from './assetPart-attachment-lookup-table-modal.component';
import { AssetPartAssetLookupTableModalComponent } from './assetPart-asset-lookup-table-modal.component';
import { AssetPartItemTypeLookupTableModalComponent } from './assetPart-itemType-lookup-table-modal.component';

export interface IAssetPartOnEdit {
    id?: number;
    parentId?: number;
    name?: string;
}

@Component({
    selector: 'createOrEditAssetPartModal',
    templateUrl: './create-or-edit-assetPart-modal.component.html'
})
export class CreateOrEditAssetPartModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('assetPartAssetPartTypeLookupTableModal', { static: true }) assetPartAssetPartTypeLookupTableModal: AssetPartAssetPartTypeLookupTableModalComponent;
    @ViewChild('assetPartAssetPartLookupTableModal', { static: true }) assetPartAssetPartLookupTableModal: AssetPartAssetPartLookupTableModalComponent;
    @ViewChild('assetPartAssetPartStatusLookupTableModal', { static: true }) assetPartAssetPartStatusLookupTableModal: AssetPartAssetPartStatusLookupTableModalComponent;
    @ViewChild('assetPartUsageMetricLookupTableModal', { static: true }) assetPartUsageMetricLookupTableModal: AssetPartUsageMetricLookupTableModalComponent;
    @ViewChild('assetPartAttachmentLookupTableModal', { static: true }) assetPartAttachmentLookupTableModal: AssetPartAttachmentLookupTableModalComponent;
    @ViewChild('assetPartAssetLookupTableModal', { static: true }) assetPartAssetLookupTableModal: AssetPartAssetLookupTableModalComponent;
    @ViewChild('assetPartItemTypeLookupTableModal', { static: true }) assetPartItemTypeLookupTableModal: AssetPartItemTypeLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() partUpdated: EventEmitter<any> = new EventEmitter<any>();
    @Output() partCreated: EventEmitter<any> = new EventEmitter<any>();

    @Input() assetId: number;

    active = false;
    saving = false;

    assetPartOnEdit: IAssetPartOnEdit = {};

    assetPart: CreateOrEditAssetPartDto = new CreateOrEditAssetPartDto();

    installDate: Date;
    assetPartTypeType = '';
    assetPartName = '';
    assetPartStatusStatus = '';
    usageMetricMetric = '';
    attachmentFilename = '';
    assetReference = '';
    itemTypeType = '';

    constructor(
        injector: Injector,
        private _assetPartsServiceProxy: AssetPartsServiceProxy
    ) {
        super(injector);
    }

    show(assetPartOnEdit: IAssetPartOnEdit): void {
    
        this.installDate = null;
        this.assetPart.assetId = this.assetId;

        if ( !assetPartOnEdit.id) {
            this.assetPart = new CreateOrEditAssetPartDto();
            this.assetPart.parentId = assetPartOnEdit.parentId;
            this.assetPart.id = assetPartOnEdit.id;
            this.assetPartTypeType = '';
            this.assetPartName = '';
            this.assetPartStatusStatus = '';
            this.usageMetricMetric = '';
            this.attachmentFilename = '';
            this.assetReference = '';
            this.itemTypeType = '';

            this.active = true;
            this.modal.show();
        } else {
            this._assetPartsServiceProxy.getAssetPartForEdit(assetPartOnEdit.id).subscribe(result => {
                this.assetPart = result.assetPart;

                if (this.assetPart.installDate) {
					this.installDate = this.assetPart.installDate.toDate();
                }
                this.assetPartTypeType = result.assetPartTypeType;
                this.assetPartName = result.assetPartName;
                this.assetPartStatusStatus = result.assetPartStatusStatus;
                this.usageMetricMetric = result.usageMetricMetric;
                this.attachmentFilename = result.attachmentFilename;
                this.assetReference = result.assetReference;
                this.itemTypeType = result.itemTypeType;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
        this.saving = true;
        this.assetPart.assetId = this.assetId;

        if (this.installDate) {
            if (!this.assetPart.installDate) {
                this.assetPart.installDate = moment(this.installDate).startOf('day');
            }
            else {
                this.assetPart.installDate = moment(this.installDate);
            }
        }
        else {
            this.assetPart.installDate = null;
        }
            this._assetPartsServiceProxy.createOrEdit(this.assetPart)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe((result: CreateOrEditAssetPartDto) => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();

                if(!this.assetPart.id) {
                    this.assetPart.id = result.id;
                    this.partCreated.emit(this.assetPart)
                }
                else{

                    var assetPartExtendedDto = new AssetPartExtendedDto();

                    assetPartExtendedDto.assetPartStatus = this.assetPartStatusStatus;
                    assetPartExtendedDto.assetPartType = this.assetPartTypeType;
                    assetPartExtendedDto.assetReference = this.assetReference;
                    assetPartExtendedDto.itemType = this.itemTypeType;
                    assetPartExtendedDto.name = this.assetPart.name;
                    assetPartExtendedDto.description = this.assetPart.description;
                    assetPartExtendedDto.serialNumber = this.assetPart.serialNumber;
                    assetPartExtendedDto.installDate = this.assetPart.installDate;
                    assetPartExtendedDto.code = this.assetPart.code;
                    assetPartExtendedDto.installed = this.assetPart.installed;
                    assetPartExtendedDto.id = this.assetPart.id;
                    
                    this.partUpdated.emit(assetPartExtendedDto)
                }
                this.modalSave.emit(null);
             });
    }

    openSelectAssetPartTypeModal() {
        this.assetPartAssetPartTypeLookupTableModal.id = this.assetPart.assetPartTypeId;
        this.assetPartAssetPartTypeLookupTableModal.displayName = this.assetPartTypeType;
        this.assetPartAssetPartTypeLookupTableModal.show();
    }
    openSelectAssetPartModal() {
        this.assetPartAssetPartLookupTableModal.id = this.assetPart.parentId;
        this.assetPartAssetPartLookupTableModal.displayName = this.assetPartName;
        this.assetPartAssetPartLookupTableModal.show();
    }
    openSelectAssetPartStatusModal() {
        this.assetPartAssetPartStatusLookupTableModal.id = this.assetPart.assetPartStatusId;
        this.assetPartAssetPartStatusLookupTableModal.displayName = this.assetPartStatusStatus;
        this.assetPartAssetPartStatusLookupTableModal.show();
    }
    openSelectUsageMetricModal() {
        this.assetPartUsageMetricLookupTableModal.id = this.assetPart.usageMetricId;
        this.assetPartUsageMetricLookupTableModal.displayName = this.usageMetricMetric;
        this.assetPartUsageMetricLookupTableModal.show();
    }
    openSelectAttachmentModal() {
        this.assetPartAttachmentLookupTableModal.id = this.assetPart.attachmentId;
        this.assetPartAttachmentLookupTableModal.displayName = this.attachmentFilename;
        this.assetPartAttachmentLookupTableModal.show();
    }
    openSelectAssetModal() {
        this.assetPartAssetLookupTableModal.id = this.assetPart.assetId;
        this.assetPartAssetLookupTableModal.displayName = this.assetReference;
        this.assetPartAssetLookupTableModal.show();
    }
    openSelectItemTypeModal() {
        this.assetPartItemTypeLookupTableModal.id = this.assetPart.itemTypeId;
        this.assetPartItemTypeLookupTableModal.displayName = this.itemTypeType;
        this.assetPartItemTypeLookupTableModal.show();
    }


    setAssetPartTypeIdNull() {
        this.assetPart.assetPartTypeId = null;
        this.assetPartTypeType = '';
    }
    setParentIdNull() {
        this.assetPart.parentId = null;
        this.assetPartName = '';
    }
    setAssetPartStatusIdNull() {
        this.assetPart.assetPartStatusId = null;
        this.assetPartStatusStatus = '';
    }
    setUsageMetricIdNull() {
        this.assetPart.usageMetricId = null;
        this.usageMetricMetric = '';
    }
    setAttachmentIdNull() {
        this.assetPart.attachmentId = null;
        this.attachmentFilename = '';
    }
    setAssetIdNull() {
        this.assetPart.assetId = null;
        this.assetReference = '';
    }
    setItemTypeIdNull() {
        this.assetPart.itemTypeId = null;
        this.itemTypeType = '';
    }


    getNewAssetPartTypeId() {
        this.assetPart.assetPartTypeId = this.assetPartAssetPartTypeLookupTableModal.id;
        this.assetPartTypeType = this.assetPartAssetPartTypeLookupTableModal.displayName;
    }
    getNewParentId() {
        this.assetPart.parentId = this.assetPartAssetPartLookupTableModal.id;
        this.assetPartName = this.assetPartAssetPartLookupTableModal.displayName;
    }
    getNewAssetPartStatusId() {
        this.assetPart.assetPartStatusId = this.assetPartAssetPartStatusLookupTableModal.id;
        this.assetPartStatusStatus = this.assetPartAssetPartStatusLookupTableModal.displayName;
    }
    getNewUsageMetricId() {
        this.assetPart.usageMetricId = this.assetPartUsageMetricLookupTableModal.id;
        this.usageMetricMetric = this.assetPartUsageMetricLookupTableModal.displayName;
    }
    getNewAttachmentId() {
        this.assetPart.attachmentId = this.assetPartAttachmentLookupTableModal.id;
        this.attachmentFilename = this.assetPartAttachmentLookupTableModal.displayName;
    }
    getNewAssetId() {
        this.assetPart.assetId = this.assetPartAssetLookupTableModal.id;
        this.assetReference = this.assetPartAssetLookupTableModal.displayName;
    }
    getNewItemTypeId() {
        this.assetPart.itemTypeId = this.assetPartItemTypeLookupTableModal.id;
        this.itemTypeType = this.assetPartItemTypeLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
