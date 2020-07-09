using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class IncidentsExcelExporter : EpPlusExcelExporterBase, IIncidentsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public IncidentsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetIncidentForViewDto> incidents)
        {
            return CreateExcelPackage(
                "Incidents.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Incidents"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Description"),
                        L("IncidentDate"),
                        L("Location"),
                        L("Remarks"),
                        L("Attachments"),
                        L("ResolvedAt"),
                        (L("IncidentPriority")) + L("Priority"),
                        (L("IncidentStatus")) + L("Status"),
                        (L("Customer")) + L("Name"),
                        (L("Asset")) + L("Reference"),
                        (L("SupportItem")) + L("Description"),
                        (L("IncidentType")) + L("Type"),
                        (L("User")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, incidents,
                        _ => _.Incident.Description,
                        _ => _timeZoneConverter.Convert(_.Incident.IncidentDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Incident.Location,
                        _ => _.Incident.Remarks,
                        _ => _timeZoneConverter.Convert(_.Incident.ResolvedAt, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.IncidentPriorityPriority,
                        _ => _.IncidentStatusStatus,
                        _ => _.CustomerName,
                        _ => _.AssetReference,
                        _ => _.SupportItemDescription,
                        _ => _.IncidentTypeType,
                        _ => _.UserName
                        );

					var incidentDateColumn = sheet.Column(2);
                    incidentDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					incidentDateColumn.AutoFit();
					var resolvedAtColumn = sheet.Column(6);
                    resolvedAtColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					resolvedAtColumn.AutoFit();
					

                });
        }
    }
}
