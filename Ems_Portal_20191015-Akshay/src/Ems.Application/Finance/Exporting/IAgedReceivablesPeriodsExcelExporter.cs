using System.Collections.Generic;
using Ems.Finance.Dtos;
using Ems.Dto;

namespace Ems.Finance.Exporting
{
    public interface IAgedReceivablesPeriodsExcelExporter
    {
        FileDto ExportToFile(List<GetAgedReceivablesPeriodForViewDto> agedReceivablesPeriods);
    }
}