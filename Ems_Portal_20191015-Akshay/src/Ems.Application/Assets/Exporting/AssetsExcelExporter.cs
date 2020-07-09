using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class AssetsExcelExporter : EpPlusExcelExporterBase, IAssetsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AssetsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAssetForViewDto> assets)
        {
            return CreateExcelPackage(
                "Assets.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Assets"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("VehicleRegistrationNo"),
                        L("IsExternalAsset"),
                        L("Location"),
                        L("SerialNumber"),
                        L("EngineNo"),
                        L("ChassisNo"),
                        L("Description"),
                        L("PurchaseOrderNo"),
                        L("PurchaseDate"),
                        L("PurchaseCost"),
                        L("AssetLoc8GUID"),
                        L("Attachments"),
                        (L("AssetClass")) + L("Class"),
                        (L("AssetStatus")) + L("Status")
                        );

                    AddObjects(
                        sheet, 2, assets,
                        _ => _.Asset.Reference,
                        _ => _.Asset.VehicleRegistrationNo,
                        _ => _.Asset.IsExternalAsset,
                        _ => _.Asset.Location,
                        _ => _.Asset.SerialNumber,
                        _ => _.Asset.EngineNo,
                        _ => _.Asset.ChassisNo,
                        _ => _.Asset.Description,
                        _ => _.Asset.PurchaseOrderNo,
                        _ => _timeZoneConverter.Convert(_.Asset.PurchaseDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Asset.PurchaseCost,
                        _ => _.Asset.AssetLoc8GUID,
                        _ => _.AssetClassClass,
                        _ => _.AssetStatusStatus
                        );

					var purchaseDateColumn = sheet.Column(10);
                    purchaseDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					purchaseDateColumn.AutoFit();
					

                });
        }
    }
}
