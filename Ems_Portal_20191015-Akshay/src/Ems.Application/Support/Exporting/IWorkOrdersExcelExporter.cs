using System.Collections.Generic;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support.Exporting
{
    public interface IWorkOrdersExcelExporter
    {
        FileDto ExportToFile(List<GetWorkOrderForViewDto> workOrders);
    }
}