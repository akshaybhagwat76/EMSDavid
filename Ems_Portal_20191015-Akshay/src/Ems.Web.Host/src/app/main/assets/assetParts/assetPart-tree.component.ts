import { OnInit, Component, EventEmitter, Injector, Output, ViewChild, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HtmlHelper } from '@shared/helpers/HtmlHelper';
import { ListResultDtoOfAssetPartExtendedDto, MoveAssetPartInput, AssetPartDto, AssetPartsServiceProxy, AssetPartExtendedDto, GetAssetPartForViewDto, AssetOwnershipAssetLookupTableDto } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { IBasicAssetPartInfo } from './basic-asset-part-info';
import { CreateOrEditAssetPartModalComponent } from './create-or-edit-assetPart-modal.component';
import { ViewAssetPartModalComponent } from './view-assetPart-modal.component';
import { TreeNode, MenuItem } from 'primeng/api';

import { ArrayToTreeConverterService } from '@shared/utils/array-to-tree-converter.service';
import { TreeDataHelperService } from '@shared/utils/tree-data-helper.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { stringify } from 'querystring';

export interface IAssetPartOnTree extends IBasicAssetPartInfo {
    id: number;
    parent: string | number;
    code: string;
    name: string;
    //memberCount: number;
    //roleCount: number;
    text: string;
    state: any;
}

@Component({
    selector: 'assetPart-tree',
    templateUrl: './assetPart-tree.component.html'
})
export class AssetPartTreeComponent extends AppComponentBase implements OnInit {

    @Input() assetId: number;

    @Output() apSelected = new EventEmitter<IBasicAssetPartInfo>();
    @Output() apUpdated = new EventEmitter<AssetPartExtendedDto>();

    @ViewChild('createOrEditAssetPartModal', {static: true}) createOrEditAssetPartModal: CreateOrEditAssetPartModalComponent;
    @ViewChild('viewAssetPartModalComponent', { static: true }) viewAssetPartModal: ViewAssetPartModalComponent;
    @ViewChild('entityTypeHistoryModal', {static: true}) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    treeData: any;
    selectedAp: TreeNode;
    apContextMenuItems: MenuItem[];
    canManageAssetParts = false;

    _entityTypeFullName = 'Abp.Assets.AssetPart';

    constructor(
        injector: Injector,
        private _assetPartsService: AssetPartsServiceProxy,
        private _arrayToTreeConverterService: ArrayToTreeConverterService,
        private _treeDataHelperService: TreeDataHelperService
    ) {
        super(injector);
    }

    totalPartCount = 0;

    ngOnInit(): void {
        this.canManageAssetParts = this.isGranted('Pages.Main.AssetParts.ManagePartTree');
        this.apContextMenuItems = this.getContextMenuItems();

        console.log("########### AssetId = " + this.assetId);
        this.getTreeDataFromServer();
    }

    nodeSelect(event) {

        var installed = "No";
        if(event.node.data.installed == true){ installed = "Yes" }
        this.apSelected.emit(<IBasicAssetPartInfo>{
            id: event.node.data.id,
            name: event.node.data.name,
            description: event.node.data.description,
            serialNumber: event.node.data.serialNumber,
            installDate: event.node.data.installDate,
            installed: installed,
            assetPartType: event.node.data.assetPartType,
            assetPartStatus: event.node.data.assetPartStatus,
            assetReference: event.node.data.assetReference,
            itemType: event.node.data.itemType,
            code: event.node.data.code
        });
    }

    nodeDrop(event) {
        this.message.confirm(
            this.l('AssetPartMoveConfirmMessage', event.dragNode.data.name, event.dropNode.data.name),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    const input = new MoveAssetPartInput();
                    input.id = event.dragNode.data.id;
                    input.newParentId = event.originalEvent.target.nodeName === 'SPAN' ? event.dropNode.data.id : event.dropNode.parent.data.id;

                    this._assetPartsService.moveAssetPart(input)
                        .pipe(catchError(error => {
                            this.revertDragDrop();
                            return Observable.throw(error);
                        }))
                        .subscribe(() => {
                            this.notify.success(this.l('SuccessfullyMoved'));
                            this.reload();
                        });
                } else {
                    this.revertDragDrop();
                }
            }
        );
    }

    revertDragDrop() {
        this.reload();
    }

    reload(): void {
        this.apSelected.emit(<IBasicAssetPartInfo>{
            assetId: this.assetId,
            id: 0,
            name: "",
            description: "",
            serialNumber: "",
            installDate: null,
            installed: "",
            assetPartType: "",
            assetPartStatus: "",
            assetReference: "",
            itemType: "",
            code: "",
        });
        this.getTreeDataFromServer();
    }

    private getTreeDataFromServer(): void {
        let self = this;
        this._assetPartsService.getAssetParts(this.assetId).subscribe((result: ListResultDtoOfAssetPartExtendedDto) => {
            this.totalPartCount = result.items.length;

            console.log("%%% RESULTS: " + result.items);
            
            this.treeData = this._arrayToTreeConverterService.createTree(result.items,
                'parentId',
                'id',
                null,
                'children',
                [
                    {
                        target: 'label',
                        targetFunction(item) {
                            return item.name;
                        }
                    }, {
                        target: 'expandedIcon',
                        value: 'fa fa-folder-open m--font-warning'
                    },
                    {
                        target: 'collapsedIcon',
                        value: 'fa fa-folder m--font-warning'
                    },
                    {
                        target: 'selectable',
                        value: true
                    }
                ]);
        });
    }

    private isEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    private getContextMenuItems(): any[] {

        const canManageAssetPartTree = this.isGranted('Pages.Main.AssetParts.ManagePartTree');

        let items = [
            {
                label: this.l('View'),
                disabled: null,
                command: (event) => {
                    console.log(this.selectedAp.data);
                    var assetPartDto = new GetAssetPartForViewDto();
                    assetPartDto.assetPart = new AssetPartDto();

                    assetPartDto.assetPart.assetId =  this.selectedAp.data.assetId;
                    assetPartDto.assetPart.code =  this.selectedAp.data.code;
                    assetPartDto.assetPart.description =  this.selectedAp.data.description;
                    assetPartDto.assetPart.id =  this.selectedAp.data.id;
                    assetPartDto.assetPart.installDate =  this.selectedAp.data.installDate;
                    assetPartDto.assetPart.installed =  this.selectedAp.data.installed;
                    assetPartDto.assetPart.name =  this.selectedAp.data.name;
                    assetPartDto.assetPart.parentId =  this.selectedAp.data.parentId;
                    assetPartDto.assetPart.serialNumber =  this.selectedAp.data.serialNumber;

                    assetPartDto.assetPartStatusStatus = this.selectedAp.data.assetPartStatus;
                    assetPartDto.assetPartTypeType = this.selectedAp.data.assetPartType;
                    assetPartDto.itemTypeType = this.selectedAp.data.itemType;
                    assetPartDto.assetReference = this.selectedAp.data.assetReference;

                    this.viewAssetPartModal.show(
                        assetPartDto //    <--- this needs to be a GetAssetPartForViewDto, rather than AssetPartExtendedDto 
                    );
                }
            },
            {
                label: this.l('Edit'),
                disabled: !canManageAssetPartTree,
                command: (event) => {
                    this.createOrEditAssetPartModal.show({
                        id: this.selectedAp.data.id,
                        name: this.selectedAp.data.name
                    });
                }
            },
            {
                label: this.l('AddComponent'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.addPart(this.selectedAp.data.id);
                }
            },
            {
                label: this.l('Delete'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.message.confirm(
                        this.l('AssetPartDeleteWarningMessage', this.selectedAp.data.name),
                        this.l('AreYouSure'),
                        isConfirmed => {
                            if (isConfirmed) {
                                this._assetPartsService.delete(this.selectedAp.data.id).subscribe(() => {
                                    this.deletePart(this.selectedAp.data.id);
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                    this.selectedAp = null;
                                    this.reload();
                                });
                            }
                        }
                    );
                }
            }
        ];

        if (this.isEntityHistoryEnabled()) {
            items.push({
                label: this.l('History'),
                disabled: false,
                command: (event) => {
                    this.entityTypeHistoryModal.show({
                        entityId: this.selectedAp.data.id.toString(),
                        entityTypeFullName: this._entityTypeFullName,
                        entityTypeDescription: this.selectedAp.data.name
                    });
                }
            });
        }

        return items;
    }

    addPart(parentId?: number): void {
        this.createOrEditAssetPartModal.show({
            parentId: parentId
        });
    }

    partCreated(ap: AssetPartDto): void {

        console.log("###############");
        console.log("#### PartCreated:");
        console.log(ap);
        if (ap.parentId) {
            let part = this._treeDataHelperService.findNode(this.treeData, { data: { id: ap.parentId } });
            if (!part) {
                return;
            }

            part.children.push({
                label: ap.name,
                expandedIcon: 'fa fa-folder-open m--font-warning',
                collapsedIcon: 'fa fa-folder m--font-warning',
                selected: true,
                children: [],
                data: ap
            });
        } else {
            this.treeData.push({
                label: ap.name,
                expandedIcon: 'fa fa-folder-open m--font-warning',
                collapsedIcon: 'fa fa-folder m--font-warning',
                selected: true,
                children: [],
                data: ap
            });
        }
        this.totalPartCount += 1;
    }

    deletePart(id) {
        let node = this._treeDataHelperService.findNode(this.treeData, { data: { id: id } });
        if (!node) {
            return;
        }

        if (!node.data.parentId) {
            _.remove(this.treeData, {
                data: {
                    id: id
                }
            });
        }

        let parentNode = this._treeDataHelperService.findNode(this.treeData, { data: { id: node.data.parentId } });
        if (!parentNode) {
            return;
        }

        _.remove(parentNode.children, {
            data: {
                id: id
            }
        });
    }

    partUpdated(ap: AssetPartExtendedDto): void {
        let item = this._treeDataHelperService.findNode(this.treeData, { data: { id: ap.id } });
        if (!item) {
            return;
        }

        item.data.name = ap.name;
        item.data.description = ap.description,
        item.data.serialNumber = ap.serialNumber,
        item.data.installDate = ap.installDate,
        item.data.installed = ap.installed,
        item.data.assetPartType = ap.assetPartType,
        item.data.assetPartStatus = ap.assetPartStatus,
        item.data.assetReference = ap.assetReference,
        item.data.itemType = ap.itemType,
        item.data.code = ap.code,
        item.label = ap.name;

        this.apUpdated.emit(ap);

        console.log(ap);
    }

}
