using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Authorization.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Authorization.Exporting
{
    public class CrossTenantPermissionsExcelExporter : EpPlusExcelExporterBase, ICrossTenantPermissionsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CrossTenantPermissionsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCrossTenantPermissionForViewDto> crossTenantPermissions)
        {
            return CreateExcelPackage(
                "CrossTenantPermissions.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("CrossTenantPermissions"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("TenantRefId"),
                        L("EntityType"),
                        L("Tenants"),
                        L("TenantType")
                        );

                    AddObjects(
                        sheet, 2, crossTenantPermissions,
                        _ => _.CrossTenantPermission.TenantRefId,
                        _ => _.CrossTenantPermission.EntityType,
                        _ => _.CrossTenantPermission.Tenants,
                        _ => _.CrossTenantPermission.TenantType
                        );

					

                });
        }
    }
}
