import { Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { SupportContractsServiceProxy, CreateOrEditSupportContractDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { SupportContractVendorLookupTableModalComponent } from './supportContract-vendor-lookup-table-modal.component';
import { SupportContractAssetOwnerLookupTableModalComponent } from './supportContract-assetOwner-lookup-table-modal.component';

import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { TokenService } from '@abp/auth/token.service';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse } from '@abp/abpHttpInterceptor';


@Component({
    selector: 'createOrEditSupportContractModal',
    templateUrl: './create-or-edit-supportContract-modal.component.html'
})
export class CreateOrEditSupportContractModalComponent extends AppComponentBase {
    
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('supportContractVendorLookupTableModal', { static: true }) supportContractVendorLookupTableModal: SupportContractVendorLookupTableModalComponent;
    @ViewChild('supportContractAssetOwnerLookupTableModal', { static: true }) supportContractAssetOwnerLookupTableModal: SupportContractAssetOwnerLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    supportContract: CreateOrEditSupportContractDto = new CreateOrEditSupportContractDto();

    vendorName = '';
    assetOwnerName = '';

    attachmentUploader: FileUploader;
    remoteServiceBaseUrl = AppConsts.remoteServiceBaseUrl;

    constructor(
        injector: Injector,
        private _supportContractsServiceProxy: SupportContractsServiceProxy,
        private _tokenService: TokenService
    ) {
        super(injector);
    }

    initializeModal(): void {
        this.initUploaders();
    }

    initUploaders(): void {
        this.attachmentUploader = this.createUploader(
            '/Attachments/UploadAttachment',
            result => {
                //do the response stuff here
            }
        );
    }

    createUploader(url: string, success?: (result: any) => void): FileUploader {
        const uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + url });

        uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        uploader.onSuccessItem = (item, response, status) => {
            const ajaxResponse = <IAjaxResponse>JSON.parse(response);
            if (ajaxResponse.success) {
                this.notify.info(this.l('SavedSuccessfully'));
                if (success) {
                    success(ajaxResponse.result);
                }
            } else {
                this.message.error(ajaxResponse.error.message);
            }
        };

        const uploaderOptions: FileUploaderOptions = {};
        uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        uploaderOptions.removeAfterUpload = true;
        uploader.setOptions(uploaderOptions);
        return uploader;
    }

    uploadAttachment(): void {
        this.attachmentUploader.uploadAll();
    }


    show(supportContractId?: number): void {
        this.initializeModal();

        if (!supportContractId) {
            this.supportContract = new CreateOrEditSupportContractDto();
            this.supportContract.id = supportContractId;
            this.supportContract.startDate = moment().startOf('day');
            this.supportContract.endDate = moment().startOf('day');
            this.supportContract.acknowledgedAt = moment().startOf('day');
            this.vendorName = '';
            this.assetOwnerName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._supportContractsServiceProxy.getSupportContractForEdit(supportContractId).subscribe(result => {
                this.supportContract = result.supportContract;

                this.vendorName = result.vendorName;
                this.assetOwnerName = result.assetOwnerName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;
            this.uploadAttachment();
            
            this._supportContractsServiceProxy.createOrEdit(this.supportContract)
            .pipe(finalize(() => { this.saving = false;}))
            .subscribe(() => {
               this.notify.info(this.l('SavedSuccessfully'));
               this.close();
               this.modalSave.emit(null);
              });
    }

        openSelectVendorModal() {
        this.supportContractVendorLookupTableModal.id = this.supportContract.vendorId;
        this.supportContractVendorLookupTableModal.displayName = this.vendorName;
        this.supportContractVendorLookupTableModal.show();
    }
        openSelectAssetOwnerModal() {
        this.supportContractAssetOwnerLookupTableModal.id = this.supportContract.assetOwnerId;
        this.supportContractAssetOwnerLookupTableModal.displayName = this.assetOwnerName;
        this.supportContractAssetOwnerLookupTableModal.show();
    }


        setVendorIdNull() {
        this.supportContract.vendorId = null;
        this.vendorName = '';
    }
        setAssetOwnerIdNull() {
        this.supportContract.assetOwnerId = null;
        this.assetOwnerName = '';
    }


        getNewVendorId() {
        this.supportContract.vendorId = this.supportContractVendorLookupTableModal.id;
        this.vendorName = this.supportContractVendorLookupTableModal.displayName;
    }
        getNewAssetOwnerId() {
        this.supportContract.assetOwnerId = this.supportContractAssetOwnerLookupTableModal.id;
        this.assetOwnerName = this.supportContractAssetOwnerLookupTableModal.displayName;
    }


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
