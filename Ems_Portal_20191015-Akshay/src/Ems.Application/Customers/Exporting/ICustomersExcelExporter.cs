using System.Collections.Generic;
using Ems.Customers.Dtos;
using Ems.Dto;

namespace Ems.Customers.Exporting
{
    public interface ICustomersExcelExporter
    {
        FileDto ExportToFile(List<GetCustomerForViewDto> customers);
    }
}