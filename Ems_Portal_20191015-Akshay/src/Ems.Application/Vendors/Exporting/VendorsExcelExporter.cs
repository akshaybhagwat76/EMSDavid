using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Vendors.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Vendors.Exporting
{
    public class VendorsExcelExporter : EpPlusExcelExporterBase, IVendorsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VendorsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVendorForViewDto> vendors)
        {
            return CreateExcelPackage(
                "Vendors.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Vendors"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Name"),
                        L("Identifier"),
                        L("LogoUrl"),
                        L("Website"),
                        L("VendorLoc8GUID"),
                        (L("SsicCode")) + L("Code"),
                        (L("Currency")) + L("Code")
                        );

                    AddObjects(
                        sheet, 2, vendors,
                        _ => _.Vendor.Reference,
                        _ => _.Vendor.Name,
                        _ => _.Vendor.Identifier,
                        _ => _.Vendor.LogoUrl,
                        _ => _.Vendor.Website,
                        _ => _.Vendor.VendorLoc8GUID,
                        _ => _.SsicCodeCode,
                        _ => _.CurrencyCode
                        );

					

                });
        }
    }
}
