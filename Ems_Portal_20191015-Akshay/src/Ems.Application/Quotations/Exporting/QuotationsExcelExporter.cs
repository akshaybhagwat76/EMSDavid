using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Quotations.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Quotations.Exporting
{
    public class QuotationsExcelExporter : EpPlusExcelExporterBase, IQuotationsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public QuotationsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetQuotationForViewDto> quotations)
        {
            return CreateExcelPackage(
                "Quotations.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Quotations"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Title"),
                        L("Description"),
                        L("StartDate"),
                        L("EndDate"),
                        L("TotalTax"),
                        L("TotalPrice"),
                        L("TotalDiscount"),
                        L("TotalCharge"),
                        L("Version"),
                        L("IsFinal"),
                        L("Remark"),
                        L("RequoteRefId"),
                        L("QuotationLoc8GUID"),
                        L("AcknowledgedBy"),
                        L("AcknowledgedAt"),
                        (L("SupportContract")) + L("Title"),
                        (L("QuotationStatus")) + L("Status"),
                        (L("WorkOrder")) + L("Subject")
                        );

                    AddObjects(
                        sheet, 2, quotations,
                        _ => _.Quotation.Reference,
                        _ => _.Quotation.Title,
                        _ => _.Quotation.Description,
                        _ => _timeZoneConverter.Convert(_.Quotation.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Quotation.EndDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Quotation.TotalTax,
                        _ => _.Quotation.TotalPrice,
                        _ => _.Quotation.TotalDiscount,
                        _ => _.Quotation.TotalCharge,
                        _ => _.Quotation.Version,
                        _ => _.Quotation.IsFinal,
                        _ => _.Quotation.Remark,
                        _ => _.Quotation.RequoteRefId,
                        _ => _.Quotation.QuotationLoc8GUID,
                        _ => _.Quotation.AcknowledgedBy,
                        _ => _timeZoneConverter.Convert(_.Quotation.AcknowledgedAt, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.SupportContractTitle,
                        _ => _.QuotationStatusStatus,
                        _ => _.WorkOrderSubject
                        );

					var startDateColumn = sheet.Column(4);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var endDateColumn = sheet.Column(5);
                    endDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endDateColumn.AutoFit();
					var acknowledgedAtColumn = sheet.Column(16);
                    acknowledgedAtColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					acknowledgedAtColumn.AutoFit();
					

                });
        }
    }
}
