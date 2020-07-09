using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class WorkOrderUpdatesExcelExporter : EpPlusExcelExporterBase, IWorkOrderUpdatesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WorkOrderUpdatesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWorkOrderUpdateForViewDto> workOrderUpdates)
        {
            return CreateExcelPackage(
                "WorkOrderUpdates.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("WorkOrderUpdates"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        (L("WorkOrderAction")) + L("Action"),
                        (L("ItemType")) + L("Type"),
                        L("Number"),
                        L("Comments"),
                        (L("WorkOrder")) + L("Subject")
                        );

                    AddObjects(
                        sheet, 2, workOrderUpdates,
                        _ => _.WorkOrderActionAction,
                        _ => _.ItemTypeType,
                        _ => _.WorkOrderUpdate.Number,
                        _ => _.WorkOrderUpdate.Comments,
                        _ => _.WorkOrderSubject
                        );

					var updatedAtColumn = sheet.Column(1);
                    updatedAtColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					updatedAtColumn.AutoFit();
					

                });
        }
    }
}
