using System.Collections.Generic;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support.Exporting
{
    public interface IIncidentsExcelExporter
    {
        FileDto ExportToFile(List<GetIncidentForViewDto> incidents);
    }
}