import { Component, Injector, ChangeDetectorRef, ElementRef } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash';
import { QuotationDetailsServiceProxy, QuotationPdfDto } from '@shared/service-proxies/service-proxies';
import { environment } from 'environments/environment';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'quotationPdf',
    templateUrl: './quotation-pdf.component.html',
    animations: [appModuleAnimation()]
})
export class QuotationPdfComponent extends AppComponentBase {

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _quotationDetailsServiceProxy: QuotationDetailsServiceProxy,
        private httpClient: HttpClient,
        //private pdfHtmlElement: ElementRef,
        private changeDetector: ChangeDetectorRef,
        private sanitization: DomSanitizer
    ) {
        super(injector);
    }

    active = false;
    saving = false;

    quotationId: number;
    qPdfInfo: QuotationPdfDto;
    username: string;
    currentDate = new Date();
    refID: string;
    vendorContactString: string;
    aelLogo: string;

    ngOnInit(): void {
        this.quotationId = this._activatedRoute.snapshot.queryParams['quotationId'];

        this.get();
        this.username = this.appSession.user.userName.trim();
        this.aelLogo = this.appRootUrl() + 'assets/common/images/aelLogo.jpg';
    }

    getSanitizedHtml() {
        return this.sanitization.bypassSecurityTrustHtml(`<title>Quotation</title> <style> @page { size: A4; margin: 10mm 10mm 10mm 10mm; } </style>`);
    }

    get(): void {
        this._quotationDetailsServiceProxy.getQuotationPDFInfo(this.quotationId)
            .subscribe((pdfResult) => {
                this.qPdfInfo = pdfResult;

                if (this.qPdfInfo && this.qPdfInfo.quotationInfo && this.qPdfInfo.quotationInfo.id > 0)
                    this.refID = 'Q' + this.padNum(this.qPdfInfo.quotationInfo.id);
                else
                    this.refID = '';

                if (this.qPdfInfo && this.qPdfInfo.vendorContact) {
                    var vContact = this.qPdfInfo.vendorContact;

                    if (vContact.phoneOffice && vContact.fax)
                        this.vendorContactString = "the undersigned at HP: " + vContact.phoneOffice + " or Fax: " + vContact.fax;
                    else if (vContact.phoneOffice)
                        this.vendorContactString = "the undersigned at HP: " + vContact.phoneOffice;
                    else if (vContact.fax)
                        this.vendorContactString = "the undersigned at Fax: " + vContact.fax;

                    this.vendorContactString += " ";
                }
                else
                    this.vendorContactString = "";

                this.changeDetector.detectChanges();
                //this.generatePDF();
            });
    }

    generatePDF(): void {
        if (this.qPdfInfo && this.qPdfInfo.authenticationKey && this.quotationId > 0) {
            this.saving = true;

            let apiEndPoint = environment.emsPdfApiUrl + environment.quoteCreatePdfEndPoint + "?quotationID=" + this.quotationId + "&username=" + this.username;
            const headerDict = new HttpHeaders().set("Authorization_Key", this.qPdfInfo.authenticationKey);//.set("Access-Control-Allow-Origin", "*");

            this.httpClient.post(apiEndPoint, null, {
                headers: headerDict
            }).subscribe(
                data => {
                    this.saving = false;

                    var res = JSON.parse(JSON.stringify(data));
                    if (res.StatusCode == 200) {
                        let mailCnt = res.Content;
                        let mailText = "mailto:" + mailCnt.ToEmail + "?subject=" + mailCnt.Subject + "&body=" + mailCnt.Body;
                        window.location.href = mailText;
                    }
                    else {
                        if (res.Message)
                            this.message.info(res.Message);
                        else
                            this.message.info("Error - Failed to send an email.");
                    }
                },
                err => {
                    this.saving = false;

                    if (err.message)
                        this.message.info(err.message);
                    else
                        this.message.info("Unknown Error - Failed to send an email.");
                }
            );
        }
        else {
            this.message.info("Details insufficient for generating the PDF");
        }



        //if (this.qPdfInfo && this.qPdfInfo.authenticationKey && this.quotationId > 0) {
        //    this.saving = true;
        //    let apiEndPoint = environment.quotationApiUrl + "?quotationID=" + this.quotationId + "&username=" + this.username + " &Authorization_Key=" + this.qPdfInfo.authenticationKey;

        //    fetch(apiEndPoint, {
        //        method: 'POST',
        //        mode: 'no-cors'
        //    })
        //        .then((result) => {
        //            this.saving = false;
        //            this.message.info("Successfully email sent");
        //        })
        //        .catch((error) => {
        //            this.saving = false;
        //            this.message.info("Error sending email.");
        //            console.error('Error:', error);
        //        });
        //}
        //else {
        //    this.message.info("Details insufficient for generating the PDF");
        //}


        //const formData = new FormData();
        //formData.append("htmlString", this.pdfHtmlElement.nativeElement.innerHTML);

        //let baseUrl = 'devandpreproduction';
        //let pdfFileName = 'SampleAel.Pdf';
        //let apiEndPoint = AppConsts.remoteServiceBaseUrl + '/EmsPdf/GeneratePdf?baseUrl=' + baseUrl + '&pdfFileName=' + pdfFileName;


        //let apiEndPoint = "http://localhost:58765/api/pdfmake/create?quotationID=1&username=admin"
        //const headers = { 'Authorization_Key': '11F850CA-3710-4CD8-840E-2BC3766C877E' }

        //this.http.post<any>(apiEndPoint, { title: 'Angular POST Request Example' }).subscribe(data => {
        //    this.message.info("Ha ha");
        //})


        //fetch(apiEndPoint, {
        //    method: "POST",
        //    body: '',
        //    headers: {
        //        "Authorization_Key": "11F850CA-3710-4CD8-840E-2BC3766C877E",
        //        "Content-Type": "application/json"
        //    },
        //    credentials: "same-origin"
        //}).then(function (response) {
        //    //response.status     //=> number 100–599
        //    //response.statusText //=> String
        //    //response.headers    //=> Headers
        //    //response.url        //=> String

        //    this.message.info(response.text());
        //    //return response.text()
        //}, function (error) {
        //    //error.message //=> String
        //    this.message.info(error.message);
        //})



        ////var result = from( // wrap the fetch in a from if you need an rxjs Observable
        //    fetch(
        //        'http://localhost:58765/api/pdfmake/create?quotationID=1&username=admin',
        //        {
        //            headers: headers,
        //            method: 'POST',
        //            mode: 'no-cors'
        //        }
        //    )
        ////);


        //const uploadReq = new HttpRequest('POST', apiEndPoint, null, {
        //    headers: new HttpHeaders(headerDict),
        //    reportProgress: true
        //});

        //this.http.request(uploadReq).subscribe(event => {
        //    //if (event.type === HttpEventType.UploadProgress)
        //    //    this.progress = Math.round(100 * event.loaded / event.total);
        //    //else if (event.type === HttpEventType.Response)
        //    //    this.message = event.body.toString();
        //});
    }

    close(): void {
        this._router.navigate(['app/main/support/quotations']);
    }

    padNum(num) {
        let size = 6;
        var s = num + "";
        while (s.length < size) s = "0" + s;
        return s;
    }
}
