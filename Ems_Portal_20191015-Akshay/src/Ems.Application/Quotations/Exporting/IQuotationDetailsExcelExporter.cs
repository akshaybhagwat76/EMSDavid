using System.Collections.Generic;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations.Exporting
{
    public interface IQuotationDetailsExcelExporter
    {
        FileDto ExportToFile(List<GetQuotationDetailForViewDto> quotationDetails);
    }
}