using System.Collections.Generic;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support.Exporting
{
    public interface IWorkOrderUpdatesExcelExporter
    {
        FileDto ExportToFile(List<GetWorkOrderUpdateForViewDto> workOrderUpdates);
    }
}