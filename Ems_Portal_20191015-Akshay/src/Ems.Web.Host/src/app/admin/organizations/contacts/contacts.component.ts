import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ContactsServiceProxy, ContactDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditContactModalComponent } from './create-or-edit-contact-modal.component';
import { ViewContactModalComponent } from './view-contact-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './contacts.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ContactsComponent extends AppComponentBase {

    @ViewChild('createOrEditContactModal', { static: true }) createOrEditContactModal: CreateOrEditContactModalComponent;
    @ViewChild('viewContactModalComponent', { static: true }) viewContactModal: ViewContactModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    headOfficeContactFilter = -1;
    contactNameFilter = '';
    phoneOfficeFilter = '';
    phoneMobileFilter = '';
    faxFilter = '';
    emailAddressFilter = '';
    addressFilter = '';
    positionFilter = '';
    departmentFilter = '';
    contactLoc8GUIDFilter = '';
    userNameFilter = '';
    vendorNameFilter = '';
    assetOwnerNameFilter = '';
    customerNameFilter = '';


    _entityTypeFullName = 'Ems.Organizations.Contact';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getContacts(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._contactsServiceProxy.getAll(
            this.filterText,
            this.headOfficeContactFilter,
            this.contactNameFilter,
            this.phoneOfficeFilter,
            this.phoneMobileFilter,
            this.faxFilter,
            this.addressFilter,
            this.emailAddressFilter,
            this.positionFilter,
            this.departmentFilter,
            this.contactLoc8GUIDFilter,
            this.userNameFilter,
            this.vendorNameFilter,
            this.assetOwnerNameFilter,
            this.customerNameFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createContact(): void {
        this.createOrEditContactModal.show();
    }

    showHistory(contact: ContactDto): void {
        this.entityTypeHistoryModal.show({
            entityId: contact.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteContact(contact: ContactDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._contactsServiceProxy.delete(contact.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._contactsServiceProxy.getContactsToExcel(
            this.filterText,
            this.headOfficeContactFilter,
            this.contactNameFilter,
            this.phoneOfficeFilter,
            this.phoneMobileFilter,
            this.faxFilter,
            this.addressFilter,
            this.emailAddressFilter,
            this.positionFilter,
            this.departmentFilter,
            this.contactLoc8GUIDFilter,
            this.userNameFilter,
            this.vendorNameFilter,
            this.assetOwnerNameFilter,
            this.customerNameFilter,
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }
}
