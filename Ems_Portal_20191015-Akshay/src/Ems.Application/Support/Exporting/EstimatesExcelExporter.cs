using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class EstimatesExcelExporter : EpPlusExcelExporterBase, IEstimatesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public EstimatesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetEstimateForViewDto> estimates)
        {
            return CreateExcelPackage(
                "Estimates.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Estimates"));
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
                        L("Remark"),
                        L("RequoteRefId"),
                        L("QuotationLoc8GUID"),
                        L("AcknowledgedBy"),
                        L("AcknowledgedAt"),
                        (L("Quotation")) + L("Title"),
                        (L("EstimateStatus")) + L("Status")
                        );

                    AddObjects(
                        sheet, 2, estimates,
                        _ => _.Estimate.Reference,
                        _ => _.Estimate.Title,
                        _ => _.Estimate.Description,
                        _ => _timeZoneConverter.Convert(_.Estimate.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Estimate.EndDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Estimate.TotalTax,
                        _ => _.Estimate.TotalPrice,
                        _ => _.Estimate.TotalDiscount,
                        _ => _.Estimate.TotalCharge,
                        _ => _.Estimate.Version,
                        _ => _.Estimate.Remark,
                        _ => _.Estimate.RequoteRefId,
                        _ => _.Estimate.QuotationLoc8GUID,
                        _ => _.Estimate.AcknowledgedBy,
                        _ => _timeZoneConverter.Convert(_.Estimate.AcknowledgedAt, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.QuotationTitle,
                        _ => _.EstimateStatusStatus
                        );

					var startDateColumn = sheet.Column(4);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var endDateColumn = sheet.Column(5);
                    endDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endDateColumn.AutoFit();
					var acknowledgedAtColumn = sheet.Column(15);
                    acknowledgedAtColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					acknowledgedAtColumn.AutoFit();
					

                });
        }
    }
}
