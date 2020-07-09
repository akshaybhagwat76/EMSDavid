using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class VendorChargeStatusesExcelExporter : EpPlusExcelExporterBase, IVendorChargeStatusesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VendorChargeStatusesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVendorChargeStatusForViewDto> vendorChargeStatuses)
        {
            return CreateExcelPackage(
                "VendorChargeStatuses.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("VendorChargeStatuses"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Status"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, vendorChargeStatuses,
                        _ => _.VendorChargeStatus.Status,
                        _ => _.VendorChargeStatus.Description
                        );

					

                });
        }
    }
}
