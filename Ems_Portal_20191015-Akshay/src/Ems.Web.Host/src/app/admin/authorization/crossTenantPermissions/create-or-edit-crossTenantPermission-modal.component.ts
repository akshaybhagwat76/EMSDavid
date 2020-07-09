import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CrossTenantPermissionsServiceProxy, CreateOrEditCrossTenantPermissionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCrossTenantPermissionModal',
    templateUrl: './create-or-edit-crossTenantPermission-modal.component.html'
})
export class CreateOrEditCrossTenantPermissionModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    crossTenantPermission: CreateOrEditCrossTenantPermissionDto = new CreateOrEditCrossTenantPermissionDto();



    constructor(
        injector: Injector,
        private _crossTenantPermissionsServiceProxy: CrossTenantPermissionsServiceProxy
    ) {
        super(injector);
    }

    show(crossTenantPermissionId?: number): void {

        if (!crossTenantPermissionId) {
            this.crossTenantPermission = new CreateOrEditCrossTenantPermissionDto();
            this.crossTenantPermission.id = crossTenantPermissionId;

            this.active = true;
            this.modal.show();
        } else {
            this._crossTenantPermissionsServiceProxy.getCrossTenantPermissionForEdit(crossTenantPermissionId).subscribe(result => {
                this.crossTenantPermission = result.crossTenantPermission;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;

			
            this._crossTenantPermissionsServiceProxy.createOrEdit(this.crossTenantPermission)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {

        this.active = false;
        this.modal.hide();
    }
}
