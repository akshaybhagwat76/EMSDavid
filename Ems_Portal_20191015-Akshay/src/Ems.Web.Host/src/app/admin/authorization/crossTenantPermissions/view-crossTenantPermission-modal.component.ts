import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetCrossTenantPermissionForViewDto, CrossTenantPermissionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCrossTenantPermissionModal',
    templateUrl: './view-crossTenantPermission-modal.component.html'
})
export class ViewCrossTenantPermissionModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCrossTenantPermissionForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCrossTenantPermissionForViewDto();
        this.item.crossTenantPermission = new CrossTenantPermissionDto();
    }

    show(item: GetCrossTenantPermissionForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
