import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetQuotationDetailForViewDto, QuotationDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewQuotationDetailModal',
    templateUrl: './view-quotationDetail-modal.component.html'
})
export class ViewQuotationDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetQuotationDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetQuotationDetailForViewDto();
        this.item.quotationDetail = new QuotationDetailDto();
    }

    show(item: GetQuotationDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
