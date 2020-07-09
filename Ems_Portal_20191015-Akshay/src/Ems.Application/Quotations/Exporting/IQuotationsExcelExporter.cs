using System.Collections.Generic;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations.Exporting
{
    public interface IQuotationsExcelExporter
    {
        FileDto ExportToFile(List<GetQuotationForViewDto> quotations);
    }
}