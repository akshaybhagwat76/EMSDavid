using System.Collections.Generic;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing.Exporting
{
    public interface IBillingEventDetailsExcelExporter
    {
        FileDto ExportToFile(List<GetBillingEventDetailForViewDto> billingEventDetails);
    }
}