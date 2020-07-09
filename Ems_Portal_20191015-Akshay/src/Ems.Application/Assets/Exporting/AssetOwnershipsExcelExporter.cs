using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class AssetOwnershipsExcelExporter : EpPlusExcelExporterBase, IAssetOwnershipsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AssetOwnershipsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAssetOwnershipForViewDto> assetOwnerships)
        {
            return CreateExcelPackage(
                "AssetOwnerships.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AssetOwnerships"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("StartDate"),
                        L("FinishDate"),
                        L("PercentageOwnership"),
                        (L("Asset")) + L("Reference"),
                        (L("AssetOwner")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, assetOwnerships,
                        _ => _timeZoneConverter.Convert(_.AssetOwnership.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.AssetOwnership.FinishDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.AssetOwnership.PercentageOwnership,
                        _ => _.AssetReference,
                        _ => _.AssetOwnerName
                        );

					var startDateColumn = sheet.Column(1);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var finishDateColumn = sheet.Column(2);
                    finishDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					finishDateColumn.AutoFit();
					

                });
        }
    }
}
