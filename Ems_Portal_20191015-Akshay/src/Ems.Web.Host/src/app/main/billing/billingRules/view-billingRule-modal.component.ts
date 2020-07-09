import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetBillingRuleForViewDto, BillingRuleDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewBillingRuleModal',
    templateUrl: './view-billingRule-modal.component.html'
})
export class ViewBillingRuleModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetBillingRuleForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetBillingRuleForViewDto();
        this.item.billingRule = new BillingRuleDto();
    }

    show(item: GetBillingRuleForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
