using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Finance.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Finance.Exporting
{
    public class AgedReceivablesPeriodsExcelExporter : EpPlusExcelExporterBase, IAgedReceivablesPeriodsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AgedReceivablesPeriodsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAgedReceivablesPeriodForViewDto> agedReceivablesPeriods)
        {
            return CreateExcelPackage(
                "AgedReceivablesPeriods.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AgedReceivablesPeriods"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Period"),
                        L("Current"),
                        L("Over30"),
                        L("Over60"),
                        L("Over90"),
                        L("Over120")
                        );

                    AddObjects(
                        sheet, 2, agedReceivablesPeriods,
                        _ => _timeZoneConverter.Convert(_.AgedReceivablesPeriod.Period, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.AgedReceivablesPeriod.Current,
                        _ => _.AgedReceivablesPeriod.Over30,
                        _ => _.AgedReceivablesPeriod.Over60,
                        _ => _.AgedReceivablesPeriod.Over90,
                        _ => _.AgedReceivablesPeriod.Over120
                        );

					var periodColumn = sheet.Column(1);
                    periodColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					periodColumn.AutoFit();
					

                });
        }
    }
}
