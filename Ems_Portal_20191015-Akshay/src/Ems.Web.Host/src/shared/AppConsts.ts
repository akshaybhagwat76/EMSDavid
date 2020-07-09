export class AppConsts {

    static readonly tenancyNamePlaceHolderInUrl = '{TENANCY_NAME}';

    static remoteServiceBaseUrl: string;
    static remoteServiceBaseUrlFormat: string;
    static appBaseUrl: string;
    static appBaseHref: string; // returns angular's base-href parameter value if used during the publish
    static appBaseUrlFormat: string;
    static recaptchaSiteKey: string;
    static subscriptionExpireNootifyDayCount: number;

    static localeMappings: any = [];

    static readonly userManagement = {
        defaultAdminUserName: 'admin'
    };

    static readonly localization = {
        defaultLocalizationSourceName: 'Ems'
    };

    static readonly authorization = {
        encrptedAuthTokenName: 'enc_auth_token'
    };

    static readonly grid = {
        defaultPageSize: 10
    };

    static readonly Submitted = 'Submitted';
    static readonly Submit = 'Submit';
    static readonly Paid = 'Paid';
    static readonly Created = 'Created';
    static readonly PDF = 'PDF';
    static readonly Details = 'Details';
    static readonly Downloaded = 'downloaded';
    static readonly Error = 'error';
    static readonly XeroConatctMessage = 'Xero contact not available';
    static readonly Refreshed = 'Refreshed';
}
