using System.Collections.Generic;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support.Exporting
{
    public interface IIncidentStatusesExcelExporter
    {
        FileDto ExportToFile(List<GetIncidentStatusForViewDto> incidentStatuses);
    }
}