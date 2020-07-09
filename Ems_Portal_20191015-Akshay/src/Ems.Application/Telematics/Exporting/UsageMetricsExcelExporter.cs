using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Telematics.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Telematics.Exporting
{
    public class UsageMetricsExcelExporter : EpPlusExcelExporterBase, IUsageMetricsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UsageMetricsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetUsageMetricForViewDto> usageMetrics)
        {
            return CreateExcelPackage(
                "UsageMetrics.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("UsageMetrics"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Metric"),
                        L("Description"),
                        (L("Asset")) + L("Reference"),
                        (L("LeaseItem")) + L("Item"),
                        (L("Uom")) + L("UnitOfMeasurement")
                        );

                    AddObjects(
                        sheet, 2, usageMetrics,
                        _ => _.UsageMetric.Metric,
                        _ => _.UsageMetric.Description,
                        _ => _.AssetReference,
                        _ => _.LeaseItemItem,
                        _ => _.UomUnitOfMeasurement
                        );

					

                });
        }
    }
}
