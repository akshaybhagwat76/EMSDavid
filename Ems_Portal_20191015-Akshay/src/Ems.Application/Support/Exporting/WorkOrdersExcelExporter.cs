using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class WorkOrdersExcelExporter : EpPlusExcelExporterBase, IWorkOrdersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WorkOrdersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWorkOrderForViewDto> workOrders)
        {
            return CreateExcelPackage(
                "WorkOrders.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("WorkOrders"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Loc8GUID"),
                        L("Subject"),
                        L("Description"),
                        L("Location"),
                        L("StartDate"),
                        L("EndDate"),
                        L("Remarks"),
                        L("Attachments"),
                        (L("WorkOrderPriority")) + L("Priority"),
                        (L("WorkOrderType")) + L("Type"),
                        (L("Vendor")) + L("Name"),
                        (L("Incident")) + L("Description"),
                        (L("SupportItem")) + L("Description"),
                        (L("User")) + L("Name"),
                        (L("Customer")) + L("Name"),
                        (L("AssetOwnership")) + L("AssetFk"),
                        (L("WorkOrderStatus")) + L("Status")
                        );

                    AddObjects(
                        sheet, 2, workOrders,
                        _ => _.WorkOrder.Loc8GUID,
                        _ => _.WorkOrder.Subject,
                        _ => _.WorkOrder.Description,
                        _ => _.WorkOrder.Location,
                        _ => _timeZoneConverter.Convert(_.WorkOrder.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.WorkOrder.EndDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.WorkOrder.Remarks,
                        _ => _.WorkOrder.Attachments,
                        _ => _.WorkOrderPriorityPriority,
                        _ => _.WorkOrderTypeType,
                        _ => _.VendorName,
                        _ => _.IncidentDescription,
                        _ => _.SupportItemDescription,
                        _ => _.UserName,
                        _ => _.CustomerName,
                        _ => _.AssetOwnershipAssetDisplayName,
                        _ => _.WorkOrderStatusStatus
                        );

					var startDateColumn = sheet.Column(5);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var endDateColumn = sheet.Column(6);
                    endDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endDateColumn.AutoFit();
					

                });
        }
    }
}
