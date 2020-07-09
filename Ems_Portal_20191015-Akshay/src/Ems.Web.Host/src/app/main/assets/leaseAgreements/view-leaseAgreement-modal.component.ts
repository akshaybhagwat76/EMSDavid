import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetLeaseAgreementForViewDto, LeaseAgreementDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewLeaseAgreementModal',
    templateUrl: './view-leaseAgreement-modal.component.html'
})
export class ViewLeaseAgreementModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetLeaseAgreementForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetLeaseAgreementForViewDto();
        this.item.leaseAgreement = new LeaseAgreementDto();
    }

    show(item: GetLeaseAgreementForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
