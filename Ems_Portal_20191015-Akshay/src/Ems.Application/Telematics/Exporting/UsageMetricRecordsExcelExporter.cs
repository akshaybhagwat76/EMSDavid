using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Telematics.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Telematics.Exporting
{
    public class UsageMetricRecordsExcelExporter : EpPlusExcelExporterBase, IUsageMetricRecordsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UsageMetricRecordsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetUsageMetricRecordForViewDto> usageMetricRecords)
        {
            return CreateExcelPackage(
                "UsageMetricRecords.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("UsageMetricRecords"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("StartTime"),
                        L("EndTime"),
                        L("UnitsConsumed"),
                        (L("Uom")) + L("UnitOfMeasurement"),
                        (L("UsageMetric")) + L("Metric")
                        );

                    AddObjects(
                        sheet, 2, usageMetricRecords,
                        _ => _.UsageMetricRecord.Reference,
                        _ => _timeZoneConverter.Convert(_.UsageMetricRecord.StartTime, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.UsageMetricRecord.EndTime, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.UsageMetricRecord.UnitsConsumed,
                        _ => _.UsageMetricMetric
                        );

					var startTimeColumn = sheet.Column(2);
                    startTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startTimeColumn.AutoFit();
					var endTimeColumn = sheet.Column(3);
                    endTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endTimeColumn.AutoFit();
					

                });
        }
    }
}
