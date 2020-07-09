using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class LeaseAgreementsExcelExporter : EpPlusExcelExporterBase, ILeaseAgreementsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LeaseAgreementsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetLeaseAgreementForViewDto> leaseAgreements)
        {
            return CreateExcelPackage(
                "LeaseAgreements.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("LeaseAgreements"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Description"),
                        L("Title"),
                        L("Terms"),
                        L("StartDate"),
                        L("EndDate"),
                        L("Attachments"),
                        (L("Contact")) + L("ContactName"),
                        (L("AssetOwner")) + L("Name"),
                        (L("Customer")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, leaseAgreements,
                        _ => _.LeaseAgreement.Reference,
                        _ => _.LeaseAgreement.Description,
                        _ => _.LeaseAgreement.Title,
                        _ => _.LeaseAgreement.Terms,
                        _ => _timeZoneConverter.Convert(_.LeaseAgreement.StartDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.LeaseAgreement.EndDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ContactContactName,
                        _ => _.AssetOwnerName,
                        _ => _.CustomerName
                        );

					var startDateColumn = sheet.Column(5);
                    startDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					startDateColumn.AutoFit();
					var endDateColumn = sheet.Column(6);
                    endDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					endDateColumn.AutoFit();
					

                });
        }
    }
}
