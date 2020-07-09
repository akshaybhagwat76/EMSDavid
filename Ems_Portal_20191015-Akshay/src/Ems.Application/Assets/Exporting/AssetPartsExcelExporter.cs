using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class AssetPartsExcelExporter : EpPlusExcelExporterBase, IAssetPartsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AssetPartsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAssetPartForViewDto> assetParts)
        {
            return CreateExcelPackage(
                "AssetParts.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AssetParts"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Description"),
                        L("SerialNumber"),
                        L("InstallDate"),
                        L("Code"),
                        L("Installed"),
                        (L("AssetPartType")) + L("Type"),
                        (L("AssetPart")) + L("Name"),
                        (L("AssetPartStatus")) + L("Status"),
                        (L("UsageMetric")) + L("Metric"),
                        (L("Attachment")) + L("Filename"),
                        (L("Asset")) + L("Reference"),
                        (L("ItemType")) + L("Type")
                        );

                    AddObjects(
                        sheet, 2, assetParts,
                        _ => _.AssetPart.Name,
                        _ => _.AssetPart.Description,
                        _ => _.AssetPart.SerialNumber,
                        _ => _timeZoneConverter.Convert(_.AssetPart.InstallDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.AssetPart.Code,
                        _ => _.AssetPart.Installed,
                        _ => _.AssetPartTypeType,
                        _ => _.AssetPartName,
                        _ => _.AssetPartStatusStatus,
                        _ => _.UsageMetricMetric,
                        _ => _.AttachmentFilename,
                        _ => _.AssetReference,
                        _ => _.ItemTypeType
                        );

					var installDateColumn = sheet.Column(4);
                    installDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					installDateColumn.AutoFit();
					

                });
        }
    }
}
