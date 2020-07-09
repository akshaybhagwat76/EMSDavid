using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class WorkOrderStatusesExcelExporter : EpPlusExcelExporterBase, IWorkOrderStatusesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WorkOrderStatusesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWorkOrderStatusForViewDto> workOrderStatuses)
        {
            return CreateExcelPackage(
                "WorkOrderStatuses.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("WorkOrderStatuses"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Status"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, workOrderStatuses,
                        _ => _.WorkOrderStatus.Status,
                        _ => _.WorkOrderStatus.Description
                        );

					

                });
        }
    }
}
