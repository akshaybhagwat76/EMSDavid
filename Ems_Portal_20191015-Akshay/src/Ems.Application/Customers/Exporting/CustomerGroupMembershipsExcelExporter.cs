using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Customers.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Customers.Exporting
{
    public class CustomerGroupMembershipsExcelExporter : EpPlusExcelExporterBase, ICustomerGroupMembershipsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CustomerGroupMembershipsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCustomerGroupMembershipForViewDto> customerGroupMemberships)
        {
            return CreateExcelPackage(
                "CustomerGroupMemberships.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("CustomerGroupMemberships"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("DateJoined"),
                        L("DateLeft"),
                        (L("CustomerGroup")) + L("Name"),
                        (L("Customer")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, customerGroupMemberships,
                        _ => _timeZoneConverter.Convert(_.CustomerGroupMembership.DateJoined, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.CustomerGroupMembership.DateLeft, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.CustomerGroupName,
                        _ => _.CustomerName
                        );

					var dateJoinedColumn = sheet.Column(1);
                    dateJoinedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateJoinedColumn.AutoFit();
					var dateLeftColumn = sheet.Column(2);
                    dateLeftColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateLeftColumn.AutoFit();
					

                });
        }
    }
}
