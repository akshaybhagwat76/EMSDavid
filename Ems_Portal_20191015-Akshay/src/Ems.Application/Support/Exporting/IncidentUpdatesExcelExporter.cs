using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class IncidentUpdatesExcelExporter : EpPlusExcelExporterBase, IIncidentUpdatesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public IncidentUpdatesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetIncidentUpdateForViewDto> incidentUpdates)
        {
            return CreateExcelPackage(
                "IncidentUpdates.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("IncidentUpdates"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Updated"),
                        L("Update"),
                        (L("User")) + L("Name"),
                        (L("Incident")) + L("Description")
                        );

                    AddObjects(
                        sheet, 2, incidentUpdates,
                        _ => _timeZoneConverter.Convert(_.IncidentUpdate.Updated, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.IncidentUpdate.Update,
                        _ => _.UserName,
                        _ => _.IncidentDescription
                        );

					var updatedColumn = sheet.Column(1);
                    updatedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					updatedColumn.AutoFit();
					

                });
        }
    }
}
