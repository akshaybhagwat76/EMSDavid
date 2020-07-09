import { Component, Injector, ViewEncapsulation, ViewChild, Output, EventEmitter } from '@angular/core';
import { UsageMetricRecordsServiceProxy, UsageMetricRecordDto, GetUsageMetricRecordForViewDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditUsageMetricRecordModalComponent } from './create-or-edit-usageMetricRecord-modal.component';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { ModalDirective } from 'ngx-bootstrap';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    selector: 'viewUsageMetricRecordModal',
    styleUrls: ['./view-usageMetricRecord-modal.component.less'],
    templateUrl: './view-usageMetricRecord-modal.component.html',
    encapsulation: ViewEncapsulation.None,
})

export class ViewUsageMetricRecordModalComponent extends AppComponentBase {

    @ViewChild('createOrEditUsageMetricRecordModal', { static: true }) createOrEditUsageMetricRecordModal: CreateOrEditUsageMetricRecordModalComponent;

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Output() modalClose: EventEmitter<any> = new EventEmitter<any>();

    usageMetricId: number;
    item: GetUsageMetricRecordForViewDto;

    constructor(
        injector: Injector,
        private _usageMetricRecordsServiceProxy: UsageMetricRecordsServiceProxy
    ) {
        super(injector);
        this.item = null;
        this.usageMetricId = 0;
    }

    show(usageMetricId: number): void {
        this.usageMetricId = usageMetricId;
        this.getUsageMetricRecord();

        this.modal.show();
    }

    getUsageMetricRecord(): void {
        this._usageMetricRecordsServiceProxy.getByUsageMetric(this.usageMetricId).subscribe(result => {
            this.item = result;
        });
    }

    createUsageMetricRecord(): void {
        this.createOrEditUsageMetricRecordModal.show(this.usageMetricId);
    }

    deleteUsageMetricRecord(usageMetricRecord: UsageMetricRecordDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._usageMetricRecordsServiceProxy.delete(usageMetricRecord.id)
                        .subscribe(() => {
                            this.item = new GetUsageMetricRecordForViewDto();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    close(): void {
        this.item = null;
        this.usageMetricId = 0;
        this.modal.hide();
        this.modalClose.emit(null);
    }
}
