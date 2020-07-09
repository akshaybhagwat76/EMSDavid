using System.Collections.Generic;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing.Exporting
{
    public interface IBillingEventsExcelExporter
    {
        FileDto ExportToFile(List<GetBillingEventForViewDto> billingEvents);
    }
}