using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class WorkOrderPrioritiesExcelExporter : EpPlusExcelExporterBase, IWorkOrderPrioritiesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WorkOrderPrioritiesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWorkOrderPriorityForViewDto> workOrderPriorities)
        {
            return CreateExcelPackage(
                "WorkOrderPriorities.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("WorkOrderPriorities"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Priority"),
                        L("PriorityLevel")
                        );

                    AddObjects(
                        sheet, 2, workOrderPriorities,
                        _ => _.WorkOrderPriority.Priority,
                        _ => _.WorkOrderPriority.PriorityLevel
                        );

					

                });
        }
    }
}
