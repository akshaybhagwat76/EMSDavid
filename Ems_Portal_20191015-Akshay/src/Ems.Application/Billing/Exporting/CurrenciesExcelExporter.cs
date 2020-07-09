using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class CurrenciesExcelExporter : EpPlusExcelExporterBase, ICurrenciesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CurrenciesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCurrencyForViewDto> currencies)
        {
            return CreateExcelPackage(
                "Currencies.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Currencies"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Code"),
                        L("Name"),
                        L("Symbol"),
                        L("Country"),
                        L("BaseCountry")
                        );

                    AddObjects(
                        sheet, 2, currencies,
                        _ => _.Currency.Code,
                        _ => _.Currency.Name,
                        _ => _.Currency.Symbol,
                        _ => _.Currency.Country,
                        _ => _.Currency.BaseCountry
                        );

					

                });
        }
    }
}
