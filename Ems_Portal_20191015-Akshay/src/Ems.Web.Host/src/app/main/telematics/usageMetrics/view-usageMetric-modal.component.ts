import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetUsageMetricForViewDto, UsageMetricDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewUsageMetricModal',
    templateUrl: './view-usageMetric-modal.component.html' 
})
export class ViewUsageMetricModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetUsageMetricForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetUsageMetricForViewDto();
        this.item.usageMetric = new UsageMetricDto();
    }

    show(item: GetUsageMetricForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
