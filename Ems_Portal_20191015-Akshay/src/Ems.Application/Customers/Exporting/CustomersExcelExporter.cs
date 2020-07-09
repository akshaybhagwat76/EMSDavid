using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Customers.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Customers.Exporting
{
    public class CustomersExcelExporter : EpPlusExcelExporterBase, ICustomersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CustomersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCustomerForViewDto> customers)
        {
            return CreateExcelPackage(
                "Customers.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Customers"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Reference"),
                        L("Name"),
                        L("Identifier"),
                        L("LogoUrl"),
                        L("Website"),
                        L("CustomerLoc8UUID"),
                        (L("CustomerType")) + L("Type"),
                        (L("Currency")) + L("Code")
                        );

                    AddObjects(
                        sheet, 2, customers,
                        _ => _.Customer.Reference,
                        _ => _.Customer.Name,
                        _ => _.Customer.Identifier,
                        _ => _.Customer.LogoUrl,
                        _ => _.Customer.Website,
                        _ => _.Customer.CustomerLoc8UUID,
                        _ => _.CustomerTypeType,
                        _ => _.CurrencyCode
                        );

					

                });
        }
    }
}
