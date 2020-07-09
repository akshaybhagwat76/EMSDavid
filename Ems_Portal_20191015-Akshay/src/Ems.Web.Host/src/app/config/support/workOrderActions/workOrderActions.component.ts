import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { WorkOrderActionsServiceProxy, WorkOrderActionDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditWorkOrderActionModalComponent } from './create-or-edit-workOrderAction-modal.component';
import { ViewWorkOrderActionModalComponent } from './view-workOrderAction-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './workOrderActions.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class WorkOrderActionsComponent extends AppComponentBase {

    @ViewChild('createOrEditWorkOrderActionModal', { static: true }) createOrEditWorkOrderActionModal: CreateOrEditWorkOrderActionModalComponent;
    @ViewChild('viewWorkOrderActionModal', { static: true }) viewWorkOrderActionModal: ViewWorkOrderActionModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    actionFilter = '';
    descriptionFilter = '';

    constructor(
        injector: Injector,
        private _workOrderActionsServiceProxy: WorkOrderActionsServiceProxy,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getWorkOrderActions(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._workOrderActionsServiceProxy.getAll(
            this.filterText,
            this.actionFilter,
            this.descriptionFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
            }
        );
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createWorkOrderAction(): void {
        this.createOrEditWorkOrderActionModal.show();
    }

    deleteWorkOrderAction(workOrderAction: WorkOrderActionDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._workOrderActionsServiceProxy.delete(workOrderAction.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._workOrderActionsServiceProxy.getWorkOrderActionsToExcel(
        this.filterText,
            this.actionFilter,
            this.descriptionFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
