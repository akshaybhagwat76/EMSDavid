import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetBillingRuleTypeForViewDto, BillingRuleTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewBillingRuleTypeModal',
    templateUrl: './view-billingRuleType-modal.component.html'
})
export class ViewBillingRuleTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetBillingRuleTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetBillingRuleTypeForViewDto();
        this.item.billingRuleType = new BillingRuleTypeDto();
    }

    show(item: GetBillingRuleTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
