using System.Collections.Generic;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing.Exporting
{
    public interface IXeroInvoicesExcelExporter
    {
        FileDto ExportToFile(List<GetXeroInvoiceForViewDto> xeroInvoices);
    }
}