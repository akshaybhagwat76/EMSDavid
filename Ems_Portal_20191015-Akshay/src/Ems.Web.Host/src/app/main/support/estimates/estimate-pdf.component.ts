import { Component, Injector, ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash';
import { EstimatesServiceProxy, EstimatePdfDto } from '@shared/service-proxies/service-proxies';
import { environment } from 'environments/environment';

import { HttpClient, HttpHeaders, HttpClientModule } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { Observable } from 'rxjs';

//declare var require: any
//const FileSaver = require('file-saver');

@Component({
    selector: 'estimatePdf',
    templateUrl: './estimate-pdf.component.html',
    animations: [appModuleAnimation()]
})
export class EstimatePdfComponent extends AppComponentBase {

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _estimatesServiceProxy: EstimatesServiceProxy,
        private httpClient: HttpClient,
        private changeDetector: ChangeDetectorRef,
        private sanitization: DomSanitizer
    ) {
        super(injector);
    }

    active = false;
    saving = false;

    estimateId: number;
    ePdfInfo: EstimatePdfDto;
    username: string;
    currentDate = new Date();
    refID: string;
    aoContactString: string;
    aelLogo: string;

    ngOnInit(): void {
        this.estimateId = this._activatedRoute.snapshot.queryParams['estimateId'];

        this.get();
        this.username = this.appSession.user.userName.trim();
        this.aelLogo = this.appRootUrl() + 'assets/common/images/aelLogo.jpg';
    }

    getSanitizedHtml() {
        return this.sanitization.bypassSecurityTrustHtml(`<title>Estimate</title> <style> @page { size: A4; margin: 10mm 10mm 10mm 10mm; } </style>`);
    }

    get(): void {
        this._estimatesServiceProxy.getEstimatePDFInfo(this.estimateId)
            .subscribe((pdfResult) => {
                this.ePdfInfo = pdfResult;

                if (this.ePdfInfo && this.ePdfInfo.estimateInfo && this.ePdfInfo.estimateInfo.id > 0)
                    this.refID = 'E' + this.padNum(this.ePdfInfo.estimateInfo.id);
                else
                    this.refID = '';

                if (this.ePdfInfo && this.ePdfInfo.assetOwnerContact) {
                    var aoContact = this.ePdfInfo.assetOwnerContact;

                    if (aoContact.phoneOffice && aoContact.fax)
                        this.aoContactString = "the undersigned at HP: " + aoContact.phoneOffice + " or Fax: " + aoContact.fax;
                    else if (aoContact.phoneOffice)
                        this.aoContactString = "the undersigned at HP: " + aoContact.phoneOffice;
                    else if (aoContact.fax)
                        this.aoContactString = "the undersigned at Fax: " + aoContact.fax;

                    this.aoContactString += " ";
                }
                else
                    this.aoContactString = "";

                this.changeDetector.detectChanges();
                //this.generatePDF();
            });
    }

    generatePDFLocalCheck(): void {
        let subject = "Estimate PDF - Cust Title Updated";
        let body = "Dear Vigneshwaran S, %0D%0A%0D%0AYou have received a new estimate from the asset owner 'AE Leasing Singapore'. Please check the attached PDF Link. %0D%0A%0D%0AThis estimate is pending approval. %0D%0A%0D%0A https://aelfilesandimages.blob.core.windows.net/aelcontainer/PDF/Estimate/202004060340411151-b0369737.pdf?sv=2018-03-28&sr=b&sig=%2FJltKv3QUh8zxnFShCb6S5nf3yIC7vOvViOp6djuc6w%3D&st=2020-04-06T15%3A39%3A42Z&se=2021-04-06T15%3A40%3A42Z&sp=r %0D%0A%0D%0A";
        let toEmail = "tonyclark69@live.com.au";

        let mailText = "mailto:" + toEmail + "?subject=" + subject + "&body=" + body; 
        window.location.href = mailText;
    }

    generatePDF(): void {
        if (this.ePdfInfo && this.ePdfInfo.authenticationKey && this.estimateId > 0) {
            this.saving = true;
            let apiEndPoint = environment.emsPdfApiUrl + environment.estimteCreatePdfEndPoint + "?estimateID=" + this.estimateId + "&Username=" + this.username + "&Environment=" + "Azure-Dev";
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

    close(): void {
        this._router.navigate(['app/main/support/estimates']);
    }

    padNum(num) {
        let size = 6;
        var s = num + "";
        while (s.length < size) s = "0" + s;
        return s;
    }
}
