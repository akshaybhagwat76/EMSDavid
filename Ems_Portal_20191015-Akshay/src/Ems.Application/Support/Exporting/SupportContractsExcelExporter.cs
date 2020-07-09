using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class SupportContractsExcelExporter : EpPlusExcelExporterBase, ISupportContractsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public SupportContractsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetSupportContractForViewDto> supportContracts)
        {
            return CreateExcelPackage(
                "SupportContracts.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("SupportContracts"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Title"),
                        L("Reference"),
                        L("Description"),
                        L("StartDate"),
                        L("EndDate"),
                        L("Attachments"),
                        L("IsRFQTemplate"),
                        L("IsAcknowledged"),
                        L("AcknowledgedBy"),
                        L("AcknowledgedAt"),
                        (L("Vendor")) + L("Name"),
                        (L("AssetOwner")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, supportContracts,
                        _ => _.SupportContract.Title,
                        _ => _.SupportContract.Reference,
                        _ => _.SupportContract.Description,
                        _ => _timeZoneConverter.Convert(_.SupportContract.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.SupportContract.EndDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.SupportContract.IsRFQTemplate,
                        _ => _.SupportContract.IsAcknowledged,
                        _ => _.SupportContract.AcknowledgedBy,
                        _ => _timeZoneConverter.Convert(_.SupportContract.AcknowledgedAt, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.VendorName,
                        _ => _.AssetOwnerName
                        );

					var startDateColumn = sheet.Column(4);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var endDateColumn = sheet.Column(5);
                    endDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endDateColumn.AutoFit();
					var acknowledgedAtColumn = sheet.Column(10);
                    acknowledgedAtColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					acknowledgedAtColumn.AutoFit();
					

                });
        }
    }
}
