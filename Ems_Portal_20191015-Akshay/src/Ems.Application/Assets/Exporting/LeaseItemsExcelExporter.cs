using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class LeaseItemsExcelExporter : EpPlusExcelExporterBase, ILeaseItemsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LeaseItemsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetLeaseItemForViewDto> leaseItems)
        {
            return CreateExcelPackage(
                "LeaseItems.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("LeaseItems"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("DateAllocated"),
                        L("AllocationPercentage"),
                        L("Terms"),
                        L("UnitRentalRate"),
                        L("UnitDepositRate"),
                        L("StartDate"),
                        L("EndDate"),
                        L("Attachments"),
                        L("RentalUomRefId"),
                        L("DepositUomRefId"),
                        L("Item"),
                        L("Description"),
                        (L("AssetClass")) + L("Class"),
                        (L("Asset")) + L("Reference"),
                        (L("LeaseAgreement")) + L("Title")
                        );

                    AddObjects(
                        sheet, 2, leaseItems,
                        _ => _timeZoneConverter.Convert(_.LeaseItem.DateAllocated, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.LeaseItem.AllocationPercentage,
                        _ => _.LeaseItem.Terms,
                        _ => _.LeaseItem.UnitRentalRate,
                        _ => _.LeaseItem.UnitDepositRate,
                        _ => _timeZoneConverter.Convert(_.LeaseItem.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.LeaseItem.EndDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.LeaseItem.RentalUomRefId,
                        _ => _.LeaseItem.DepositUomRefId,
                        _ => _.LeaseItem.Item,
                        _ => _.LeaseItem.Description,
                        _ => _.AssetClassClass,
                        _ => _.AssetReference,
                        _ => _.LeaseAgreementTitle
                        );

					var dateAllocatedColumn = sheet.Column(1);
                    dateAllocatedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateAllocatedColumn.AutoFit();
					var startDateColumn = sheet.Column(6);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var endDateColumn = sheet.Column(7);
                    endDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endDateColumn.AutoFit();
					

                });
        }
    }
}
