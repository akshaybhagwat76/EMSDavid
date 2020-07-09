using System.Collections.Generic;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing.Exporting
{
    public interface ICurrenciesExcelExporter
    {
        FileDto ExportToFile(List<GetCurrencyForViewDto> currencies);
    }
}