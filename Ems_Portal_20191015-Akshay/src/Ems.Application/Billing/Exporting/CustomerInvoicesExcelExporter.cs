using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class CustomerInvoicesExcelExporter : EpPlusExcelExporterBase, ICustomerInvoicesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CustomerInvoicesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCustomerInvoiceForViewDto> customerInvoices)
        {
            return CreateExcelPackage(
                "CustomerInvoices.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("CustomerInvoices"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("CustomerReference"),
                        L("Description"),
                        L("DateIssued"),
                        L("DateDue"),
                        L("TotalTax"),
                        L("TotalPrice"),
                        L("TotalNet"),
                        L("TotalDiscount"),
                        L("TotalCharge"),
                        L("InvoiceRecipient"),
                        L("Remarks"),
                        (L("Customer")) + L("Name"),
                        (L("Currency")) + L("Code"),
                        (L("BillingRule")) + L("Name"),
                        (L("BillingEvent")) + L("Purpose"),
                        (L("InvoiceStatus")) + L("Status")
                        );

                    AddObjects(
                        sheet, 2, customerInvoices,
                        _ => _.CustomerInvoice.CustomerReference,
                        _ => _.CustomerInvoice.Description,
                        _ => _timeZoneConverter.Convert(_.CustomerInvoice.DateIssued, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.CustomerInvoice.DateDue, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.CustomerInvoice.TotalTax,
                        _ => _.CustomerInvoice.TotalPrice,
                        _ => _.CustomerInvoice.TotalNet,
                        _ => _.CustomerInvoice.TotalDiscount,
                        _ => _.CustomerInvoice.TotalCharge,
                        _ => _.CustomerInvoice.InvoiceRecipient,
                        _ => _.CustomerInvoice.Remarks,
                        _ => _.CustomerName,
                        _ => _.CurrencyCode,
                        _ => _.BillingRuleName,
                        _ => _.BillingEventPurpose,
                        _ => _.InvoiceStatusStatus
                        );

					var dateIssuedColumn = sheet.Column(3);
                    dateIssuedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateIssuedColumn.AutoFit();
					var dateDueColumn = sheet.Column(4);
                    dateDueColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateDueColumn.AutoFit();
					

                });
        }
    }
}
