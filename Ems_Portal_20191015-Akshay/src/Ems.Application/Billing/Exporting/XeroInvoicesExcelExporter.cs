using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class XeroInvoicesExcelExporter : EpPlusExcelExporterBase, IXeroInvoicesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public XeroInvoicesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetXeroInvoiceForViewDto> xeroInvoices)
        {
            return CreateExcelPackage(
                "XeroInvoices.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("XeroInvoices"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("XeroInvoiceCreated"),
                        L("ApiResponse"),
                        L("Failed"),
                        L("Exception"),
                        L("XeroReference"),
                        (L("CustomerInvoice")) + L("CustomerReference")
                        );

                    AddObjects(
                        sheet, 2, xeroInvoices,
                        _ => _.XeroInvoice.XeroInvoiceCreated,
                        _ => _.XeroInvoice.ApiResponse,
                        _ => _.XeroInvoice.Failed,
                        _ => _.XeroInvoice.Exception,
                        _ => _.XeroInvoice.XeroReference,
                        _ => _.CustomerInvoiceCustomerReference
                        );

					
					
                });
        }
    }
}
