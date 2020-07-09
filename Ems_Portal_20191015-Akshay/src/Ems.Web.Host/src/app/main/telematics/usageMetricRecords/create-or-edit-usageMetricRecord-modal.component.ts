import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { UsageMetricRecordsServiceProxy, UsageMetricsServiceProxy, CreateOrEditUsageMetricRecordDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { environment } from 'environments/environment';


@Component({
    selector: 'createOrEditUsageMetricRecordModal',
    templateUrl: './create-or-edit-usageMetricRecord-modal.component.html'
})
export class CreateOrEditUsageMetricRecordModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    usageMetricRecord: CreateOrEditUsageMetricRecordDto = new CreateOrEditUsageMetricRecordDto();

    startTime: Date;
    endTime: Date;
    usageMetricMetric = '';


    constructor(
        injector: Injector,
        private _usageMetricRecordsServiceProxy: UsageMetricRecordsServiceProxy,
        private _usageMetricsServiceProxy: UsageMetricsServiceProxy
    ) {
        super(injector);
    }

    show(usageMetricId: number, usageMetricRecordId?: number): void {
        this.startTime = null;
        this.endTime = null;

        this._usageMetricsServiceProxy.getUsageMetricForView(usageMetricId).subscribe(result => {
            this.usageMetricMetric = result.usageMetric.metric;
        });

        if (!usageMetricRecordId) {
            this.usageMetricRecord = new CreateOrEditUsageMetricRecordDto();
            this.usageMetricRecord.id = usageMetricRecordId;
            this.usageMetricRecord.usageMetricId = usageMetricId;
            //this.uomUnitOfMeasurement = '';
            //this.usageMetricMetric = '';

            this.active = true;
            this.modal.show();
        } else {
            this._usageMetricRecordsServiceProxy.getUsageMetricRecordForEdit(usageMetricRecordId).subscribe(result => {
                this.usageMetricRecord = result.usageMetricRecord;

                if (this.usageMetricRecord.startTime) {
                    this.startTime = this.usageMetricRecord.startTime.toDate();
                }
                if (this.usageMetricRecord.endTime) {
                    this.endTime = this.usageMetricRecord.endTime.toDate();
                }
                //this.uomUnitOfMeasurement = result.uomUnitOfMeasurement;
                //this.usageMetricMetric = result.usageMetricMetric;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;


        if (this.startTime) {
            if (!this.usageMetricRecord.startTime) {
                this.usageMetricRecord.startTime = moment(this.startTime).startOf('day');
            }
            else {
                this.usageMetricRecord.startTime = moment(this.startTime);
            }
        }
        else {
            this.usageMetricRecord.startTime = null;
        }
        if (this.endTime) {
            if (!this.usageMetricRecord.endTime) {
                this.usageMetricRecord.endTime = moment(this.endTime).startOf('day');
            }
            else {
                this.usageMetricRecord.endTime = moment(this.endTime);
            }
        }
        else {
            this.usageMetricRecord.endTime = null;
        }
        this._usageMetricRecordsServiceProxy.createOrEdit(this.usageMetricRecord)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            },
            () => {
                this.saving = false;
            }
        );
    }

    //openSelectUomModal() {
    //    this.uomLookupTableModal.id = this.usageMetricRecord.uomId;
    //    this.uomLookupTableModal.displayName = this.uomUnitOfMeasurement;
    //    this.uomLookupTableModal.show();
    //}
    //openSelectUsageMetricModal() {
    //    this.usageMetricRecordUsageMetricLookupTableModal.id = this.usageMetricRecord.usageMetricId;
    //    this.usageMetricRecordUsageMetricLookupTableModal.displayName = this.usageMetricMetric;
    //    this.usageMetricRecordUsageMetricLookupTableModal.show();
    //}


    //setUomIdNull() {
    //    this.usageMetricRecord.uomId = null;
    //    this.uomUnitOfMeasurement = '';
    //}
    //setUsageMetricIdNull() {
    //    this.usageMetricRecord.usageMetricId = null;
    //    this.usageMetricMetric = '';
    //}


    //getNewUomId() {
    //    this.usageMetricRecord.uomId = this.uomLookupTableModal.id;
    //    this.uomUnitOfMeasurement = this.uomLookupTableModal.displayName;
    //}
    //getNewUsageMetricId() {
    //    this.usageMetricRecord.usageMetricId = this.usageMetricRecordUsageMetricLookupTableModal.id;
    //    this.usageMetricMetric = this.usageMetricRecordUsageMetricLookupTableModal.displayName;
    //}


    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
