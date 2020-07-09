using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class AssetOwnersExcelExporter : EpPlusExcelExporterBase, IAssetOwnersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AssetOwnersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAssetOwnerForViewDto> assetOwners)
        {
            return CreateExcelPackage(
                "AssetOwners.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AssetOwners"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Name"),
                        L("Identifier"),
                        L("LogoUrl"),
                        L("Website"),
                        (L("Currency")) + L("Code"),
                        (L("SsicCode")) + L("Code")
                        );

                    AddObjects(
                        sheet, 2, assetOwners,
                        _ => _.AssetOwner.Reference,
                        _ => _.AssetOwner.Name,
                        _ => _.AssetOwner.Identifier,
                        _ => _.AssetOwner.LogoUrl,
                        _ => _.AssetOwner.Website,
                        _ => _.CurrencyCode,
                        _ => _.SsicCodeCode
                        );

					

                });
        }
    }
}
