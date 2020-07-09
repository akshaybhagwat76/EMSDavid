using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class InventoryItemsExcelExporter : EpPlusExcelExporterBase, IInventoryItemsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public InventoryItemsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetInventoryItemForViewDto> inventoryItems)
        {
            return CreateExcelPackage(
                "InventoryItems.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("InventoryItems"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Reference"),
                        L("QtyInWarehouse"),
                        L("RestockLimit"),
                        L("QtyOnOrder"),
                        (L("ItemType")) + L("Type"),
                        (L("Asset")) + L("Reference")
                        );

                    AddObjects(
                        sheet, 2, inventoryItems,
                        _ => _.InventoryItem.Name,
                        _ => _.InventoryItem.Reference,
                        _ => _.InventoryItem.QtyInWarehouse,
                        _ => _.InventoryItem.RestockLimit,
                        _ => _.InventoryItem.QtyOnOrder,
                        _ => _.ItemTypeType,
                        _ => _.AssetReference
                        );

					

                });
        }
    }
}
