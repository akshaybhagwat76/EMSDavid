import { environment } from 'environments/environment';
import { HttpClient, HttpHeaders, HttpClientModule } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { CreateOrEditCustomerInvoiceDetailModalComponent } from './create-or-edit-customerInvoiceDetail-modal.component';
import { ViewCustomerInvoiceDetailModalComponent } from './view-customerInvoiceDetail-modal.component';
import { CustomerInvoicesServiceProxy, CustomerInvoiceDto, EstimateDto, WorkOrderDto, CustomerInvoiceDetailsServiceProxy, CustomerInvoiceDetailDto, GetCustomerInvoiceForViewDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCustomerInvoiceModalComponent } from './create-or-edit-customerInvoice-modal.component';
import * as moment from 'moment';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { AppConsts } from 'shared/AppConsts';

@Component({
    selector: 'app-invoice-pdf',
    templateUrl: './invoice-pdf.component.html',
    styleUrls: ['./invoice-pdf.component.css']
})
export class InvoicePDFComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _customerInvoiceServiceProxy: CustomerInvoicesServiceProxy,
        private httpClient: HttpClient,
        private sanitization: DomSanitizer
    ) {
        super(injector);
    }

    active = false;
    saving = false;
    note: string;

    customerInvoiceId: number;
    ePdfInfo: GetCustomerInvoiceForViewDto;
    username: string;
    currentDate = new Date();
    refID: string;
    aoContactString: string;
    aelLogo: string;
    isPDF = false;
    statusFlag: string;

    ngOnInit(): void {
        this.customerInvoiceId = this._activatedRoute.snapshot.queryParams['invoiceId'];
        this.statusFlag = this._activatedRoute.snapshot.queryParams['flag'];
        this.isPDF = this.statusFlag == AppConsts.PDF ? true : false;
        this.ePdfInfo = new GetCustomerInvoiceForViewDto();
        this.get();
        this.username = this.appSession.user.userName.trim();
        this.aelLogo = this.appRootUrl() + 'assets/common/images/aelLogo.jpg';
        this.note = this.l('AuthorizedNote')
    }

    getSanitizedHtml() {
        return this.sanitization.bypassSecurityTrustHtml(`<title>Invoice</title> <style> @page { size: A4; margin: 10mm 10mm 10mm 10mm; } </style>`);
    }

    get(): void {
        this._customerInvoiceServiceProxy.getCustomerInvoiceForPDF(this.customerInvoiceId)
            .subscribe((pdfResult) => {
                this.ePdfInfo = pdfResult;

                if (this.ePdfInfo && this.ePdfInfo.customerInvoice && this.ePdfInfo.customerInvoice.id > 0)
                    this.refID = 'INV-' + this.padNum(this.ePdfInfo.customerInvoice.id.toString());
                else
                    this.refID = '';
            });
    }

    base64ToArrayBuffer(base64: any): ArrayBuffer {
        var binary_string = window.atob(base64);
        var len = binary_string.length;
        var bytes = new Uint8Array(len);
        for (var i = 0; i < len; i++) {
            bytes[i] = binary_string.charCodeAt(i);
        }
        return bytes.buffer;
    }

    padNum(num) {
        let size = 6;
        var s = num + "";
        while (s.length < size) s = "0" + s;
        return s;
    }

    generatePDF(): void {
        if (this.ePdfInfo && this.ePdfInfo.authenticationKey && this.customerInvoiceId > 0) {
            this.saving = true;
            let apiEndPoint = environment.emsPdfApiUrl + environment.invoiceCreatePdfEndPoint + "?InvoiceID=" + this.customerInvoiceId + "&Username=" + this.username + "&Environment=" + "Azure-Dev";
            const headerDict = new HttpHeaders().set("Authorization_Key", this.ePdfInfo.authenticationKey).set("Content-Type", "application/json").set("Environment", "Azure-Dev");

            this.httpClient.post(apiEndPoint, null, {
                headers: headerDict
            }).subscribe(
                data => {
                    this.saving = false;
                    var res = JSON.parse(JSON.stringify(data));
                    if (res.StatusCode === 200) {
                        let mailCnt = res.Content;
                        let mailText = "mailto:" + mailCnt.ToEmail + "?subject=" + mailCnt.Subject + "&body=" + mailCnt.Body;
                        window.location.href = mailText;
                    }
                    else {
                        if (res.Message)
                            this.message.info(res.Message);
                        else
                            this.message.info(this.l('ErrorFailEmail'));
                    }
                },
                err => {
                    this.saving = false;

                    if (err.message)
                        this.message.info(err.message);
                    else
                        this.message.info(this.l('UnknownFailEmail'));
                }
            );
        }
        else {
            this.message.info(this.l('InsufficientDetails'));
        }
    }

    xeroConnection(flag): void {
        this.saving = true;

        this._customerInvoiceServiceProxy.xeroCommunication(this.customerInvoiceId, flag)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(
                (data: any) => {
                    if (data.result == AppConsts.Submitted) {
                        this.notify.info(this.l('InvoiceSubmittedSuccessfully'));
                        this.generatePDF();
                        this._router.navigate(['app/main/billing/customerInvoices']);
                    } else if (data.result == AppConsts.Details) {
                        this.notify.info(this.l('InvoiceDetailNotExist'));
                    } else if (data.result == AppConsts.XeroConatctMessage) {
                        this.notify.info(this.l('XeroConatctMessage'));
                    } else if (data.result == AppConsts.Error) {
                        this.notify.info(this.l('Somethingwentwrongmsg'));
                    }
                    else {
                        location.href = data.result;
                    }
                },
                error => {
                    this.notify.info(this.l('Somethingwentwrongmsg'));
                });
    }

}
