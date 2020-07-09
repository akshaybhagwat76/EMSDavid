using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class IncidentPrioritiesExcelExporter : EpPlusExcelExporterBase, IIncidentPrioritiesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public IncidentPrioritiesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetIncidentPriorityForViewDto> incidentPriorities)
        {
            return CreateExcelPackage(
                "IncidentPriorities.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("IncidentPriorities"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Priority"),
                        L("Description"),
                        L("PriorityLevel")
                        );

                    AddObjects(
                        sheet, 2, incidentPriorities,
                        _ => _.IncidentPriority.Priority,
                        _ => _.IncidentPriority.Description,
                        _ => _.IncidentPriority.PriorityLevel
                        );

					

                });
        }
    }
}
