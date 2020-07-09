using System.Collections.Generic;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support.Exporting
{
    public interface IIncidentUpdatesExcelExporter
    {
        FileDto ExportToFile(List<GetIncidentUpdateForViewDto> incidentUpdates);
    }
}