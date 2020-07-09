using System.Collections.Generic;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations.Exporting
{
    public interface IRfqsExcelExporter
    {
        FileDto ExportToFile(List<GetRfqForViewDto> rfqs);
    }
}