using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Vendors.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Vendors.Exporting
{
    public class VendorChargesExcelExporter : EpPlusExcelExporterBase, IVendorChargesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VendorChargesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVendorChargeForViewDto> vendorCharges)
        {
            return CreateExcelPackage(
                "VendorCharges.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("VendorCharges"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Description"),
                        L("DateIssued"),
                        L("DateDue"),
                        L("TotalTax"),
                        L("TotalPrice"),
                        (L("Vendor")) + L("Name"),
                        (L("SupportContract")) + L("Title"),
                        (L("WorkOrder")) + L("Subject"),
                        (L("VendorChargeStatus")) + L("Status")
                        );

                    AddObjects(
                        sheet, 2, vendorCharges,
                        _ => _.VendorCharge.Reference,
                        _ => _.VendorCharge.Description,
                        _ => _timeZoneConverter.Convert(_.VendorCharge.DateIssued, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.VendorCharge.DateDue, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.VendorCharge.TotalTax,
                        _ => _.VendorCharge.TotalPrice,
                        _ => _.VendorName,
                        _ => _.SupportContractTitle,
                        _ => _.WorkOrderSubject,
                        _ => _.VendorChargeStatusStatus
                        );

					var dateIssuedColumn = sheet.Column(3);
                    dateIssuedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateIssuedColumn.AutoFit();
					var dateDueColumn = sheet.Column(4);
                    dateDueColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateDueColumn.AutoFit();
					

                });
        }
    }
}
