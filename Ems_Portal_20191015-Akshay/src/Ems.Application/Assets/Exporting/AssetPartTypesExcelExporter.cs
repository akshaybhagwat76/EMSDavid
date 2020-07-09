using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class AssetPartTypesExcelExporter : EpPlusExcelExporterBase, IAssetPartTypesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AssetPartTypesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAssetPartTypeForViewDto> assetPartTypes)
        {
            return CreateExcelPackage(
                "AssetPartTypes.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AssetPartTypes"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Type"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, assetPartTypes,
                        _ => _.AssetPartType.Type,
                        _ => _.AssetPartType.Description
                        );

					

                });
        }
    }
}
