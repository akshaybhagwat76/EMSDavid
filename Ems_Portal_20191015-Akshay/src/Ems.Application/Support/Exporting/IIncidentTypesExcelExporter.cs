using System.Collections.Generic;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support.Exporting
{
    public interface IIncidentTypesExcelExporter
    {
        FileDto ExportToFile(List<GetIncidentTypeForViewDto> incidentTypes);
    }
}