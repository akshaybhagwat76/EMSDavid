using System.Collections.Generic;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing.Exporting
{
    public interface ICustomerInvoicesExcelExporter
    {
        FileDto ExportToFile(List<GetCustomerInvoiceForViewDto> customerInvoices);
    }
}