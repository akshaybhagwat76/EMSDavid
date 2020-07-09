using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class SupportItemsExcelExporter : EpPlusExcelExporterBase, ISupportItemsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public SupportItemsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetSupportItemForViewDto> supportItems)
        {
            return CreateExcelPackage(
                "SupportItems.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("SupportItems"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Description"),
                        L("UnitPrice"),
                        L("Frequency"),
                        L("IsAdHoc"),
                        L("IsChargeable"),
                        L("IsStandbyReplacementUnit"),
                        L("Attachments"),
                        (L("Asset")) + L("Reference"),
                        (L("AssetClass")) + L("Class"),
                        (L("Uom")) + L("UnitOfMeasurement"),
                        (L("SupportContract")) + L("Title"),
                        (L("ConsumableType")) + L("Type"),
                        (L("SupportType")) + L("Type")
                        );

                    AddObjects(
                        sheet, 2, supportItems,
                        _ => _.SupportItem.Description,
                        _ => _.SupportItem.UnitPrice,
                        _ => _.SupportItem.Frequency,
                        _ => _.SupportItem.IsAdHoc,
                        _ => _.SupportItem.IsChargeable,
                        _ => _.SupportItem.IsStandbyReplacementUnit,
                        _ => _.AssetReference,
                        _ => _.AssetClassClass,
                        _ => _.UomUnitOfMeasurement,
                        _ => _.SupportContractTitle,
                        _ => _.ConsumableTypeType,
                        _ => _.SupportTypeType
                        );

					

                });
        }
    }
}
