import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { UsageMetricsServiceProxy, CreateOrEditUsageMetricDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
//import { UsageMetricLeaseItemLookupTableModalComponent } from './usageMetric-leaseItem-lookup-table-modal.component';
import { UsageMetricAssetLookupTableModalComponent } from './usageMetric-asset-lookup-table-modal.component';
import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';


@Component({
    selector: 'createOrEditUsageMetricModal',
    templateUrl: './create-or-edit-usageMetric-modal.component.html'
})
export class CreateOrEditUsageMetricModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    //@ViewChild('usageMetricLeaseItemLookupTableModal', { static: true }) usageMetricLeaseItemLookupTableModal: UsageMetricLeaseItemLookupTableModalComponent;
    @ViewChild('usageMetricAssetLookupTableModal', { static: true }) usageMetricAssetLookupTableModal: UsageMetricAssetLookupTableModalComponent;
    @ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    usageMetric: CreateOrEditUsageMetricDto = new CreateOrEditUsageMetricDto();

    //leaseItemItem = '';
    assetReference = '';
    showAsset: boolean;
    uomUnitOfMeasurement = '';


    constructor(
        injector: Injector,
        private _usageMetricsServiceProxy: UsageMetricsServiceProxy
    ) {
        super(injector);
        this.showAsset = false;
    }

    show(showAsset: boolean, assetId: number, usageMetricId?: number): void {
        this.showAsset = showAsset;

        if (!usageMetricId) {
            this.usageMetric = new CreateOrEditUsageMetricDto();
            this.usageMetric.id = usageMetricId;
            this.usageMetric.assetId = assetId;
            //this.leaseItemItem = '';
            this.assetReference = '';
            this.uomUnitOfMeasurement = '';

            this.active = true;
            this.modal.show();
        } else {
            this._usageMetricsServiceProxy.getUsageMetricForEdit(usageMetricId).subscribe(result => {
                this.usageMetric = result.usageMetric;

                //this.leaseItemItem = result.leaseItemItem;
                this.assetReference = result.assetReference;
                this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;

        this._usageMetricsServiceProxy.createOrEdit(this.usageMetric)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    //openSelectLeaseItemModal() {
    //    this.usageMetricLeaseItemLookupTableModal.id = this.usageMetric.leaseItemId;
    //    this.usageMetricLeaseItemLookupTableModal.displayName = this.leaseItemItem;
    //    this.usageMetricLeaseItemLookupTableModal.show();
    //}


    //setLeaseItemIdNull() {
    //    this.usageMetric.leaseItemId = null;
    //    this.leaseItemItem = '';
    //}


    //getNewLeaseItemId() {
    //    this.usageMetric.leaseItemId = this.usageMetricLeaseItemLookupTableModal.id;
    //    this.leaseItemItem = this.usageMetricLeaseItemLookupTableModal.displayName;
    //}

    openSelectAssetModal() {
        this.usageMetricAssetLookupTableModal.id = this.usageMetric.assetId;
        this.usageMetricAssetLookupTableModal.displayName = this.assetReference;
        this.usageMetricAssetLookupTableModal.show();
    }

    setAssetIdNull() {
        this.usageMetric.assetId = null;
        this.assetReference = '';
    }

    getNewAssetId() {
        this.usageMetric.assetId = this.usageMetricAssetLookupTableModal.id;
        this.assetReference = this.usageMetricAssetLookupTableModal.displayName;
    }

    openSelectUomModal() {
        this.uomLookupTableModal.id = this.usageMetric.uomId;
        this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
        this.uomLookupTableModal.show();
    }

    setUomIdNull() {
        this.usageMetric.uomId = null;
        this.uomUnitOfMeasurement = '';
    }

    getNewUomId() {
        this.usageMetric.uomId = this.uomLookupTableModal.id;
        this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    }

    close(): void {
        this.active = false;
        this.showAsset = false;
        this.modal.hide();
    }
}
