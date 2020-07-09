using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class BillingEventsExcelExporter : EpPlusExcelExporterBase, IBillingEventsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public BillingEventsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetBillingEventForViewDto> billingEvents)
        {
            return CreateExcelPackage(
                "BillingEvents.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("BillingEvents"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("BillingEventDate"),
                        L("TriggeredBy"),
                        L("Purpose"),
                        L("WasInvoiceGenerated"),
                        (L("LeaseAgreement")) + L("Title"),
                        (L("VendorCharge")) + L("Reference"),
                        (L("BillingEventType")) + L("Type")
                        );

                    AddObjects(
                        sheet, 2, billingEvents,
                        _ => _timeZoneConverter.Convert(_.BillingEvent.BillingEventDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.BillingEvent.TriggeredBy,
                        _ => _.BillingEvent.Purpose,
                        _ => _.BillingEvent.WasInvoiceGenerated,
                        _ => _.LeaseAgreementTitle,
                        _ => _.VendorChargeReference,
                        _ => _.BillingEventTypeType
                        );

					var billingEventDateColumn = sheet.Column(1);
                    billingEventDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					billingEventDateColumn.AutoFit();
					

                });
        }
    }
}
