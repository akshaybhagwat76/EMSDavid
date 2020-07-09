using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class EstimateStatusesExcelExporter : EpPlusExcelExporterBase, IEstimateStatusesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public EstimateStatusesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetEstimateStatusForViewDto> estimateStatuses)
        {
            return CreateExcelPackage(
                "EstimateStatuses.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("EstimateStatuses"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Status"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, estimateStatuses,
                        _ => _.EstimateStatus.Status,
                        _ => _.EstimateStatus.Description
                        );

					

                });
        }
    }
}
