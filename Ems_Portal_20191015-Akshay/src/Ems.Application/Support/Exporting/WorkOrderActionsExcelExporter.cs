using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class WorkOrderActionsExcelExporter : EpPlusExcelExporterBase, IWorkOrderActionsExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WorkOrderActionsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWorkOrderActionForViewDto> workOrderActions)
        {
            return CreateExcelPackage(
                "WorkOrderActions.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("WorkOrderActions"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Action"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, workOrderActions,
                        _ => _.WorkOrderAction.Action,
                        _ => _.WorkOrderAction.Description
                        );
                });
        }
    }
}
