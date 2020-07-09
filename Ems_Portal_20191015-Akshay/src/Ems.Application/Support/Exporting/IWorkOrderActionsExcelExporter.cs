using Ems.Dto;
using Ems.Support.Dtos;
using System.Collections.Generic;

namespace Ems.Support.Exporting
{
    public interface IWorkOrderActionsExcelExporter
    {
        FileDto ExportToFile(List<GetWorkOrderActionForViewDto> workOrderActions);
    }
}
