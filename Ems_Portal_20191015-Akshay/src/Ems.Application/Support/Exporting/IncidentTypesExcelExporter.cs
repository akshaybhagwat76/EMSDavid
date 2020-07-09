using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class IncidentTypesExcelExporter : EpPlusExcelExporterBase, IIncidentTypesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public IncidentTypesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetIncidentTypeForViewDto> incidentTypes)
        {
            return CreateExcelPackage(
                "IncidentTypes.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("IncidentTypes"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Type"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, incidentTypes,
                        _ => _.IncidentType.Type,
                        _ => _.IncidentType.Description
                        );

					

                });
        }
    }
}
