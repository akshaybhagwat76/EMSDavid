using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Organizations.Exporting
{
    public class SsicCodesExcelExporter : EpPlusExcelExporterBase, ISsicCodesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public SsicCodesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetSsicCodeForViewDto> ssicCodes)
        {
            return CreateExcelPackage(
                "SsicCodes.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("SsicCodes"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Code"),
                        L("SSIC")
                        );

                    AddObjects(
                        sheet, 2, ssicCodes,
                        _ => _.SsicCode.Code,
                        _ => _.SsicCode.SSIC
                        );

					

                });
        }
    }
}
