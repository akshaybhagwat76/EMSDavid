using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class AssetClassesExcelExporter : EpPlusExcelExporterBase, IAssetClassesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AssetClassesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAssetClassForViewDto> assetClasses)
        {
            return CreateExcelPackage(
                "AssetClasses.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AssetClasses"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Manufacturer"),
                        L("Model"),
                        L("Specification"),
                        L("Class"),
                        (L("AssetType")) + L("Type")
                        );

                    AddObjects(
                        sheet, 2, assetClasses,
                        _ => _.AssetClass.Manufacturer,
                        _ => _.AssetClass.Model,
                        _ => _.AssetClass.Specification,
                        _ => _.AssetClass.Class,
                        _ => _.AssetTypeType
                        );

					

                });
        }
    }
}
