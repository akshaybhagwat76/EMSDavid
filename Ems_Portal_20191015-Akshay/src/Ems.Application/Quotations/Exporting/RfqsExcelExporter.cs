using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Quotations.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Quotations.Exporting
{
    public class RfqsExcelExporter : EpPlusExcelExporterBase, IRfqsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RfqsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRfqForViewDto> rfqs)
        {
            return CreateExcelPackage(
                "Rfqs.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Rfqs"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Title"),
                        L("RequestDate"),
                        L("RequiredBy"),
                        L("Description"),
                        L("Requirements"),
                        (L("RfqType")) + L("Type"),
                        (L("AssetOwner")) + L("Name"),
                        (L("Customer")) + L("Name"),
                        (L("AssetClass")) + L("Class"),
                        (L("Incident")) + L("Description"),
                        (L("Vendor")) + L("Name"),
                        (L("User")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, rfqs,
                        _ => _.Rfq.Title,
                        _ => _timeZoneConverter.Convert(_.Rfq.RequestDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Rfq.RequiredBy, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Rfq.Description,
                        _ => _.Rfq.Requirements,
                        _ => _.RfqTypeType,
                        _ => _.AssetOwnerName,
                        _ => _.CustomerName,
                        _ => _.AssetClassClass,
                        _ => _.IncidentDescription,
                        _ => _.VendorName,
                        _ => _.UserName
                        );

					var requestDateColumn = sheet.Column(2);
                    requestDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					requestDateColumn.AutoFit();
					var requiredByColumn = sheet.Column(3);
                    requiredByColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					requiredByColumn.AutoFit();
					

                });
        }
    }
}
